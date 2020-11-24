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
    if (
      (carta.Cor as number) === (this.Cor as number) ||
      (carta.Valor as number) === (this.Valor as number)
    ) {
      return true;
    }
    if (
      (this.Valor as number) === (VALOR.CORINGA as number) ||
      (this.Valor as number) === (VALOR.CORINGA_MAIS_QUATRO as number)
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
