// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using AutoMapper;
using Azure.Messaging.ServiceBus;
using Defra.Trade.Common.Functions.Isolated.Models;
using Establishment = Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Inbound.Establishment;
using SignUp = Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Inbound.SignUp;

namespace Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Validators;

public sealed class ServiceBusReceiveMessageProfile : Profile
{
    public ServiceBusReceiveMessageProfile()
    {
        CreateMap<ServiceBusReceivedMessage, SignUp.Create.MessageHeader>()
            .IncludeBase<ServiceBusReceivedMessage, TradeEventMessageHeader>();

        CreateMap<ServiceBusReceivedMessage, SignUp.Update.MessageHeader>()
            .IncludeBase<ServiceBusReceivedMessage, TradeEventMessageHeader>();

        CreateMap<ServiceBusReceivedMessage, Establishment.Create.MessageHeader>()
            .IncludeBase<ServiceBusReceivedMessage, TradeEventMessageHeader>();

        CreateMap<ServiceBusReceivedMessage, Establishment.Update.MessageHeader>()
            .IncludeBase<ServiceBusReceivedMessage, TradeEventMessageHeader>();
    }
}
