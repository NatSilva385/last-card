import { db } from "./db";
import { Usuario, UsuarioDb } from "./usuario";
import express from "express";
import bodyParser from "body-parser";

let app = express();

let jsonParser = bodyParser.json();

const port = 3000;

var usuarioDb = new UsuarioDb(db);

app.post("/usuarios/create", jsonParser, (req, res) => {
  let usuario: Usuario = {
    email: req.body.email,
    nUsuario: req.body.nUsuario,
    hash: req.body.password,
  };
    console.log(req.body);
    
  usuarioDb.adicionarUsuario(usuario);

  res.status(201).send("Usuário inserido com sucesso");
});

app.listen(port, () => {
  console.log(`Servidor está escutando na porta ${port}`);
});
