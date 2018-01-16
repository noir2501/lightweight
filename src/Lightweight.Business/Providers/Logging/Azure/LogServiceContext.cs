using System.Linq;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.DataServices;
using System;

namespace Lightweight.Business.Providers.Logging.Azure
{
    internal class LogServiceContext : TableServiceContext
    {
        private readonly string _table;

        public LogServiceContext(string baseAddress, StorageCredentials credentials, string table)
            : base(new CloudTableClient(new Uri(baseAddress), credentials))
        {
            _table = table;
        }

        private LogServiceContext(string baseAddress, StorageCredentials credentials)
            : base(new CloudTableClient(new Uri(baseAddress), credentials))
        {

        }

        internal void Log(AzureLogEntry logEntry)
        {
            AddObject(_table, logEntry);
            SaveChanges();
        }
        public IQueryable<AzureLogEntry> LogEntries
        {
            get
            {
                return CreateQuery<AzureLogEntry>(_table);
            }
        }
    }
}
