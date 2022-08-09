using ProyectoFinal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ProyectoFinal.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Servicio : ContentPage
    {
        Usuario pusuario;
        Cuenta pcuenta;

        public Servicio(Usuario usuario)
        {
            InitializeComponent();
            pusuario = usuario;
            idcliente.Text = ""+pusuario.IdCliente;
        }

        private async void btnvolver_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private void bdetalles_Clicked(object sender, EventArgs e)
        {
            btntransferir.IsEnabled = true;
        }
    }
}