using Godot;
using System;
using System.Text.Json;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
public class FrmCadastro : PanelContainer
{
    private string email;
    private string nUsuario;
    private string senha;
    private string rSenha;


    private LineEdit txtEmail;

    private LineEdit txtNome;
    private LineEdit txtSenha;
    private LineEdit txtRSenha;

    private AcceptDialog erro;

    private PopupDialog carregando;



    static readonly HttpClient client = new HttpClient();

    public override void _Ready()
    {
        var txts = GetTree().GetNodesInGroup("txt");

        erro = GetParent().GetParent().GetNode<AcceptDialog>("SemPreencherDialog");

        carregando = GetParent().GetNode<PopupDialog>("LoadingDialog");

        for (int i = 0; i < txts.Count; i++)
        {
            if ((txts[i] as Node).Name == "txtEmail")
            {
                txtEmail = txts[i] as LineEdit;
            }
            else if ((txts[i] as Node).Name == "txtNome")
            {
                txtNome = txts[i] as LineEdit;
            }
            else if ((txts[i] as Node).Name == "txtSenha")
            {
                txtSenha = txts[i] as LineEdit;
            }
            else if ((txts[i] as Node).Name == "txtRSenha")
            {
                txtRSenha = txts[i] as LineEdit;
            }
        }
    }
    private async void _on_btnCadastrar_pressed()
    {
        if (!checaPreenchimento())
        {
            return;
        }
        Usuario usuario = new Usuario();
        usuario.email = email;
        usuario.nUsuario = nUsuario;
        usuario.password = senha;

        string json = JsonSerializer.Serialize(usuario);
        GD.Print(json);
        var httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var buffer = System.Text.Encoding.UTF8.GetBytes(json);
        var byteContent = new ByteArrayContent(buffer);

        desabilitaTxt();
        try
        {
            carregando.Visible = true;
            var response = await client.PostAsync("http://localhost:3000/usuarios/create", httpContent);
            carregando.Visible = false;
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                erro.DialogText = "Usuário cadastrado com sucesso";
                erro.Visible = true;
            }
            else
            {
                string message = await response.Content.ReadAsStringAsync();
                erro.DialogText = message;
                erro.Visible = true;
            }

        }
        catch (Exception e)
        {
            carregando.Visible = false;
            erro.DialogText = e.Message;
            erro.Visible = true;
        }



    }

    private bool checaPreenchimento()
    {
        if (email == null)
        {
            erro.DialogText = "Preencha o campo do Email";
            erro.Visible = true;
            return false;
        }
        else if (email.Length == 0)
        {
            erro.DialogText = "Preencha o campo do Email";
            erro.Visible = true;
            return false;
        }
        else
        {
            Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", RegexOptions.Compiled);
            MatchCollection matches = regex.Matches(email);
            if (matches.Count == 0)
            {
                erro.DialogText = "Digite um email válido";
                erro.Visible = true;
                return false;
            }
        }

        return true;
    }

    private void desabilitaTxt()
    {
        txtEmail.Editable = false;
        txtNome.Editable = false;
        txtRSenha.Editable = false;
        txtSenha.Editable = false;
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
