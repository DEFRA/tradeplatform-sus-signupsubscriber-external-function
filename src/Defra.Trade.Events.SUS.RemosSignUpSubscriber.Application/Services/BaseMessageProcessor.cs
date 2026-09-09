// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using AutoMapper;
using Defra.Trade.Common.Functions.Isolated.Interfaces;
using Defra.Trade.Common.Functions.Isolated.Models;
using Defra.Trade.Crm;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Inbound.Interfaces;
using Microsoft.Extensions.Logging;

namespace Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Services;

public abstract class BaseMessageProcessor<TRequest, TMessageHeader, TMessageProcessor> : IMessageProcessor<TRequest, TMessageHeader>
    where TMessageHeader : TradeEventMessageHeader, ICommonMessageHeader
    where TMessageProcessor : BaseMessageProcessor<TRequest, TMessageHeader, TMessageProcessor>
{
    protected BaseMessageProcessor(ICrmClient client, IMapper mapper, ILogger<BaseMessageProcessor<TRequest, TMessageHeader, TMessageProcessor>> logger)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(mapper);
        ArgumentNullException.ThrowIfNull(logger);
    }

    public Task<CustomMessageHeader> BuildCustomMessageHeaderAsync()
    {
        return Task.FromResult(new CustomMessageHeader());
    }

    public Task<string> GetSchemaAsync(TMessageHeader messageHeader)
    {
        return Task.FromResult(string.Empty);
    }

    public abstract Task<StatusResponse<TRequest>> ProcessAsync(TRequest model, TMessageHeader messageHeader);

    public Task<bool> ValidateMessageLabelAsync(TMessageHeader messageHeader)
    {
        return Task.FromResult(true);
    }
}
