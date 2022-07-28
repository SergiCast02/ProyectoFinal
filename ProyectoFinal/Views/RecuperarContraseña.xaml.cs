using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using ProyectoFinal.Models;
using System.Net.Mail;
using Acr.UserDialogs;

namespace ProyectoFinal.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecuperarContraseña : ContentPage
    {
        public RecuperarContraseña()
        {
            InitializeComponent();
        }

        private async void goLogIn(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void btnenviarcontraseñat_Clicked(object sender, EventArgs e)
        {
            var usuario = await App.DBase.obtenerUsuario(3, txtemail.Text);
            if (usuario != null)
            {
                if(usuario.CodigoVerificacion == "")
                {
                    usuario.ContraseñaTemporal = CodigoAleatorio();
                    await App.DBase.UsuarioSave(usuario); //Se inserta (actualiza el usuario) la contraseña temporal en la base de datos
                    enviarcorreo(usuario);
                    await DisplayAlert("Envío exitoso", "Revisa tu bandeja de entrada y sigue las instrucciones para habilitar tu nueva contraseña.", "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Aviso", "Eres un usuario nuevo, debes iniciar sesión por lo menos una vez para cambiar tu contraseña.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Aviso", "El correo electrónico que ingresaste no está ligado a ninguna cuenta existente.", "OK");
            }
            
        }

        #region Enviar e-mail
        void enviarcorreo(Usuario user)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("starbankteam@gmail.com");
                mail.To.Add(user.Email);
                mail.Subject = "STARBANK | Código de verificación";
                mail.Body = "<html><body><p>¡Hola <b>" + user.NombreCompleto + "!</b></p><br><br><p>Gracias por elegir STARBANK.</p><br><br><p>Esta es tu contraseña temporal: <b>"+user.ContraseñaTemporal+"<b></p><br><p>Con ella ingresarás a la aplicación STARBANK y se te solicitará que escribas una contraseña nueva la cual <b>será tu nueva contraseña.</b></p></body></html>";
                mail.IsBodyHtml = true; 
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

        string CodigoAleatorio()
        {
            Random rdn = new Random();
            //string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890%$#@";
            string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890#@";
            int longitud = caracteres.Length;
            char letra;
            int longitudContrasenia = 8;
            string contraseniaAleatoria = string.Empty;
            for (int i = 0; i < longitudContrasenia; i++)
            {
                letra = caracteres[rdn.Next(longitud)];
                contraseniaAleatoria += letra.ToString();
            }
            return contraseniaAleatoria;
        }

        private void txtemail_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnenviarcontraseñat.IsEnabled = true;
            if (txtemail.Text == "") {
                btnenviarcontraseñat.IsEnabled = false; 
            }
            
        }

        private void makeToast(string mensaje, double duracion)
        {
            var ToastConfig = new ToastConfig(mensaje);
            ToastConfig.SetDuration(TimeSpan.FromSeconds(duracion));
            ToastConfig.SetPosition(ToastPosition.Bottom);
            UserDialogs.Instance.Toast(ToastConfig);
        }
    }
}