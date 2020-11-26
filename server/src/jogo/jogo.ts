import { v4 } from "uuid";
import { Jogada, Sala } from "../main";
import { Baralho } from "./baralho";
import { Carta, COR, VALOR } from "./carta";
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
  private aguardaEscolhaCor = false;
  private temQueEscolher = false;
  private temQueComprar = false;
  private qtdeCartasComprar = 0;

  private _destruir = false;
  public get Destruir() {
    return this._destruir;
  }
  public set Destruir(value) {
    this._destruir = value;
  }

  private ordemJogadas: JogadorOrdem[] = [];

  constructor(sala: Sala, io: any) {
    this.descarte = new Descarte();
    this.baralho = new Baralho(this.descarte, io, sala.name, sala.jogadores);
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
              let tmp: string[] = [];
              this.ordemJogadas.forEach((value) => tmp.push(value.socketID));
              console.log(this.sala.jogadores[i].SocketID);
              console.log(this.sala.jogadores[i].ControladoComputador);
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
    this.comecaTurno = false;
    this.aguardaComecaTurno = true;
    this.jogada = false;
    this.aguardaJogada = false;
    this.aguardaEscolhaCor = false;
    this.aguardando = false;
    if (this.turnoAtual > this.ordemJogadas.length - 1) {
      this.turnoAtual = 0;
    } else if (this.turnoAtual < 0) {
      this.turnoAtual = this.ordemJogadas.length - 1;
    }
    let podeCarta = false;
    let turno: ComecoTurno = {
      jogadorId: this.turnoAtual,
      cartas: [],
    };
    /**Checa se alguma carta já foi jogada*/
    if (this.descarte.cartaNoTopo() == undefined) {
      podeCarta = true;
    } else {
      /**checa se precisa comprar alguma carta */
      if (this.temQueComprar) {
        /**checa se o jogador tem na mão alguma carta que o impedirá de comprar */
        let impede = false;
        let ultimaCarta = this.descarte.cartaNoTopo();

        for (
          let x = 0;
          x <
          this.sala.jogadores[this.ordemJogadas[this.turnoAtual].id].Mao.length;
          x++
        ) {
          if (ultimaCarta!.Valor == VALOR.CORINGA_MAIS_QUATRO) {
            if (
              this.sala.jogadores[this.ordemJogadas[this.turnoAtual].id].Mao[x]
                .Cor == ultimaCarta!.Cor &&
              this.sala.jogadores[this.ordemJogadas[this.turnoAtual].id].Mao[x]
                .Valor == VALOR.MAIS_DOIS
            ) {
              impede = true;
            } else if (
              this.sala.jogadores[this.ordemJogadas[this.turnoAtual].id].Mao[x]
                .Valor == VALOR.CORINGA_MAIS_QUATRO
            ) {
              impede = true;
            }
          } else {
            if (
              this.sala.jogadores[this.ordemJogadas[this.turnoAtual].id].Mao[x]
                .Valor == VALOR.MAIS_DOIS ||
              this.sala.jogadores[this.ordemJogadas[this.turnoAtual].id].Mao[x]
                .Valor == VALOR.CORINGA_MAIS_QUATRO
            ) {
              impede = true;
            }
          }
        }
        /**se o jogador não tem nenhuma carta na mão para impedir a compra, ele compra novas cartas */
        if (!impede) {
          for (let x = 0; x < this.qtdeCartasComprar; x++) {
            let carta = this.baralho.comprarCarta();
            this.sala.jogadores[this.ordemJogadas[this.turnoAtual].id].Mao.push(
              carta
            );
            turno.cartas.push(carta);
            this.temQueComprar = false;
          }
          this.qtdeCartasComprar = 0;
        }
      }
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
          podeCarta = true;
        }
      }
    }

    /**Se o jogador precisar comprar uma carta, compra */
    if (!podeCarta) {
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

    console.log(this.turnoAtual);
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
      await this.timeout(500);
      let carta: Carta = new Carta();
      /**se nenhuma carta tiver sido jogada o computador escolhe a primeira carta da sua mão */
      if (this.descarte.cartaNoTopo() == undefined) {
        carta = this.sala.jogadores[this.ordemJogadas[this.turnoAtual].id]
          .Mao[0];
      } else {
        /**se ele tiver que comprar carta, checa se ele tem alguma carta para jogar */

        if (this.temQueComprar) {
          let impede = false;
          let pos = 0;
          let ultimaCarta = this.descarte.cartaNoTopo();
          let jogadorAtual = this.sala.jogadores[
            this.ordemJogadas[this.turnoAtual].id
          ];
          for (let x = 0; x < jogadorAtual.Mao.length; x++) {
            if (ultimaCarta?.Valor == VALOR.CORINGA_MAIS_QUATRO) {
              if (
                jogadorAtual.Mao[x].Valor == VALOR.MAIS_DOIS &&
                jogadorAtual.Mao[x].Cor == ultimaCarta.Cor
              ) {
                carta = jogadorAtual.Mao[x];
                break;
              } else if (
                jogadorAtual.Mao[x].Valor == VALOR.CORINGA_MAIS_QUATRO
              ) {
                carta = jogadorAtual.Mao[x];
                break;
              }
            } else {
              if (
                jogadorAtual.Mao[x].Valor == VALOR.MAIS_DOIS ||
                jogadorAtual.Mao[x].Valor == VALOR.CORINGA_MAIS_QUATRO
              ) {
                carta = jogadorAtual.Mao[x];
                break;
              }
            }
          }
        } else {
          /**caso alguma carta tiver sido jogada ele escolhe a primeira carta que pode jogar */
          for (
            i = 0;
            i <
            this.sala.jogadores[this.ordemJogadas[this.turnoAtual].id].Mao
              .length;
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
      }

      /**remove a carta da mão do computador e coloca no descarte */
      this.sala.jogadores[this.ordemJogadas[this.turnoAtual].id].Mao.splice(
        i,
        1
      );
      this.descarte.adicionarCarta(carta);

      let jogada: Jogada = {
        carta: carta,
        sala: this.sala.name,
        jogadorId: this.turnoAtual,
      };

      if (carta.Valor == VALOR.INVERTER) {
        this.incrementoTurno *= -1;
        console.log("jogada invertida");
      } else if (carta.Valor == VALOR.BLOQUEAR) {
        this.turnoAtual += this.incrementoTurno;
        if (this.turnoAtual > this.ordemJogadas.length - 1) {
          this.turnoAtual = 0;
        } else if (this.turnoAtual < 0) {
          this.turnoAtual = this.ordemJogadas.length - 1;
        }
      } else if (carta.Valor == VALOR.MAIS_DOIS) {
        this.temQueComprar = true;
        this.qtdeCartasComprar += 2;
      } else if (carta.Valor == VALOR.CORINGA_MAIS_QUATRO) {
        this.temQueComprar = true;
        this.qtdeCartasComprar += 4;
      }

      /**transmite a jogada para os outros jogadores */
      this.io.to(this.sala.name).emit("jogada", jogada);

      /**checa se o computador jogou um coringa, caso tenha jogado, notifica que irá escolher uma cor */
      if (
        carta.Valor == VALOR.CORINGA ||
        carta.Valor == VALOR.CORINGA_MAIS_QUATRO
      ) {
        console.log("o computador precisará escolher a cor");
        console.log(carta.Valor);
        console.log(carta.Cor);
        this.temQueEscolher = true;
        this.aguardando = false;
        this.comecaTurno = false;
        this.aguardaComecaTurno = false;
        this.jogada = false;
        this.aguardaEscolhaCor = true;
        this.aguardaJogada = false;
      } else {
        this.aguardando = false;
        this.comecaTurno = false;
        this.aguardaComecaTurno = false;
        this.jogada = false;
        this.aguardaEscolhaCor = false;
        this.aguardaJogada = true;
      }
    }
  }

  /**
   * Checa se é a vez do computador de escolher qual cor será usada após jogar um coringa
   */

  async escolheCor() {
    /**Para facilitar salva o jogador atual em uma variavel */
    let jogadorAtual = this.sala.jogadores[
      this.ordemJogadas[this.turnoAtual].id
    ];
    /**Primeiro checa se o jogador é um computador, caso seja ele realiza a jogada */
    if (
      this.sala.jogadores[this.ordemJogadas[this.turnoAtual].id]
        .ControladoComputador
    ) {
      /**simula o tempo necessario para escolher a cor */
      await this.timeout(500);

      /**armazena em um vetor quantas vezes cada cor aparece na mão do jogador */
      let qtde: number[] = [0, 0, 0, 0, 0];
      for (let i = 0; i < jogadorAtual.Mao.length; i++) {
        qtde[jogadorAtual.Mao[i].Cor]++;
      }

      /**armazena em um indice qual é a cor que mais aparece na mão do jogador */
      let maior: number = 0;
      for (let i = 0; i < qtde.length; i++) {
        if (qtde[i] > qtde[maior]) {
          maior = i;
        }
      }

      /**se não houver nenhuma carta na mão, escolhe a cor amarelo */
      if (maior == COR.SEMCOR) {
        maior = COR.AMARELO;
      }

      let carta = this.descarte.cartaNoTopo();
      carta!.Cor = maior;

      let jogada: Jogada = {
        carta: carta!,
        sala: this.sala.name,
        jogadorId: this.turnoAtual,
      };

      console.log("O PC escolheu cor: " + carta!.Cor);

      this.temQueEscolher = false;

      this.aguardando = false;
      this.comecaTurno = false;
      this.aguardaComecaTurno = false;
      this.jogada = false;
      this.aguardaEscolhaCor = false;
      this.aguardaJogada = true;

      this.io.to(this.sala.name).emit("escolhe-cor", jogada);
    }
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
    /* let achou = false;
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
    }*/

    /**checa se existe alguma carta no descarta */
    if (this.descarte.cartaNoTopo() == undefined) {
      return true;
    }
    /**checa se é possivel jogar a carta selecionada */
    if (
      this.sala.jogadores[this.ordemJogadas[jogadorId].id].possuiCarta(carta)
    ) {
      if (this.temQueComprar) {
        let ultimaCarta = this.descarte.cartaNoTopo();
        if (ultimaCarta?.Valor == VALOR.CORINGA_MAIS_QUATRO) {
          if (carta.Valor == VALOR.MAIS_DOIS && carta.Cor == ultimaCarta.Cor) {
            return true;
          }
        }
        if (
          carta.Valor != VALOR.MAIS_DOIS &&
          carta.Valor != VALOR.CORINGA_MAIS_QUATRO
        ) {
          return false;
        }
      }
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
  jogaCarta(carta: Carta | null, jogadorId: number) {
    /**checa se é possivel jogar a carta */
    /*if (!this.podeJogarCarta(carta, jogadorId)) {
      return;
    }

    /**localiza a carta na mão do jogador */
    if (carta != null) {
      let i = 0;
      for (
        i = 0;
        i < this.sala.jogadores[this.ordemJogadas[jogadorId].id].Mao.length;
        i++
      ) {
        if (
          this.sala.jogadores[this.ordemJogadas[jogadorId].id].Mao[i].Cor ==
            carta.Cor &&
          this.sala.jogadores[this.ordemJogadas[jogadorId].id].Mao[i].Valor ==
            carta.Valor
        ) {
          break;
        }
      }

      /**remove a carta da mão do jogador e a adiciona no descarte */
      this.sala.jogadores[this.ordemJogadas[jogadorId].id].Mao.splice(i, 1);
      this.descarte.adicionarCarta(carta);
    }

    /**se a carta for nula, cria uma carta em branco para enviar aos outros jogadores */
    if (carta == null) {
      carta = new Carta();
      carta.Cor = COR.SEMCOR;
      carta.Valor = VALOR.SEM_VALOR;
      console.log(`carta nula id do jogador: ${jogadorId}`);
    }

    let jogada: Jogada = {
      carta: carta,
      jogadorId: jogadorId,
      sala: this.sala.name,
    };

    if (carta.Valor == VALOR.INVERTER) {
      this.incrementoTurno *= -1;
      console.log("jogada invertida");
    } else if (carta.Valor == VALOR.BLOQUEAR) {
      this.turnoAtual += this.incrementoTurno;
      if (this.turnoAtual > this.ordemJogadas.length - 1) {
        this.turnoAtual = 0;
      } else if (this.turnoAtual < 0) {
        this.turnoAtual = this.ordemJogadas.length - 1;
      }
    } else if (carta.Valor == VALOR.MAIS_DOIS) {
      this.temQueComprar = true;
      this.qtdeCartasComprar += 2;
    } else if (carta.Valor == VALOR.CORINGA_MAIS_QUATRO) {
      this.temQueComprar = true;
      this.qtdeCartasComprar += 4;
    }

    this.aguardando = false;
    this.comecaTurno = false;
    this.aguardaComecaTurno = false;
    this.jogada = false;
    this.aguardaEscolhaCor = true;
    this.aguardaJogada = false;

    /**notifica para os outros jogadores da jogada realizada */
    this.sala.jogadores[this.ordemJogadas[jogadorId].id]
      .Socket!.to(this.sala.name)
      .emit("jogada", jogada);

    /**caso a carta jogada seja um coringa, espera que o jogador informe a cor jogada*/
    if (
      carta.Valor == VALOR.CORINGA ||
      carta.Valor == VALOR.CORINGA_MAIS_QUATRO
    ) {
      this.aguardando = false;
      this.comecaTurno = false;
      this.aguardaComecaTurno = false;
      this.jogada = false;
      this.aguardaEscolhaCor = true;
      this.aguardaJogada = false;
    } else {
      this.aguardando = false;
      this.comecaTurno = false;
      this.aguardaComecaTurno = false;
      this.jogada = false;
      this.aguardaEscolhaCor = false;
      this.aguardaJogada = true;
    }
  }

  /**
   * Checa se a mudança de cor que o jogador pretende fazer é valida
   * @param carta a carta que será jogada
   * @param jogadorId o o numero da ordem do jogador que pretende realizar a jogada
   */
  podeTrocarCor(carta: Carta, jogadorId: number): boolean {
    /**checa se é o turno do jogador que está solicitando a jogada */
    if (jogadorId != this.turnoAtual) {
      return false;
    }

    /**checa se existe uma carta no descarte e se é possivel mudar a cor dela */
    if (
      this.descarte.cartaNoTopo() == undefined ||
      this.descarte.cartaNoTopo()!.Cor != COR.SEMCOR
    ) {
      return false;
    }

    /**checa se o jogador enviou uma cor valida para realizar a troca */
    if (carta.Cor == COR.SEMCOR) {
      return false;
    }
    return true;
  }

  trocarCor(cor: COR, jogadorId: number) {
    var cartaNoTopo: Carta = this.descarte.cartaNoTopo()!;
    cartaNoTopo.Cor = cor;

    let jogada: Jogada = {
      carta: cartaNoTopo,
      jogadorId: jogadorId,
      sala: this.sala.name,
    };
    this.sala.jogadores[this.ordemJogadas[jogadorId].id]
      .Socket!.to(this.sala.name)
      .emit("escolhe-cor", jogada);

    console.log("O jogador escolheu a cor");
    this.aguardando = false;
    this.comecaTurno = false;
    this.aguardaComecaTurno = false;
    this.jogada = false;
    this.aguardaEscolhaCor = false;
    this.aguardaJogada = true;
  }

  /**
   * Espera os jogadores concluirem as animações para ir para a proxima etapa
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
          this.sala.jogadores.forEach((jogador) => {
            jogador.Aguardando = true;
          });
          if (this.temQueEscolher) {
            console.log("Terá que escolher");
            this.escolheCor();
            break;
          }
          if (this.comecaTurno) {
            this.comecaTurno = false;
            this.comecarTurno();
          } else if (this.aguardaComecaTurno) {
            this.aguardaComecaTurno = false;
            this.aguardaEscolhaCor = true;
            this.io.to(this.sala.name).emit("comecar-jogada", this.turnoAtual);
            this.jogadaComputador();
          } else if (this.aguardaEscolhaCor) {
            this.aguardaEscolhaCor = false;
          } else if (this.aguardaJogada) {
            this.aguardaJogada = false;
            this.turnoAtual += this.incrementoTurno;
            if (this.turnoAtual > this.ordemJogadas.length - 1) {
              console.log("mudou de turno");
              this.turnoAtual = 0;
            } else if (this.turnoAtual < 0) {
              this.turnoAtual = this.ordemJogadas.length - 1;
            }
            this.comecarTurno();
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
