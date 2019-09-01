using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TestAPI.Entities;
using TestAPI.Services;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace TestAPI.Controllers
{
    [Route("api/answers")]
    public class ValuesController : Controller
    {


        private readonly IProductService _productService;
        private IConfiguration configuration;

        

        public ValuesController(IConfiguration iConfig , IProductService productService)
        {
            configuration = iConfig;
            _productService = productService;
        }

        [HttpGet("Get")]
        public string Get()
        {
            return "Products Api";

        }


        [HttpGet("user")]
        public ActionResult getUser()
        {
            try
            {
                User user = _productService.GetUserNameAndToken(configuration);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest("An error has occured required.");
            }

        }

        

        [HttpGet("sort")]
        public ActionResult SortProducts(string sortOption)
        {
            try
            {
                List<Product> productsList = _productService.GetSortedProducts(sortOption, configuration);

                return Ok(productsList);
            }
            catch (Exception ex)
            {
                return BadRequest("An error has occured required.");
            }

        }

        

        [HttpPost("trolleyTotal")]
        public int PostTrolleyTotal([FromBody]Trolley trolley)
        {
            var user = new User();
            user.name = configuration.GetValue<string>("ApiSettings:UserName");
            user.token = configuration.GetValue<string>("ApiSettings:Token"); 
            string trolleyCalculatorUrl = configuration.GetValue<string>("ApiSettings:BaseUrl") + "/" +
                         configuration.GetValue<string>("ApiSettings:trolleyCalculatorUrl") +
                         "?token=" + user.token;

            string response = "";

            using (var httpClient = new HttpClient())
            {
                Uri u = new Uri(trolleyCalculatorUrl);

                string strPayload = JsonConvert.SerializeObject(trolley);
                HttpContent c = new StringContent(strPayload, Encoding.UTF8, "application/json");
                var t = Task.Run(() => SendURI(u, c));
                t.Wait();
                response = t.Result;

            }

            double total;
            bool res = double.TryParse(response, out total);
            if (res == false)
            {
                total = 0;
            }

            return Convert.ToInt32(total);

        }

        static async Task<string> SendURI(Uri u, HttpContent c)
        {
            var response = string.Empty;
            using (var client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = u,
                    Content = c
                };

                HttpResponseMessage result = await client.SendAsync(request);
                if (result.IsSuccessStatusCode)
                {
                    response = await result.Content.ReadAsStringAsync();
                }
            }
            return response;
        }

    }
}
