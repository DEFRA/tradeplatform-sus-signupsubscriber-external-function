// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Defra.Trade.Common.Functions.Isolated.Extensions;
using Defra.Trade.Common.Functions.Isolated.Interfaces;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Extensions;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Defra.Trade.Events.SUS.RemosSignUpSubscriber.Functions;

public sealed class RemosSignUpSubscriberServiceBusTriggerFunction
{
    private readonly IMessageExecutorFactory _messageExecutorFactory;
    private readonly ServiceBusSender _eventStoreSender;
    private readonly ILogger<RemosSignUpSubscriberServiceBusTriggerFunction> _logger;

    public RemosSignUpSubscriberServiceBusTriggerFunction(
        IMessageExecutorFactory messageExecutorFactory,
        ServiceBusClient serviceBusClient,
        ILogger<RemosSignUpSubscriberServiceBusTriggerFunction> logger)
    {
        _messageExecutorFactory = messageExecutorFactory;
        _eventStoreSender = serviceBusClient.CreateSender(RemosSignUpSubscriberSettings.TradeEventInfo);
        _logger = logger;
    }

    [Function(nameof(RemosSignUpSubscriberServiceBusTriggerFunction))]
    public async Task RunAsync(
        [ServiceBusTrigger(queueName: RemosSignUpSubscriberSettings.DefaultQueueName, Connection = RemosSignUpSubscriberSettings.ConnectionStringConfigurationKey, IsSessionsEnabled = false)] ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions,
        FunctionContext functionContext)
    {
        await RunInternalAsync(message, messageActions, _eventStoreSender, functionContext, _logger);
    }

    private static string? GetCrmRequestType(ServiceBusReceivedMessage message) => message.Label() switch
    {
        RemosSignUpServiceHeaderConstants.SignUp.Create.Label => CrmRequestConstants.Create,
        RemosSignUpServiceHeaderConstants.SignUp.Update.Label => CrmRequestConstants.Update,
        RemosSignUpServiceHeaderConstants.Establishment.Create.Label => CrmRequestConstants.Create,
        RemosSignUpServiceHeaderConstants.Establishment.Update.Label => CrmRequestConstants.Update,
        _ => throw new ArgumentOutOfRangeException(nameof(message))
    };

    private async Task RunInternalAsync(
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions,
        ServiceBusSender eventStoreSender,
        FunctionContext functionContext,
        ILogger logger)
    {
        try
        {
            logger.MessageReceived(message.MessageId, functionContext.FunctionDefinition.Name);

            await _messageExecutorFactory
                .CreateMessageExecutor(message)
                .ExecuteAsync(
                    message,
                    messageActions,
                    eventStoreSender,
                    functionContext,
                    RemosSignUpSubscriberSettings.DefaultQueueName,
                    RemosSignUpSubscriberSettings.PublisherId,
                    RemosSignUpSubscriberSettings.PublisherId,
                    GetCrmRequestType(message));

            logger.LogInformation("Finished processing Messages Id : {MessageId} received on {FunctionName}", message.MessageId, functionContext.FunctionDefinition.Name);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, ex.Message);
        }
    }
}
