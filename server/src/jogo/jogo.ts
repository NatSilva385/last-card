import { Sala } from "../main";
import { Baralho } from "./baralho";

export class Jogo {
  private baralho: Baralho;
  private sala: Sala;
  private vezesEsperada = 0;
  private maxVezesEsperada = 4;
  private io: any;

  constructor(sala: Sala, io: any) {
    this.baralho = new Baralho();
    this.sala = sala;
    this.baralho.embaralhar();
    this.baralho.embaralhar();
    this.io = io;
  }

  timeout(ms: number) {
    return new Promise((resolve) => setTimeout(resolve, ms));
  }

  async esperaJogadores() {
    while (true) {
      if (this.sala.maxNumUsers < this.sala.qtdeUser) {
        this.vezesEsperada++;
        if (this.vezesEsperada > this.maxVezesEsperada) {
          this.io.to(this.sala.name).emit("carrega-jogo");
          break;
        } else {
          await this.timeout(1801);
        }
      } else {
        this.io.to(this.sala.name).emit("carrega-jogo");
        break;
      }
    }
  }
}
