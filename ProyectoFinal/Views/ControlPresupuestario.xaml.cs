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
    public partial class ControlPresupuestario : ContentPage
    {
        Usuario pusuario;
        Cuenta pcuenta;

        public ControlPresupuestario(Usuario usuario)
        {
            InitializeComponent();

            pusuario = usuario;
        }

        public ControlPresupuestario(Usuario usuario, Cuenta cuenta)
        {
            InitializeComponent();

            pusuario = usuario;
            pcuenta = cuenta;
        }

        protected async override void OnAppearing()
        {
            int iselected = 0;

            List<string> cuentas = new List<string>();
            cuentas.Add("Todas");

            var _cuentas = await App.DBase.obtenerCuentasUsuario(pusuario.Id);

            if(_cuentas.Count < 1)
            {
                pckccuenta.IsEnabled = false;
                pckccuenta.Title = "no hay cuentas disponibles";
            }
            else
            {
                for (int i = 0; i < _cuentas.Count; i++)
                {
                    cuentas.Add(_cuentas[i].CodigoCuenta);
                }

                if (cuentas.Count != 3) //el elemento Todas es el primer elemento de la lista y suma
                {
                    cuentas.Remove("Todas");
                    iselected = 0; //Siempre quedara escogida la primer cuenta creada al aparecer la pagina
                }
                else
                {
                    iselected = 1; //Siempre quedara escogida la primer cuenta creada al aparecer la pagina
                }

                pckccuenta.ItemsSource = cuentas;
                pckccuenta.SelectedItem = cuentas[iselected];

                if(pcuenta != null) { pckccuenta.SelectedItem = pcuenta.CodigoCuenta; }
            }
        }

        private async void pckccuenta_SelectedIndexChanged(object sender, EventArgs e)
        {
            actualizar(pckccuenta.SelectedItem.ToString());
        }

        async void actualizar(string ccuenta)
        {
            List<Transferencia> lcreditos = new List<Transferencia>();
            List<Transferencia> ldebitos = new List<Transferencia>();

            double entrado = 0, salido = 0;
            string moneda = "";

            //1 Todos
            //2 Créditos
            //3 Débitos
            if (ccuenta != "Todas")
            {
                var cuenta = await App.DBase.obtenerCuenta(ccuenta);
                moneda = cuenta.Moneda;

                lcreditos = await App.DBase.obtenerTransferenciasCuenta(2, ccuenta); //trae las transferencias que tengan que ver con esta cuenta
                ldebitos = await App.DBase.obtenerTransferenciasCuenta(3, ccuenta); //trae las transferencias que tengan que ver con esta cuenta

                for (int i = 0; i < lcreditos.Count; i++) { entrado += await normalizarMoneda(moneda, lcreditos[i]); }
                for (int i = 0; i < ldebitos.Count; i++) { salido += await normalizarMoneda(moneda, ldebitos[i]); }
            }
            else
            {
                List<string> listccuenta = new List<string>();
                moneda = "HNL";

                for (int i = 0; i < pckccuenta.ItemsSource.Count-1; i++)
                {
                    listccuenta.Add(pckccuenta.ItemsSource[i+1].ToString());
                }

                lcreditos = await App.DBase.obtenerTransferenciasCuentas(2, listccuenta); //trae las transferencias que tengan que ver con esta lista de cuentas
                ldebitos = await App.DBase.obtenerTransferenciasCuentas(3, listccuenta); //trae las transferencias que tengan que ver con esta lista de cuentas

                for (int i = 0; i < lcreditos.Count; i++) { entrado += await normalizarMoneda(moneda, lcreditos[i]); }
                for (int i = 0; i < ldebitos.Count; i++) { salido += await normalizarMoneda(moneda, ldebitos[i]); }
            }

            txtdinet.Text = moneda + " " + String.Format("{0:0.00}", entrado);
            txtdinst.Text = moneda + " " + String.Format("{0:0.00}", salido);
            txtentradast.Text = "" + lcreditos.Count;
            txtsalidast.Text = "" + ldebitos.Count;
        }

        public async Task<double> normalizarMoneda(string moneda, Transferencia transferencia)
        {
            if (moneda == transferencia.Moneda)
            {
                return transferencia.Valor;
            }
            else
            {
                if (moneda == "HNL")
                {
                    return transferencia.Valor * 24; // transferencia fue en dolares
                }
                else
                {
                    return transferencia.Valor / 24; // transferencia fue en lempiras
                }
            }
        }
    }
}