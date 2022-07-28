using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using ProyectoFinal.Models;
using System.IO;

namespace ProyectoFinal.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUp : ContentPage
    {
        Plugin.Media.Abstractions.MediaFile FileFoto = null;
        byte[] FileFotoBytes = null;

        public SignUp()
        {
            InitializeComponent();
        }

        private async void goSingUp2(object sender, EventArgs e)
        {
            /*try
            {
                var result = await App.DBase.UsuarioDeleteAll();
            }
            catch (Exception error)
            {

            }*/

            Usuario usuario = new Usuario
            {
                Fotografia = FileFotoBytes,
                NombreCompleto = txtnombrecompleto.Text,
                FechaNacimiento = dtfechanacimiento.Date.ToString("yyyy/MM/dd"),
                Sexo = pcksexo.SelectedItem.ToString(),
                Direccion = txtdireccion.Text
            };

            await Navigation.PushAsync(new SignUp2(usuario));
        }

        private async void tomarfoto(object sender, EventArgs e)
        {
            FileFoto = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Fotos_starbank",
                Name = "fotografia.jpg",
                SaveToAlbum = true
            });
            // await DisplayAlert("Path directorio", FileFoto.Path, "OK");


            if (FileFoto != null)
            {
                imgpersona.Source = ImageSource.FromStream(() =>
                {
                    return FileFoto.GetStream();
                });

                //Pasamos la foto a imagen a byte[] almacenandola en FileFotoBytes
                using (System.IO.MemoryStream memory = new MemoryStream())
                {
                    Stream stream = FileFoto.GetStream();
                    stream.CopyTo(memory);
                    FileFotoBytes = memory.ToArray();
                    /*string base64Val = Convert.ToBase64String(FileFotoBytes);
                    FileFotoBytes = Convert.FromBase64String(base64Val);*/
                }
            }
        }
    }
}