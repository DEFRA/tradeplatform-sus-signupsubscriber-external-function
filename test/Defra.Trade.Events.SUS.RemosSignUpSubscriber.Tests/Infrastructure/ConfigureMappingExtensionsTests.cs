// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Defra.Trade.Events.SUS.RemosSignUpSubscriber.Infrastructure;

public static class ConfigureMappingExtensionsTests
{
    [Fact]
    public static void ConfigureMapper_ShouldRegisterAValidMapper()
    {
        // arrange
        var services = new ServiceCollection();

        // act
        ConfigureMappingExtensions.ConfigureMapper(services);

        // assert
        var provider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true,
            ValidateScopes = true
        });
        var config = provider.GetRequiredService<IMapper>().ConfigurationProvider;
        config.AssertConfigurationIsValid();
    }
}
