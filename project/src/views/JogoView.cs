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
                    ordemJogada.Add(mao2);
                    GD.Print("Você joga primeiro");
                }
                else
                {
                    var mao2 = GetNode<MaoView>("MaoJogador2");
                    mao2.ID = ordem[0];
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
                    ordemJogada.Add(mao2);
                    mao2 = GetNode<MaoView>("MaoJogador2");
                    mao2.ID = ordem[2];
                    ordemJogada.Add(mao2);
                    mao2 = GetNode<MaoView>("MaoJogador3");
                    mao2.ID = ordem[3];
                    ordemJogada.Add(mao2);
                }
                else if (ordem[1] == ID)
                {
                    var mao2 = GetNode<MaoView>("MaoJogador1");
                    mao2.ID = ordem[0];
                    ordemJogada.Add(mao2);
                    ordemJogada.Add(mao);
                    jogadorPosicao = 1;
                    mao2 = GetNode<MaoView>("MaoJogador2");
                    mao2.ID = ordem[2];
                    ordemJogada.Add(mao2);
                    mao2 = GetNode<MaoView>("MaoJogador3");
                    mao2.ID = ordem[3];
                    ordemJogada.Add(mao2);
                }
                else if (ordem[2] == ID)
                {
                    var mao2 = GetNode<MaoView>("MaoJogador1");
                    mao2.ID = ordem[0];
                    ordemJogada.Add(mao2);
                    mao2 = GetNode<MaoView>("MaoJogador2");
                    mao2.ID = ordem[1];
                    ordemJogada.Add(mao2);
                    ordemJogada.Add(mao);
                    jogadorPosicao = 2;
                    mao2 = GetNode<MaoView>("MaoJogador3");
                    mao2.ID = ordem[3];
                    ordemJogada.Add(mao2);
                }
                else if (ordem[3] == ID)
                {
                    var mao2 = GetNode<MaoView>("MaoJogador1");
                    mao2.ID = ordem[0];
                    ordemJogada.Add(mao2);
                    mao2 = GetNode<MaoView>("MaoJogador2");
                    mao2.ID = ordem[1];
                    ordemJogada.Add(mao2);
                    mao2 = GetNode<MaoView>("MaoJogador3");
                    mao2.ID = ordem[2];
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

        Client.On("comecar-jogada", response =>
        {
            GD.Print("Comeca jogada " + response.ToString());
        });

        Client.On("jogada", response =>
        {
            GD.Print("Jogada " + response.ToString());
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
                GD.Print("completo");
                break;
            }
            else
            {
                await Task.Delay(100);
            }
        }
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
        carta c = new carta();
        c._cor = (int)carta.Carta.Cor;
        c._valor = (int)carta.Carta.Valor;
        Jogada jogada = new Jogada();
        jogada.carta = c;
        jogada.jogadorId = jogadorPosicao;
        jogada.sala = NumeroSala;
        string resp = "";
        await Client.EmitAsync("jogada", response =>
        {
            resp = response.ToString();
        }, jogada);
        GD.Print(resp);
        //descarte.addCarta(carta);
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