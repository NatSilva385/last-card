import { Carta, COR, VALOR } from "./carta";

export class Baralho {
  private cartas: Carta[] = [];

  constructor() {
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
    return this.cartas.pop()!;
  }
}
