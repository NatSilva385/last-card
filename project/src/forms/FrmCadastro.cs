using Godot;
using System;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;
public class FrmCadastro : PanelContainer
{
    private string email;
    private string nUsuario;
    private string senha;
    private string rSenha;

    static readonly HttpClient client = new HttpClient();

    public override void _Ready()
    {

    }
    private void _on_btnCadastrar_pressed()
    {
        Usuario usuario = new Usuario();
        usuario.email = email;
        usuario.nUsuario = nUsuario;
        usuario.password = senha;

        string json = JsonSerializer.Serialize(usuario);
        GD.Print(json);
        var httpContent = new StringContent(json, System.Text.Encoding.UTF8);
        var buffer = System.Text.Encoding.UTF8.GetBytes(json);
        var byteContent = new ByteArrayContent(buffer);
        client.PostAsync("http://localhost:3000/usuarios/create", httpContent);

    }

    private void _on_txtEmail_text_changed(string new_text)
    {
        email = new_text;
    }

    private void _on_txtNome_text_changed(string new_text)
    {
        nUsuario = new_text;
    }

    private void _on_txtSenha_text_changed(string new_text)
    {
        senha = new_text;
    }

    private void _on_txtRSenha_text_changed(string new_text)
    {
        rSenha = new_text;
    }

}

class Usuario
{
    public string email { get; set; }

    public string nUsuario { get; set; }

    public string password { get; set; }

}
