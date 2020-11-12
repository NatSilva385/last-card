using System;
using System.Collections.Generic;
using project.src.models;
using Godot;

namespace project.src.controller
{
    public class JogoController : JogoInterface
    {
        List<Carta> baralho;
        List<Carta> descarte = new List<Carta>();
        public JogoController()
        {
            baralho = embaralhar(initBaralho());
            //baralho = initBaralho();
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
            rnd.Randomize();
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

        public bool jogarCarta(Carta carta)
        {
            bool podeJogar = false;
            if (descarte.Count == 0)
            {
                descarte.Add(carta);
                podeJogar = true;
            }
            else
            {
                var last = descarte.Count - 1;
                var ultimaCarta = descarte[last];
                if (carta.podeJogar(ultimaCarta))
                {
                    descarte.Add(carta);
                    podeJogar = true;
                }
            }
            return podeJogar;
        }

        public bool podeJogarCarta(Carta carta)
        {
            bool podeJogar = false;
            if (descarte.Count == 0)
            {
                podeJogar = true;
            }
            else
            {
                var last = descarte.Count - 1;
                var ultimaCarta = descarte[last];
                if (carta.podeJogar(ultimaCarta))
                {
                    podeJogar = true;
                }
            }
            return podeJogar;
        }

        public bool mudaCor(COR cor)
        {
            var last = descarte.Count - 1;
            var ultimaCarta = descarte[last];
            if (ultimaCarta.Cor == COR.SEMCOR)
            {
                ultimaCarta.Cor = cor;
                return true;
            }
            return false;
        }
    }
}