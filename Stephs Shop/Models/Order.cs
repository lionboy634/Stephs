using System;

namespace Stephs_Shop.Models
{
	public class Order
	{ 
		public int Id { get; set; }
		public decimal Total { get; set; }
		public bool Delivered { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }	
		public DateTimeOffset DeletedAt { get; set; }

	}
	public class OrderItems
	{
		public int Id { get; set; }
		public string orderId { get; set; }
		public int quantity { get; set;}
	}
}
