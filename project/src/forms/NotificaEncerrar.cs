using Godot;
using System;
using System.Threading.Tasks;

public class NotificaEncerrar : Control
{
    Label lblMensagem;
    PopupDialog areaMensagem;
    bool encerra;
    public override void _Ready()
    {
        var area = GetTree().GetNodesInGroup("encerrar");
        foreach (var a in area)
        {
            if ((a as Node).Name == "lblMensagem")
            {
                lblMensagem = a as Label;
            }
        }
        areaMensagem = GetChild<PopupDialog>(0);
    }

    public async Task<bool> exibeMensagem(bool ganhou)
    {
        this.Visible = true;
        areaMensagem.Visible = true;
        if (ganhou)
        {
            lblMensagem.Text = "Parabens voce ganhou!!!";
        }
        else
        {
            lblMensagem.Text = "Você perdeu, mais sorte na próxima vez";
        }
        encerra = false;
        while (true)
        {
            if (encerra)
            {
                break;
            }
            else
            {
                await Task.Delay(200);
            }
        }

        return true;
    }

    private void _on_btnFinaliar_pressed()
    {
        encerra = true;
    }

}
