import { Carta, COR, VALOR } from "./carta";
import { Descarte } from "./descarte";
import { Jogador } from "./jogador";

export class Baralho {
  private cartas: Carta[] = [];
  private descarte: Descarte;
  private io: any;
  private sala: string;
  private jogadores: Jogador[];
  constructor(descarte: Descarte, io: any, sala: string, jogadores: Jogador[]) {
    this.io = io;
    this.descarte = descarte;
    this.sala = sala;
    this.jogadores = jogadores;
    for (let cor in COR) {
      if (isNaN(Number(cor))) {
        if (cor != COR[COR.SEMCOR]) {
          for (let valor in VALOR) {
            if (isNaN(Number(valor))) {
              if (valor != VALOR[VALOR.SEM_VALOR]) {
                let c = cor as keyof typeof COR;
                let v = valor as keyof typeof VALOR;
                let carta = new Carta();
                carta.Cor = COR[c];
                carta.Valor = VALOR[v];
                this.cartas.push(carta);
              }
            }
          }
        }
      }
    }
  }

  tamanho(): number {
    return this.cartas.length;
  }

  embaralhar() {
    for (let i = 0; i < this.cartas.length; i++) {
      const j = Math.floor(Math.random() * (i + 1));
      let aux: Carta;
      aux = this.cartas[i];
      this.cartas[i] = this.cartas[j];
      this.cartas[j] = aux;
    }
  }

  comprarCarta(): Carta {
    if (this.cartas.length == 0) {
      console.log("precisou comprar");
      this.adicionarNovasCartas();
    }
    return this.cartas.pop()!;
  }

  timeout(ms: number) {
    return new Promise((resolve) => setTimeout(resolve, ms));
  }

  async adicionarNovasCartas() {
    let novasCartas = this.descarte.removeTodasCartasMenosUltima();
    let tamanho = novasCartas.length;

    for (let i = 0; i < tamanho; i++) {
      this.cartas.push(novasCartas.pop()!);
    }
    this.jogadores.forEach((jogador) => {
      jogador.AguardaNovoBaralho = true;
    });

    console.log("tentando emitir o mover-descarte-baralho");
    this.jogadores.forEach((jogador) => {
      if (!jogador.ControladoComputador) {
        this.io.to(jogador.SocketID).emit("mover-descarte-baralho", tamanho);
      }
    });

    //this.io.in(this.sala).emit("mover-descarte-baralho", tamanho);

    console.log("comecando a aguardar o mover-descarte-baralho");
    while (true) {
      let aguardando = false;
      for (let i = 0; i < this.jogadores.length; i++) {
        if (!this.jogadores[i].ControladoComputador) {
          if (!this.jogadores[i].AguardaNovoBaralho) {
            aguardando = true;
          }
        }
      }
      if (!aguardando) {
        this.embaralhar();
        this.embaralhar();
        break;
      } else {
        await this.timeout(100);
      }
    }
  }
}
