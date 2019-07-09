using System.Threading.Tasks;

namespace Application
{
	public interface ISubsidyApi
	{
		Task<double> GetSubsidy(string membershipId);
	}
}