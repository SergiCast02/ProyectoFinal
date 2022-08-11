﻿using ProyectoFinal.Models;
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
        Dolar pdolar;

        public ControlPresupuestario(Usuario usuario, Dolar dolar)
        {
            InitializeComponent();

            pusuario = usuario;
            pdolar = dolar;
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

            var _cuentas = await App.DBase.obtenerCuentasUsuario(pusuario.NumeroIdentidad);

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

            List<Transferencia> lcreditos2 = new List<Transferencia>();
            List<Transferencia> ldebitos2 = new List<Transferencia>();

            double entrado = 0, salido = 0, entrado2 = 0, salido2 = 0;
            string moneda = "", moneda2 = "";

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
                /*List<string> listccuenta = new List<string>();
                moneda = "HNL";

                for (int i = 0; i < pckccuenta.ItemsSource.Count-1; i++)
                {
                    listccuenta.Add(pckccuenta.ItemsSource[i+1].ToString());
                }

                lcreditos = await App.DBase.obtenerTransferenciasCuentas(2, listccuenta); //trae las transferencias que tengan que ver con esta lista de cuentas
                ldebitos = await App.DBase.obtenerTransferenciasCuentas(3, listccuenta); //trae las transferencias que tengan que ver con esta lista de cuentas

                for (int i = 0; i < lcreditos.Count; i++) { entrado += await normalizarMoneda(moneda, lcreditos[i]); }
                for (int i = 0; i < ldebitos.Count; i++) { salido += await normalizarMoneda(moneda, ldebitos[i]); }*/

                List<string> listccuenta = new List<string>();

                for (int i = 0; i < pckccuenta.ItemsSource.Count - 1; i++) { listccuenta.Add(pckccuenta.ItemsSource[i + 1].ToString()); }

                var cuenta = await App.DBase.obtenerCuenta(listccuenta[0]);
                moneda = cuenta.Moneda;

                lcreditos = await App.DBase.obtenerTransferenciasCuenta(2, listccuenta[0]); //trae las transferencias que tengan que ver con esta cuenta
                ldebitos = await App.DBase.obtenerTransferenciasCuenta(3, listccuenta[0]); //trae las transferencias que tengan que ver con esta cuenta

                for (int i = 0; i < lcreditos.Count; i++) { entrado += await normalizarMoneda(moneda, lcreditos[i]); }
                for (int i = 0; i < ldebitos.Count; i++) { salido += await normalizarMoneda(moneda, ldebitos[i]); }

                var cuenta2 = await App.DBase.obtenerCuenta(listccuenta[1]);
                moneda2 = cuenta2.Moneda;

                lcreditos2 = await App.DBase.obtenerTransferenciasCuenta(2, listccuenta[1]); //trae las transferencias que tengan que ver con esta cuenta
                ldebitos2 = await App.DBase.obtenerTransferenciasCuenta(3, listccuenta[1]); //trae las transferencias que tengan que ver con esta cuenta

                for (int i = 0; i < lcreditos2.Count; i++) { entrado2 += await normalizarMoneda(moneda2, lcreditos2[i]); }
                for (int i = 0; i < ldebitos2.Count; i++) { salido2 += await normalizarMoneda(moneda2, ldebitos2[i]); }

                if(moneda == "USD") { entrado = entrado * pdolar.Precio; salido = salido * pdolar.Precio; }
                if (moneda2 == "USD") {
                    entrado2 = entrado2 * pdolar.Precio;
                    salido2 = salido2 * pdolar.Precio;
                }
            }

            txtdinet.Text = moneda + " " + string.Format("{0:C}", (entrado+entrado2)).Replace("$", string.Empty);
            txtdinst.Text = moneda + " " + string.Format("{0:C}", (salido + salido2)).Replace("$", string.Empty);
            txtentradast.Text = "" + (lcreditos.Count + lcreditos2.Count);
            txtsalidast.Text = "" + (ldebitos.Count + ldebitos2.Count);
        }

        public async Task<double> normalizarMoneda(string moneda, Transferencia transferencia)
        {
            var date = transferencia.Fecha;
            date = date.Substring(0, 10);
            var dolar = await App.DBase.obtenerPrecioDolar(date);

            if (moneda == transferencia.Moneda)
            {
                return transferencia.Valor;
            }
            else
            {
                if (moneda == "HNL")
                {
                    return transferencia.Valor * dolar.Precio; // transferencia fue en dolares
                }
                else
                {
                    return transferencia.Valor / dolar.Compra; // transferencia fue en lempiras
                }
            }

        }
    }
}