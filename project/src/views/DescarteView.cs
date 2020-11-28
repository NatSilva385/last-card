using Godot;
using System;
using System.Collections.Generic;
using project.src.models;
/// <summary>
/// Armazena as cartas usadas pelos jogadores durante a partida
/// </summary>
public class DescarteView : Spatial
{
    /// <summary>
    /// Referencia ao jogo que está sendo jogado
    /// </summary>
    /// <value>Jogo</value>
    public JogoView Jogo { get; set; }

    /// <summary>
    /// Armazena todas as cartas que foram jogadas durante a partida
    /// </summary>
    /// <typeparam name="CartaView"></typeparam>
    /// <returns></returns>
    private List<CartaView> cartasDescartadas = new List<CartaView>();

    private bool moveCartasBaralho = false;

    public BaralhoView Baralho { get; set; }
    public override void _Ready()
    {

    }

    public Carta ultimaCarta()
    {
        if (cartasDescartadas.Count == 0)
        {
            return null;
        }
        int ultimoIndice = cartasDescartadas.Count - 1;
        CartaView carta = cartasDescartadas[ultimoIndice];
        return carta.Carta;
    }

    public CartaView ultimaCartaView()
    {
        if (cartasDescartadas.Count == 0)
        {
            return null;
        }
        int ultimoIndice = cartasDescartadas.Count - 1;
        CartaView carta = cartasDescartadas[ultimoIndice];
        return carta;
    }
    public void addCarta(CartaView carta)
    {
        GD.Print("Carta Descartada");
        var posIni = this.Translation;

        var r = new RandomNumberGenerator();
        r.Randomize();
        var rRot = r.RandfRange(-20, 20);
        var rot = new Vector3(-90, rRot, 0);
        posIni = new Vector3(posIni.x, posIni.y + (cartasDescartadas.Count * 0.02f), posIni.z);
        var tween = GetNode<Tween>("Tween");
        tween.InterpolateProperty(carta, "translation", carta.Translation, posIni, 0.5f, Tween.TransitionType.Quart, Tween.EaseType.Out);
        tween.InterpolateProperty(carta, "rotation_degrees", carta.RotationDegrees, rot, 0.5f, Tween.TransitionType.Linear, Tween.EaseType.InOut);
        tween.Start();
        cartasDescartadas.Add(carta);
    }

    public void moverCartasBaralho()
    {
        int ultimoIndice = cartasDescartadas.Count - 1;
        CartaView ultimaCarta = cartasDescartadas[ultimoIndice];
        cartasDescartadas.Remove(ultimaCarta);
        int tamanho = cartasDescartadas.Count;
        var tween = GetNode<Tween>("Tween");
        var rot = new Vector3(90, 0, 180);

        float delay = 0;
        for (int i = tamanho - 1; i >= 0; i--)
        {
            CartaView tmp = cartasDescartadas[i];
            cartasDescartadas.Remove(tmp);
            tween.InterpolateProperty(tmp, "translation", tmp.Translation, Baralho.Translation, 0.5f, Tween.TransitionType.Quart, Tween.EaseType.InOut, delay);
            tween.InterpolateProperty(tmp, "rotation_degrees", tmp.RotationDegrees, rot, 0.5f, Tween.TransitionType.Linear, Tween.EaseType.InOut, delay);
            delay += 0.08f;
            Baralho.addCarta(tmp);
        }
        moveCartasBaralho = true;
        tween.Start();
    }



    private void _on_Tween_tween_all_completed()
    {
        if (moveCartasBaralho)
        {
            moveCartasBaralho = false;
            Jogo.terminarEncherBaralho();
        }
        else
        {
            Jogo.terminouAnimacaoJogada();
        }

    }

}
