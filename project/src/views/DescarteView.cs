using Godot;
using System;
using System.Collections.Generic;
using project.src.models;
public class DescarteView : Spatial
{

    List<CartaView> cartas = new List<CartaView>();
    private Jogo _jogo;

    public Jogo Jogo { get => _jogo; set => _jogo = value; }

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
        posIni = new Vector3(posIni.x, posIni.y + (cartas.Count * 0.02f), posIni.z);
        var tween = GetNode<Tween>("Tween");
        tween.InterpolateProperty(carta, "translation", carta.Translation, posIni, 0.5f, Tween.TransitionType.Quart, Tween.EaseType.Out);
        tween.InterpolateProperty(carta, "rotation_degrees", carta.RotationDegrees, rot, 0.5f, Tween.TransitionType.Linear, Tween.EaseType.InOut);
        tween.Start();
        cartas.Add(carta);
    }

    private void _on_Tween_tween_completed(Godot.Object @object, NodePath key)
    {
        if (@object is CartaView)
        {
            if ((@object as CartaView).Carta.Cor == COR.SEMCOR)
            {
                Jogo.habilitarEscolhaCor(@object as CartaView);
            }
        }
    }

    public void mudaCorCarta(COR cor)
    {
        var last = cartas.Count - 1;
        var ultimaCarta = cartas[last];
        ultimaCarta.CorCarta = cor;
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
