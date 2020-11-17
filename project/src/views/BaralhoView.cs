using Godot;
using System;
using System.Collections.Generic;
using project.src.models;

public class BaralhoView : Spatial
{
    private List<CartaView> baralhoCartas = new List<CartaView>();
    public JogoView Jogo { get; set; }

    public void criarCartas()
    {
        PackedScene cenaOriginal = ResourceLoader.Load<PackedScene>("res://scene/Carta.tscn");
        GD.Print(cenaOriginal);
        float x = 0, y = 0;
        foreach (COR cor in Enum.GetValues(typeof(COR)))
        {
            if (cor != COR.SEMCOR)
            {
                foreach (VALOR valor in Enum.GetValues(typeof(VALOR)))
                {
                    if (valor != VALOR.SEM_VALOR)
                    {
                        CartaView carta = cenaOriginal.Instance() as CartaView;
                        carta.Translation = new Vector3(x, y, 0);
                        x = x + 1.4f;
                        baralhoCartas.Add(carta);
                        Jogo.AddChild(carta);
                    }
                }

            }
            x = 0;
            y = y + 2;
        }
    }
    public override void _Ready()
    {

    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
