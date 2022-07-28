using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProyectoFinal.Models;

namespace ProyectoFinal.Api
{
    public class UsuarioApi
    {
        private static readonly string URL_SITIOS = "https://pm2examen2.000webhostapp.com/apiproyecto/";
        private static HttpClient client = new HttpClient();

        public static async Task<List<Usuario>> GetAllSite()
        {
            List<Usuario> ListaUsuarios = new List<Usuario>();
            try
            {
                var uri = new Uri(URL_SITIOS + "listausuarios.php");
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    ListaUsuarios = JsonConvert.DeserializeObject<List<Usuario>>(content);
                    return ListaUsuarios;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return ListaUsuarios;
        }

        public async static Task<bool> DeleteSite(string id)
        {
            try
            {
                var uri = new Uri(URL_SITIOS + "eliminarsitio.php?id=" + id);
                var result = await client.GetAsync(uri);
                if (result.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }


        public async static Task<bool> CreateUsuario(Usuario usuario)
        {
            try
            {
                Uri requestUri = new Uri(URL_SITIOS + "crearusuario.php");
                var jsonObject = JsonConvert.SerializeObject(usuario);
                var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(requestUri, content);

                Console.WriteLine(response.ToString());

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }


        public async static Task<bool> UpdateSitio(Usuario sitio)
        {
            try
            {
                Uri requestUri = new Uri(URL_SITIOS + "actualizarsitio.php");
                var jsonObject = JsonConvert.SerializeObject(sitio);
                var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                //var response = await client.PutAsync(requestUri, content);
                var response = await client.PostAsync(requestUri, content);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
    }
}
