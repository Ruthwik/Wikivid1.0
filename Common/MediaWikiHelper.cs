using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Wikivid1._0.Common
{
    class MediaWikiHelper
    {
        public static bool CheckIfExists(string _Query)
        {
            string query = Uri.EscapeDataString(_Query);
            string URL = @"http://en.wikipedia.org/w/api.php?action=query&format=xml&titles=" + query + "&continue";
           
          /*  using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // New code:
                Task<HttpResponseMessage> response = client.GetAsync("");
               // if (response.IsSuccessStatusCode)
               // {
                    //Product product = await response.Content.ReadAsAsync>Product>();
                    //Console.WriteLine("{0}\t${1}\t{2}", product.Name, product.Price, product.Category);
               // }
            }*/
            return false;
        }
    }
}
