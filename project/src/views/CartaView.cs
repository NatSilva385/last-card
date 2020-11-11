using Godot;
using System;
using project.src.models;
using System.Collections.Generic;
public class CartaView : Spatial
{

    private COR _corCarta;
    private VALOR _valorCarta;
    private Texture difuse;
    private Texture normal;
    private Color _valorCorCarta;

    private Texture[] _normals;
    private Texture[] _difuses;

    private Carta _carta;

    public Carta Carta
    {
        get
        {
            return _carta;
        }
        set
        {
            _carta = value;
            ValorCarta = Carta.Valor;
            CorCarta = Carta.Cor;

        }
    }


    public COR CorCarta
    {
        get => _corCarta;
        set
        {
            if (ValorCarta == VALOR.CORINGA || ValorCarta == VALOR.CORINGA_MAIS_QUATRO)
            {
                _corCarta = COR.SEMCOR;
            }
            else
            {
                _corCarta = value;
            }

            ValorCorCarta = cores[(int)CorCarta];
        }
    }
    public VALOR ValorCarta
    {
        get => _valorCarta; set
        {
            _valorCarta = value;
            difuse = Difuses[(int)ValorCarta];
            normal = Normals[(int)ValorCarta];
        }
    }

    public Color ValorCorCarta
    {
        get
        {
            return _valorCorCarta;
        }
        set
        {
            _valorCorCarta = value;
            var original = GetNode<MeshInstance>("mesh").GetSurfaceMaterial(0) as ShaderMaterial;
            original.SetShaderParam("carta_cor", ValorCorCarta);
            original.SetShaderParam("carta_d_c", difuse);
            original.SetShaderParam("carta_n", normal);
        }
    }

    public Texture[] Normals { get => _normals; set => _normals = value; }
    public Texture[] Difuses { get => _difuses; set => _difuses = value; }

    public Color[] cores;




    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    public void init(COR corCarta, VALOR valorCarta, Color[] cores, ShaderMaterial frente, Texture[] difuses, Texture[] normals)
    {
        var original = GetNode<MeshInstance>("mesh");
        original.SetSurfaceMaterial(0, frente);
        Difuses = difuses;
        Normals = normals;
        this.cores = cores;
        ValorCarta = valorCarta;
        CorCarta = corCarta;
    }

    public void _on_Area_mouse_entered()
    {
        // GetNode<MeshInstance>("outline").Visible = true;
        GD.Print(GetNode<VisualInstance>("mesh").Layers);
        GetNode<VisualInstance>("mesh").Layers = 3;
    }

    public void _on_Area_mouse_exited()
    {
        // GetNode<MeshInstance>("outline").Visible = false;
        GetNode<VisualInstance>("mesh").Layers = 1;
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
