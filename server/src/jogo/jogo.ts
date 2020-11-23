import { Sala } from "../main";
import { Baralho } from "./baralho";

export class Jogo {
  private baralho: Baralho;
  private sala: Sala;
  private vezesEsperada = 0;
  private maxVezesEsperada = 4;
  private io: any;
  private esperandoJogador = false;

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
    if (!this.esperandoJogador) {
      this.esperandoJogador = true;
      while (true) {
        if (this.sala.maxNumUsers > this.sala.qtdeUser) {
          this.vezesEsperada++;
          if (this.vezesEsperada > this.maxVezesEsperada) {
            this.io.to(this.sala.name).emit("carrega-jogo");
            break;
          } else {
            console.log(`Começando a esperar vez número ${this.vezesEsperada}`);
            await this.timeout(5801);
          }
        } else {
          this.io.to(this.sala.name).emit("carrega-jogo");
          break;
        }
      }
    }
  }
}
