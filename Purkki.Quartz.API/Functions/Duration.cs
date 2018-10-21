using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Threading.Tasks;

namespace Purkki.Quartz.API.Functions
{
	public static class Duration
	{
		[FunctionName("Duration")]
		public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "{id}")]HttpRequest req, string id)
		{
			var client = Helpers.GetCloudTableClient();
			var measurement = await Helpers.GetMeasurementAsync(client, id);
			if (measurement == null)
			{
				return new NotFoundResult();
			}

			return new OkObjectResult(new
			{
				Start = measurement.Start.ToString(Constants.ISO8601DateTimeFormat),
				Running = measurement.Running,
				Duration = Helpers.GetMeasurementDuration(measurement)
			});
		}
	}
}
