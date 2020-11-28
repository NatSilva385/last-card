import { Carta } from "./carta";

export class Descarte {
  private _descarte: Carta[] = [];

  adicionarCarta(carta: Carta): boolean {
    if (this._descarte.length == 0) {
      this._descarte.push(carta);
      return true;
    } else {
      let lastDigito = this._descarte.length - 1;
      if (carta.podeJogar(this._descarte[lastDigito])) {
        this._descarte.push(carta);
        return true;
      }
    }
    return false;
  }

  cartaNoTopo(): Carta | undefined {
    let ultima = this._descarte.pop();
    if (ultima != undefined) {
      this._descarte.push(ultima);
    }
    return ultima;
  }

  removeTodasCartasMenosUltima(): Carta[] {
    let ultimaCarta = this._descarte.pop();
    let cartas: Carta[] = [];
    let tamanho = this._descarte.length;
    for (let i = 0; i < tamanho; i++) {
      cartas.push(this._descarte.pop()!);
    }
    this._descarte.push(ultimaCarta!);
    return cartas;
  }
}
