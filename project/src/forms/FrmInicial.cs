using Godot;
using System;
using SocketIOClient;
using project.src.models;
public class FrmInicial : Control
{

    private string numeroDaSala;
    private SocketIO client;

    private WindowDialog loadDialog;
    private string id;
    public override void _Ready()
    {
        loadDialog = GetNode<WindowDialog>("LoadingDialog");
        //Viewport root = GetTree().GetRoot();
        Viewport root = GetNode<Viewport>("/root");
        CurrentScene = root.GetChild(root.GetChildCount() - 1);
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

        newJogo.NomeJogador = "";
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
        // Add it to the active scene, as child of root.
        GetNode("/root").AddChild(jogoView);
        //GetTree().GetRoot().AddChild(jogoView);

        // Optionally, to make it compatible with the SceneTree.change_scene() API.
        CurrentScene.Free();
        CurrentScene = jogoView;
        //GetTree().SetCurrentScene(jogoView);

    }


}