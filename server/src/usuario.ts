import { table } from "console";
import Knex from "knex";
export interface Usuario {
  id?: number;
  email: string;
  nUsuario: string;
  hash?: string;
  salt?: string;
  nivel: number;
  experiencia: number;
}
export class UsuarioDb {
  db: Knex;

  constructor(db: Knex) {
    this.db = db;
    db<Usuario>("usuario")
      .select("*")
      .catch((err) => {
        this.criaTabela();
      });
  }
  criaTabela() {
    this.db.schema
      .createTable("usuario", (table) => {
        table.increments("id"),
          table.string("email").notNullable().unique(),
          table.string("nUsuario").notNullable().unique(),
          table.string("hash").notNullable(),
          table.string("salt").notNullable(),
          table.integer("nivel").unsigned(),
          table.integer("experiencia").unsigned();
      })
      .catch((err) => {
        console.log(err);
      });
  }
}
