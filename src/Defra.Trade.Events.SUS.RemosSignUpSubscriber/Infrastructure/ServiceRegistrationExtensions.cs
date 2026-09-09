// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Defra.Trade.Common.Config;
using Defra.Trade.Common.Functions.Isolated.EventStore;
using Defra.Trade.Common.Functions.Isolated.Extensions;
using Defra.Trade.Common.Functions.Isolated.Interfaces;
using Defra.Trade.Common.Functions.Isolated.Validation;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Inbound;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Infrastructure;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Models;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Services;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Validators;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Establishment = Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Inbound.Establishment;
using SignUp = Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Inbound.SignUp;

namespace Defra.Trade.Events.SUS.RemosSignUpSubscriber.Infrastructure;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddServiceRegistrations(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddEventStore()
            .AddValidators()
            .AddConfigurations(configuration)
            .AddMessagePipelines();
    }

    private static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ServiceBusQueuesSettings>().Bind(configuration.GetSection(ServiceBusSettings.OptionsName));
        var gcConfig = configuration.GetSection(RemosSignUpSubscriberSettings.RemosSignUpSubscriberSettingsName);
        services.AddOptions<RemosSignUpSubscriberSettings>().Bind(gcConfig);
        services.Configure<ServiceBusSettings>(configuration.GetSection(ServiceBusSettings.OptionsName));

        services.AddCrm(configuration.GetSection("SuSRemosSubscriber:Dynamics"));

        return services;
    }

    private static IServiceCollection AddEventStore(this IServiceCollection services)
    {
        return services
            .AddEventStoreConfiguration()
            .AddSingleton<IMessageCollector, EventStoreCollector>();
    }

    private static IServiceCollection AddMessagePipelines(this IServiceCollection services)
    {
        return services
            .AddMessagePipeline<SignUp.Create.Request, SignUp.Create.MessageHeader, RemosSignUpMessageProcessor>(MessageFilter.IsSignUpCreateMessage)
            .AddMessagePipeline<SignUp.Update.Request, SignUp.Update.MessageHeader, RemosUpdateMessageProcessor>(MessageFilter.IsSignUpUpdateMessage)
            .AddMessagePipeline<Establishment.Create.Request, Establishment.Create.MessageHeader, RemosEstablishmentCreateMessageProcessor>(MessageFilter.IsEstablishmentCreateMessage)
            .AddMessagePipeline<Establishment.Update.Request, Establishment.Update.MessageHeader, RemosEstablishmentUpdateMessageProcessor>(MessageFilter.IsEstablishmentUpdateMessage);
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        return services
            .AddSingleton<ICustomValidatorFactory, CustomValidatorFactory>()

            .AddSingleton<AbstractValidator<SignUp.Create.MessageHeader>, SignUpCreateMessageHeaderValidator>()
            .AddSingleton<AbstractValidator<SignUp.Update.MessageHeader>, SignUpUpdateMessageHeaderValidator>()
            .AddSingleton<AbstractValidator<Establishment.Create.MessageHeader>, EstablishmentCreateMessageHeaderValidator>()
            .AddSingleton<AbstractValidator<Establishment.Update.MessageHeader>, EstablishmentUpdateMessageHeaderValidator>()

            .AddSingleton<AbstractValidator<SignUp.Create.Request>, SignUpCreateRequestValidator>()
            .AddSingleton<AbstractValidator<SignUp.Update.Request>, SignUpUpdateRequestValidator>()
            .AddSingleton<AbstractValidator<Establishment.Create.Request>, EstablishmentCreateRequestValidator>()
            .AddSingleton<AbstractValidator<Establishment.Update.Request>, EstablishmentUpdateRequestValidator>()

            .AddSingleton<IValidator<SignUp.Create.TradeParty>, TradePartySignUpCreateValidator>()
            .AddSingleton<IValidator<SignUp.Update.TradeParty>, TradePartySignUpValidator>()
            .AddSingleton<IValidator<Establishment.Create.TradeParty>, TradePartyEstablishmentCreateValidator>()
            .AddSingleton<IValidator<Establishment.Update.TradeParty>, TradePartyEstablishmentUpdateValidator>()

            .AddSingleton<IValidator<TradeContactUpdate>, TradeContactUpdateValidator>()
            .AddSingleton<IValidator<TradeContactSignUp>, TradeContactSignUpValidator>()

            .AddSingleton<IValidator<LogisticsLocation>, LogisticsLocationValidator>()
            .AddSingleton<IValidator<LogisticsLocationEstablishmentCreate>, LogisticsLocationEstablishmentValidator>()
            .AddSingleton<IValidator<LogisticsLocationEstablishmentUpdate>, LogisticsLocationEstablishmentUpdateValidator>()

            .AddSingleton<IValidator<AuthorisedSignatory>, AuthorisedSignatoryValidator>()
            .AddSingleton<IValidator<Address>, AddressValidator>()
            .AddTransient<ISchemaValidator, SchemaValidator>();
    }
}
