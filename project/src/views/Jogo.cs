using Godot;
using System;
using project.src.controller;
public class Jogo : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public JogoInterface jogo;
    BaralhoView baralho;
    MaoView mao;
    DescarteView descarte;

    public override void _Ready()
    {
        jogo = new JogoController();
        baralho = GetNode<BaralhoView>("Baralho");
        baralho.jogo = this;
        baralho.criacartas();
        mao = GetNode<MaoView>("Mao");
        mao.jogo = this;
        descarte = GetNode<DescarteView>("Descarte");
        //baralho.embaralhar();
    }

    public void animaCarta(CartaView carta)
    {
        var rot = GetNode<Camera>("Camera").RotationDegrees;
        mao.addCarta(carta, rot);
    }

    public bool jogarCarta(CartaView carta)
    {
        if (jogo.podeJogarCarta(carta.Carta))
        {
            mao.removeCarta(carta);
            descarte.addCarta(carta);
            jogo.jogarCarta(carta.Carta);
            return true;
        }
        return false;
    }

    public void liberaCompra()
    {
        baralho.podeClicar();
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
