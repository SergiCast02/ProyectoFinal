using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using ProyectoFinal.Models;
using System.Net.Mail;
using ProyectoFinal.Api;

namespace ProyectoFinal.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Transferencias : ContentPage
    {
        Usuario pusuario;
        Cuenta pcuenta;
        Dolar pdolar;

        public Transferencias(Usuario usuario, Dolar dolar)
        {
            InitializeComponent();

            pusuario = usuario;
            pdolar = dolar;

            //Enableds
            cuentaa.IsEnabled = false;
            monto.IsEnabled = false;
            concepto.IsEnabled = false;
            chkcorreo.IsEnabled = false;
            btntransferir.IsEnabled = false;

            monedaconversion.Text = "HNL";
            valorconversion.Text = "0.00";
        }

        public Transferencias(Usuario usuario, Cuenta cuenta, Dolar dolar)
        {
            InitializeComponent();

            pusuario = usuario;
            pcuenta = cuenta;
            pdolar = dolar;

            codigocuenta.Text = cuenta.CodigoCuenta;
            moneda.Text = cuenta.Moneda;
            saldo.Text = ""+cuenta.Saldo;

            cardcuenta.IsVisible = true;

            //Enableds
            cuentaa.IsEnabled = true;
            monto.IsEnabled = true;
            concepto.IsEnabled = true;
            chkcorreo.IsEnabled = true;
            btntransferir.IsEnabled = true;

            //Tipo de moneda en el entry monto dependiendo de la moneda de la cuenta
            monto.Placeholder = monto.Placeholder + " (" + cuenta.Moneda + ")";


            if (pcuenta.Moneda == "HNL")
            {
                monedaconversion.Text = "USD";
            }
            else
            {
                monedaconversion.Text = "HNL";
            }

            valorconversion.Text = "0.00";
        }

        protected override async void OnAppearing()
        {
            try
            {
                var listacuentas = await App.DBase.obtenerListaPersistencia();
                listacuentas.RemoveAt(0); //Elimino el Id de los registros porque es el que guarda el usuario al inicio de sesion

                if (listacuentas.Count > 0) { ListaCuentas.ItemsSource = listacuentas; }
            }
            catch (Exception error)
            {

            }
             
        }

        private async void btntransferir_Clicked(object sender, EventArgs e)
        {
            var cuenta = await App.DBase.obtenerCuenta(cuentaa.Text);
            if (cuentaa.Text == pcuenta.CodigoCuenta) { await DisplayAlert("Aviso", "No puedes transferir a tu misma cuenta", "OK"); return; }
            if(cuenta == null) { await DisplayAlert("Aviso", "La cuenta a acreditar que ingresaste no existe.", "OK"); }
            else
            {
                if(monto.Text == "" || monto.Text == null) { await DisplayAlert("Aviso", "Ingresa un monto de transferencia", "OK"); return; }
                if(double.Parse(monto.Text) <= 0) { await DisplayAlert("Aviso", "Ingrese un valor válido para el monto de la transferencia", "OK"); return; }
                if(double.Parse(monto.Text) > pcuenta.Saldo)
                {
                    await DisplayAlert("Aviso", "El monto de la transacción supera el saldo de tu cuenta", "OK");
                }
                else
                {
                    Transferencia transferencia = new Transferencia
                    {
                        Accion = "débito",
                        Moneda = pcuenta.Moneda,
                        Valor = double.Parse(monto.Text),
                        Envia = pcuenta.CodigoCuenta,
                        Recibe = cuentaa.Text,
                        Fecha = await UsuarioApi.GetFechaServidor(),
                        Comentario = concepto.Text
                    };

                    if (pcuenta.Moneda == cuenta.Moneda)
                    {
                        cuenta.Saldo += double.Parse(monto.Text);
                    }
                    else
                    {
                        if(pcuenta.Moneda == "HNL")
                        {
                            cuenta.Saldo += double.Parse(monto.Text)/pdolar.Compra;
                        }
                        else
                        {
                            cuenta.Saldo += double.Parse(monto.Text)*pdolar.Precio;
                        }
                    }

                    var resultado = await App.DBase.CuentaSave(2, cuenta);

                    if (resultado == 1) { 
                        pcuenta.Saldo -= double.Parse(monto.Text); //No se hace ninguna conversion en la reduccion del saldo de la cuenta (actualizaciond e estado luego de la transaccion) porque se valida en el picker que la moneda ingresada sea igual a la de la cuenta
                        var resultado2 = await App.DBase.CuentaSave(2, pcuenta);

                        if(resultado2 == 1)
                        {
                            await App.DBase.TransferenciaSave(transferencia);


                            var persistencia = new Persistencia();
                            persistencia.Campo = cuenta.CodigoCuenta;
                            
                            bool ciclo = true;
                            int indice = 1; // para que empiece en 2 en adelante ver mas en Persistencia.cs
                            while(ciclo == true)
                            {
                                indice++;

                                if (await App.DBase.obtenerPersistencia(indice) == null) {
                                    persistencia.Id = indice;
                                    await App.DBase.PersistenciaSave(persistencia);
                                    break;
                                }
                            }

                            var debitante = await App.DBase.obtenerUsuario(1, "" + pcuenta.CodigoUsuario); //CodigoUsuario es el Id de la tabla Usuario
                            var acreditante = await App.DBase.obtenerUsuario(1, "" + cuenta.CodigoUsuario); //CodigoUsuario es el Id de la tabla Usuario

                            if (chkcorreo.IsChecked) { enviarcorreo(debitante, acreditante, pcuenta, cuenta, transferencia); }

                            await DisplayAlert("Aviso", "¡Transferencia realizada con éxito!", "OK");
                            await Navigation.PushAsync(new Tablero(pusuario, pdolar));
                        }
                    }
                }
            }


        }

        private async void btnscuenta_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Cuentas(pusuario, 1, pdolar)); //1 para ocultar boton volver (1es solamente la vista de seleccion de cuentas)
        }

        #region Enviar e-mail
        void enviarcorreo(Usuario usuariod, Usuario usuarioa, Cuenta cuentad, Cuenta cuentaa, Transferencia transferencia)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("starbankteam@gmail.com");
                mail.To.Add(usuariod.Email);
                mail.Subject = "STARBANK | Código de verificación";
                mail.Body = "<html> <Body> <h1>Comprobante de Transacción</h1> <br><br> <p>Cliente: <b>" + usuariod.NombreCompleto+ "</b></p> <br> <p>Cuenta Saliente: <b>" + cuentad.CodigoCuenta + "</b></p> <br> <p>Cuenta Entrante: <b>" + cuentaa.CodigoCuenta + "</b></p> <br><br> <h3>MONTO DE LA TRANSFERENCIA: "+cuentad.Moneda+ String.Format("{0:0.00}", transferencia.Valor) +"</h3> </Body> </html>";
                mail.IsBodyHtml = true;
                SmtpServer.Port = 587;
                SmtpServer.Host = "smtp.gmail.com";
                SmtpServer.EnableSsl = true;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential("starbankteam@gmail.com", "ptkyllujqfluvnls");

                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                DisplayAlert("Mensaje del Programador", ex.Message, "OK");
            }
        }
        #endregion

        private void monto_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(monto.Text != "" && monto.Text != null)
            {
                if (pcuenta.Moneda == "HNL")
                {
                    string valor = string.Format("{0:C}", (double.Parse(monto.Text) / pdolar.Compra));
                    valorconversion.Text = valor.Replace("$", string.Empty);
                }
                else
                {
                    string valor = string.Format("{0:C}", (double.Parse(monto.Text) * pdolar.Precio));
                    valorconversion.Text = valor.Replace("$", string.Empty);
                }
            }
            else
            {
                valorconversion.Text = "0.00";
            }
        }

        private void lblenviarcopia_Tapped(object sender, EventArgs e)
        {
            if (chkcorreo.IsChecked) { chkcorreo.IsChecked = false; }
            else { chkcorreo.IsChecked = true; }
        }

        private void cuentaa_TextChanged(object sender, TextChangedEventArgs e)
        {
            dropdowncuentas.IsVisible = false;
        }

        private void cuentaa_Focused(object sender, FocusEventArgs e)
        {
            dropdowncuentas.IsVisible = true;
        }

        private void ListaCuentas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Persistencia persistencia = (Persistencia)e.CurrentSelection[0];
            cuentaa.Text = persistencia.Campo;
        }

    }
}