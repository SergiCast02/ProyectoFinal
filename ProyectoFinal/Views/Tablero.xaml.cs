using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ProyectoFinal.Models;
using ProyectoFinal.Api;

namespace ProyectoFinal.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Tablero : ContentPage
    {
        Usuario pusuario;
        Dolar pdolar;

        public Tablero(Usuario usuario)
        {
            InitializeComponent();

            pusuario = usuario;

            imgusuario.Source = ImageSource.FromStream(() => new System.IO.MemoryStream(usuario.Fotografia));
            txtnombrecompleto.Text = usuario.NombreCompleto;
            txtnombreusuario.Text = usuario.NombreUsuario;


        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        protected override async void OnAppearing()
        {
            var precio = await PrecioDolar.GetPrecioDolar(await UsuarioApi.GetFechaServidor());
            preciodolarc.Text = string.Format("{0:f4}", precio.Compra);
            preciodolarv.Text = string.Format("{0:f4}", precio.Venta);

            pdolar = precio;
        }

        private async void btncuentas_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Cuentas(pusuario));
        }

        private async void btnservicios_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Servicios(pusuario));
        }

        private async void btntransferencias_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Transferencias(pusuario));
        }

        private async void btncontrolp_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ControlPresupuestario(pusuario));
        }



        private async void btnsoporte_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Soporte(pusuario));
        }

        private async void btnlogout_Clicked(object sender, EventArgs e)
        {
            bool respuesta = await DisplayAlert("Cerrando sesión", "¿Realmente quieres cerrar sesión?", "Si", "No");

            if (respuesta) { await Navigation.PushAsync(new LogIn()); }
        }

        private async void perfil_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Perfil(pusuario));
        }
    }
}