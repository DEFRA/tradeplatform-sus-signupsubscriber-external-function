// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Shouldly;
using Xunit;

namespace Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Infrastructure;

public class ServiceBusQueuesSettingsTests
{
    [Fact]
    public void ServiceBusQueuesSettings_ShouldBe_AsExpected()
    {
        var sut = new ServiceBusQueuesSettings
        {
            ConnectionString = "mocked-connection",
            FullyQualifiedNamespace = "mocked-namespace",
            QueueNameEhcoRemosCreate = "mocked-create-queue",
            QueueNameEhcoRemosEnrichment = "mocked-enrich-queue"
        };


        // Assert
        sut.ConnectionString.ShouldBe("mocked-connection");
        sut.FullyQualifiedNamespace.ShouldBe("mocked-namespace");
        sut.QueueNameEhcoRemosCreate.ShouldBe("mocked-create-queue");
        sut.QueueNameEhcoRemosEnrichment.ShouldBe("mocked-enrich-queue");
    }
}
