// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using AutoMapper;
using Defra.Trade.Common.Functions.Isolated.Models;
using Defra.Trade.Crm;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Dynamics;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Inbound.SignUp.Update;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Extensions;
using Microsoft.Extensions.Logging;

namespace Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Services
{
    public sealed class RemosUpdateMessageProcessor(ICrmClient client, IMapper mapper, ILogger<RemosUpdateMessageProcessor> logger) : BaseMessageProcessor<Request, MessageHeader, RemosUpdateMessageProcessor>(client, mapper, logger)
    {
        private readonly ICrmClient _client = client;
        private readonly ILogger<RemosUpdateMessageProcessor> _logger = logger;
        private readonly IMapper _mapper = mapper;

        public override async Task<StatusResponse<Request>> ProcessAsync(Request messageRequest, MessageHeader messageHeader)
        {
            string orgId = messageHeader.OrganisationId!;

            _logger.UpdateMessageProcessorMappingStart(orgId);
            var organisation = MapToDynamicsModels(messageRequest);
            _logger.UpdateMessageProcessorMappingSuccess(orgId);

            _logger.UpdateMessageProcessorSendToDynamicsStart(orgId);

            try
            {
                await _client.UpdateAsync(organisation);
            }
            catch (Exception ex)
            {
                _logger.UpdateMessageProcessorSendToDynamicsFailure(ex, orgId);
                throw;
            }

            _logger.UpdateMessageProcessorSendToDynamicsSuccess(orgId);

            return new()
            {
                ForwardMessage = false,
                Response = messageRequest
            };
        }

        private OrganisationUpdate MapToDynamicsModels(Request messageRequest)
        {
            try
            {
                return _mapper.Map<OrganisationUpdate>(messageRequest);
            }
            catch (Exception ex)
            {
                _logger.UpdateMessageProcessorMappingFailure(ex);
                throw;
            }
        }
    }
}
