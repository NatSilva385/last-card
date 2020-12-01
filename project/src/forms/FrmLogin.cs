using Godot;
using System;
using System.Text.Json;
using System.Net.Http;
using project.src.models;
/// <summary>
/// Classe responsável por receber os dados do cliente para o
/// </summary>
public class FrmLogin : Control
{
    private LineEdit txtEmail;
    private LineEdit txtSenha;

    PopupDialog carregando;

    AcceptDialog erroDialog;

    private FrmCadastro frmCadastro;

    private string email;
    private string senha;



    static readonly HttpClient client = new HttpClient();
    public override void _Ready()
    {
        var dados = GetTree().GetNodesInGroup("dados");
        foreach (var dado in dados)
        {
            if ((dado as Node).Name == "txtEmail")
            {
                txtEmail = dado as LineEdit;
            }
            else if ((dado as Node).Name == "txtSenha")
            {
                txtSenha = dado as LineEdit;
            }
        }
        carregando = GetNode<PopupDialog>("LoadingDialog");
        erroDialog = GetNode<AcceptDialog>("SemPreencherDialog");
    }

    private void _on_txtEmail_text_changed(string new_text)
    {
        email = new_text;
    }

    private void _on_txtSenha_text_changed(string new_text)
    {
        senha = new_text;
    }

    private async void _on_btnLogin_pressed()
    {
        LoginUsuario login = new LoginUsuario();
        login.email = email;
        login.password = senha;
        string json = JsonSerializer.Serialize(login);

        var httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var buffer = System.Text.Encoding.UTF8.GetBytes(json);
        var byteContent = new ByteArrayContent(buffer);

        try
        {
            carregando.Visible = true;
            var response = await client.PostAsync("http://localhost:3000/login", httpContent);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                GD.Print("OK");
                string message = await response.Content.ReadAsStringAsync();
                Usuario user = JsonSerializer.Deserialize<Usuario>(message);
                var cena = ResourceLoader.Load<PackedScene>("res://scene/TelaInicial.tscn");
                FrmInicial frmInicial = cena.Instance() as FrmInicial;
                frmInicial.Usuario = user;
                Viewport root = GetNode<Viewport>("/root");
                Node currentScene = root.GetChild(root.GetChildCount() - 1);
                GetNode("/root").AddChild(frmInicial);

                currentScene.Free();
                currentScene = frmInicial;
            }
            else
            {
                carregando.Visible = false;
                erroDialog.DialogText = "Usuário ou senha não encontrados";
                erroDialog.Visible = true;
            }
        }
        catch (Exception e)
        {
            carregando.Visible = false;
            erroDialog.DialogText = e.Message;
            erroDialog.Visible = true;
        }

    }
    private void _on_btnCadastrar_pressed()
    {
        GetTree().ChangeScene("res://scene/CadastroUsuario.tscn");
    }
}

public class LoginUsuario
{
    public string email { get; set; }

    public string password { get; set; }
}



