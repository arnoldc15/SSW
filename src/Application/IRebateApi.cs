using System.Threading.Tasks;

namespace Application
{
	public interface IRebateApi
	{
		Task<double> GetRebate(string membershipId);
	}
}