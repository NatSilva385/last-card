import { Carta } from "./carta";

export class Jogador {
  private _terminouCarregar: boolean = false;
  public get TerminouCarregar(): boolean {
    return this._terminouCarregar;
  }
  public set TerminouCarregar(value: boolean) {
    this._terminouCarregar = value;
  }

  private _socketID: string;
  public get SocketID(): string {
    return this._socketID;
  }
  public set SocketID(value: string) {
    this._socketID = value;
  }

  private _name: string;
  public get Name(): string {
    return this._name;
  }
  public set Name(value: string) {
    this._name = value;
  }

  private _controladoComputador: boolean = true;
  public get ControladoComputador(): boolean {
    return this._controladoComputador;
  }
  public set ControladoComputador(value: boolean) {
    this._controladoComputador = value;
  }

  private _mao: Carta[] = [];
  public get Mao(): Carta[] {
    return this._mao;
  }
  public set Mao(value: Carta[]) {
    this._mao = value;
  }

  constructor(socketID: string, name: string) {
    this._socketID = socketID;
    this._name = name;
  }
}
