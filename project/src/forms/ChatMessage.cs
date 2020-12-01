using Godot;
using System;

public class ChatMessage : Control
{
    public string NomeClient
    {
        get { return lblNome.Text; }
        set { lblNome.Text = value; }
    }

    public string Mensagem
    {
        get { return lblMensagem.Text; }

        set { lblMensagem.Text = value; }
    }

    Label lblNome;
    Label lblMensagem;
    public override void _Ready()
    {
        var tools = GetTree().GetNodesInGroup("tool");
        foreach (var tool in tools)
        {
            if ((tool as Node).Name == "lblNome")
            {
                lblNome = tool as Label;
            }
            else if ((tool as Node).Name == "lblMessage")
            {
                lblMensagem = tool as Label;
            }
        }
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
