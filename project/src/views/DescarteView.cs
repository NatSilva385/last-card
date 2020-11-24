using Godot;
using System;
using System.Collections.Generic;
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
    public override void _Ready()
    {

    }

    public void addCarta(CartaView carta)
    {

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

}
