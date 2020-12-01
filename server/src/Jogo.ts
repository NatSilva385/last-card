import { Baralho } from "./Baralho";
import { Jogador } from "./Jogador";
import { Sala } from "./main";

export class Jogo {
  private numTurno: number = 0;
  private jogadores: Jogador[] = [];
  private _sala: Sala;
  private vezesEsperada = 0;
  private maxVezes = 3;

  private _carregando = false;
  public get carregando() {
    return this._carregando;
  }
  public set carregando(value) {
    this._carregando = value;
  }

  baralho: Baralho;

  constructor(sala: Sala) {
    this.baralho = new Baralho(this);
    this.baralho.embaralhar();
    this.baralho.embaralhar();
    this._sala = sala;
    this.esperaJogadores();
  }

  adicionaJogador(jogador: Jogador) {
    this.jogadores.push(jogador);
  }

  defineOrdem() {
    for (let i = 0; i < this.jogadores.length; i++) {
      const j = Math.floor(Math.random() * (i + 1));
      let aux: Jogador;
      aux = this.jogadores[i];
      this.jogadores[i] = this.jogadores[j];
      this.jogadores[j] = aux;
    }
  }

  timeout(ms: number) {
    return new Promise((resolve) => setTimeout(resolve, ms));
  }
  async esperaJogadores() {
    while (true) {
      if (this._sala.maxNumUsers < this._sala.qtdeUsers) {
        this.vezesEsperada++;
        await this.timeout(100);
      } else {
        break;
      }
    }
  }

  async checaTerminouCarregar() {
    if (!this.carregando) {
      this.carregando = true;

      while (true) {
        let terminou = true;
        this.jogadores.forEach((jogador) => {
          if (!jogador.terminouCarregar) {
            terminou = false;
          }
        });
        if (terminou) {
          break;
        } else {
          await this.timeout(100);
        }
      }
    }
  }
}
