import { table } from "console";
import Knex from "knex";
import crypto from "crypto";
import { rejects, strict } from "assert";
import { resolve } from "path";
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
  nivel?: number;
  experiencia?: number;
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

  adicionarUsuario(usuario: Usuario) {
    usuario.salt = this.generateSalt(12);
    let password: string;
    if (usuario.hash != undefined) {
      password = usuario.hash!;
    } else {
      password = "";
    }

    usuario.hash = this.hasher(password, usuario.salt);
    this.db<Usuario>("usuario")
      .insert({
        email: usuario.email,
        experiencia: 0,
        hash: usuario.hash,
        nUsuario: usuario.nUsuario,
        nivel: 1,
        salt: usuario.salt,
      })
      .catch((err) => {
        console.log(err);
      });
  }

  async emailExiste(email: string): Promise<boolean> {
    let promise = new Promise<boolean>((resolve, rejects) => {
      this.db<Usuario>("usuario")
        .where("email", email)
        .select()
        .then((rows) => {
          if (rows.length > 0) {
            return resolve(true);
          }
          return resolve(false);
        })
        .catch((err) => {
          throw err;
        });
    });
    return promise;
  }

  async campoExiste(campo: string, value: any): Promise<boolean> {
    let promise = new Promise<boolean>((resolve, rejects) => {
      this.db<Usuario>("usuario")
        .where(campo, value)
        .select()
        .then((rows) => {
          if (rows.length > 0) {
            return resolve(true);
          }
          return resolve(false);
        })
        .catch((err) => {
          console.log(err);
          throw err;
        });
    });
    return promise;
  }

  async findOne(email: string): Promise<Usuario | null> {
    let promise = new Promise<Usuario | null>((resolve, rejects) => {
      this.db<Usuario>("usuario")
        .where("email", email)
        .select()
        .then((rows) => {
          if (rows.length > 0) {
            return resolve(rows[0]);
          } else {
            return resolve(null);
          }
        });
    });

    return promise;
  }

  async findUserById(id: number): Promise<Usuario | null> {
    let promise = new Promise<Usuario | null>((resolve, rejects) => {
      this.db<Usuario>("usuario")
        .where("id", id)
        .select()
        .then((rows) => {
          if (rows.length > 0) {
            return resolve(rows[0]);
          } else {
            return resolve(null);
          }
        });
    });

    return promise;
  }
}
