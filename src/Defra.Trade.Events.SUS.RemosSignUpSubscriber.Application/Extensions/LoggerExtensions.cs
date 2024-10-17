// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Extensions;

[ExcludeFromCodeCoverage(Justification = "Extensions cannot be feasibly tested")]
public static partial class LoggerExtensions
{
    // Entry point

    [LoggerMessage(
        EventId = 0,
        EventName = nameof(MessageReceived),
        Level = LogLevel.Information,
        Message = "Message Id : {MessageId} received on {FunctionName}")]
    public static partial void MessageReceived(this ILogger logger, string messageId, string functionName);

    // SignUpMessageProcessor (Create)

    [LoggerMessage(
    EventId = 10,
    EventName = nameof(SignUpMessageProcessorMappingStart),
    Level = LogLevel.Information,
    Message = "Mapping signup inbound messages to dynamics data structures for org id : {OrgId}")]
    public static partial void SignUpMessageProcessorMappingStart(this ILogger logger, string orgId);

    [LoggerMessage(
    EventId = 11,
    EventName = nameof(SignUpMessageProcessorMappingSuccess),
    Level = LogLevel.Information,
    Message = "Mapping signup inbound messages to dynamics data structures succeeded for org id : {OrgID}")]
    public static partial void SignUpMessageProcessorMappingSuccess(this ILogger logger, string orgId);

    [LoggerMessage(
    EventId = 12,
    EventName = nameof(SignUpMessageProcessorMappingFailure),
    Level = LogLevel.Error,
    Message = "Mapping signup inbound messages to dynamics data structures failed")]
    public static partial void SignUpMessageProcessorMappingFailure(this ILogger logger, Exception ex);

    [LoggerMessage(
    EventId = 13,
    EventName = nameof(SignUpMessageProcessorSendToDynamicsStart),
    Level = LogLevel.Information,
    Message = "Sending signup organisation and inspection location to dynamics for org id : {OrgId}")]
    public static partial void SignUpMessageProcessorSendToDynamicsStart(this ILogger logger, string OrgId);

    [LoggerMessage(
    EventId = 14,
    EventName = nameof(SignUpMessageProcessorSendToDynamicsSuccess),
    Level = LogLevel.Information,
    Message = "Sending signup organisation and inspection location to dynamics succeeded for org id : {OrgId}")]
    public static partial void SignUpMessageProcessorSendToDynamicsSuccess(this ILogger logger, string orgId);

    [LoggerMessage(
    EventId = 15,
    EventName = nameof(SignUpMessageProcessorSendToDynamicsFailure),
    Level = LogLevel.Error,
    Message = "Sending signup organisation and inspection location to dynamics failed")]
    public static partial void SignUpMessageProcessorSendToDynamicsFailure(this ILogger logger, Exception ex);

    // UpdateMessageProcessor

    [LoggerMessage(
    EventId = 20,
    EventName = nameof(UpdateMessageProcessorMappingStart),
    Level = LogLevel.Information,
    Message = "Mapping update inbound messages to dynamics data structures for org id : {OrgId}")]
    public static partial void UpdateMessageProcessorMappingStart(this ILogger logger, string orgId);

    [LoggerMessage(
    EventId = 21,
    EventName = nameof(UpdateMessageProcessorMappingSuccess),
    Level = LogLevel.Information,
    Message = "Mapping update inbound messages to dynamics data structures succeeded for org id : {OrgId}")]
    public static partial void UpdateMessageProcessorMappingSuccess(this ILogger logger, string orgId);

    [LoggerMessage(
    EventId = 22,
    EventName = nameof(UpdateMessageProcessorMappingFailure),
    Level = LogLevel.Error,
    Message = "Mapping update inbound messages to dynamics data structures failed")]
    public static partial void UpdateMessageProcessorMappingFailure(this ILogger logger, Exception ex);

    [LoggerMessage(
    EventId = 23,
    EventName = nameof(UpdateMessageProcessorSendToDynamicsStart),
    Level = LogLevel.Information,
    Message = "Sending update organisation location to dynamics for org id : {OrgId}")]
    public static partial void UpdateMessageProcessorSendToDynamicsStart(this ILogger logger, string orgId);

    [LoggerMessage(
    EventId = 24,
    EventName = nameof(UpdateMessageProcessorSendToDynamicsSuccess),
    Level = LogLevel.Information,
    Message = "Sending update organisation location to dynamics succeeded for org id : {OrgId}")]
    public static partial void UpdateMessageProcessorSendToDynamicsSuccess(this ILogger logger, string orgId);

    [LoggerMessage(
    EventId = 25,
    EventName = nameof(UpdateMessageProcessorSendToDynamicsFailure),
    Level = LogLevel.Error,
    Message = "Sending update organisation location to dynamics failed for org id : {OrgId}")]
    public static partial void UpdateMessageProcessorSendToDynamicsFailure(this ILogger logger, Exception ex, string orgId);

    // EstablishmentCreateMessageProcessor

    [LoggerMessage(
    EventId = 30,
    EventName = nameof(EstablishmentCreateProcessorMappingStart),
    Level = LogLevel.Information,
    Message = "Mapping inspection location inbound messages to dynamics data structures for org id : {OrgId}")]
    public static partial void EstablishmentCreateProcessorMappingStart(this ILogger logger, string orgId);

    [LoggerMessage(
    EventId = 31,
    EventName = nameof(EstablishmentCreateProcessorMappingSuccess),
    Level = LogLevel.Information,
    Message = "Mapping inspection location inbound messages to dynamics data structures succeeded for org id : {OrgId}")]
    public static partial void EstablishmentCreateProcessorMappingSuccess(this ILogger logger, string orgId);

    [LoggerMessage(
    EventId = 32,
    EventName = nameof(EstablishmentCreateProcessorMappingFailure),
    Level = LogLevel.Error,
    Message = "Mapping inspection location inbound messages to dynamics data structures failed")]
    public static partial void EstablishmentCreateProcessorMappingFailure(this ILogger logger, Exception ex);

    [LoggerMessage(
    EventId = 33,
    EventName = nameof(EstablishmentCreateProcessorSendToDynamicsStart),
    Level = LogLevel.Information,
    Message = "Sending inspection location to dynamics for org id : {OrgId}")]
    public static partial void EstablishmentCreateProcessorSendToDynamicsStart(this ILogger logger, string orgId);

    [LoggerMessage(
    EventId = 34,
    EventName = nameof(EstablishmentCreateProcessorSendToDynamicsSuccess),
    Level = LogLevel.Information,
    Message = "Sending inspection location to dynamics succeeded for org id : {OrgId}")]
    public static partial void EstablishmentCreateProcessorSendToDynamicsSuccess(this ILogger logger, string orgId);

    [LoggerMessage(
    EventId = 35,
    EventName = nameof(EstablishmentCreateProcessorSendToDynamicsFailure),
    Level = LogLevel.Error,
    Message = "Sending inspection location to dynamics failed for org id : {OrgId}")]
    public static partial void EstablishmentCreateProcessorSendToDynamicsFailure(this ILogger logger, Exception ex, string orgId);

    // EstablishmentUpdateMessageProcessor

    [LoggerMessage(
    EventId = 40,
    EventName = nameof(EstablishmentUpdateProcessorMappingStart),
    Level = LogLevel.Information,
    Message = "Mapping inspection location inbound messages to dynamics data structures for org id : {OrgId}")]
    public static partial void EstablishmentUpdateProcessorMappingStart(this ILogger logger, string orgId);

    [LoggerMessage(
    EventId = 41,
    EventName = nameof(EstablishmentUpdateProcessorMappingSuccess),
    Level = LogLevel.Information,
    Message = "Mapping inspection location inbound messages to dynamics data structures succeeded for org id : {OrgId}")]
    public static partial void EstablishmentUpdateProcessorMappingSuccess(this ILogger logger, string orgId);

    [LoggerMessage(
    EventId = 42,
    EventName = nameof(EstablishmentUpdateProcessorMappingFailure),
    Level = LogLevel.Error,
    Message = "Mapping inspection location inbound messages to dynamics data structures failed")]
    public static partial void EstablishmentUpdateProcessorMappingFailure(this ILogger logger, Exception ex);

    [LoggerMessage(
    EventId = 43,
    EventName = nameof(EstablishmentUpdateProcessorSendToDynamicsStart),
    Level = LogLevel.Information,
    Message = "Sending inspection location to dynamics for org id : {OrgId}")]
    public static partial void EstablishmentUpdateProcessorSendToDynamicsStart(this ILogger logger, string orgId);

    [LoggerMessage(
    EventId = 44,
    EventName = nameof(EstablishmentUpdateProcessorSendToDynamicsSuccess),
    Level = LogLevel.Information,
    Message = "Sending inspection location to dynamics succeeded for org id : {OrgId}")]
    public static partial void EstablishmentUpdateProcessorSendToDynamicsSuccess(this ILogger logger, string orgId);

    [LoggerMessage(
    EventId = 45,
    EventName = nameof(EstablishmentUpdateProcessorSendToDynamicsFailure),
    Level = LogLevel.Error,
    Message = "Sending inspection location to dynamics failed for org id : {OrgId}")]
    public static partial void EstablishmentUpdateProcessorSendToDynamicsFailure(this ILogger logger, Exception ex, string orgId);
}
