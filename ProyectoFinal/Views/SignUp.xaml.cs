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

            try
            {
                //CORRECION el dato de DateTime.Now debe ser traido desde la API no localmente
                dtfechanacimiento.MaximumDate = DateTime.Parse(DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + (DateTime.Now.Year - 18) );

            }
            catch (Exception error)
            {

            }

        }


        private void imgpersona_Tapped(object sender, EventArgs e)
        {
            tomarfoto();
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

            try
            {
                if (FileFotoBytes == null)
                {
                    bool resp = await DisplayAlert("Aviso", "Tomarse una fotografía es requerido para poder aperturar su cuenta de usuario", "Tomar Foto", "OK");
                    if (resp) { tomarfoto(); }
                    return;
                }

                if (txtnombrecompleto.Text == null || txtnombrecompleto.Text == "")
                {
                    await DisplayAlert("Aviso", "Su nombre completo es requerido para poder aperturar su cuenta de usuario", "OK"); return;
                }

                if(dtfechanacimiento.Date == null)
                {
                    await DisplayAlert("Aviso", "Es requerido colocar su fecha de nacimiento para poder aperturar su cuenta de usuario", "OK"); return;
                }

                if(pcksexo.SelectedItem == null)
                {
                    await DisplayAlert("Aviso", "Es requerido seleccionar su sexo para poder aperturar su cuenta de usuario", "OK"); return;
                }

                if(txtdireccion.Text == null || txtdireccion.Text == "")
                {
                    await DisplayAlert("Aviso", "Su dirección es requerida para poder aperturar su cuenta de usuario", "OK"); return;
                }
                
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return;
            }

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


        private async void tomarfoto()
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