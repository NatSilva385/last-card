import { db } from "./db";
import { Usuario, UsuarioDb } from "./usuario";
import express from "express";
import bodyParser from "body-parser";
import { createServer } from "http";
import { Socket } from "socket.io";
import session from "express-session";
import { v4 } from "uuid";
import { Jogo } from "./jogo/jogo";
import { getDefaultSettings } from "http2";
import { Jogador } from "./jogo/jogador";
import { Carta } from "./jogo/carta";

const passport = require("passport");
require("./config/auth");

let app = express();

let jsonParser = bodyParser.json();

app.use(jsonParser);
const port = 3000;

var usuarioDb = new UsuarioDb(db);
const server = createServer(app);

app.use(
  session({
    secret: "secret",
    resave: true,
    saveUninitialized: true,
  })
);

app.use(passport.initialize());
app.use(passport.session());

var io = require("socket.io")(server);

app.use(express.static("public"));

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

app.post("/login", (req, res, next) => {
  passport.authenticate("login", {
    successRedirect: "/dashboard",
  })(req, res, next);
});

app.get("/dashboard", (req, res, next) => {
  res.send(req.user);
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
      salas[salaEscolhida].jogadores[x - 1].Socket = socket;
    } else {
      let x = salas[salaEscolhida].jogadores.push(
        new Jogador(socket.id, msg.NomeJogador)
      );
      salas[salaEscolhida].jogadores[x - 1].ControladoComputador = false;
      salas[salaEscolhida].jogadores[x - 1].Socket = socket;
      salas[salaEscolhida].qtdeUser++;
    }

    console.log(`Adicionou o usuario ${socket.id} a sala ${salaEscolhida}`);
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

  socket.on("terminar-aguardar", (msg) => {
    salas[msg].jogo!.aguardar();
    salas[msg].jogadores.forEach((jogador) => {
      if (jogador.SocketID == socket.id) {
        jogador.Aguardando = false;
      }
    });
  });

  socket.on("mover-descarte-baralho", (msg) => {
    console.log(
      "o servidor está aguardando mover as cartas do descarte para o baralho"
    );
    salas[msg].jogadores.forEach((jogador) => {
      if (jogador.SocketID == socket.id) {
        jogador.AguardaNovoBaralho = false;
      }
    });
  });

  socket.on("jogada", (msg, ack) => {
    let joga = false;
    let carta: Carta | null;
    let numJoga: number;
    if (msg.carta == null) {
      carta = null;
      joga = true;
    } else {
      carta = new Carta();
      carta.Cor = msg.carta._cor;
      carta.Valor = msg.carta._valor;
      joga = salas[msg.sala].jogo!.podeJogarCarta(carta, msg.jogadorId);
    }
    if (joga) {
      numJoga = 1;
    } else {
      numJoga = 0;
    }
    ack(numJoga);
    if (joga) {
      salas[msg.sala].jogo!.jogaCarta(carta, msg.jogadorId);
    }
  });

  socket.on("escolhe-cor", (msg, ack) => {
    let joga = false;
    let numJoga = 0;
    let carta = new Carta();
    carta.Cor = msg.carta._cor;
    carta.Valor = msg.carta._valor;
    joga = salas[msg.sala].jogo!.podeTrocarCor(msg.carta, msg.jogadorId);
    if (joga) {
      numJoga = 1;
    } else {
      numJoga = 0;
    }
    ack(numJoga);
    if (joga) {
      salas[msg.sala].jogo!.trocarCor(msg.carta._cor, msg.jogadorId);
    }
  });

  socket.on("disconnect", (msg) => {
    console.log("usuario desconectou");
    console.log(socket.id);
    console.log(msg);
    let salaJogador: number;
    let salasOcupadas = Object.keys(salas).map((key) => salas[key]);
    let i: number;
    let remove: number;
    let salaEncontrada: string = "";
    let achou: boolean = false;
    for (i = 0; i < salasOcupadas.length; i++) {
      achou = false;
      for (let x = 0; x < salasOcupadas[i].qtdeUser; i++) {
        if (salasOcupadas[i].jogadores[x].SocketID == socket.id) {
          salasOcupadas[i].jogadores[x].ControladoComputador = true;
          salaEncontrada = salasOcupadas[i].name;
          salas[salaEncontrada].jogadores[x].ControladoComputador = true;
          achou = true;
          break;
        }
      }
      if (achou) {
        remove = i;
        break;
      }
    }

    for (let x = 0; x < salas[salaEncontrada].maxNumUsers; x++) {
      if (!salas[salaEncontrada].jogadores[x].ControladoComputador) {
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
      console.log("excluindo a sala");
    }
  });
});

server.listen(port, () => {
  console.log(`Servidor está escutando na porta ${port}`);
});

export interface Jogada {
  carta: Carta | null;
  jogadorId: number;
  sala: string;
}
