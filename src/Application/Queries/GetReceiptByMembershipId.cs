using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;

namespace Application.Queries
{
	public class GetReceiptByMembershipId : IRequest<ReceiptModel>
	{
		private readonly string _membershipId;

		public GetReceiptByMembershipId(string membershipId)
		{
			_membershipId = membershipId;
		}

		public sealed class Handler : IRequestHandler<GetReceiptByMembershipId, ReceiptModel>
		{
			private readonly IRebateApi _rebateApi;
			private readonly ISubsidyApi _subsidyApi;
			private readonly IDiscountApi _discountApi;

			public Handler(IRebateApi rebateApi, ISubsidyApi subsidyApi, IDiscountApi discountApi)
			{
				_rebateApi = rebateApi;
				_subsidyApi = subsidyApi;
				_discountApi = discountApi;
			}

			public async Task<ReceiptModel> Handle(GetReceiptByMembershipId request, CancellationToken cancellationToken)
			{
				// Fan out
				Task<double> rebate = _rebateApi.GetRebate(request._membershipId);
				Task<double> subsidy = _subsidyApi.GetSubsidy(request._membershipId);
				Task<double> discount = _discountApi.GetDiscount(request._membershipId);

				// Fan in
				double[] results = await Task.WhenAll(rebate, subsidy, discount);

				// build new domain entity
				var r = new Receipt(results[0], results[1], results[2]);

				// project to query model
				// Use imapper instead
				return new ReceiptModel()
				{
					DiscountPercentage = r.DiscountPercentage,
					MembershipId = r.MembershipId,
					ReceiptId = r.ReceiptId,
					FinalAmount = r.Calculate()
				};
			}
		}
	}
	
}
