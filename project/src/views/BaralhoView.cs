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
        var shape = GetChild<Area>(0).GetChild<CollisionShape>(0).Shape as BoxShape;
        GetChild<Area>(0).GetChild<CollisionShape>(0).GlobalTranslate(new Vector3(pos.x, pos.y, pos.z - 1));
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
                        local += new Vector3(0, altura, 0);
                        AddChild(carta);
                        //carta.GlobalTranslate(local);
                        carta.Translation = local;
                        carta.Rotate(Vector3.Right, Mathf.Pi / 2);
                        carta.Rotate(Vector3.Up, Mathf.Pi);
                        cartas.Add(carta);
                        altura += 0.02f;
                        GetChild<Area>(0).GetChild<CollisionShape>(0).Translation = new Vector3(local.x - 0.4f, local.y, local.z);
                        shape.Extents = new Vector3(0.7f, altura / 2, 1);
                        x += 1.4f;
                    }
                }
                y += 2;
                x = 0;
            }

        }


    }

    public void _on_Area_mouse_entered()
    {
        GD.Print("Entrou");
    }

    public void _on_Area_mouse_exited()
    {
        GD.Print("Saiu");
    }
    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
