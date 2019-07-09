using System;
using System.Threading.Tasks;

namespace Application
{
	public interface IDiscountApi
	{
		Task<double> GetDiscount(string membershipId);
	}

}
