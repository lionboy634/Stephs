using Stephs_Shop.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System;
namespace Stephs_Shop.Models
{
    public class CustomerSession
    {
        public string Id { get; set; }
        public string Customer { get; set; }
        public int Total { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
