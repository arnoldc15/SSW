using System.Threading.Tasks;
using Application;

namespace Infrastructure
{
	public class RealSubsidyApi : ISubsidyApi
	{
		public Task<double> GetSubsidy(string membershipId)
		{
			return Task.FromResult(10.0);
		}
	}
}