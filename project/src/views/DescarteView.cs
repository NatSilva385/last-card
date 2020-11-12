using Godot;
using System;
using System.Collections.Generic;
public class DescarteView : Spatial
{

    List<CartaView> cartas = new List<CartaView>();

    public override void _Ready()
    {

    }

    public void addCarta(CartaView carta)
    {
        var posIni = this.Translation;
        var rot = new Vector3(-90, 0, 0);
        posIni = new Vector3(posIni.x, posIni.y + (cartas.Count * 0.02f), posIni.z);
        var tween = GetNode<Tween>("Tween");
        tween.InterpolateProperty(carta, "translation", carta.Translation, posIni, 0.5f, Tween.TransitionType.Quart, Tween.EaseType.Out);
        tween.InterpolateProperty(carta, "rotation_degrees", carta.RotationDegrees, rot, 0.5f, Tween.TransitionType.Linear, Tween.EaseType.InOut);
        tween.Start();
        cartas.Add(carta);
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
