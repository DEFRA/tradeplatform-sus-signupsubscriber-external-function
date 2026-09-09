// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using AutoMapper;
using Defra.Trade.Common.Functions.Isolated.Models;
using Defra.Trade.Crm;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Dynamics;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Inbound.Establishment.Update;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Extensions;
using Microsoft.Extensions.Logging;

namespace Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Services;

public sealed class RemosEstablishmentUpdateMessageProcessor(
    ICrmClient client,
    IMapper mapper,
    ILogger<RemosEstablishmentUpdateMessageProcessor> logger) : BaseMessageProcessor<Request, MessageHeader, RemosEstablishmentUpdateMessageProcessor>(client, mapper, logger)
{
    private readonly ICrmClient _client = client;
    private readonly ILogger<RemosEstablishmentUpdateMessageProcessor> _logger = logger;
    private readonly IMapper _mapper = mapper;

    public override async Task<StatusResponse<Request>> ProcessAsync(Request messageRequest, MessageHeader messageHeader)
    {
        string orgId = messageHeader.OrganisationId!;

        _logger.EstablishmentUpdateProcessorMappingStart(orgId);
        var inspectionLocation = MapToDynamicsModels(messageRequest);

        inspectionLocation.StatusCode = (int?)StatusCode.Inactive;
        inspectionLocation.StateCode = (int?)StateCode.Inactive;

        _logger.EstablishmentUpdateProcessorMappingSuccess(orgId);
        _logger.EstablishmentUpdateProcessorSendToDynamicsStart(orgId);

        try
        {
            await _client.UpdateAsync(inspectionLocation);
        }
        catch (Exception ex)
        {
            _logger.EstablishmentUpdateProcessorSendToDynamicsFailure(ex, orgId);
            throw;
        }

        _logger.EstablishmentUpdateProcessorSendToDynamicsSuccess(orgId);

        return new()
        {
            ForwardMessage = false,
            Response = messageRequest
        };
    }

    private InspectionLocation MapToDynamicsModels(Request messageRequest)
    {
        try
        {
            return _mapper.Map<InspectionLocation>(messageRequest);
        }
        catch (Exception ex)
        {
            _logger.EstablishmentUpdateProcessorMappingFailure(ex);
            throw;
        }
    }
}
