using Godot;
using System;
using project.src.models;
using System.Collections.Generic;
using SocketIOClient;
using System.Threading.Tasks;
public class JogoView : Spatial
{
    BaralhoView baralho;
    MaoView mao;

    public bool PodeJogar { get; set; }
    DescarteView descarte;
    MaoView adversario;
    List<MaoView> ordemJogada = new List<MaoView>();
    public bool TurnoDoJogador { get; set; }
    private int jogadorPosicao;
    public SocketIO Client { get; set; }

    public string ID { get; set; }

    public string NumeroSala { get; set; }

    AreaMensagens areaMensagens;

    Spatial corSelecao;

    NotificaEncerrar notifica;

    public Node CurrentScene { get; set; }
    Jogo jogo;

    public Usuario Usuario { get; set; }
    public async override void _Ready()
    {
        baralho = GetNode<BaralhoView>("Baralho");
        baralho.Jogo = this;
        baralho.criarCartas();
        mao = GetNode<MaoView>("Mao");
        mao.Jogo = this;

        descarte = GetNode<DescarteView>("Descarte");
        descarte.Jogo = this;
        descarte.Baralho = baralho;

        areaMensagens = GetNode<AreaMensagens>("AreaMensagens");

        corSelecao = GetNode<Spatial>("cor_selecao");

        notifica = GetNode<NotificaEncerrar>("NotificaEncerrar");

        Viewport root = GetNode<Viewport>("/root");
        CurrentScene = root.GetChild(root.GetChildCount() - 1);

        //adversario = GetNode<MaoView>("MaoJogador2");
        await Client.EmitAsync("terminou-carregar", NumeroSala);
        eventosClient();

    }

    public void eventosClient()
    {
        Client.On("pronto-comecar", response =>
        {

            var cartas = response.GetValue<CartaRecebida[]>();
            List<Carta> listaDeCartas = new List<Carta>();
            foreach (var carta in cartas)
            {
                var c = new Carta();
                c.Cor = (COR)carta._cor;
                c.Valor = (VALOR)carta._valor;
                listaDeCartas.Add(c);
            }
            mao.addCartas(baralho.comprarCartas(listaDeCartas));
            var ordem = response.GetValue<String[]>(1);
            listaDeCartas.Clear();
            foreach (var carta in cartas)
            {
                listaDeCartas.Add(new Carta()
                {
                    Cor = COR.SEMCOR,
                    Valor = VALOR.SEM_VALOR
                });
            }
            mao.ID = ID;
            if (ordem.Length == 2)
            {
                if (ordem[0] == ID)
                {
                    ordemJogada.Add(mao);
                    jogadorPosicao = 0;
                    var mao2 = GetNode<MaoView>("MaoJogador2");
                    mao2.ID = ordem[1];
                    mao2.Jogo = this;
                    ordemJogada.Add(mao2);
                }
                else
                {
                    var mao2 = GetNode<MaoView>("MaoJogador2");
                    mao2.ID = ordem[0];
                    mao2.Jogo = this;
                    ordemJogada.Add(mao2);
                    ordemJogada.Add(mao);
                    jogadorPosicao = 1;
                }
            }
            else
            {
                if (ordem[0] == ID)
                {
                    ordemJogada.Add(mao);
                    jogadorPosicao = 0;
                    var mao2 = GetNode<MaoView>("MaoJogador1");
                    mao2.ID = ordem[1];
                    mao2.Jogo = this;
                    ordemJogada.Add(mao2);
                    mao2 = GetNode<MaoView>("MaoJogador2");
                    mao2.ID = ordem[2];
                    mao2.Jogo = this;
                    ordemJogada.Add(mao2);
                    mao2 = GetNode<MaoView>("MaoJogador3");
                    mao2.ID = ordem[3];
                    mao2.Jogo = this;
                    ordemJogada.Add(mao2);
                }
                else if (ordem[1] == ID)
                {
                    var mao2 = GetNode<MaoView>("MaoJogador1");
                    mao2.ID = ordem[0];
                    mao2.Jogo = this;
                    ordemJogada.Add(mao2);
                    ordemJogada.Add(mao);
                    jogadorPosicao = 1;
                    mao2 = GetNode<MaoView>("MaoJogador2");
                    mao2.ID = ordem[2];
                    mao2.Jogo = this;
                    ordemJogada.Add(mao2);
                    mao2 = GetNode<MaoView>("MaoJogador3");
                    mao2.ID = ordem[3];
                    mao2.Jogo = this;
                    ordemJogada.Add(mao2);
                }
                else if (ordem[2] == ID)
                {
                    var mao2 = GetNode<MaoView>("MaoJogador1");
                    mao2.ID = ordem[0];
                    mao2.Jogo = this;
                    ordemJogada.Add(mao2);
                    mao2 = GetNode<MaoView>("MaoJogador2");
                    mao2.ID = ordem[1];
                    mao2.Jogo = this;
                    ordemJogada.Add(mao2);
                    ordemJogada.Add(mao);
                    jogadorPosicao = 2;
                    mao2 = GetNode<MaoView>("MaoJogador3");
                    mao2.ID = ordem[3];
                    mao2.Jogo = this;
                    ordemJogada.Add(mao2);
                }
                else if (ordem[3] == ID)
                {
                    var mao2 = GetNode<MaoView>("MaoJogador1");
                    mao2.ID = ordem[0];
                    mao2.Jogo = this;
                    ordemJogada.Add(mao2);
                    mao2 = GetNode<MaoView>("MaoJogador2");
                    mao2.ID = ordem[1];
                    mao2.Jogo = this;
                    ordemJogada.Add(mao2);
                    mao2 = GetNode<MaoView>("MaoJogador3");
                    mao2.ID = ordem[2];
                    mao2.Jogo = this;
                    ordemJogada.Add(mao2);
                    ordemJogada.Add(mao);
                    jogadorPosicao = 3;
                }

            }
            foreach (var jogador in ordemJogada)
            {
                if (jogador.ID != ID)
                {
                    jogador.addCartas(baralho.comprarCartas(listaDeCartas));
                }
            }
            areaMensagens.mostraMensage("Começar");
            aguardarAnimacaoCompra();
        });

        Client.On("comecar-turno", response =>
        {
            GD.Print(baralho.tamanho());
            ComecoTurno dados = response.GetValue<ComecoTurno>();
            if (dados.jogadorId == jogadorPosicao)
            {
                areaMensagens.mostraMensage("Seu Turno");
                TurnoDoJogador = true;
            }
            if (dados.cartas.Length > 0)
            {
                List<Carta> listaDeCartas = new List<Carta>();
                foreach (var carta in dados.cartas)
                {
                    listaDeCartas.Add(new Carta()
                    {
                        Cor = (COR)carta._cor,
                        Valor = (VALOR)carta._valor
                    });
                }
                ordemJogada[dados.jogadorId].addCartas(baralho.comprarCartas(listaDeCartas));
            }

            aguardarAnimacaoCompra();
        });

        Client.On("comecar-jogada", async response =>
         {
             var carta = descarte.ultimaCarta();
             int turno = response.GetValue<int>();
             if (carta == null)
             {
                 PodeJogar = true;
             }
             else if (turno == jogadorPosicao)
             {
                 if (ordemJogada[jogadorPosicao].podeJogar(carta))
                 {
                     PodeJogar = true;
                 }
                 else
                 {
                     areaMensagens.mostraMensage("Sem Cartas para jogar");
                     PodeJogar = false;
                     await Task.Delay(500);
                     Jogada jogada = new Jogada();
                     jogada.carta = null;

                     jogada.sala = NumeroSala;
                     jogada.jogadorId = jogadorPosicao;
                     await Client.EmitAsync("jogada", ack =>
                     {
                         terminouAnimacaoJogada();
                     }, jogada);
                 }
             }
         });

        Client.On("jogada", async response =>
       {
           var jogada = response.GetValue<Jogada>();
           Carta carta = new Carta();
           carta.Cor = (COR)jogada.carta._cor;
           carta.Valor = (VALOR)jogada.carta._valor;
           if (carta.Valor == VALOR.SEM_VALOR)
           {
               GD.Print("O Computador vai escolher a cor da carta");
               await Task.Delay(100);
               await Client.EmitAsync("terminar-aguardar", NumeroSala);
           }
           else

           {

               ordemJogada[jogada.jogadorId].removeCarta(carta);
               aguardarAnimacaoCompra();
           }

           //aguardarAnimacaoCompra();
       });

        Client.On("escolhe-cor", response =>
        {
            var jogada = response.GetValue<Jogada>();
            project.src.models.Carta carta = new project.src.models.Carta();
            carta.Cor = (COR)jogada.carta._cor;
            carta.Valor = (VALOR)jogada.carta._valor;
            carta.setCor((COR)jogada.carta._cor);

            descarte.ultimaCartaView().Carta = carta;
            descarte.ultimaCartaView().Carta = carta;

            aguardarAnimacaoCompra();
        });

        Client.On("mover-descarte-baralho", response =>
        {
            GD.Print("Recebeu a notificação de mover-descarte-baralho");
            descarte.moverCartasBaralho();
        });

        Client.On("fim-jogo", async response =>
        {
            var vitorioso = response.GetValue<int>();
            bool ganhou;

            GD.Print("Chegou o valor " + vitorioso);
            GD.Print("Minha posicao: " + jogadorPosicao);
            if (vitorioso == jogadorPosicao)
            {
                ganhou = true;
            }
            else
            {
                ganhou = false;
            }

            var resp = await notifica.exibeMensagem(ganhou);

            if (resp)
            {
                await Client.DisconnectAsync();
                voltarMenuInicial();
            }
        });
    }

    private void voltarMenuInicial()
    {
        var nextScene = ResourceLoader.Load<PackedScene>("res://scene/TelaInicial.tscn");

        //await Client.DisconnectAsync();

        var telaInicia = nextScene.Instance() as FrmInicial;
        telaInicia.Usuario = Usuario;

        GetNode("/root").AddChild(telaInicia);

        //await Client.DisconnectAsync();
        CurrentScene.Free();
        CurrentScene = telaInicia;

    }

    public void init()
    {
    }


    private async void aguardarAnimacaoCompra()
    {
        while (true)
        {
            bool completo = true;
            foreach (var mao in ordemJogada)
            {
                if (!mao.TerminouAnimacao)
                {
                    completo = false;
                }
            }

            if (completo)
            {
                await Client.EmitAsync("terminar-aguardar", NumeroSala);
                break;
            }
            else
            {
                await Task.Delay(100);
            }
        }
    }

    public async void terminouAnimacaoJogada()
    {
        await Client.EmitAsync("terminar-aguardar", NumeroSala);
    }
    public void comprarCarta(CartaView carta)
    {
        mao.addCarta(carta);
    }

    /// <summary>
    /// Tenta jogar uma carta
    /// </summary>
    /// <param name="carta"></param>
    public async void jogarCarta(CartaView carta)
    {
        PodeJogar = false;
        carta c = new carta();
        c._cor = (int)carta.Carta.Cor;
        c._valor = (int)carta.Carta.Valor;
        Jogada jogada = new Jogada();
        jogada.carta = c;
        jogada.jogadorId = jogadorPosicao;
        jogada.sala = NumeroSala;
        await Client.EmitAsync("jogada", response =>
        {
            int x = response.GetValue<int>();
            if (x == 1)
            {
                ordemJogada[jogadorPosicao].removeCarta(carta.Carta);
                aguardarAnimacaoCompra();
                if (carta.Carta.Valor == VALOR.CORINGA || carta.Carta.Valor == VALOR.CORINGA_MAIS_QUATRO)
                {
                    corSelecao.Visible = true;
                    PodeJogar = false;
                }
            }
            else
            {
                PodeJogar = true;
            }
        }, jogada);

    }

    public void animarCarta(CartaView carta)
    {
        descarte.addCarta(carta);
    }

    public async void terminarEncherBaralho()
    {
        Jogada jogada = new Jogada();
        jogada.carta = null;
        jogada.jogadorId = jogadorPosicao;
        jogada.sala = NumeroSala;
        await Client.EmitAsync("mover-descarte-baralho", NumeroSala);
    }


    public async void escolherCor(COR cor)
    {
        carta c = new carta();
        c._cor = (int)cor;
        c._valor = (int)descarte.ultimaCarta().Valor;
        Jogada jogada = new Jogada();
        project.src.models.Carta au = new project.src.models.Carta();
        au.Cor = cor;
        au.Valor = descarte.ultimaCarta().Valor;
        jogada.carta = c;
        jogada.jogadorId = jogadorPosicao;
        jogada.sala = NumeroSala;
        au.setCor(cor);

        areaMensagens.mostraMensage("Cor escolhida: " + cor);
        await Client.EmitAsync("escolhe-cor", response =>
        {

            int resp = response.GetValue<int>();

            if (resp == 1)
            {
                descarte.ultimaCartaView().Carta = au;
                corSelecao.Visible = false;
                aguardarAnimacaoCompra();

            }
        }, jogada);
    }
    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}

public class CartaRecebida
{
    public int _cor { get; set; }
    public int _valor { get; set; }
}

public class ComecoTurno
{
    public int jogadorId { get; set; }
    public carta[] cartas { get; set; }
}

public class carta
{
    public int _cor { get; set; }
    public int _valor { get; set; }
}

public class Jogada
{
    public carta carta { get; set; }
    public int jogadorId { get; set; }
    public string sala { get; set; }
}