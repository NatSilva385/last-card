namespace project.src.models
{
    public class Carta
    {
        public bool podeJogar(Carta carta)
        {
            if (carta.Cor == this.Cor || carta.Valor == this.Valor)
            {
                return true;
            }
            if (this.Valor == VALOR.CORINGA || this.Valor == VALOR.CORINGA_MAIS_QUATRO)
            {
                return true;
            }
            return false;
        }


        public COR Cor
        {
            get;
            set;
        }

        private VALOR _valor;
        public VALOR Valor
        {
            get { return _valor; }
            set
            {
                _valor = value;
                if (value == VALOR.CORINGA || value == VALOR.CORINGA_MAIS_QUATRO)
                {
                    Cor = COR.SEMCOR;
                }
            }
        }
    }

    public enum VALOR
    {
        SEM_VALOR, UM, DOIS, TRES, QUATRO, CINCO, SEIS, SETE, OITO, NOVE, MAIS_DOIS, BLOQUEAR, INVERTER, CORINGA, CORINGA_MAIS_QUATRO
    }
    public enum COR
    {
        SEMCOR,
        AZUL,
        VERMELHO,
        VERDE,
        AMARELO
    }

    public enum SHADER_CARTA_FRENTE
    {
        PADRAO,
        METALICO,
        CRISTAL
    }

}