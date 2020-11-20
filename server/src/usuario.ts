import { table } from "console";
import Knex from "knex";
import crypto from "crypto";
interface SaltedPassword {
  salt: string;
  hashedPassword: string;
}
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

  generateSalt(rounds: number): string {
    return crypto
      .randomBytes(Math.ceil(rounds / 2))
      .toString("hex")
      .slice(0, rounds);
  }

  hasher(password: string, salt: string): string {
    let hash = crypto.createHmac("sha512", salt);
    hash.update(password);
    let value = hash.digest("hex");
    return value;
  }
}
