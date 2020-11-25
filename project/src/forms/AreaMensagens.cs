using Godot;
using System;
using System.Threading.Tasks;
public class AreaMensagens : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.

    Label lblMensagem;
    public override void _Ready()
    {
        var area = GetTree().GetNodesInGroup("mensagem");
        foreach (var a in area)
        {
            if ((a as Node).Name == "lblMensagem")
            {
                lblMensagem = a as Label;
            }
        }
        lblMensagem.Visible = false;
    }

    public void mostraMensage(string mensagem)
    {
        mostraMensage(mensagem, 2000);
    }

    public async void mostraMensage(string mensagem, int delay)
    {
        lblMensagem.Text = mensagem;
        lblMensagem.Visible = true;
        await Task.Delay(delay);
        lblMensagem.Visible = false;
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
