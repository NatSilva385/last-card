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

    Jogo jogo;
    public async override void _Ready()
    {
        baralho = GetNode<BaralhoView>("Baralho");
        baralho.Jogo = this;
        baralho.criarCartas();
        mao = GetNode<MaoView>("Mao");
        mao.Jogo = this;

        descarte = GetNode<DescarteView>("Descarte");
        descarte.Jogo = this;

        //adversario = GetNode<MaoView>("MaoJogador2");
        await Client.EmitAsync("terminou-carregar", NumeroSala);
        eventosClient();
    }

    public void eventosClient()
    {
        Client.On("pronto-comecar", response =>
        {
            GD.Print("Pronto começar");
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
                    GD.Print("Você joga primeiro");
                }
                else
                {
                    var mao2 = GetNode<MaoView>("MaoJogador2");
                    mao2.ID = ordem[0];
                    mao2.Jogo = this;
                    ordemJogada.Add(mao2);
                    ordemJogada.Add(mao);
                    jogadorPosicao = 1;
                    GD.Print("Você joga segundo");
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
            aguardarAnimacaoCompra();
        });

        Client.On("comecar-turno", response =>
        {
            GD.Print("Comecar turno");
            ComecoTurno dados = response.GetValue<ComecoTurno>();
            if (dados.jogadorId == jogadorPosicao)
            {
                GD.Print("Seu Turno");
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
             GD.Print("Comeca jogada " + response.ToString());
             var carta = descarte.ultimaCarta();
             if (carta == null)
             {
                 PodeJogar = true;
             }
             else
             {
                 if (ordemJogada[jogadorPosicao].podeJogar(carta))
                 {
                     PodeJogar = true;
                 }
                 else
                 {
                     PodeJogar = false;
                     await Task.Delay(500);
                     Jogada jogada = new Jogada();
                     jogada.carta = null;
                     jogada.sala = NumeroSala;
                     jogada.jogadorId = jogadorPosicao;
                     await Client.EmitAsync("jogada", ack=>{}, jogada);
                 }
             }
         });

        Client.On("jogada", response =>
        {
            GD.Print(response.ToString());
            var jogada = response.GetValue<Jogada>();
            Carta carta = new Carta();
            carta.Cor = (COR)jogada.carta._cor;
            carta.Valor = (VALOR)jogada.carta._valor;
            ordemJogada[jogada.jogadorId].removeCarta(carta);
            aguardarAnimacaoCompra();
        });

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
                GD.Print("Compra completa");
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
        GD.Print("Jogada Completa");
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
        bool resp = false;
        await Client.EmitAsync("jogada", response =>
        {
            if(response.GetValue<string>()=="True")
            {
                resp = true;
            }
        }, jogada);
        GD.Print(resp);
        if (resp)
        {
            ordemJogada[jogadorPosicao].removeCarta(carta.Carta);
            aguardarAnimacaoCompra();
        }
        else
        {
            PodeJogar = true;
        }
        //descarte.addCarta(carta);
    }

    public void animarCarta(CartaView carta)
    {
        GD.Print("Tenta descartar carta");
        descarte.addCarta(carta);
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