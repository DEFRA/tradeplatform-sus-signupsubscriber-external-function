﻿// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

namespace Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Inbound;

public sealed class LogisticsLocationEstablishmentUpdate
{
    public Guid? Id { get; set; }
    public string? TradePartyId { get; set; }
    public string? InspectionLocationId { get; set; }
    public string? Status { get; set; }
}