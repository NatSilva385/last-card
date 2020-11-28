using Godot;
using System;

public class CorSelecaoView : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }
    private void _on_Area_mouse_entered()
    {
        GetChild<VisualInstance>(0).Layers = 3;
    }

    private void _on_Area_mouse_exited()
    {
        GetChild<VisualInstance>(0).Layers = 1;
    }

    private void _on_Area_input_event(object camera, object @event, Vector3 click_position, Vector3 click_normal, int shape_idx)
    {
        if (@event is InputEventMouse e)
        {
            if (e.ButtonMask == (int)ButtonList.Left)
            {
                var jogo = GetParent().GetParent<JogoView>();
                if (jogo != null)
                {
                    if (this.Name == "cor_vermelho")
                    {
                        jogo.escolherCor(project.src.models.COR.VERMELHO);
                    }
                    else if (this.Name == "cor_azul")
                    {
                        jogo.escolherCor(project.src.models.COR.AZUL);
                    }
                    else if (this.Name == "cor_amarelo")
                    {
                        jogo.escolherCor(project.src.models.COR.AMARELO);
                    }
                    else if (this.Name == "cor_verde")
                    {
                        jogo.escolherCor(project.src.models.COR.VERDE);
                    }
                }
            }
        }
    }
}
