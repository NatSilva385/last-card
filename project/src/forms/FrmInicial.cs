using Godot;
using System;
using SocketIOClient;
using project.src.models;
public class FrmInicial : Control
{

    private string numeroDaSala;
    private SocketIO client;

    private WindowDialog loadDialog;
    public override void _Ready()
    {
        loadDialog = GetNode<WindowDialog>("LoadingDialog");
    }


    private async void _on_btnBuscarDois_pressed()
    {
        client = new SocketIO("http://localhost:3000/");

        client.On("sala-numero", (response) =>
        {

            numeroDaSala = response.GetValue<string>();
        });

        client.On("carrega-jogo", response =>
        {
            GD.Print("Começar a carregar");
        });

        await client.ConnectAsync();

        NovoJogoReq newJogo = new NovoJogoReq();

        newJogo.NomeJogador = "";
        newJogo.QtdeJogadores = 2;

        await client.EmitAsync("Novo-Jogador", newJogo);
        loadDialog.Visible = true;
    }


}