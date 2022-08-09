using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using ProyectoFinal.Models;
using Xamarin.Essentials;

namespace ProyectoFinal.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Soporte : ContentPage
    {
        Usuario pusuario;

        public Soporte(Usuario usuario)
        {
            InitializeComponent();
        }

        private async void btncorreo_Clicked(object sender, EventArgs e)
        {
            await SendEmail();
        }

        #region SendEmail (abre la aplicación de Gmail como borrador)
        //public async Task SendEmail(string subject, string body, List<string> recipients)
        public async Task SendEmail()
        {
            List<string> correocontacto = new List<string>();
            correocontacto.Add("starbankteam@gmail.com");

            try
            {
                var message = new EmailMessage
                {
                    Subject = "Solicitud de Asistencia",
                    Body = null,
                    To = correocontacto,
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
    }
}