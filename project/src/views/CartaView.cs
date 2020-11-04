using Godot;
using System;
using project.src.models;
public class CartaView : Spatial
{

    private COR _corCarta;
    private VALOR _valorCarta;
    private Texture difuse;
    private Texture normal;
    private Color _valorCorCarta;

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
            difuse = ResourceLoader.Load<Texture>($"res://assets/texture/numeros/padrao/{Enum.GetName(typeof(VALOR), ValorCarta).ToLower()}_d_c.png");
            normal = ResourceLoader.Load<Texture>($"res://assets/texture/numeros/padrao/{Enum.GetName(typeof(VALOR), ValorCarta).ToLower()}_n.png");
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

    public Color[] cores;




    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    public void init(COR corCarta, VALOR valorCarta, Color[] cores, ShaderMaterial frente)
    {
        var original = GetNode<MeshInstance>("mesh");
        original.SetSurfaceMaterial(0, frente);
        this.cores = cores;
        ValorCarta = valorCarta;
        CorCarta = corCarta;
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
