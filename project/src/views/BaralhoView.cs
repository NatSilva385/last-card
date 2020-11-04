using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;
using project.src.models;
public class BaralhoView : Spatial
{

    //private List<CartaView> cartas = new List<CartaView>();

    // Called when the node enters the scene tree for the first time.
    List<CartaView> cartas = new List<CartaView>();

    public override void _Ready()
    {
        criacartas();
    }

    private void criacartas()
    {
        var cartaScene = ResourceLoader.Load<PackedScene>("res://scene/Carta.tscn");

        File file = new File();
        file.Open("res://assets/material/carta-frente-padrao.json", File.ModeFlags.Read);
        var content = file.GetAsText();
        file.Close();
        var shaderCartaFrente = ResourceLoader.Load("res://assets/material/carta-frente-padrao.tres") as ShaderMaterial;
        List<CartaCor> val = JsonSerializer.Deserialize<List<CartaCor>>(content);
        Color[] cores = new Color[5];
        var pos = this.Translation;
        float altura = 0;
        float x = 0, y = 0;
        for (int i = 0; i < 5; i++)
        {
            cores[(int)val[i].CorCarta] = new Color(val[i].CorShader);


        }

        foreach (COR c in Enum.GetValues(typeof(COR)))
        {
            if (c != COR.SEMCOR)
            {
                foreach (VALOR valor in Enum.GetValues(typeof(VALOR)))
                {
                    if (valor != VALOR.SEMVALOR)
                    {
                        var carta = cartaScene.Instance() as CartaView;
                        var shader = shaderCartaFrente.Duplicate(true) as ShaderMaterial;
                        carta.init(c, valor, cores, shader);
                        var local = pos;
                        local += new Vector3(x, y, 0);
                        //carta.GlobalTranslate(local);
                        carta.Translation = local;
                        //carta.Rotate(Vector3.Right, Mathf.Pi / 2);
                        //carta.Rotate(Vector3.Up, Mathf.Pi);
                        cartas.Add(carta);
                        AddChild(carta);
                        altura += 2f;
                        x += 1.4f;
                    }
                }
                y += 2;
                x = 0;
            }

        }


    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
