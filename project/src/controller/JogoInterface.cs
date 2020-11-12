using System.Collections.Generic;
using project.src.models;

namespace project.src.controller
{
    public interface JogoInterface
    {
        Carta comprarUmaCarta();
        List<Carta> comprarCartas(int qtdeCartas);

        bool jogarCarta(Carta carta);

        bool podeJogarCarta(Carta carta);
    }
}