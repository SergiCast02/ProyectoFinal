using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using ProyectoFinal.Models;

namespace ProyectoFinal.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Servicios : ContentPage
    {
        Usuario pusuario;
        public Servicios(Usuario usuario)
        {
            InitializeComponent();

            pusuario = usuario;
        }

        private async void TapGestureRecognizer_agua(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Servicio(pusuario));
        }

        private async void TapGestureRecognizer_energia(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Servicio(pusuario));
        }

        private void ListPromociones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}