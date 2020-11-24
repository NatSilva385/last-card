export class Carta {
  private _cor: COR = COR.SEMCOR;
  public get Cor(): COR {
    return this._cor;
  }
  public set Cor(value: COR) {
    this._cor = value;
  }
  private _valor: VALOR = VALOR.SEM_VALOR;
  public get Valor(): VALOR {
    return this._valor;
  }
  public set Valor(value: VALOR) {
    this._valor = value;
    if (
      this._valor == VALOR.CORINGA ||
      this._valor == VALOR.CORINGA_MAIS_QUATRO
    ) {
      this.Cor = COR.SEMCOR;
    }
  }

  podeJogar(carta: Carta): boolean {
    if (carta.Cor == this.Cor || carta.Valor == this.Valor) {
      return true;
    }
    if (
      this.Valor == VALOR.CORINGA ||
      this.Valor == VALOR.CORINGA_MAIS_QUATRO
    ) {
      return true;
    }
    return false;
  }
}
export enum COR {
  SEMCOR,
  AZUL,
  VERMELHO,
  VERDE,
  AMARELO,
}
export enum VALOR {
  SEM_VALOR,
  UM,
  DOIS,
  TRES,
  QUATRO,
  CINCO,
  SEIS,
  SETE,
  OITO,
  NOVE,
  MAIS_DOIS,
  BLOQUEAR,
  INVERTER,
  CORINGA,
  CORINGA_MAIS_QUATRO,
}
