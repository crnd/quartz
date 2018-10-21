using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Purkki.Quartz.API.Entities;
using System;
using System.Threading.Tasks;

namespace Purkki.Quartz.API.Functions
{
	public static class StartTimer
	{
		[FunctionName("StartTimer")]
		public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "start")]HttpRequest req)
		{
			var measurement = new Measurement
			{
				PartitionKey = Constants.MeasurementDefaultPartitionKey,
				RowKey = Guid.NewGuid().ToString(),
				Start = DateTime.UtcNow,
				Stop = Helpers.MinimumTableStorageDateTime(),
				Running = true
			};
			var client = Helpers.GetCloudTableClient();
			await Helpers.InsertMeasurementAsync(client, measurement);
			
			return new OkObjectResult(new { Id = measurement.RowKey });
		}
	}
}
