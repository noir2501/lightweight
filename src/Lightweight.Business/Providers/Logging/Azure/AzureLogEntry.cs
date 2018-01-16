using System;
using Microsoft.WindowsAzure.Storage.Table.DataServices;

namespace Lightweight.Business.Providers.Logging.Azure
{
    public class AzureLogEntry : TableServiceEntity, ILogEntry
    {
        /// <summary>
        /// Given a DateTime Return a Parition Key
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string GetParitionKey(DateTime time)
        {
            return time.ToString("yyyyMMddHH");
        }

        public static string GetRowKey(Guid id)
        {
            return id.ToString().Replace("-", "").ToLower();
        }

        public AzureLogEntry(DateTime timeUtc, Guid Id)
            : base(GetParitionKey(timeUtc), GetRowKey(Id))
        {
            TimeUtc = timeUtc;
            ID = Id;
        }

        protected AzureLogEntry(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {
        }

        [Obsolete("Provided For Serialization From Windows Azure Do No Call Directly")]
        public AzureLogEntry()
        {

        }

        public Guid ID { get; set; }
        public string Message { get; set; }

        public string Host { get; set; }
        public string Type { get; set; }
        public string Level { get; set; }
        public string Source { get; set; }

        public string User { get; set; }
        public DateTime TimeUtc { get; set; }
        public int Code { get; set; }
        public string ErrorXml { get; set; }

        public string Logger { get; set; }
       
    }
}
