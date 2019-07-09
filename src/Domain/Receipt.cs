namespace Domain
{
	public class Receipt
	{
		public Receipt()
		{

		}

		public Receipt(double rebate, double subsidy, double discount)
		{
			this.RebateAmount = rebate;
			this.SubsidyAmount = subsidy;
			this.DiscountPercentage = discount;
		}

		public string ReceiptId { get; set; }
		public string MembershipId { get; set; }
		public double RebateAmount { get; set; }
		public double SubsidyAmount { get; set; }
		public double DiscountPercentage { get; set; }
		public double FinalAmount { get; set; }

		public double Calculate()
		{
			return (123 - (this.RebateAmount + this.SubsidyAmount)) * this.DiscountPercentage;
		}
	}
}