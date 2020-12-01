import { db } from "./db";
import { Usuario, UsuarioDb } from "./usuario";
import express from "express";
import { createServer } from "http";
import bodyParser from "body-parser";
import { nextTick } from "process";
import session from "express-session";
import { Socket } from "socket.io";
import { v4 } from "uuid";
import { Jogo } from "./Jogo";
import { Jogador } from "./Jogador";

const passport = require("passport");
require("./config/auth");

let app = express();

let jsonParser = bodyParser.json();

app.use(jsonParser);

const port = 3000;

var usuarioDb = new UsuarioDb(db);
const server = createServer(app);

var io = require("socket.io")(server);

app.use(
  session({
    secret: "secret",
    resave: true,
    saveUninitialized: true,
  })
);

app.use(passport.initialize());
app.use(passport.session());

app.use(express.static("public"));

app.post("/login", (req, res, next) => {
  passport.authenticate("login", {
    successRedirect: "/dashboard",
  })(req, res, next);
});

app.get("/dashboard", (req, res, next) => {
  res.send(req.user);
});

app.post("/usuarios/create", jsonParser, (req, res) => {
  let usuario: Usuario = {
    email: req.body.email,
    nUsuario: req.body.nUsuario,
    hash: req.body.password,
  };

  if (req.body == {}) {
    return res.status(400).send("bad request");
  }

  let cadastra = true;
  usuarioDb.campoExiste("email", usuario.email).then((exist) => {
    if (exist) {
      res.status(403).send("email");
    } else {
      usuarioDb.campoExiste("nUsuario", usuario.nUsuario).then((exist) => {
        if (exist) {
          res.status(403).send("nome");
        } else {
          usuarioDb.adicionarUsuario(usuario);

          res.status(201).send("Usuário inserido com sucesso");
        }
      });
    }
  });
});

app.get("/", (req, res) => {
  res.sendFile("/index.html");
});

var rooms: RoomArray = {};

export interface Sala {
  maxNumUsers: number;
  qtdeUsers: number;
  users: UserArray;
  name: string;
  jogo?: Jogo;
}

interface RoomArray {
  [index: string]: Sala;
}

interface UserArray {
  [index: string]: Jogador;
}

io.on("connection", (socket: Socket) => {
  socket.on("new-user", (msg) => {
    let iEscolhido: string = "";
    console.log(socket.id);
    var salas = Object.keys(rooms).map((keys) => rooms[keys]);

    for (let i = 0; i < salas.length; i++) {
      if (salas[i].qtdeUsers < salas[i].maxNumUsers) {
        iEscolhido = salas[i].name;
        salas[i].qtdeUsers++;
      }
    }

    if (iEscolhido == "") {
      iEscolhido = v4();
      rooms[iEscolhido] = {
        maxNumUsers: 4,
        qtdeUsers: 1,
        users: {},
        name: iEscolhido,
        jogo: new Jogo(rooms[iEscolhido]),
      };
    }
    rooms[iEscolhido].users[socket.id].name = msg;

    console.log(iEscolhido);
    socket.join(iEscolhido);

    socket.emit("room-number", iEscolhido);
  });

  socket.on("chat-message", (msg) => {
    console.log(socket.id);
    console.log(msg);
    socket.to(msg.Room).broadcast.emit("chat-message", {
      Nome: rooms[msg.Room].users[socket.id].name,
      Menssagem: msg.Messagem,
    });
  });

  socket.on("terminou-carregar", (msg) => {
    rooms[msg.Room].users[socket.id].terminouCarregar = true;
    rooms[msg.Room].jogo!.esperaJogadores();
  });
});

server.listen(port, () => {
  console.log(`Servidor está escutando na porta ${port}`);
});
