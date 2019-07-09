using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace RebateFunctionApp
{
    public static class Function2
    {
	    [FunctionName("Function2_HttpStart")]
	    public static async Task<HttpResponseMessage> HttpStart(
		    [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req,
		    [OrchestrationClient]DurableOrchestrationClient starter,
		    ILogger log)
	    {
		    // Function input comes from the request content.
		    string instanceId = await starter.StartNewAsync("Function2", null);

		    log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

		    return starter.CreateCheckStatusResponse(req, instanceId);
	    }

		[FunctionName("Function2")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context,
            ILogger log)
        {
            var outputs = new List<string>();

			// Orchestrations are Deterministic
			// NO reading of config
			// No randomness
			// No Guids
			// No datetime.now.. use context.CurrentUtcDateTime
			// CAREFUL ABOUT LOGGING
			
            // Replace "hello" with the name of your Durable Activity Function.
            outputs.Add(await context.CallActivityAsync<string>("Function2_Hello", ""));
            outputs.Add(await context.CallActivityAsync<string>("Function2_Hello", "Seattle"));

            //if (!context.IsReplaying)
            //{
	           // log.LogInformation("hello");
            //}

			outputs.Add(await context.CallActivityAsync<string>("Function2_Hello", "London"));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        [FunctionName("Function2_Hello")]
        public static string SayHello([ActivityTrigger] string name, ILogger log)
        {
	        log.LogInformation($"Saying hello to {name}.");
	        return $"Hello {name}!";
        }
    }
}