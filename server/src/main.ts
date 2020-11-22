import { db } from "./db";
import { Usuario, UsuarioDb } from "./usuario";
import express from "express";
import bodyParser from "body-parser";
import { createServer } from "http";
import { Socket } from "socket.io";
import { v4 } from "uuid";

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

interface Sala {
  maxNumUsers: number;
  qtdeUser: number;
  name: string;
}

interface SalaArray {
  [index: string]: Sala;
}

let salas: SalaArray = {};

io.on("connection", (socket: Socket) => {
  socket.on("Novo-Jogador", (msg) => {
    let salaEscolhida = "";
    let salasOcupadas = Object.keys(salas).map((key) => salas[key]);

    for (let i = 0; i < salasOcupadas.length; i++) {
      if (salasOcupadas[i].maxNumUsers < salasOcupadas[i].qtdeUser) {
        if (salasOcupadas[i].maxNumUsers == msg.QtdeJogadores) {
          salaEscolhida = salasOcupadas[i].name;
          salasOcupadas[i].qtdeUser++;
        }
      }
    }

    if (salaEscolhida == "") {
      salaEscolhida = v4();
      salas[salaEscolhida] = {
        maxNumUsers: msg.QtdeJogadores,
        qtdeUser: 1,
        name: "",
      };
    }

    socket.join(salaEscolhida);
    socket.emit("sala-numero", salaEscolhida);
  });
});

server.listen(port, () => {
  console.log(`Servidor está escutando na porta ${port}`);
});
