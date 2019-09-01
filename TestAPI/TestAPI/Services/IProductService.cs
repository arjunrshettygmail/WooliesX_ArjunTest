using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Edm.Expressions;
using Microsoft.Extensions.Configuration;
using TestAPI.Entities;

namespace TestAPI.Services
{
    

    public interface IProductService
    {

        User GetUserNameAndToken(IConfiguration configuration);
        List<Product> GetSortedProducts(string sortOption, IConfiguration configuration);


    }


}
