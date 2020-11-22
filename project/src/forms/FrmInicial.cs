using Godot;
using System;
using SocketIOClient;
public class FrmInicial : Control
{

    private SocketIO client;

    private WindowDialog loadDialog;
    public override void _Ready()
    {
        loadDialog = GetNode<WindowDialog>("LoadingDialog");
    }

    private async void _on_btnBuscarDois_pressed()
    {
        client = new SocketIO("http://localhost:3000/");

        await client.ConnectAsync();

        await client.EmitAsync("Novo-Jogador", "");
        loadDialog.Visible = true;
    }


}