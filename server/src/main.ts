import { db } from "./db";
import { Usuario, UsuarioDb } from "./usuario";
import express from "express";
import bodyParser from "body-parser";
import { createServer } from "http";
import { Socket } from "socket.io";

let app = express();

let jsonParser = bodyParser.json();

app.use(jsonParser);
const port = 3000;

var usuarioDb = new UsuarioDb(db);
const server = createServer(app);
var io = require("socket.io")(server);
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

io.on("connection", (socket: Socket) => {
  socket.on("Novo-Jogador", (msg) => {
    console.log("Usuário solicitou um novo jogo");
  });
});

server.listen(port, () => {
  console.log(`Servidor está escutando na porta ${port}`);
});
