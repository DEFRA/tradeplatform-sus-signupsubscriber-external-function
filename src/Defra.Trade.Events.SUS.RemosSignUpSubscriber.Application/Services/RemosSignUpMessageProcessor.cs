// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Runtime.ExceptionServices;
using AutoMapper;
using Defra.Trade.Common.Functions.Isolated.Models;
using Defra.Trade.Crm;
using Defra.Trade.Crm.Clients;
using Defra.Trade.Crm.Exceptions;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Dynamics;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Inbound.SignUp.Create;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Extensions;
using Microsoft.Extensions.Logging;

namespace Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Services;

public sealed class RemosSignUpMessageProcessor(
    ICrmClient client,
    IMapper mapper,
    ILogger<RemosSignUpMessageProcessor> logger) : BaseMessageProcessor<Request, MessageHeader, RemosSignUpMessageProcessor>(client, mapper, logger)
{
    private readonly ICrmClient _client = client;
    private readonly ILogger<RemosSignUpMessageProcessor> _logger = logger;
    private readonly IMapper _mapper = mapper;

    public override async Task<StatusResponse<Request>> ProcessAsync(Request messageRequest, MessageHeader messageHeader)
    {
        string orgId = messageHeader.OrganisationId!;

        _logger.SignUpMessageProcessorMappingStart(orgId);
        var (organisation, inspectionLocations) = MapToDynamicsModels(messageRequest);
        _logger.SignUpMessageProcessorMappingSuccess(orgId);

        _logger.SignUpMessageProcessorSendToDynamicsStart(orgId);
        await SendToDynamics(organisation, inspectionLocations);
        _logger.SignUpMessageProcessorSendToDynamicsSuccess(orgId);

        return new()
        {
            ForwardMessage = false,
            Response = messageRequest
        };
    }

    private static async Task<Exception?> SendToDynamics(IBoundCrmChangesetBuilder builder)
    {
        try
        {
            await builder.SendAsync();
            return null;
        }
        catch (CrmException ex)
        {
            return ex;
        }
    }

    private static async Task<Exception?> SendToDynamics(OrganisationSignup organisation, IBoundCrmChangesetBuilder builder)
    {
        try
        {
            builder.AddUpsert(organisation, out var task);
            await task;
            return null;
        }
        catch (CrmException ex)
        {
            return ExceptionDispatchInfo.SetCurrentStackTrace(
                new CrmException(ex.Code, ex.StatusCode ?? 0, $"Failed to create organisation {organisation.Id}", ex));
        }
    }

    private static async Task<Exception?> SendToDynamics(InspectionLocation location, IBoundCrmChangesetBuilder builder)
    {
        try
        {
            builder.AddCreate(location, out var task);
            await task;
            return null;
        }
        catch (CrmException ex)
        {
            return ExceptionDispatchInfo.SetCurrentStackTrace(
                new CrmException(ex.Code, ex.StatusCode ?? 0, $"Failed to create Inspection Location {location.RmsEstablishmentNumber}", ex));
        }
    }

    private (OrganisationSignup, IEnumerable<InspectionLocation>) MapToDynamicsModels(Request messageRequest)
    {
        try
        {
            return (
                _mapper.Map<OrganisationSignup>(messageRequest),
                _mapper.Map<IEnumerable<InspectionLocation>>(messageRequest)
            );
        }
        catch (Exception ex)
        {
            _logger.SignUpMessageProcessorMappingFailure(ex);
            throw;
        }
    }

    private async Task SendToDynamics(OrganisationSignup organisation, IEnumerable<InspectionLocation> inspectionLocations)
    {
        try
        {
            var changeset = _client.Changeset();

            var tasks = inspectionLocations
                .Select(l => SendToDynamics(l, changeset))
                .Prepend(SendToDynamics(organisation, changeset))
                .ToList();

            tasks = tasks.Prepend(SendToDynamics(changeset))
                .ToList();

            var results = await Task.WhenAll(tasks);

            var errors = results.Where(e => e is not null)
                .ToList();

            if (errors.Count > 0)
                throw new AggregateException(errors!);
        }
        catch (Exception ex)
        {
            _logger.SignUpMessageProcessorSendToDynamicsFailure(ex);
            throw;
        }
    }
}
