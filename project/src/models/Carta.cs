namespace project.src.models
{
    public class Carta
    {


        public COR Cor
        {
            get;
            set;
        }

        public VALOR Valor
        {
            get;
            set;
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