using System.Collections.Generic;

namespace TestAPI.Entities
{
    public class ShopperHistory
    {
        public int customerId { get; set; }
        public List<Product> products { get; set; }
    }


}
