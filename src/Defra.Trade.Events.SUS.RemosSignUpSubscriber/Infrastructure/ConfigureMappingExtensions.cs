// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Defra.Trade.Events.SUS.RemosSignUpSubscriber.Infrastructure;

public static class ConfigureMappingExtensions
{
    public static IServiceCollection ConfigureMapper(this IServiceCollection services)
    {
        var assembly = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName is string fullName && fullName.Contains("Defra"))
            .OrderBy(a => a.FullName)
            .ToList();

        services.AddAutoMapper(assembly);

        return services;
    }
}
