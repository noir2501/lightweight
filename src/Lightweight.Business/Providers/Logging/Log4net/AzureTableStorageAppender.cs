using System;
using System.Data.Services.Client;
using Elmah;
using Lightweight.Business.Exceptions;
using Lightweight.Business.Providers.Logging.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using log4net.Appender;
using log4net.Core;

namespace Lightweight.Business.Providers.Logging.Log4net
{
    public class AzureTableStorageAppender : AppenderSkeleton
    {
        public string storageConnectionString { get; set; }
        public string storageTableName { get; set; }

        private LogServiceContext _ctx;

        public override void ActivateOptions()
        {
            base.ActivateOptions();
            System.Diagnostics.Trace.Write("Initializing Azure table storage appender...");

            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
            CloudTableClient tclient = new CloudTableClient(new Uri(account.TableEndpoint.AbsoluteUri), account.Credentials);

            if (!string.IsNullOrEmpty(storageTableName))
                tclient.GetTableReference(storageTableName).CreateIfNotExists();
            else
                throw new ArgumentException("Logs table name not defined in configuration.", "StorageTableName");

            _ctx = new LogServiceContext(account.TableEndpoint.AbsoluteUri, account.Credentials, storageTableName);
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            try
            {
                Guid Id = Guid.NewGuid();

                // if exception is of type BusinessException, use the Correlation ID as the row key for the log entry
                if (loggingEvent.ExceptionObject != null && loggingEvent.ExceptionObject.GetType().IsAssignableFrom(typeof(BusinessException)))
                {
                    Id = ((BusinessException)loggingEvent.ExceptionObject).CorrelationID;
                }

                AzureLogEntry logEntry = new AzureLogEntry(DateTime.Now, Id)
                {
                    Logger = loggingEvent.LoggerName,
                    Timestamp = loggingEvent.TimeStamp,
                    Message = loggingEvent.RenderedMessage,
                    Level = loggingEvent.Level.Name,
                    Source = loggingEvent.LocationInformation.FullInfo,
                    User = loggingEvent.UserName
                };

                Exception exception = loggingEvent.ExceptionObject as LogException ?? new LogException(loggingEvent.RenderedMessage);
                logEntry.ErrorXml = ErrorXml.EncodeString(new Error(exception));

                _ctx.Log(logEntry);
            }
            catch (Exception e)
            {
                ErrorHandler.Error(string.Format("{0}: Could not write log entry to {1}: {2}",
                    GetType().AssemblyQualifiedName, storageTableName, e.Message));
            }
        }
    }
}
