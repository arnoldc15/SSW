using System;
using System.Threading.Tasks;
using Domain;

namespace Application
{
	public class MyReceiptBuilder
	{
		private readonly IRebateApi _rebateApi;
		private readonly ISubsidyApi _subsidyApi;
		private readonly IDiscountApi _discountApi;

		public MyReceiptBuilder(
			IRebateApi rebateApi,
			ISubsidyApi subsidyApi,
			IDiscountApi discountApi)
		{
			_rebateApi = rebateApi;
			_subsidyApi = subsidyApi;
			_discountApi = discountApi;
		}

		public Task<string> CallOutToMicroservice()
		{
			return Task.FromResult("I am a result from another microservice");
		}

		public async Task<Receipt> CalculateReceipt(string membershipId)
		{
			// Fan out
			Task<double> rebate = _rebateApi.GetRebate(membershipId);
			Task<double> subsidy = _subsidyApi.GetSubsidy(membershipId);
			Task<double> discount = _discountApi.GetDiscount(membershipId);

			// Fan in
			double[] results = await Task.WhenAll(rebate, subsidy, discount);

			return new Receipt()
			{
				RebateAmount = results[0],
				SubsidyAmount = results[1],
				DiscountPercentage = results[2],
				MembershipId = membershipId,
				ReceiptId = Guid.NewGuid().ToString("N"),
				FinalAmount = 123
			};
		}
	}
}