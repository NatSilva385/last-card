import { db } from "./db";
import { Usuario, UsuarioDb } from "./usuario";
import express from "express";
import bodyParser from "body-parser";
import { createServer } from "http";
import { Socket } from "socket.io";
import { v4 } from "uuid";
import { Jogo } from "./jogo/jogo";
import { getDefaultSettings } from "http2";
import { Jogador } from "./jogo/jogador";

let app = express();

let jsonParser = bodyParser.json();

app.use(jsonParser);
const port = 3000;

var usuarioDb = new UsuarioDb(db);
const server = createServer(app);
var io = require("socket.io")(server);

app.use(express.static("public"));

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

app.get("/", (req, res) => {
  res.sendFile("index.html");
});

export interface Sala {
  maxNumUsers: number;
  qtdeUser: number;
  name: string;
  jogo?: Jogo;
  jogadores: Jogador[];
}

interface SalaArray {
  [index: string]: Sala;
}

interface JogadorArray {
  [index: string]: Jogador;
}

let salas: SalaArray = {};

io.on("connection", (socket: Socket) => {
  socket.on("Novo-Jogador", (msg) => {
    let salaEscolhida = "";
    let salasOcupadas = Object.keys(salas).map((key) => salas[key]);

    for (let i = 0; i < salasOcupadas.length; i++) {
      console.log(salasOcupadas[i].name);
      if (salasOcupadas[i].maxNumUsers > salasOcupadas[i].qtdeUser) {
        if (salasOcupadas[i].maxNumUsers == msg.QtdeJogadores) {
          salaEscolhida = salasOcupadas[i].name;
          salasOcupadas[i].jogadores.push(
            new Jogador(socket.id, msg.NomeJogador)
          );
          salasOcupadas[i].jogadores![i].ControladoComputador = false;
          salasOcupadas[i].qtdeUser++;
        }
      }
    }

    if (salaEscolhida == "") {
      salaEscolhida = v4();
      console.log("Sala escolhida " + salaEscolhida);
      salas[salaEscolhida] = {
        maxNumUsers: msg.QtdeJogadores,
        qtdeUser: 1,
        name: "",
        jogadores: [],
      };
      salas[salaEscolhida].jogo = new Jogo(salas[salaEscolhida], io);
      let x = salas[salaEscolhida].jogadores.push(
        new Jogador(socket.id, msg.NomeJogador)
      );
      salas[salaEscolhida].jogadores[x - 1].ControladoComputador = false;
    }

    socket.join(salaEscolhida);
    salas[salaEscolhida].name = salaEscolhida;
    salas[salaEscolhida].jogo!.esperaJogadores();
    socket.emit("sala-numero", salaEscolhida, socket.id);
  });

  socket.on("terminou-carregar", (msg) => {
    salas[msg].jogo!.esperaTerminarCarregar();
    salas[msg].jogadores.forEach((value) => {
      if (value.SocketID == socket.id) {
        value.TerminouCarregar = true;
      }
    });
  });

  socket.on("terminar-turno", (msg) => {
    salas[msg].jogo!.aguardar();
    salas[msg].jogadores.forEach((jogador) => {
      if (jogador.SocketID == socket.id) {
        jogador.Aguardando = false;
      }
    });
  });

  socket.on("disconnect", (msg) => {
    console.log("usuario desconectou");
    console.log(socket.id);
    console.log(msg);
    let salaJogador: number;
    let salasOcupadas = Object.keys(salas).map((key) => salas[key]);
    let i: number;
    let remove: number;
    let achou: boolean = false;
    for (i = 0; i < salasOcupadas.length; i++) {
      achou = false;
      for (let x = 0; x < salasOcupadas[i].qtdeUser; i++) {
        if (salasOcupadas[i].jogadores[x].SocketID == socket.id) {
          salasOcupadas[i].jogadores[x].ControladoComputador = true;
          achou = true;
          break;
        }
      }
      if (achou) {
        remove = i;
        break;
      }
    }

    for (let x = 0; x < salasOcupadas[i].qtdeUser; x++) {
      if (!salasOcupadas[i].jogadores[x].ControladoComputador) {
        achou = false;
      }
    }

    if (achou) {
      salasOcupadas[i].jogo!.Destruir = true;
      salasOcupadas[i].jogo = undefined;
      salas[salasOcupadas[i].name] = {
        jogadores: [],
        maxNumUsers: 0,
        name: "",
        qtdeUser: 0,
      };
    }
  });
});

server.listen(port, () => {
  console.log(`Servidor está escutando na porta ${port}`);
});
