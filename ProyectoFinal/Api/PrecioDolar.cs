using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProyectoFinal.Models;

namespace ProyectoFinal.Api
{
    public class PrecioDolar
    {
        private static readonly string URL_SITIOS = "https://pm2examen2.000webhostapp.com/apiproyecto/";
        private static HttpClient client = new HttpClient();

        public static async Task<Dolar> GetPrecioDolar(string fecha)
        {
            List<Dolar> precio = new List<Dolar>();

            try
            {
                var uri = new Uri(URL_SITIOS + "listasingleprecio.php?fecha='"+fecha+"'");
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    precio = JsonConvert.DeserializeObject<List<Dolar>>(content);
                    return precio[0];
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return precio[0];
        }

        public static async Task<List<Dolar>> GetListaPrecioDolar()
        {
            List<Dolar> precios = new List<Dolar>();

            try
            {
                var uri = new Uri(URL_SITIOS + "listaprecio.php");
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    precios = JsonConvert.DeserializeObject<List<Dolar>>(content);
                    return precios;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return precios;
        }
    }
}
