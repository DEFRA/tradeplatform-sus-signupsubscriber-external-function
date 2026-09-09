// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Defra.Trade.Common.Config;
using Defra.Trade.Common.Function.Health.Extensions;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Infrastructure;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Defra.Trade.Events.SUS.RemosSignUpSubscriber;

public static class HealthChecksRegistration
{
    public static void RegisterHealthChecks(
      IHealthChecksBuilder builder,
      IServiceCollection services,
      IConfiguration configuration)
    {
        var serviceBusQueuesSettings = configuration.GetSection(ServiceBusSettings.OptionsName).Get<ServiceBusQueuesSettings>() ?? new ServiceBusQueuesSettings();

        builder.AddAzureServiceBusQueueCheck(serviceBusQueuesSettings, serviceBusQueuesSettings.QueueNameEhcoRemosEnrichment ?? string.Empty);
        builder.AddAzureServiceBusQueueCheck(serviceBusQueuesSettings, serviceBusQueuesSettings.QueueNameEhcoRemosCreate ?? string.Empty);
    }
}
