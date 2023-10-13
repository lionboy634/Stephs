using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stephs_Shop.Models
{
    public class Cart
    {
        public int id { get; set; }
        public Product product { get; set; }
        public string sessionId { get; set; }
		public int Quantity { get; set; }

        public decimal price => product.price * Quantity;
	}
}
