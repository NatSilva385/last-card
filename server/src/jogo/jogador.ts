import { Socket } from "socket.io";
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

  private _aguardando: boolean = false;
  public get Aguardando(): boolean {
    return this._aguardando;
  }
  public set Aguardando(value: boolean) {
    this._aguardando = value;
  }

  private _socket: Socket | undefined;
  public get Socket(): Socket | undefined {
    return this._socket;
  }
  public set Socket(value: Socket | undefined) {
    this._socket = value;
  }

  private _aguardaNovoBaralho: boolean = false;
  public get AguardaNovoBaralho(): boolean {
    return this._aguardaNovoBaralho;
  }
  public set AguardaNovoBaralho(value: boolean) {
    this._aguardaNovoBaralho = value;
  }

  private _id: number;
  public get Id(): number {
    return this._id;
  }
  public set Id(value: number) {
    this._id = value;
  }

  constructor(socketID: string, name: string, id: number) {
    this._socketID = socketID;
    this._name = name;
    this._id = id;
  }

  possuiCarta(carta: Carta): boolean {
    let possui = false;
    this.Mao.forEach((c) => {
      if (c.Cor == carta.Cor && c.Valor == carta.Valor) {
        possui = true;
      }
    });
    return possui;
  }
}
