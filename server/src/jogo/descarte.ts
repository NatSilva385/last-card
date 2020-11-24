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
}
