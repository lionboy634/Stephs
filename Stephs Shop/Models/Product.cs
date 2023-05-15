using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stephs_Shop.Models
{
    public class Product
    {
        public int id { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }
        public string description { get; set; }
        public string image_url { get; set; }
        public int inventory_id { get; set; }
        public int category_id { get; set; }
        public DateTimeOffset created_at { get; set; }
        public DateTimeOffset modified_at { get; set; }
        public DateTimeOffset deleted_at { get; set; }

    }
}
