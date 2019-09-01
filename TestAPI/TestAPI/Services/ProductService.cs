using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestAPI.Entities;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Newtonsoft.Json;

namespace TestAPI.Services
{
    
    public class ProductService : IProductService
    {
        public User GetUserNameAndToken(IConfiguration configuration)
        {
            var user = new User();
            user.name = configuration.GetValue<string>("ApiSettings:UserName");
            user.token = configuration.GetValue<string>("ApiSettings:Token"); // "11deb9c1-cc4f-4634-a2c8-41fcfe0f1e56";
            return user;
        }

        public List<Product> GetSortedProducts(string sortOption,IConfiguration configuration)
        {
            var user = new User();
            user.name = configuration.GetValue<string>("ApiSettings:UserName");
            user.token = configuration.GetValue<string>("ApiSettings:Token"); // "11deb9c1-cc4f-4634-a2c8-41fcfe0f1e56";
            string productsUrl = configuration.GetValue<string>("ApiSettings:BaseUrl") + "/" +
                         configuration.GetValue<string>("ApiSettings:ProductsUrl") +
                         "?token=" + user.token;
            string shopperHistoryUrl = configuration.GetValue<string>("ApiSettings:BaseUrl") + "/" +
                         configuration.GetValue<string>("ApiSettings:shopperHistoryUrl") +
                         "?token=" + user.token;


            string response = "";
            List<Product> productsList;
            GetProductList(productsUrl, out response, out productsList);

            if (sortOption == SortOption.Low.ToString())
            {
                productsList = productsList.OrderBy(p => p.price).ToList();
            }
            else if (sortOption == SortOption.High.ToString())
            {
                productsList = productsList.OrderByDescending(p => p.price).ToList();
            }
            else if (sortOption == SortOption.Ascending.ToString())
            {
                productsList = productsList.OrderBy(p => p.name).ToList();
            }
            else if (sortOption == SortOption.Descending.ToString())
            {
                productsList = productsList.OrderByDescending(p => p.name).ToList();
            }
            else if (sortOption == SortOption.Recommended.ToString())
            {
                List<ShopperHistory> shopperHistory;
                GetShopperHistory(shopperHistoryUrl, out response, out shopperHistory);
                PopulateQuantityInProductListBasedOnShopperHistory(productsList, shopperHistory);
                productsList = productsList.OrderByDescending(p => p.quantity).ToList();

            }

            return productsList;
        }

        private static string Fetchdata(string url)
        {
            string response;
            using (var httpClient = new HttpClient())
            {
                response = httpClient.GetStringAsync(new Uri(url)).Result;
            }

            return response;
        }

        private static void GetProductList(string productsUrl, out string response, out List<Product> productsList)
        {
            response = Fetchdata(productsUrl);

            productsList = JsonConvert.DeserializeObject<List<Product>>(response);
        }

        private static void GetShopperHistory(string shopperHistoryUrl, out string response, out List<ShopperHistory> shopperHistory)
        {
            
            response = Fetchdata(shopperHistoryUrl);
            shopperHistory = JsonConvert.DeserializeObject<List<ShopperHistory>>(response);
        }

        private static void PopulateQuantityInProductListBasedOnShopperHistory(List<Product> productsList, List<ShopperHistory> shopperHistory)
        {
            foreach (var history in shopperHistory)
            {
                foreach (var product in history.products)
                {
                    var foundproduct = productsList.Where(x => x.name == product.name).FirstOrDefault();
                    if (foundproduct != null)
                    {
                        foundproduct.quantity = foundproduct.quantity + product.quantity;
                    }
                }
            }
        }

    }
}
