using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Messaging.EventHubs.Producer;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DAL.Models;
using DAL.Repositories;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace BLL.Eventhub
{
    public class ApprovedListener : IHostedService, IDisposable
    {
        private readonly EventProcessorClient processor;
        private readonly ILogger _logger;
        private IUnitOfWork _unitOfWork;
        private IServiceScopeFactory _serviceScopeFactory;

        public ApprovedListener(
            IConfiguration config, 
            ILogger<ApprovedListener> logger, 
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            string topic = config.GetValue<string>("EventHub:ApprovedEvent");
            string azureContainername = config.GetValue<string>("AzureStorage:AzureContainerEventHub");
            string eventHubConn = config.GetValue<string>("EventHub:ConnectionString");
            string azStorageConn = config.GetValue<string>("AzureStorage:ConnectionString");
            string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

            BlobContainerClient storageClient = new BlobContainerClient(azStorageConn, azureContainername);

            processor = new EventProcessorClient(storageClient, consumerGroup, eventHubConn, topic);

            processor.ProcessEventAsync += ProcessEventHandler;
            processor.ProcessErrorAsync += ProcessErrorHandler;
        }
        public async Task ProcessEventHandler(ProcessEventArgs eventArgs)
        {
            await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
            var string_data = Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray());
            JObject json_data = JObject.Parse(string_data);
            _logger.LogInformation(string_data);
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                IUnitOfWork _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var oid = new Guid(json_data["OrderId"].ToString());
                
                var orderFromDb = _unitOfWork.OrderRepository.GetAll().Where(x => x.Id == oid).First();
                orderFromDb.Status = "Approved";
                _unitOfWork.OrderRepository.Edit(orderFromDb);
                await _unitOfWork.SaveAsync();
               
                
                
                
                //Do your stuff
            }
        }

        public async Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
        {
            _logger.LogError(eventArgs.Exception.Message);

        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await processor.StartProcessingAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await processor.StopProcessingAsync();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~MessageListernerService()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion
    }
}
