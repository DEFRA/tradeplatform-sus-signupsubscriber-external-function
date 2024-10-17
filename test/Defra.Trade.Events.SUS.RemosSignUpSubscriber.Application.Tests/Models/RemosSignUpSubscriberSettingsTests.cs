// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Shouldly;
using Xunit;

namespace Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Models;

public class RemosSignUpSubscriberSettingsTests
{
    [Fact]
    public void Settings_ShouldBe_AsExpected()
    {
        // Act
        var sut = new RemosSignUpSubscriberSettings();

        // Assert
        RemosSignUpSubscriberSettings.RemosSignUpSubscriberSettingsName.ShouldBe("EhcoGcSubscriber");

#if DEBUG
        RemosSignUpSubscriberSettings.ConnectionStringConfigurationKey.ShouldBe("ServiceBus:ConnectionString");
#else
        RemosSignUpSubscriberSettings.ConnectionStringConfigurationKey.ShouldBe("ServiceBus");
#endif

        RemosSignUpSubscriberSettings.DefaultQueueName.ShouldBe("defra.trade.sus.remos.signup");
        RemosSignUpSubscriberSettings.PublisherId.ShouldBe("REMOS");
        RemosSignUpSubscriberSettings.TradeEventInfo.ShouldBe("defra.trade.events.info");
        RemosSignUpSubscriberSettings.AppConfigSentinelName.ShouldBe("Sentinel");
        sut.RemosSignUpCreatedQueue.ShouldBe("defra.trade.sus.remos.signup");
    }

}
