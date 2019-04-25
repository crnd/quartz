using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Purkki.Quartz.API.Entities;
using System;
using System.Threading.Tasks;

namespace Purkki.Quartz.API
{
	public static class Helpers
	{
		public static CloudTableClient GetCloudTableClient()
		{
			var storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
			return storageAccount.CreateCloudTableClient();
		}

		public static async Task<Measurement> GetMeasurementAsync(CloudTableClient client, string id)
		{
			if (client == null)
			{
				throw new ArgumentNullException(nameof(client));
			}

			if (string.IsNullOrWhiteSpace(id))
			{
				throw new ArgumentNullException(nameof(id));
			}

			return await GetTableEntityAsync<Measurement>(
				client,
				Constants.MeasurementsTableName,
				Constants.MeasurementDefaultPartitionKey,
				id);
		}

		public static async Task InsertMeasurementAsync(CloudTableClient client, Measurement measurement)
		{
			if (client == null)
			{
				throw new ArgumentNullException(nameof(client));
			}

			if (measurement == null)
			{
				throw new ArgumentNullException(nameof(measurement));
			}

			await InsertTableEntityAsync(client, Constants.MeasurementsTableName, measurement);
		}

		public static async Task ReplaceMeasurementAsync(CloudTableClient client, Measurement measurement)
		{
			if (client == null)
			{
				throw new ArgumentNullException(nameof(client));
			}

			if (measurement == null)
			{
				throw new ArgumentNullException(nameof(measurement));
			}

			await ReplaceTableEntityAsync(client, Constants.MeasurementsTableName, measurement);
		}

		private static async Task<T> GetTableEntityAsync<T>(CloudTableClient client, string tableName, string partitionKey, string rowKey) where T : TableEntity
		{
			if (client == null)
			{
				throw new ArgumentNullException(nameof(client));
			}

			if (string.IsNullOrWhiteSpace(tableName))
			{
				throw new ArgumentNullException(nameof(tableName));
			}

			if (string.IsNullOrWhiteSpace(partitionKey))
			{
				throw new ArgumentNullException(nameof(partitionKey));
			}

			if (string.IsNullOrWhiteSpace(rowKey))
			{
				throw new ArgumentNullException(nameof(rowKey));
			}

			var table = client.GetTableReference(tableName);
			var retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
			var result = await table.ExecuteAsync(retrieveOperation);
			if (result == null)
			{
				return default(T);
			}

			return (T)result.Result;
		}

		private static async Task InsertTableEntityAsync<T>(CloudTableClient client, string tableName, T entity) where T : TableEntity
		{
			if (client == null)
			{
				throw new ArgumentNullException(nameof(client));
			}

			if (string.IsNullOrWhiteSpace(tableName))
			{
				throw new ArgumentNullException(nameof(tableName));
			}

			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			var table = client.GetTableReference(tableName);
			try
			{
				await table.ExecuteAsync(TableOperation.Insert(entity));
			}
			catch (StorageException e) when (e.RequestInformation.ExtendedErrorInformation.ErrorCode == Constants.TableNotFoundError)
			{
				await table.CreateIfNotExistsAsync();
				await table.ExecuteAsync(TableOperation.Insert(entity));
			}
		}

		private static async Task ReplaceTableEntityAsync<T>(CloudTableClient client, string tableName, T entity) where T : TableEntity
		{
			if (client == null)
			{
				throw new ArgumentNullException(nameof(client));
			}

			if (string.IsNullOrWhiteSpace(tableName))
			{
				throw new ArgumentNullException(nameof(tableName));
			}

			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			var table = client.GetTableReference(tableName);
			await table.ExecuteAsync(TableOperation.Replace(entity));
		}

		public static int GetMeasurementDuration(Measurement measurement, DateTime now)
		{
			if (measurement == null)
			{
				throw new ArgumentNullException(nameof(measurement));
			}

			if (measurement.Running)
			{
				return (int)(now - measurement.Start).TotalSeconds;
			}

			return (int)(measurement.Stop - measurement.Start).TotalSeconds;
		}

		/// <summary>
		/// Returns the minimum <see cref="DateTime"/> value that Azure Table Storage supports.
		/// </summary>
		/// <remarks>
		/// https://docs.microsoft.com/en-us/rest/api/storageservices/Understanding-the-Table-Service-Data-Model#property-types
		/// </remarks>
		public static DateTime MinimumTableStorageDateTime() => new DateTime(1601, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
	}
}
