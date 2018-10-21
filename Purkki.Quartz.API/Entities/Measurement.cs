using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Purkki.Quartz.API.Entities
{
	public class Measurement : TableEntity
	{
		public DateTime Start { get; set; }
		public DateTime Stop { get; set; }
		public bool Running { get; set; }
	}
}
