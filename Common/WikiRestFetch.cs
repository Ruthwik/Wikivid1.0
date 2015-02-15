//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Text;
//using System.Threading.Tasks;

//namespace Wikivid1._0.Common
//{
//    public class WikiRestFetch
//    {
//        static void Main()
//        {
//            RunAsync().Wait();
//        }

//        static async Task RunAsync()
//        {
//            using (var client = new HttpClient())
//            {
//                // New code:
//                client.BaseAddress = new Uri("http://localhost:9000/");
//                client.DefaultRequestHeaders.Accept.Clear();
//                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
//                HttpResponseMessage response = await client.GetAsync("api/products/1");
//                if (response.IsSuccessStatusCode)
//                {
//                    Product product = await response.Content.ReadAsAsync>Product>();
//                    Console.WriteLine("{0}\t${1}\t{2}", product.Name, product.Price, product.Category);
//                }
//            }
//        }
//    }
//}
