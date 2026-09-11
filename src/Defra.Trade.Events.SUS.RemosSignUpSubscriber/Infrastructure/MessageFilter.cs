// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Azure.Messaging.ServiceBus;
using Defra.Trade.Common.Functions.Isolated.Extensions;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Models;

namespace Defra.Trade.Events.SUS.RemosSignUpSubscriber.Infrastructure;

public static class MessageFilter
{
    public static bool IsEstablishmentCreateMessage(ServiceBusReceivedMessage message)
    {
        return message.Label() == RemosSignUpServiceHeaderConstants.Establishment.Create.Label;
    }

    public static bool IsEstablishmentUpdateMessage(ServiceBusReceivedMessage message)
    {
        return message.Label() == RemosSignUpServiceHeaderConstants.Establishment.Update.Label;
    }

    public static bool IsSignUpCreateMessage(ServiceBusReceivedMessage message)
    {
        return message.Label() == RemosSignUpServiceHeaderConstants.SignUp.Create.Label;
    }

    public static bool IsSignUpUpdateMessage(ServiceBusReceivedMessage message)
    {
        return message.Label() == RemosSignUpServiceHeaderConstants.SignUp.Update.Label;
    }
}
