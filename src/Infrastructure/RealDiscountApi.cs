using System.Threading.Tasks;
using Application;

namespace Infrastructure
{
	public class RealDiscountApi : IDiscountApi
	{
		public Task<double> GetDiscount(string membershipId)
		{
			return Task.FromResult(0.1);
		}
	}
}
