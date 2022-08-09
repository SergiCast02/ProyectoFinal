using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinal.Api
{
    public class HoraUniversal
    {
        private static readonly string URL_SITIO = "http://worldtimeapi.org/api/timezone/America/Tegucigalpa";
        private static HttpClient client = new HttpClient();

        public static async Task<string> getHoraUTC()
        {
            try
            {
                var uri = new Uri(URL_SITIO);
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    //return JsonConvert.DeserializeObject<string>(content);
                    return content;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

    }
}
