namespace project.src.models
{
    public class Carta
    {
        public COR Cor { get; set; }
        public VALOR Valor { get; set; }

        public bool podeJogar(Carta carta)
        {
            if (this.Valor == VALOR.CORINGA || this.Valor == VALOR.CORINGA_MAIS_QUATRO)
            {
                return true;
            }
            if (carta.Cor == this.Cor || carta.Valor == this.Valor)
            {
                return true;
            }
            return false;
        }
    }

    public enum COR
    {
        SEMCOR,
        AZUL,
        VERMELHO,
        VERDE,
        AMARELO
    }

    public enum VALOR
    {
        SEMVALOR,
        UM,
        DOIS,
        TRES,
        QUATRO,
        CINCO,
        SEIS,
        SETE,
        OITO,
        NOVE,
        MAIS_DOIS,
        BLOQUEAR,
        INVERTER,
        CORINGA,
        CORINGA_MAIS_QUATRO
    }

    public enum SHADER_CARTA_FRENTE
    {
        PADRAO,
        METALICO,
        CRISTAL
    }

}