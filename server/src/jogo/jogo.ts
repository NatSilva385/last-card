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
  private aguardaComecaTurno = false;
  private jogada = false;
  private aguardaJogada = false;

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

  /**
   * Método que lida com o começo do turno, checando se o jogador precisa comprar alguma carta e
   * e sinalizando que pode começar
   */
  comecarTurno() {
    console.log("funcao comecar turno");
    this.comecaTurno = false;
    this.aguardaComecaTurno = true;
    this.jogada = false;
    this.aguardaJogada = false;
    this.aguardando = false;
    if (this.turnoAtual > this.ordemJogadas.length) {
      this.turnoAtual = 0;
    }
    let podeJogarCarta = false;
    let turno: ComecoTurno = {
      jogadorId: this.turnoAtual,
      cartas: [],
    };
    /**Checa se alguma carta já foi jogada*/
    if (this.descarte.cartaNoTopo() == undefined) {
      podeJogarCarta = true;
    } else {
      /**caso já tenha sido jogada alguma carta, checa se o jogador atual precisa comprar uma nova carta */
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

    /**Se o jogador precisar comprar uma carta, compra */
    if (!podeJogarCarta) {
      let carta = this.baralho.comprarCarta();
      this.sala.jogadores[this.ordemJogadas[this.turnoAtual].id].Mao.push(
        carta
      );
      turno.cartas.push(carta);
    }

    /**
     * prepara os jogadores para aguardar a animação concluir
     */
    for (let i = 0; i < this.sala.jogadores.length; i++) {
      this.sala.jogadores[i].Aguardando = true;
    }

    /**
     * Informação que será enviada aos outros jogadores indicando quantas cartas o jogador do turno atual comprou
     */
    let turnoOutros: ComecoTurno = {
      cartas: [],
      jogadorId: this.turnoAtual,
    };
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
      this.sala.jogadores[this.ordemJogadas[this.turnoAtual].id]
        .Socket!.to(this.sala.name)
        .emit("comecar-turno", turnoOutros);
    } else {
      this.io.to(this.sala.name).emit("comecar-turno", turnoOutros);
    }
  }

  /**
   * Joga uma carta para o jogador do turno atual caso ele seja controlado pelo computador
   */
  async jogadaComputador() {
    /**primeiro checa se o jogador atual é um computador */
    if (
      this.sala.jogadores[this.ordemJogadas[this.turnoAtual].id]
        .ControladoComputador
    ) {
      /**espera para simular um jogador decidindo qual carta jogar */
      let i = 0;
      await this.timeout(2000);
      let carta: Carta = new Carta();
      /**se nenhuma carta tiver sido jogada o computador escolhe a primeira carta da sua mão */
      if (this.descarte.cartaNoTopo() == undefined) {
        carta = this.sala.jogadores[this.ordemJogadas[this.turnoAtual].id]
          .Mao[0];
      } else {
        /**caso alguma carta tiver sido jogada ele escolhe a primeira carta que pode jogar */
        for (
          i = 0;
          i <
          this.sala.jogadores[this.ordemJogadas[this.turnoAtual].id].Mao.length;
          i++
        ) {
          if (
            this.sala.jogadores[this.ordemJogadas[this.turnoAtual].id].Mao[
              i
            ].podeJogar(this.descarte.cartaNoTopo()!)
          ) {
            carta = this.sala.jogadores[this.ordemJogadas[this.turnoAtual].id]
              .Mao[i];
            break;
          }
        }
      }

      /**remove a carta da mão do computador e coloca no descarte */
      this.sala.jogadores[this.ordemJogadas[this.turnoAtual].id].Mao.splice(
        i,
        1
      );
      this.descarte.adicionarCarta(carta);

      /**transmite a jogada para os outros jogadores */
      console.log("PC jogou " + carta);
      this.io.to(this.sala.name).emit("jogada", carta);
    }

    /**inicia a espera para finalizar as animações */
    this.comecaTurno = false;
    this.aguardaComecaTurno = false;
    this.jogada = false;
    this.aguardaJogada = true;
  }

  /**
   * Checa se é possivel jogar uma determinada Carta
   * @param carta a carta que será jogada
   * @param jogadorId o id do jogador que está tentando realizar a jogada
   */
  podeJogarCarta(carta: Carta, jogadorId: number): boolean {
    /**checa para ver ser o jogador que está tentando jogar é o atual */
    if (jogadorId != this.turnoAtual) {
      return false;
    }

    /**checa se a carta existe na mao do jogador */
    let achou = false;
    for (let i = 0; i < this.sala.jogadores[jogadorId].Mao.length; i++) {
      if (
        this.sala.jogadores[jogadorId].Mao[i].Cor == carta.Cor &&
        this.sala.jogadores[jogadorId].Mao[i].Valor == carta.Valor
      ) {
        achou = true;
        break;
      }
    }
    if (!achou) {
      return false;
    }

    /**checa se existe alguma carta no descarta */
    if (this.descarte.cartaNoTopo() == undefined) {
      return true;
    }
    /**checa se é possivel jogar a carta selecionada */
    if (this.sala.jogadores[jogadorId].possuiCarta(carta)) {
      if (carta.podeJogar(this.descarte.cartaNoTopo()!)) {
        return true;
      }
    }
    return false;
  }

  /**
   * Joga uma carta, adicionando ela a mao de um jogador e notificando os outros da jogada
   * @param carta a carta que será jogada
   * @param jogadorId o id do jogador que está jogando a carta
   */
  jogaCarta(carta: Carta, jogadorId: number) {
    /**checa se é possivel jogar a carta */
    if (!this.podeJogarCarta(carta, jogadorId)) {
      return;
    }

    /**localiza a carta na mão do jogador */
    let i = 0;
    for (i = 0; i < this.sala.jogadores[jogadorId].Mao.length; i++) {
      if (
        this.sala.jogadores[jogadorId].Mao[i].Cor == carta.Cor &&
        this.sala.jogadores[jogadorId].Mao[i].Valor == carta.Valor
      ) {
        break;
      }
    }

    /**remove a carta da mão do jogador e a adiciona no descarte */
    this.sala.jogadores[jogadorId].Mao.splice(i, 1);
    console.log(this.sala.jogadores[jogadorId].Mao);
    this.descarte.adicionarCarta(carta);

    /**notifica para os outros jogadores da jogada realizada */
    this.sala.jogadores[jogadorId]
      .Socket!.to(this.sala.name)
      .emit("jogada", carta);

    /**inicia a espera para finalizar as animações */
    this.comecaTurno = false;
    this.aguardaComecaTurno = false;
    this.jogada = false;
    this.aguardaJogada = true;
  }

  /**
   * Espera os jogadores concluirem as animações para ir para a proxima etapa
   */
  async aguardar() {
    if (!this.aguardando) {
      console.log("comecnado a aguardar");
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
          this.sala.jogadores.forEach((jogador) => {
            jogador.Aguardando = true;
          });
          console.log("aqui");
          if (this.comecaTurno) {
            console.log("comecar turno");
            this.comecaTurno = false;
            this.aguardaComecaTurno = true;
            this.jogada = false;
            this.aguardaJogada = false;
            this.comecarTurno();
          } else if (this.aguardaComecaTurno) {
            console.log("comecar a jogar");
            this.comecaTurno = false;
            this.aguardaComecaTurno = false;
            this.jogada = true;
            this.aguardaJogada = false;
            this.io.to(this.sala.name).emit("comecar-jogada", this.turnoAtual);
            this.jogadaComputador();
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
