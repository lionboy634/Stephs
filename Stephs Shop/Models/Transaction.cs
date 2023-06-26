using System;

namespace Stephs_Shop.Models
{
	public class Transaction
	{
		public Guid TransactionId { get; set; }
		public string CustomerId { get; set; }
		public string SessionId { get; set; }
		public long OrderId { get; set; }
		public DateTime? CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
	}
}
