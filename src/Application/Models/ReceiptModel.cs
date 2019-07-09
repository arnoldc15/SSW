using System;

namespace Application.Queries
{
	public class ReceiptModel
	{
		public DateTimeOffset GeneratedAt { get; set; }

		public string ReceiptId { get; set; }
		public string MembershipId { get; set; }
		public double DiscountPercentage { get; set; }
		public double FinalAmount { get; set; }
	}
}