export class Jogador {
  private _terminouCarregar = false;
  private _name: string = "";
  public get name(): string {
    return this._name;
  }
  public set name(value: string) {
    this._name = value;
  }
  public get terminouCarregar() {
    return this._terminouCarregar;
  }
  public set terminouCarregar(value) {
    this._terminouCarregar = value;
  }
}
