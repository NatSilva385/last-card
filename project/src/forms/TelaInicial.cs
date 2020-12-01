using Godot;
using System;
using SocketIOClient;
public class TelaInicial : Control
{
    SocketIO client;

    FrmChat chat;
    public override void _Ready()
    {
        chat = GetNode<FrmChat>("ChatWindow");

    }

    private async void _on_btnConectar_pressed()
    {
        client = new SocketIO("http://localhost:3000/");
        client.On("chat-message", response =>
        {
            MensagemRecebida mensagem = response.GetValue<MensagemRecebida>();
            GD.Print($"{mensagem.Menssagem}");
            chat.recebeMensagem(mensagem);
        });
        client.On("room-number", response =>
        {
            chat.Room = response.GetValue<string>();
            GD.Print(chat.Room);
        });
        await client.ConnectAsync();

        await client.EmitAsync("new-user", "");
        chat.Visible = true;
        chat.Client = client;
    }

    private async void _on_btnJogar_pressed()
    {
        client = new SocketIO("http://localhost:3000/");
        client.On("room-number", response =>
       {
           chat.Room = response.GetValue<string>();
           GD.Print(chat.Room);
       });
        await client.ConnectAsync();
        await client.ConnectAsync();

        await client.EmitAsync("new-user", "");
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
