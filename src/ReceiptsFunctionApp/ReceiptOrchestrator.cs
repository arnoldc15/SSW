using System;
using System.Net.Http;
using System.Threading.Tasks;
using Application;
using Domain;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace FunctionApp1
{
	public class DurableReceipt
	{
		private readonly IRebateApi _rebateApi;
		private readonly ISubsidyApi _subsidyApi;
		private readonly IDiscountApi _discountApi;

		public DurableReceipt(IRebateApi rebateApi,
			ISubsidyApi subsidyApi,
			IDiscountApi discountApi)
		{
			_rebateApi = rebateApi;
			_subsidyApi = subsidyApi;
			_discountApi = discountApi;
		}

		[FunctionName(nameof(CalculateReceipt))]
		public async Task<HttpResponseMessage> CalculateReceipt(
			[HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequestMessage req,
			[OrchestrationClient]DurableOrchestrationClient durableClient,
			ILogger log)
		{
			ReceiptRequestDto receiptRequestDto = await req.Content.ReadAsAsync<ReceiptRequestDto>();

			// Function input comes from the request content.
			string instanceId = await durableClient.StartNewAsync(nameof(RunReceiptOrchestrator), receiptRequestDto);

			log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

			return await durableClient.WaitForCompletionOrCreateCheckStatusResponseAsync(req, instanceId);

			// return durableClient.CreateCheckStatusResponse(req, instanceId);
		}

		[FunctionName(nameof(RunReceiptOrchestrator))]
		public async Task<Receipt> RunReceiptOrchestrator(
			[OrchestrationTrigger] DurableOrchestrationContext context)
		{
			var membershipDto = context.GetInput<ReceiptRequestDto>();

			context.SetCustomStatus("Getting membership...");
			Membership membership = await context.CallActivityAsync<Membership>(nameof(GetMembership), membershipDto.MembershipId);

			context.SetCustomStatus("Getting rebate...");
			double rebateAmount = await context.CallActivityAsync<double>(nameof(GetRebate), membership);

			context.SetCustomStatus("Getting subsidy...");
			double subsidyAmount = await context.CallActivityAsync<double>(nameof(GetSubsidy), membership);

			context.SetCustomStatus("Getting discount...");
			double discountPercentage = await context.CallActivityAsync<double>(nameof(GetDiscount), membership);

			context.SetCustomStatus("Building Receipt...");
			return new Receipt()
			{
				ReceiptId = Guid.NewGuid().ToString("N"),
				MembershipId = membership.MembershipId,
				FinalAmount = (123 + rebateAmount + subsidyAmount) * discountPercentage,
				DiscountPercentage = discountPercentage,
				RebateAmount = rebateAmount,
				SubsidyAmount = subsidyAmount
			};
		}
		
		[FunctionName(nameof(GetRebate))]
		public async Task<double> GetRebate([ActivityTrigger] Membership membership, ILogger log)
		{
			log.LogInformation("Getting Rebate for {membershipId}", membership.MembershipId);
			double rebate = await _rebateApi.GetRebate(membership.MembershipId);
			return rebate;
		}

		[FunctionName(nameof(GetSubsidy))]
		public async Task<double> GetSubsidy([ActivityTrigger] Membership membership, ILogger log)
		{
			log.LogInformation("Getting Subsidy for {membershipId}", membership.MembershipId);
			await Task.Delay(1000);
			return 0.74;
		}

		[FunctionName(nameof(GetDiscount))]
		public async Task<double> GetDiscount([ActivityTrigger] Membership membership, ILogger log)
		{
			log.LogInformation("Getting Discount for {membershipId}", membership.MembershipId);
			await Task.Delay(1000);
			return 0.2f;
		}
		
		[FunctionName(nameof(GetMembership))]
		public async Task<Membership> GetMembership([ActivityTrigger] string membershipId, ILogger log)
		{
			log.LogInformation($"Getting membership: {membershipId}.");

			await Task.Delay(1000);

			var membership = new Membership()
			{
				MembershipId = Guid.NewGuid().ToString("N"),
				MembershipLevel = 9000
			};
 
			return membership;
		}
	}
}