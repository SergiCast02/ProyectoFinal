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
    public partial class Cuentas : ContentPage
    {
        Usuario pusuario;
        Dolar pdolar;
        int operacion = 0;

        //0 pagina normal por defecto (dirige a administracion de cuenta)
        //1 seleccionar cuenta para redirigir a la pantalla Transferencias

        public Cuentas(Usuario usuario, Dolar dolar)
        {
            InitializeComponent();
            pusuario = usuario;
            pdolar = dolar;
        }

        public Cuentas(Usuario usuario, int op, Dolar dolar)
        {
            InitializeComponent();
            pusuario = usuario;
            operacion = op;
            btnvolver.IsVisible = false;
            btncrearcuenta.IsVisible = true;

            pdolar = dolar;
        }

        protected override async void OnAppearing()
        {
            ListCuentas.ItemsSource = await App.DBase.obtenerCuentasUsuario(pusuario.Id);
        }

        private async void btncreacuenta_Clicked(object sender, EventArgs e)
        {
            var resultado = await App.DBase.obtenerCuentasUsuario(pusuario.Id);

            if (resultado.Count >= 2)
            {
                await DisplayAlert("Aviso", "Ya tienes 2 cuentas de ahorro, haz alcanzado el límite.", "OK");
            }
            else
            {
                await Navigation.PushAsync(new CrearCuenta(pusuario));
            }
            
        }

        private async void btnvolver_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void ListCuentas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Cuenta cuenta = (Cuenta)e.CurrentSelection[0];
            //[0] porque es el indice de los elementos seleccionados, como es seleccion unica (se configura como parametro en el xaml) siempre sera el indice [0]

            if (operacion == 0)
            {
                await Navigation.PushAsync(new AdministracionCuenta(cuenta, pusuario));
            }
            else if (operacion == 1)
            {
                await Navigation.PushAsync(new Transferencias(pusuario, cuenta, pdolar));
            }
        }
    }
}