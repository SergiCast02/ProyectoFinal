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
    public partial class AdministracionCuenta : ContentPage
    {
        Cuenta pcuenta;
        public AdministracionCuenta(Cuenta cuenta)
        {
            InitializeComponent();

            pcuenta = cuenta;
        }

        protected override async void OnAppearing()
        {
            if (pcuenta.Tipo == "ahorro") { txttipocuenta.Text = "Cuenta de ahorros"; }
            txtmoneda.Text = pcuenta.Moneda;
            txtsaldo.Text = "" + pcuenta.Saldo;
            txtcodigocuenta.Text = pcuenta.CodigoCuenta;
            //txtmesactual.Text = await obtenerMesUTC();
            txtmesactual.Text = "Agosto";
            List <Transferencia> lista = await App.DBase.obtenerTransferenciasCuenta(1, pcuenta.CodigoCuenta);

            lista = Enumerable.Reverse(lista).ToList(); //Invierte la lista, la ultima transaccion hecha tiene que estar mas arriba

            for (int i = 0; i < lista.Count; i++)
            {
                //Convertir a la moneda de la cuenta

                if (pcuenta.Moneda != lista[i].Moneda)
                {
                    if (pcuenta.Moneda == "HNL")
                    {
                        lista[i].Valor = lista[i].Valor * 24; // transferencia fue en dolares
                        lista[i].Moneda = "HNL";
                    }
                    else
                    {
                        lista[i].Valor = lista[i].Valor / 24; // transferencia fue en lempiras
                        lista[i].Moneda = "USD";
                    }
                }

                if (lista[i].Envia != pcuenta.CodigoCuenta) { lista[i].Accion = "crédito"; }
            }

            ListTransferenciasMes.ItemsSource = lista;
        }

        private void ListTransferenciasMes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var transferencia = e.CurrentSelection[0];
            //[0] porque es el indice de los elementos seleccionados, como es seleccion unica (se configura como parametro en el xaml) siempre sera el indice [0]
        }

        private async Task<string> obtenerMesUTC()
        {
            string date = await HoraUniversal.getHoraUTC();
            date = date.Substring(date.IndexOf("datetime") + 16, 2); //el primer valor es el indice del cual empieza a obtener el texto y el otro indice a donde termina la extraccion dentro del string (esta muy pulseado)

            switch (date)
            {
                case "01":
                    date = "Enero";
                    break;
                case "02":
                    date = "Febrero";
                    break;
                case "03":
                    date = "Marzo";
                    break;
                case "04":
                    date = "Abril";
                    break;
                case "05":
                    date = "Mayo";
                    break;
                case "06":
                    date = "Junio";
                    break;
                case "07":
                    date = "Julio";
                    break;
                case "08":
                    date = "Agosto";
                    break;
                case "09":
                    date = "Septiembre";
                    break;
                case "10":
                    date = "Octubre";
                    break;
                case "11":
                    date = "Noviembre";
                    break;
                case "12":
                    date = "Diciembre";
                    break;
            }

            return date;
        }

        private async void btnhtransacciones_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HistorialTransacciones(pcuenta));
        }

        private async void btnvolver_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}