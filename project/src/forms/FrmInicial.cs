using Godot;
using System;
using SocketIOClient;
using project.src.models;
using System.Net.Http;
using System.Text.Json;
public class FrmInicial : Control
{

    private string numeroDaSala;
    private SocketIO client;

    public Usuario Usuario { get; set; }
    Label lblDerrotas;
    Label lblVitorias;
    Label lblJogador;

    private WindowDialog loadDialog;

    static readonly HttpClient cliente = new HttpClient();
    private string id;
    public override void _Ready()
    {
        loadDialog = GetNode<WindowDialog>("LoadingDialog");
        var dados = GetTree().GetNodesInGroup("dados");
        foreach (var dado in dados)
        {
            if ((dado as Node).Name == "lblDerrotas")
            {
                lblDerrotas = dado as Label;
            }
            else if ((dado as Node).Name == "lblVitorias")
            {
                lblVitorias = dado as Label;
            }
            else if ((dado as Node).Name == "lblJogador")
            {
                lblJogador = dado as Label;
            }
        }
        //Viewport root = GetTree().GetRoot();
        Viewport root = GetNode<Viewport>("/root");
        CurrentScene = root.GetChild(root.GetChildCount() - 1);
        carregaDados();

    }

    private async void carregaDados()
    {
        var response = await cliente.GetAsync($"http://localhost:3000/{Usuario.id}");

        string message = await response.Content.ReadAsStringAsync();

        Usuario = JsonSerializer.Deserialize<Usuario>(message);
        lblDerrotas.Text = Usuario.experiencia.ToString();
        lblVitorias.Text = Usuario.nivel.ToString();
        lblJogador.Text = Usuario.nUsuario;
    }

    public Node CurrentScene { get; set; }

    private async void _on_btnBuscarDois_pressed()
    {
        client = new SocketIO("http://localhost:3000/");
        client.On("sala-numero", (response) =>
        {

            numeroDaSala = response.GetValue<string>();
            id = response.GetValue<string>(1);
        });

        client.On("carrega-jogo", response =>
        {
            GD.Print("Começar a carregar");
            DeferredGotoScene();
        });

        await client.ConnectAsync();

        NovoJogoReq newJogo = new NovoJogoReq();

        newJogo.NomeJogador = Usuario.nUsuario;
        newJogo.Id = Usuario.id;
        newJogo.QtdeJogadores = 2;

        await client.EmitAsync("Novo-Jogador", newJogo);
        loadDialog.Visible = true;
    }

    public void DeferredGotoScene()
    {
        // It is now safe to remove the current scene


        // Load a new scene.
        var nextScene = ResourceLoader.Load<PackedScene>("res://scene/Main.tscn");

        // Instance the new scene.
        var jogoView = nextScene.Instance() as JogoView;
        jogoView.Client = client;
        jogoView.NumeroSala = numeroDaSala;
        jogoView.ID = id;
        jogoView.Usuario = Usuario;
        // Add it to the active scene, as child of root.
        GetNode("/root").AddChild(jogoView);

        CurrentScene.Free();
        CurrentScene = jogoView;

    }


}