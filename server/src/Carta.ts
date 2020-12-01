export class Carta {
  private _valor: VALOR = VALOR.SEM_VALOR;
  private _cor: COR = COR.SEMCOR;

  get Valor(): VALOR {
    return this._valor;
  }

  set Valor(value: VALOR) {
    this._valor = value;
  }

  get Cor(): COR {
    return this._cor;
  }

  set Cor(value: COR) {
    this._cor = value;
  }
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

export enum COR {
  SEMCOR,
  AZUL,
  VERMELHO,
  VERDE,
  AMARELO,
}
