using Godot;
using System;
using System.Collections.Generic;
using project.src.models;
using project.src.factory;
public class BaralhoView : Spatial
{
    private List<CartaView> baralhoCartas = new List<CartaView>();
    public JogoView Jogo { get; set; }
    [Export]
    public float altura = 0.02f;

    public void criarCartas()
    {
        PackedScene cenaOriginal = ResourceLoader.Load<PackedScene>("res://scene/Carta.tscn");
        CartaFactory cartaOriginal = new CartaFactory();
        float y = 0;
        Vector3 posicao = this.Translation;
        foreach (COR cor in Enum.GetValues(typeof(COR)))
        {
            if (cor != COR.SEMCOR)
            {
                foreach (VALOR valor in Enum.GetValues(typeof(VALOR)))
                {
                    if (valor != VALOR.SEM_VALOR)
                    {
                        Carta carta1 = new Carta();
                        carta1.Cor = cor;
                        carta1.Valor = valor;


                        CartaView carta = cartaOriginal.carregaCarta(carta1);
                        carta.Translation = new Vector3(posicao.x, posicao.y + y, posicao.z);
                        carta.Rotate(Vector3.Right, Mathf.Pi / 2);
                        carta.Rotate(Vector3.Up, Mathf.Pi);
                        y = y + altura;
                        carta.Jogo = Jogo;
                        baralhoCartas.Add(carta);
                        Jogo.AddChild(carta);
                    }
                }

            }

        }
    }
    public override void _Ready()
    {

    }

    public int tamanho()
    {
        return baralhoCartas.Count;
    }
    private void _on_Area_input_event(object camera, object @event, Vector3 click_position, Vector3 click_normal, int shape_idx)
    {
        if (@event is InputEventMouse e)
        {
            if (e.ButtonMask == (int)ButtonList.Left)
            {
                if (baralhoCartas.Count == 0)
                {
                    return;
                }
                int ultima = baralhoCartas.Count - 1;
                CartaView ultimaCarta = baralhoCartas[ultima];
                Jogo.comprarCarta(ultimaCarta);
                baralhoCartas.Remove(ultimaCarta);
            }
        }
    }

    public List<CartaView> comprarCartas(List<Carta> cartas)
    {
        List<CartaView> cards = new List<CartaView>();

        for (int i = 0; i < cartas.Count; i++)
        {
            int ultima = baralhoCartas.Count - 1;
            CartaView ultimaCarta = baralhoCartas[ultima];
            ultimaCarta.Carta = cartas[i];
            cards.Add(ultimaCarta);
            baralhoCartas.Remove(ultimaCarta);
        }

        return cards;
    }

    public void addCarta(CartaView carta)
    {
        carta.Carta = new project.src.models.Carta();
        baralhoCartas.Add(carta);
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
