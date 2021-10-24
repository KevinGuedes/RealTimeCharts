using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OperationResult;
using RealTimeCharts.Microservices.ClientDispatcher.Interfaces;
using RealTimeCharts.Shared.Models;
using System;
using System.Threading.Tasks;

namespace RealTimeCharts.Microservices.ClientDispatcher.Services
{
    public class DispatcherService : IDispatcherService
    {
        private readonly ILogger<DispatcherService> _logger;
        private readonly HubConnection _hubConnection;

        public DispatcherService(ILogger<DispatcherService> logger, HubConnection hubConnection)
        {
            _logger = logger;
            _hubConnection = hubConnection;
        }

        public async Task<Result> DispatchData(DataPoint dataPoint, string connectionId)
            => await TryToDisptach(async () =>
                 {
                     _logger.LogInformation($"Dispatching data generation finished notification to client with id {connectionId}");
                     await _hubConnection.InvokeAsync("DataPointDispatched", SerializeObject(dataPoint), connectionId);
                 });

        public async Task<Result> DispatchDataGenerationFinishedNotification(bool success, string connectionId)
            => await TryToDisptach(async () =>
                {
                    _logger.LogInformation($"Dispatching data generation finished notification to client with id {connectionId}");
                    await _hubConnection.InvokeAsync("DataGenerationFinished", success, connectionId);
                });

        private async Task<Result> TryToDisptach(Func<Task> dispatch)
        {
            try
            {
                await dispatch();
                _logger.LogInformation("Data dispatched to client(s)");
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Failed to dispatch via signalR");
                return Result.Error(ex);
            }
        }

        private static string SerializeObject(object data)
            => JsonConvert.SerializeObject(data, new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
    }
}
