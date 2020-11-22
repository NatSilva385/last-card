import { Sala } from "../main";
import { Baralho } from "./baralho";

export class Jogo {
  private baralho: Baralho;
  private sala: Sala;
  private vezesEsperada = 0;
  private maxVezesEsperada = 4;

  constructor(sala: Sala) {
    this.baralho = new Baralho();
    this.sala = sala;
    this.baralho.embaralhar();
    this.baralho.embaralhar();
  }

  timeout(ms: number) {
    return new Promise((resolve) => setTimeout(resolve, ms));
  }

  async esperaJogadores() {
    while (true) {
      if (this.sala.maxNumUsers < this.sala.qtdeUser) {
        this.vezesEsperada++;
        if (this.vezesEsperada > this.maxVezesEsperada) {
          break;
        } else {
          await this.timeout(200);
        }
      } else {
        break;
      }
    }
  }
}
