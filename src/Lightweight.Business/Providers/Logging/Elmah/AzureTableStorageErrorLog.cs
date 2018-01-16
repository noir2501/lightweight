using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elmah;
using Lightweight.Business.Exceptions;
using Lightweight.Business.Providers.Logging.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using ApplicationException = System.ApplicationException;

namespace Lightweight.Business.Providers.Logging.Elmah
{
    public class AzureTableStorageErrorLog : ErrorLog
    {

        private string storageConnectionString { get; set; }
        private string storageTableName { get; set; }

        private readonly LogServiceContext _ctx;

        public AzureTableStorageErrorLog(IDictionary config)
        {
            if (!(config["storageConnectionString"] is string) ||
                string.IsNullOrWhiteSpace((string)config["storageConnectionString"]))
            {
                throw new ApplicationException("Connection string is missing for the Windows Azure error log.");
            }

            storageConnectionString = config["storageConnectionString"] as string;

            if (!(config["storageTableName"] is string) ||
                string.IsNullOrWhiteSpace((string)config["storageTableName"]))
            {
                throw new ApplicationException("Table name is missing for the Windows Azure error log.");
            }

            storageTableName = config["storageTableName"] as string;

            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
            CloudTableClient tclient = new CloudTableClient(new Uri(account.TableEndpoint.AbsoluteUri),
                                                            account.Credentials);

            if (!string.IsNullOrEmpty(storageTableName))
                tclient.GetTableReference(storageTableName).CreateIfNotExists();
            else
                throw new ArgumentException("Logs table name not defined in configuration.", "StorageTableName");

            _ctx = new LogServiceContext(account.TableEndpoint.AbsoluteUri, account.Credentials, storageTableName);
        }

        public override string Log(Error error)
        {
            Guid Id = Guid.NewGuid();

            // if exception is of type BusinessException, use the Correlation ID as the row key for the log entry
            if (error.Exception != null && error.Exception.GetType().IsAssignableFrom(typeof(BusinessException)))
            {
                Id = ((BusinessException)error.Exception).CorrelationID;
            }

            AzureLogEntry entity = new AzureLogEntry(error.Time, Id)
                                       {
                                           Host = error.HostName,
                                           Type = error.Type,
                                           ErrorXml = ErrorXml.EncodeString(error),
                                           Message = error.Message,
                                           Code = error.StatusCode,
                                           User = error.User,
                                           Source = error.Source,
                                           Logger = "Elmah"
                                       };

            _ctx.AddObject(storageTableName, entity);
            _ctx.SaveChanges();

            return entity.ID.ToString();
        }

        public override ErrorLogEntry GetError(string id)
        {
            var query = from entity in _ctx.CreateQuery<AzureLogEntry>(storageTableName)
                        where AzureLogEntry.GetRowKey(Guid.Parse(id)) == entity.RowKey
                        select entity;

            AzureLogEntry errorEntity = query.FirstOrDefault();
            if (errorEntity == null)
            {
                return null;
            }

            return new ErrorLogEntry(this, id, GetElmahError(errorEntity.ErrorXml));
        }

        public override int GetErrors(int pageIndex, int pageSize, IList errorEntryList)
        {
            if (pageIndex < 0)
                throw new ArgumentOutOfRangeException("pageIndex", pageIndex, null);

            if (pageSize < 0)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, null);

            // WWB: Server Side Call To Get All Data
            AzureLogEntry[] serverSideQuery = _ctx.CreateQuery<AzureLogEntry>(storageTableName).Execute().ToArray();

            // WWB: Sorted in Reverse Order So Oldest are First
            var sorted = serverSideQuery.OrderByDescending(entity => entity.TimeUtc);

            // WWB: Trim To Just a Page From The End
            AzureLogEntry[] page = sorted.Skip(pageIndex * pageSize).Take(pageSize).ToArray();

            // WWB: Convert To ErrorLogEntry classes From Windows Azure Table Entities
            IEnumerable<ErrorLogEntry> errorLogEntries =
                page.Select(
                    errorEntity =>
                    new ErrorLogEntry(this, errorEntity.ID.ToString(), GetElmahError(errorEntity.ErrorXml)));

            // WWB: Stuff them into the class we were passed
            foreach (var errorLogEntry in errorLogEntries)
            {
                errorEntryList.Add(errorLogEntry);
            }
            ;

            return serverSideQuery.Length;
        }

        private static Error GetElmahError(string errorXml)
        {
            if (!string.IsNullOrEmpty(errorXml))
                return ErrorXml.DecodeString(errorXml);
            return new Error(new Exception("Unknown error format."));
        }
    }
}