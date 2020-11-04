using Godot;
using System;
using project.src.models;
using System.Collections.Generic;
using System.Text.Json;
[Tool]
public class CartaView : Spatial
{
	[Export]
	public COR Cor
	{
		get
		{
			return _cor;
		}
		set
		{
			_cor = value;
			GD.Print(listaCores);
			if (listaCores == null)
			{
				listaCores = new List<CartaCor>();
			}
			GD.Print(listaCores.Count);
			int i = 0;
			for (i = 0; i < listaCores.Count; i++)
			{
				if (listaCores[i].CorCarta == Cor)
				{
					CorFrente = new Color(listaCores[i].CorShader);
					GD.Print(new Color(listaCores[i].CorShader));
					break;
				}
			}
			PropertyListChangedNotify();
		}
	}

	private COR _cor;

	private bool _salvarParametrosShaderFrente;

	[Export]
	public bool SalvarParametrosShaderFrente
	{
		get
		{
			return _salvarParametrosShaderFrente;
		}
		set
		{
			_salvarParametrosShaderFrente = false;
			salvarShaderParametros();
		}

	}
	private bool _resetarParametrosShaderFrente;

	[Export]
	public bool ResetarParametrosShaderFrente
	{
		get
		{
			return _resetarParametrosShaderFrente;
		}
		set
		{
			_resetarParametrosShaderFrente = false;
			listaCores = new List<CartaCor>();
		}

	}

	private List<CartaCor> listaCores = new List<CartaCor>();

	private Color _corFrente;

	[Export]
	public Color CorFrente
	{
		get
		{
			return _corFrente;
		}
		set
		{
			_corFrente = value;
			mudaCorShaderFrente();
			if (listaCores.Count == 0)
			{
				CartaCor carta = new CartaCor();
				carta.CorCarta = Cor;
				carta.CorShader = CorFrente.ToRgba64();
				listaCores.Add(carta);
			}
			else
			{
				bool achaCor = false;
				int i = 0;
				for (i = 0; i < listaCores.Count; i++)
				{
					if (listaCores[i].CorCarta == Cor)
					{
						listaCores[i].CorShader = CorFrente.ToRgba64();
						achaCor = true;
						break;
					}
				}
				if (achaCor == false)
				{
					CartaCor carta = new CartaCor();
					carta.CorCarta = Cor;
					carta.CorShader = CorFrente.ToRgba64();
					listaCores.Add(carta);
				}
			}
		}
	}


	private SHADER_CARTA_FRENTE _CARTA_FRENTE;
	[Export]
	public SHADER_CARTA_FRENTE CARTA_FRENTE
	{
		get
		{
			return _CARTA_FRENTE;
		}
		set
		{
			_CARTA_FRENTE = value;
			mudaShaderFrente();
			carregarShaderParametros();
		}
	}
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	private void trocaCor()
	{
		Material material = ResourceLoader.Load<Material>("res://assets/material/carta-frente-semcor.tres");
		if (Cor == COR.AMARELO)
		{
			material = ResourceLoader.Load<Material>("res://assets/material/carta-frente-amarelo.tres");
		}
		else if (Cor == COR.VERMELHO)
		{
			material = ResourceLoader.Load<Material>("res://assets/material/carta-frente-vermelho.tres");
		}
		else if (Cor == COR.VERDE)
		{
			material = ResourceLoader.Load<Material>("res://assets/material/carta-frente-verde.tres");
		}
		else if (Cor == COR.AZUL)
		{
			material = ResourceLoader.Load<Material>("res://assets/material/carta-frente-azul.tres");
		}

		MeshInstance mesh = GetChild<MeshInstance>(0);
		mesh.SetSurfaceMaterial(0, material);

	}

	private void mudaShaderFrente()
	{
		Material mat = ResourceLoader.Load<Material>($"res://assets/material/carta-frente-{Enum.GetName(typeof(SHADER_CARTA_FRENTE), CARTA_FRENTE).ToLower()}.tres");

		MeshInstance mesh = GetChild<MeshInstance>(0);
		mesh.SetSurfaceMaterial(0, mat);
	}

	private void mudaCorShaderFrente()
	{
		ShaderMaterial shaderMaterial = GetChild<MeshInstance>(0).GetSurfaceMaterial(0) as ShaderMaterial;
		shaderMaterial.SetShaderParam("carta_cor", CorFrente);
	}
	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//      
	//  }

	private void salvarShaderParametros()
	{
		string json;
		json = JsonSerializer.Serialize(listaCores);
		File parametros = new File();
		parametros.Open($"res://assets/material/carta-frente-{Enum.GetName(typeof(SHADER_CARTA_FRENTE), CARTA_FRENTE).ToLower()}.json", File.ModeFlags.Write);
		parametros.StoreString(json);
		parametros.Close();
	}

	private void carregarShaderParametros()
	{
		File parametros = new File();
		parametros.Open($"res://assets/material/carta-frente-{Enum.GetName(typeof(SHADER_CARTA_FRENTE), CARTA_FRENTE).ToLower()}.json", File.ModeFlags.Read);
		string conteudo = parametros.GetAsText();
		parametros.Close();
		if (String.IsNullOrEmpty(conteudo))
		{
			GD.Print("aqui");
			return;
		}
		listaCores = JsonSerializer.Deserialize<List<CartaCor>>(conteudo);

		for (int i = 0; i < listaCores.Count; i++)
		{
			if (listaCores[i].CorCarta == Cor)
			{
				CorFrente = new Color(listaCores[i].CorShader);
			}
		}
		PropertyListChangedNotify();
	}
}
