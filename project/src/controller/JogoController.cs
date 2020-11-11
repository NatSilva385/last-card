using System;
using System.Collections.Generic;
using project.src.models;
using Godot;

namespace project.src.controller
{
    public abstract class JogoController : JogoInterface
    {
        List<Carta> baralho;
        public JogoController()
        {
            baralho = embaralhar(initBaralho());
        }

        private List<Carta> initBaralho()
        {
            List<Carta> cartas = new List<Carta>();
            foreach (COR c in Enum.GetValues(typeof(COR)))
            {
                if (c != COR.SEMCOR)
                {
                    foreach (VALOR valor in Enum.GetValues(typeof(VALOR)))
                    {
                        if (valor != VALOR.SEMVALOR)
                        {
                            Carta carta = new Carta();
                            carta.Cor = c;
                            carta.Valor = valor;
                            cartas.Add(carta);
                        }
                    }
                }

            }
            return cartas;
        }

        private List<Carta> embaralhar(List<Carta> cartas)
        {
            List<Carta> temp = cartas;
            var rnd = new RandomNumberGenerator();
            for (int i = 0; i < temp.Count; i++)
            {
                int x = rnd.RandiRange(0, i);
                Carta aux = temp[i];
                temp[i] = temp[x];
                temp[x] = aux;
            }
            return temp;
        }

        public Carta comprarUmaCarta()
        {
            int ultimaCarta = baralho.Count;
            ultimaCarta--;
            Carta carta = null;
            if (ultimaCarta >= 0)
            {
                carta = baralho[ultimaCarta];
                baralho.Remove(carta);
            }

            return carta;
        }

        public List<Carta> comprarCartas(int qtdeCartas)
        {
            throw new NotImplementedException();
        }
    }
}