// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Defra.Trade.Common.Functions.Isolated.Models.Enum;

namespace Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Inbound.Interfaces;

public interface ICommonMessageHeader
{
    public string? CausationId { get; set; }
    public string? ContentType { get; set; }
    public string? CorrelationId { get; set; }
    public string? EntityKey { get; set; }
    public string? MessageId { get; set; }
    public string? OrganisationId { get; set; }
    public string? PublisherId { get; set; }
    public long TimestampUtc { get; set; }
    public EventType Type { get; set; }
}
