using System;
using System.Net.Http;
using System.Threading.Tasks;
using Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace FunctionApp1
{
	public class FunctionWithHttp
	{
		private readonly MyReceiptBuilder _svc;
		private readonly HttpClient _httpClient;

		public FunctionWithHttp(MyReceiptBuilder svc, HttpClient httpClient	)
		{
			_svc = svc;
			_httpClient = httpClient;
		}

		[FunctionName(nameof(CalculateDocket))]
		public async Task<IActionResult> CalculateDocket(
			[HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequestMessage req,
			ILogger log)
		{
			ReceiptRequestDto receiptRequestDto = await req.Content.ReadAsAsync<ReceiptRequestDto>();

			log.LogInformation($"Started docket builder with ID = '{receiptRequestDto.MembershipId}'.");

			// http://api.icndb.com/jokes/random?firstName=John&amp;lastName=Doe

			// Fan Out
			Task<HttpResponseMessage> resp1 = _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, new Uri("http://api.icndb.com/jokes/random")));
			Task<HttpResponseMessage> resp2 = _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, new Uri("http://api.icndb.com/jokes/random")));
			Task<HttpResponseMessage> resp3 = _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, new Uri("http://localhost:7071/api/CalculateSomething")));

			// Fan In
			HttpResponseMessage[] results = await Task.WhenAll(resp1, resp2, resp3);

			// Aggregate
			var final = "";
			foreach (HttpResponseMessage res in results)
			{
				final += "\n" + await res.Content.ReadAsStringAsync();
			}

			string resp4 = await _svc.CallOutToMicroservice();
			final += "\n" + resp4;

			return new OkObjectResult(final);
		}
	}

	public class FunctionWithHttp2
	{
		[FunctionName(nameof(CalculateSomething))]
		public IActionResult CalculateSomething(
			[HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequestMessage req,
			ILogger log)
		{
			log.LogInformation($"Started something ....");

			return new OkObjectResult("I did something");
		}
	}
}