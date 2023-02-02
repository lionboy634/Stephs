using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stephs_Shop.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
        public DateTimeOffset DeletedAt { get; set; }
        public bool IsDeleted { get; set; }

    }
}
