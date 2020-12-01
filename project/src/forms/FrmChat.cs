using Godot;
using System;
using SocketIOClient;
public class FrmChat : WindowDialog
{

    LineEdit txtMensage;
    VBoxContainer chatContainer;

    private string mensagemDigitada;

    Button btnEnviar;
    public SocketIO Client { get; set; }

    public string Room { get; set; }

    PackedScene chatMessage;

    public override void _Ready()
    {
        var tools = GetTree().GetNodesInGroup("tools");
        foreach (var tool in tools)
        {
            if ((tool as Node).Name == "txtMessage")
            {
                txtMensage = tool as LineEdit;
            }
            else if ((tool as Node).Name == "chatContainer")
            {
                chatContainer = tool as VBoxContainer;
            }
            else if ((tool as Node).Name == "btnEnviar")
            {
                btnEnviar = tool as Button;
            }
        }

        chatMessage = ResourceLoader.Load<PackedScene>("res://scene/Chat/ChatMessage.tscn");
    }

    private async void _on_btnEnviar_pressed()
    {
        var mensagem = new MensagemEnviada()
        {
            Messagem = mensagemDigitada,
            Room = Room
        };


        recebeMensagem(new MensagemRecebida()
        {
            Menssagem = mensagemDigitada,
            Nome = "Você"
        });
        txtMensage.Text = "";
        mensagemDigitada = "";
        await Client.EmitAsync("chat-message", mensagem);
    }

    public void recebeMensagem(MensagemRecebida mensagem)
    {
        ChatMessage chat = chatMessage.Instance() as ChatMessage;
        HBoxContainer row = new HBoxContainer();

        Label nome = new Label();
        nome.Text = mensagem.Nome;
        row.AddChild(nome);

        Label msg = new Label();
        msg.Text = mensagem.Menssagem;
        row.AddChild(msg);
        /*chatContainer.AddChild(chat);
        chat.NomeClient = mensagem.Nome;
        chat.Mensagem = mensagem.Menssagem;*/

        chatContainer.AddChild(row);


    }

    private void _on_txtMessage_text_changed(string new_text)
    {
        mensagemDigitada = new_text;
    }
}

public class MensagemEnviada
{
    public string Room { get; set; }

    public string Messagem { get; set; }
}

public class MensagemRecebida
{
    public string Nome { get; set; }
    public string Menssagem { get; set; }
}

