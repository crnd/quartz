using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System;
using System.Threading.Tasks;

namespace Purkki.Quartz.API.Functions
{
	public static class StopTimer
	{
		[FunctionName("StopTimer")]
		public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "{id}/stop")]HttpRequest req, string id)
		{
			var now = DateTime.UtcNow;
			var client = Helpers.GetCloudTableClient();
			var measurement = await Helpers.GetMeasurementAsync(client, id);
			if (measurement == null)
			{
				return new NotFoundResult();
			}

			if (!measurement.Running)
			{
				return new BadRequestResult();
			}

			measurement.Stop = now;
			measurement.Running = false;
			await Helpers.ReplaceMeasurementAsync(client, measurement);

			return new OkResult();
		}
	}
}
