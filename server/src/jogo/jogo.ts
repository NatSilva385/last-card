import { v4 } from "uuid";
import { Sala } from "../main";
import { Baralho } from "./baralho";
import { Carta } from "./carta";
import { Descarte } from "./descarte";
import { Jogador } from "./jogador";

export class Jogo {
  private baralho: Baralho;
  private sala: Sala;
  private vezesEsperada = 0;
  private maxVezesEsperada = 4;
  private io: any;
  private esperandoJogador = false;
  private esperandoCarregar = false;
  private turnoAtual = 0;
  private incrementoTurno = 1;
  private descarte: Descarte;
  private aguardando = false;
  private comecaTurno = true;
  private esperaJogada = false;

  private _destruir = false;
  public get Destruir() {
    return this._destruir;
  }
  public set Destruir(value) {
    this._destruir = value;
  }

  private ordemJogadas: JogadorOrdem[] = [];

  constructor(sala: Sala, io: any) {
    this.baralho = new Baralho();
    this.sala = sala;
    this.baralho.embaralhar();
    this.baralho.embaralhar();
    this.io = io;
    this.descarte = new Descarte();
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
            for (
              let i = this.sala.qtdeUser + 1;
              i <= this.sala.maxNumUsers;
              i++
            ) {
              this.sala.jogadores.push(new Jogador(v4(), ""));
              this.sala.qtdeUser++;
            }
            for (let i = 0; i < this.sala.qtdeUser; i++) {
              this.ordemJogadas.push({
                socketID: this.sala.jogadores[i].SocketID,
                id: i,
              });
            }
            this.embaralhajogadores();
            this.embaralhajogadores();
            this.io.to(this.sala.name).emit("carrega-jogo");
            break;
          } else {
            console.log(`Começando a esperar vez número ${this.vezesEsperada}`);
            await this.timeout(1801);
            if (this.Destruir) {
              break;
            }
          }
        } else {
          for (let i = 0; i < this.sala.qtdeUser; i++) {
            console.log(this.sala.jogadores[i].SocketID);
            this.ordemJogadas.push({
              socketID: this.sala.jogadores[i].SocketID,
              id: i,
            });
          }
          this.embaralhajogadores();
          this.embaralhajogadores();

          this.io.to(this.sala.name).emit("carrega-jogo");
          break;
        }
      }
    }
  }

  async esperaTerminarCarregar() {
    if (!this.esperandoCarregar) {
      this.esperandoCarregar = true;
      while (true) {
        let carregou = true;
        for (let i = 0; i < this.sala.jogadores.length; i++) {
          if (
            !this.sala.jogadores[i].ControladoComputador &&
            !this.sala.jogadores[i].TerminouCarregar
          ) {
            carregou = false;
          }
        }

        if (carregou) {
          for (let i = 0; i < this.sala.qtdeUser; i++) {
            let maoInicial = this.obterMao(4);
            this.sala.jogadores[i].Mao = maoInicial;

            if (!this.sala.jogadores[i].ControladoComputador) {
              console.log(maoInicial);

              let tmp: string[] = [];
              this.ordemJogadas.forEach((value) => tmp.push(value.socketID));
              console.log(tmp);
              this.io
                .to(this.sala.jogadores[i].SocketID)
                .emit("pronto-comecar", maoInicial, tmp);
            }
          }
          break;
        } else {
          await this.timeout(2000);
          if (this.Destruir) {
            break;
          }
        }
      }
    }
  }

  obterMao(qtde: number): Carta[] {
    let mao: Carta[] = [];
    for (let i = 0; i < qtde; i++) {
      mao.push(this.baralho.comprarCarta());
    }
    return mao;
  }

  embaralhajogadores() {
    for (let i = 0; i < this.ordemJogadas.length; i++) {
      const j = Math.floor(Math.random() * (i + 1));
      let aux: JogadorOrdem;
      aux = this.ordemJogadas[i];
      this.ordemJogadas[i] = this.ordemJogadas[j];
      this.ordemJogadas[j] = aux;
    }
  }

  comecarTurno() {
    this.comecaTurno = false;
    this.esperaJogada = true;
    if (this.turnoAtual > this.ordemJogadas.length) {
      this.turnoAtual = 0;
    }
    let podeJogarCarta = false;
    let turno: ComecoTurno = {
      jogadorId: this.turnoAtual,
      cartas: [],
    };
    if (this.descarte.cartaNoTopo() == undefined) {
      podeJogarCarta = true;
    } else {
      for (
        let i = 0;
        i <
        this.sala.jogadores[this.ordemJogadas[this.turnoAtual].id].Mao.length;
        i++
      ) {
        let carta = this.sala.jogadores[this.ordemJogadas[this.turnoAtual].id]
          .Mao[i];
        if (carta.podeJogar(this.descarte.cartaNoTopo()!)) {
          podeJogarCarta = true;
        }
      }
    }

    if (!podeJogarCarta) {
      let carta = this.baralho.comprarCarta();
      this.sala.jogadores[this.ordemJogadas[this.turnoAtual].id].Mao.push(
        carta
      );
      turno.cartas.push(carta);
    }

    for (let i = 0; i < this.sala.jogadores.length; i++) {
      this.sala.jogadores[i].Aguardando = true;
    }

    let turnoOutros: ComecoTurno = {
      cartas: [],
      jogadorId: this.turnoAtual,
    };
    console.log("comecando turno");
    /**
     * Coloca cartas vazias na carta a ser enviada aos jogadores que não vão jogar no turno atual
     */
    turno.cartas.forEach((carta) => turnoOutros.cartas.push(new Carta()));

    /**
     * Checa para ver se o jogador do turno atual é um computador,
     * caso não seja envia a carta comprada ao jogador do turno atual
     * caso o jogador seja controlado pelo computador, envia as cartas vazias a todos os jogadores da sala
     */
    if (
      !this.sala.jogadores[this.ordemJogadas[this.turnoAtual].id]
        .ControladoComputador
    ) {
      this.io
        .to(this.sala.jogadores[this.ordemJogadas[this.turnoAtual].id].SocketID)
        .emit("comecar-turno", turno);
      this.io
        .to(this.sala.jogadores[this.ordemJogadas[this.turnoAtual].id].SocketID)
        .broadcast.emit("comecar-turno", turnoOutros);
    } else {
      this.io.to(this.sala.name).emit("comecar-turno", turnoOutros);
    }
  }

  /**
   * Espera os jogadores
   */
  async aguardar() {
    if (!this.aguardando) {
      while (true) {
        this.aguardando = true;
        let carregando = false;
        for (let i = 0; i < this.sala.jogadores.length; i++) {
          if (!this.sala.jogadores[i].ControladoComputador) {
            if (this.sala.jogadores[i].Aguardando) {
              carregando = true;
              break;
            }
          }
        }
        if (carregando) {
          await this.timeout(100);
        } else {
          console.log("aqui");
          if (this.comecaTurno) {
            this.comecaTurno = false;
            this.esperaJogada = true;
            this.comecarTurno();
          } else {
            this.comecaTurno = true;
            this.esperaJogada = false;
          }
          break;
        }
      }
    }
  }
}

interface JogadorOrdem {
  socketID: string;
  id: number;
}

interface ComecoTurno {
  jogadorId: number;
  cartas: Carta[];
}
