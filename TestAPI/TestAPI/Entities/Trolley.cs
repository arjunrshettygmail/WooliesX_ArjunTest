using System.Collections.Generic;

namespace TestAPI.Entities
{
    public class Trolley
    {
        public List<TrolleyProduct> products { get; set; }
        public List<TrolleySpecial> specials { get; set; }
        public List<TrolleyQuantity> quantities { get; set; }
    }


}
