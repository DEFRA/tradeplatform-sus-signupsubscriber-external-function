// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Linq;
using Azure.Messaging.ServiceBus;
using Defra.Trade.Common.AppConfig;
using Defra.Trade.Common.Logging.Extensions;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Models;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Infrastructure;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((context, configBuilder) =>
    {
        configBuilder.ConfigureTradeAppConfiguration(config =>
        {
            config.UseKeyVaultSecrets = true;
            config.RefreshKeys.Add($"{RemosSignUpSubscriberSettings.RemosSignUpSubscriberSettingsName}:{RemosSignUpSubscriberSettings.AppConfigSentinelName}");
        });
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        services
            .AddSingleton(_ => {
#if DEBUG
                string? connectionString = configuration.GetValue<string>(RemosSignUpSubscriberSettings.ConnectionStringConfigurationKey);
                return new ServiceBusClient(connectionString);
#else
                var ns = configuration.GetValue<string>("ServiceBusFQN");
                return new ServiceBusClient(ns, new Azure.Identity.DefaultAzureCredential());
#endif
            })

            .AddTradeAppConfiguration(configuration)
            .AddServiceRegistrations(configuration)
            .AddApplication()
            .ConfigureMapper();

        services.AddFunctionLogging("RemosSignUpSubscriber");

        var healthChecksBuilder = services.AddHealthChecks();
        HealthChecksRegistration.RegisterHealthChecks(healthChecksBuilder, services, configuration);
    })
    .Build();

await host.RunAsync();
