namespace project.src.models
{
    public class Usuario
    {
        public int id { get; set; }
        public string email { get; set; }

        public string nUsuario { get; set; }

        public string hash { get; set; }

        public string salt { get; set; }

        public int nivel { get; set; }

        public int experiencia { get; set; }
    }
}