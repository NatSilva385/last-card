using Godot;
using System;
using System.Collections.Generic;

public class MaoView : Spatial
{
    [Export]
    public float larguraCarta = 1.4f;

    [Export]
    public float maximoMao = 9;

    [Export]
    public float distanciaCartas = 0.001f;

    public Jogo jogo;
    List<CartaView> cartas = new List<CartaView>();
    public override void _Ready()
    {

    }

    public void addCarta(CartaView carta, Vector3 rot)
    {
        cartas.Add(carta);
        var novasPosicoes = calculaPosicoes();
        var tween = GetNode("Tween") as Tween;
        var animacoes = new List<Tween>();
        for (int i = 0; i < cartas.Count; i++)
        {
            tween.InterpolateProperty(cartas[i], "translation", cartas[i].Translation, novasPosicoes[i], 0.5f, Tween.TransitionType.Sine, Tween.EaseType.InOut);
            tween.InterpolateProperty(cartas[i], "rotation_degrees", cartas[i].RotationDegrees, rot, 0.5f, Tween.TransitionType.Sine, Tween.EaseType.InOut);
        }

        tween.Start();
    }

    private void ordenaCartas()
    {
        var novasPosicoes = calculaPosicoes();
        var tween = GetNode("Tween") as Tween;
        var animacoes = new List<Tween>();
        for (int i = 0; i < cartas.Count; i++)
        {
            tween.InterpolateProperty(cartas[i], "translation", cartas[i].Translation, novasPosicoes[i], 0.5f, Tween.TransitionType.Sine, Tween.EaseType.InOut);
        }

        tween.Start();
    }

    private List<Vector3> calculaPosicoes()
    {
        var novasPosicoes = new List<Vector3>();
        var posInicial = this.Translation;
        float larguraTemporaria = larguraCarta, nX;
        GD.Print(posInicial);

        if (cartas.Count == 0)
        {
            novasPosicoes.Add(Vector3.Zero);
            return novasPosicoes;
        }

        if (cartas.Count < maximoMao)
        {
            nX = ((cartas.Count * larguraTemporaria) / 2) * -1;
            nX += larguraTemporaria / 2;
        }
        else
        {
            nX = ((maximoMao * larguraTemporaria) / 2) * -1;
            nX += larguraTemporaria / 2;
            larguraTemporaria = (nX * 2 * -1) / (cartas.Count - 1);
        }
        float dz = 0;
        foreach (var carta in cartas)
        {
            var p = posInicial;
            p += new Vector3(nX, 0, dz);
            novasPosicoes.Add(p);
            nX += larguraTemporaria;
            dz += distanciaCartas;
        }

        return novasPosicoes;

    }

    private void _on_Tween_tween_completed(Godot.Object @object, NodePath key)
    {
        // Replace with function body.
        jogo.liberaCompra();
    }

    public void removeCarta(CartaView carta)
    {
        cartas.Remove(carta);
        ordenaCartas();
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
