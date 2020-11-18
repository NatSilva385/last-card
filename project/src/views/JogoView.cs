using Godot;
using System;

public class JogoView : Spatial
{
    BaralhoView baralho;
    MaoView mao;
    public override void _Ready()
    {
        baralho = GetNode<BaralhoView>("Baralho");
        baralho.Jogo = this;
        baralho.criarCartas();
        mao = GetNode<MaoView>("Mao");
        mao.Jogo = this;
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
