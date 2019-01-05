using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Moq;
using Purkki.Quartz.API;
using Purkki.Quartz.API.Entities;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Purkki.Quartz.Tests
{
	public class HelpersUnitTests
	{
		private CloudTableClient GetGenericCloudTableClient() => new Mock<CloudTableClient>().Object;
		private readonly Uri Uri = new Uri("https://example.com");

		[Fact]
		public void GetMeasurementDurationThrowsIfMeasurementNull()
		{
			Assert.Throws<ArgumentNullException>(() => Helpers.GetMeasurementDuration(null, DateTime.UtcNow));
		}

		[Fact]
		public void GetMeasurementDurationReturnsStartStopDifferenceWhenNotRunning()
		{
			const int difference = 178;
			var now = DateTime.UtcNow;
			var start = now.AddDays(-50);
			var stop = start.AddSeconds(difference);
			var measurement = new Measurement
			{
				Start = start,
				Stop = stop,
				Running = false
			};

			Assert.StrictEqual(difference, Helpers.GetMeasurementDuration(measurement, now));
		}

		[Fact]
		public void GetMeasurementDurationReturnsStartNowDifferenceWhenRunning()
		{
			const int difference = 178;
			var now = DateTime.UtcNow;
			var start = now.AddSeconds(-difference);
			var measurement = new Measurement
			{
				Start = start,
				Running = true
			};

			Assert.StrictEqual(difference, Helpers.GetMeasurementDuration(measurement, now));
		}

		[Fact]
		public void GetMeasurementAsyncThrowsIfNullClient()
		{
			Assert.ThrowsAsync<ArgumentNullException>(() => Helpers.GetMeasurementAsync(null, Guid.NewGuid().ToString()));
		}

		[Fact]
		public void GetMeasurementAsyncThrowsIfNullId()
		{
			Assert.ThrowsAsync<ArgumentNullException>(() => Helpers.GetMeasurementAsync(GetGenericCloudTableClient(), null));
		}

		[Fact]
		public void GetMeasurementAsyncThrowsIfEmptyId()
		{
			Assert.ThrowsAsync<ArgumentNullException>(() => Helpers.GetMeasurementAsync(GetGenericCloudTableClient(), string.Empty));
		}

		[Fact]
		public void GetMeasurementAsyncThrowsIfWhiteSpaceId()
		{
			Assert.ThrowsAsync<ArgumentNullException>(() => Helpers.GetMeasurementAsync(GetGenericCloudTableClient(), "   "));
		}

		[Fact]
		public async Task GetMeasurementAsyncReturnsNullWhenNoMeasurementFound()
		{
			const string id = "abc-123";
			var table = new Mock<CloudTable>(Uri);
			table.Setup(t => t.ExecuteAsync(It.Is<TableOperation>(o => o.OperationType == TableOperationType.Retrieve)))
				.ReturnsAsync((TableResult)null);
			var client = new Mock<CloudTableClient>(Uri, new StorageCredentials());
			client.Setup(c => c.GetTableReference(It.Is<string>(s => s == Constants.MeasurementsTableName)))
				.Returns(table.Object);

			Assert.Null(await Helpers.GetMeasurementAsync(client.Object, id));
		}

		[Fact]
		public async Task GetMeasurementAsyncReturnsMeasurementWhenFound()
		{
			const string id = "abc-123";
			var table = new Mock<CloudTable>(Uri);
			table.Setup(t => t.ExecuteAsync(It.Is<TableOperation>(o => o.OperationType == TableOperationType.Retrieve)))
				.ReturnsAsync(new TableResult { Result = new Measurement() });
			var client = new Mock<CloudTableClient>(Uri, new StorageCredentials());
			client.Setup(c => c.GetTableReference(It.Is<string>(s => s == Constants.MeasurementsTableName)))
				.Returns(table.Object);

			Assert.IsType<Measurement>(await Helpers.GetMeasurementAsync(client.Object, id));
		}

		[Fact]
		public void InsertMeasurementAsyncThrowsIfNullClient()
		{
			Assert.ThrowsAsync<ArgumentNullException>(() => Helpers.InsertMeasurementAsync(null, new Measurement()));
		}

		[Fact]
		public void InsertMeasurementAsyncThrowsIfNullMeasurement()
		{
			Assert.ThrowsAsync<ArgumentNullException>(() => Helpers.InsertMeasurementAsync(GetGenericCloudTableClient(), null));
		}

		[Fact]
		public void ReplaceMeasurementAsyncThrowsIfNullClient()
		{
			Assert.ThrowsAsync<ArgumentException>(() => Helpers.ReplaceMeasurementAsync(null, new Measurement()));
		}

		[Fact]
		public void ReplaceMeasurementAsyncThrowsIfNullMeasurement()
		{
			Assert.ThrowsAsync<ArgumentNullException>(() => Helpers.ReplaceMeasurementAsync(GetGenericCloudTableClient(), null));
		}
	}
}
