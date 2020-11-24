using Godot;
using System;
using project.src.models;
using System.Collections.Generic;

public class CartaView : Spatial
{
    private Carta _carta;
    public Carta Carta
    {
        get { return _carta; }
        set
        {
            _carta = value;
            Frente.SetShaderParam("carta_cor", Cores[(int)Carta.Cor]);
            Frente.SetShaderParam("carta_d_c", Difuses[(int)Carta.Valor]);
            Frente.SetShaderParam("carta_n", Normals[(int)Carta.Valor]);
        }
    }

    public Texture[] Difuses { get; set; }

    public Texture[] Normals { get; set; }

    public ShaderMaterial Frente
    {
        get
        {
            return GetNode<MeshInstance>("mesh").GetSurfaceMaterial(0) as ShaderMaterial;
        }
        set
        {
            GetNode<MeshInstance>("mesh").SetSurfaceMaterial(0, value);
        }
    }

    public Color[] Cores { get; set; }

    public void init(Texture[] normals, Texture[] difuses, Color[] cores, ShaderMaterial frente)
    {
        Normals = normals;
        Difuses = difuses;
        Cores = cores;
        Frente = frente;

    }

    public override void _Ready()
    {

    }

    private void _on_Area_mouse_entered()
    {
        GetNode<VisualInstance>("mesh").Layers = 3;
    }

    private void _on_Area_mouse_exited()
    {
        GetNode<VisualInstance>("mesh").Layers = 1;
    }

}
