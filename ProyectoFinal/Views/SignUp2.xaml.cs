using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Net.Mail;

using ProyectoFinal.Models;
using ProyectoFinal.Api;

namespace ProyectoFinal.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUp2 : ContentPage
    {
        Usuario usuariocompleto;

        public SignUp2(Usuario usuario)
        {
            InitializeComponent();

            //Se llena el objeto usuario con los datos de la anterior pagina
             usuariocompleto = usuario;
        }

        private async void btnregistrarme(object sender, EventArgs e)
        {
            //enviarcorreo();
            usuariocompleto.NumeroIdentidad = txtnumeroidentidad.Text;
            usuariocompleto.NombreUsuario = txtusuario.Text;
            usuariocompleto.Email = txtemail.Text;
            usuariocompleto.Contraseña = txtcontraseña.Text;

            //Añadimos Codigo Temporal a Usuario
            usuariocompleto.CodigoVerificacion = CodigoAleatorio();

            try
            {
                //SQLITE
                var usuariosqlite = await App.DBase.obtenerUsuario(2, usuariocompleto.NombreUsuario);

                if(usuariosqlite == null)
                {
                    var result = await App.DBase.UsuarioSave(usuariocompleto);
                    persistenciaSUsuario(usuariocompleto);
                    enviarcorreo(usuariocompleto);
                    await DisplayAlert("Registro Completado", "Hemos enviado un código de verificación a su correo electrónico que se le solicitará únicamente la primera vez que entre a su cuenta.", "OK");
                    for (var counter = 1; counter < 2; counter++) //2 es el numero de paginas a retroceder
                    {
                        Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
                    }
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Aviso", "El nombre de usuario que usted ha ingresado ya existe.\n\nIngrese otro nombre de usuario.", "OK");
                }
            }
            catch (Exception error)
            {
                await DisplayAlert("Error", "Se produjo un error al enviarte el correo", "OK");
            }

            /*bool estado = await UsuarioApi.CreateUsuario(usuariocompleto);
            if (estado) { await DisplayAlert("Aviso", "Usuario adicionado con éxito", "OK"); }*/
        }

        string CodigoAleatorio()
        {
            Random rdn = new Random();
            //string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890%$#@";
            string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890%$#@";
            int longitud = caracteres.Length;
            char letra;
            int longitudContrasenia = 6;
            string contraseniaAleatoria = string.Empty;
            for (int i = 0; i < longitudContrasenia; i++)
            {
                letra = caracteres[rdn.Next(longitud)];
                contraseniaAleatoria += letra.ToString();
            }
            return contraseniaAleatoria;
        }

        #region SendEmail (abre la aplicación de Gmail como borrador)
        public async Task SendEmail(string subject, string body, List<string> recipients)
        {
            try
            {
                var message = new EmailMessage
                {
                    Subject = subject,
                    Body = body,
                    To = recipients,
                    //Cc = ccRecipients,
                    //Bcc = bccRecipients
                };
                await Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                // Email is not supported on this device
            }
            catch (Exception ex)
            {
                // Some other exception occurred
            }
        }
        #endregion
        #region Enviar e-mail
        void enviarcorreo(Usuario usuariocompleto)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("starbankteam@gmail.com");
                mail.To.Add(usuariocompleto.Email);
                mail.Subject = "STARBANK | Código de verificación";
                mail.Body = "¡Hola <b>"+usuariocompleto.NombreCompleto+"</b>, Gracias por elegir STARBANK.\n\nEste es tu código de verificación: "+usuariocompleto.CodigoVerificacion;
                SmtpServer.Port = 587;
                SmtpServer.Host = "smtp.gmail.com";
                SmtpServer.EnableSsl = true;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential("starbankteam@gmail.com", "ptkyllujqfluvnls");

                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                DisplayAlert("Mensaje del Programador", ex.Message, "OK");
            }
        }
        #endregion

        public async void persistenciaSUsuario(Usuario usuario)
        {
            //PERSISTENCIA insertar
            var persistencia = new Persistencia
            {
                Id = 1,
                Campo = "" + usuario.Id
            }; //1 porque es Usuario (ver más en Persistencia.cs)
            var estado = await App.DBase.PersistenciaSave(persistencia);
        }
    }
}