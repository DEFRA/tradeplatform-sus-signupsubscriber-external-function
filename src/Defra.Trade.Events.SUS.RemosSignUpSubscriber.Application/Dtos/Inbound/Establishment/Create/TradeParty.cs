﻿// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

namespace Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Inbound.Establishment.Create;

public sealed class TradeParty
{
    public Guid? Id { get; set; }
    public LogisticsLocation? LogisticsLocation { get; set; }
    public string? OrgId { get; set; }
}