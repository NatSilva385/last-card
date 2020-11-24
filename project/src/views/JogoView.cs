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

    MaoView adversario;
    List<MaoView> ordemJogada = new List<MaoView>();
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
            GD.Print(response.GetValue(1).ToString());
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
                    var mao2 = GetNode<MaoView>("MaoJogador2");
                    mao2.ID = ordem[1];
                    ordemJogada.Add(mao2);
                }
                else
                {
                    var mao2 = GetNode<MaoView>("MaoJogador2");
                    mao2.ID = ordem[0];
                    ordemJogada.Add(mao2);
                    ordemJogada.Add(mao);
                }
            }
            else
            {
                if (ordem[0] == ID)
                {
                    ordemJogada.Add(mao);
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
                }

            }
            foreach (var jogador in ordemJogada)
            {
                GD.Print(jogador.ID);
                if (jogador.ID != ID)
                {
                    jogador.addCartas(baralho.comprarCartas(listaDeCartas));
                }
            }
            aguardarAnimacaoCompra();
        });

        Client.On("comecar-turno", response =>
        {
            GD.Print(response.ToString());
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
}
