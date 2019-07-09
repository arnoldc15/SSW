using System;
using System.Net.Http;
using System.Threading.Tasks;
using Application;
using Microsoft.Extensions.Configuration;

namespace Infrastructure
{
	public class RealRebateApi : IRebateApi
	{
		private readonly IConfiguration _config;
		private readonly HttpClient _client;

		public RealRebateApi(IConfiguration config, HttpClient client)
		{
			_config = config;
			_client = client;
		}

		public async Task<double> GetRebate(string membershipId)
		{
			string rebateUrl = _config["rebateUrl"];

			HttpResponseMessage resp1 = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Get, new Uri("http://api.icndb.com/jokes/random")));return (await resp1.Content.ReadAsStringAsync()).Length;
		}
	}
}