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
    public Jogo jogo;

    [Export]
    public float offsetBox = 0;
    private bool _podeClicar = true;

    public void podeClicar()
    {
        _podeClicar = true;
    }

    public override void _Ready()
    {
        //criacartas();
    }

    public void embaralhar()
    {
        var rnd = new RandomNumberGenerator();
        for (int i = 0; i < cartas.Count; i++)
        {
            int x = rnd.RandiRange(0, i);
            CartaView aux = cartas[i];
            cartas[i] = cartas[x];
            cartas[x] = aux;
        }
    }

    public void criacartas()
    {
        var cartaScene = ResourceLoader.Load<PackedScene>("res://scene/Carta.tscn");

        File file = new File();
        file.Open("res://assets/material/carta-frente-padrao.json", File.ModeFlags.Read);
        var content = file.GetAsText();
        file.Close();
        var shaderCartaFrente = ResourceLoader.Load("res://assets/material/carta-frente-padrao.tres") as ShaderMaterial;
        List<CartaCor> val = JsonSerializer.Deserialize<List<CartaCor>>(content);
        Color[] cores = new Color[5];
        Texture[] difuses = new Texture[Enum.GetNames(typeof(VALOR)).Length];
        Texture[] normals = new Texture[Enum.GetNames(typeof(VALOR)).Length];
        foreach (VALOR valor in Enum.GetValues(typeof(VALOR)))
        {
            if (valor != VALOR.SEMVALOR)
            {
                difuses[(int)valor] = ResourceLoader.Load<Texture>($"res://assets/texture/numeros/padrao/{Enum.GetName(typeof(VALOR), valor).ToLower()}_d_c.png");
                normals[(int)valor] = ResourceLoader.Load<Texture>($"res://assets/texture/numeros/padrao/{Enum.GetName(typeof(VALOR), valor).ToLower()}_n.png");
            }
        }

        var pos = Translation;
        GD.Print(pos);
        var shape = GetChild<Area>(0).GetChild<CollisionShape>(0).Shape as BoxShape;
        //GetChild<Area>(0).GetChild<CollisionShape>(0).GlobalTranslate(new Vector3(pos.x, pos.y, pos.z - 1));
        var posShape = GetChild<Area>(0).GetChild<CollisionShape>(0).Transform.origin;
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
                        carta.init(c, valor, cores, shader, difuses, normals);
                        carta.Jogo = jogo;
                        var local = pos;
                        local += new Vector3(0, altura, 0);
                        jogo.AddChild(carta);
                        //AddChild(carta);
                        //carta.GlobalTranslate(local);
                        carta.Translation = local;
                        carta.Rotate(Vector3.Right, Mathf.Pi / 2);
                        carta.Rotate(Vector3.Up, Mathf.Pi);
                        cartas.Add(carta);
                        altura += 0.02f;
                        shape.Extents = new Vector3(0.7f, altura / 2, 1);
                        GetChild<Area>(0).GetChild<CollisionShape>(0).Translation = new Vector3(0 + offsetBox, altura, 0);
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

    private void _on_Area_input_event(object camera, object @event, Vector3 click_position, Vector3 click_normal, int shape_idx)
    {
        if (@event is InputEventMouse e)
        {
            if (e.ButtonMask == (int)ButtonList.Left)
            {
                if (_podeClicar)
                {
                    int last = cartas.Count;
                    last--;
                    if (last >= 0)
                    {
                        var r = cartas[last];
                        GD.Print("" + r.CorCarta + " " + r.ValorCarta);
                        r.Carta = jogo.jogo.comprarUmaCarta();
                        GD.Print("" + r.Carta.Cor + " " + r.Carta.Valor);
                        //GD.Print(r.Translation);
                        _podeClicar = false;
                        cartas.Remove(r);
                        jogo.animaCarta(r);
                        //RemoveChild(r);
                    }
                }

            }
        }
    }
    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
