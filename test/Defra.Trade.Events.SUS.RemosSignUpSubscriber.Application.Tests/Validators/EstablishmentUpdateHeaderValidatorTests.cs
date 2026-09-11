// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Defra.Trade.Common.Functions.Isolated.Models.Enum;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Inbound.Establishment.Update;
using Shouldly;
using Xunit;

namespace Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Validators;

public sealed class EstablishmentUpdateHeaderValidatorTests : ValidatorTestBase<MessageHeader>
{
    private readonly EstablishmentUpdateMessageHeaderValidator _sut;

    public EstablishmentUpdateHeaderValidatorTests()
    {
        _sut = new EstablishmentUpdateMessageHeaderValidator();
    }

    public static TheoryData<ModelSetter[], ErrorDetails[]> GetTestData()
    {
        var noErrors = Array.Empty<ErrorDetails>();

        return new()
        {
            { Array.Empty<ModelSetter>(), noErrors },
            { new[] { Set(x => x.CausationId, null) }, noErrors },
            { new[] { Set(x => x.CausationId, "abc") }, new ErrorDetails[]{ new("CausationId", "Causation Id is not a valid guid") } },
            { new[] { Set(x => x.CausationId, "df6ed36f-2208-495b") }, new ErrorDetails[]{ new("CausationId", "Causation Id is not a valid guid") } },
            { new[] { Set(x => x.ContentType, null) }, new ErrorDetails[]{ new("ContentType", "Content Type must be application/json") } },
            { new[] { Set(x => x.ContentType, "abc") }, new ErrorDetails[]{ new("ContentType", "Content Type must be application/json") } },
            { new[] { Set(x => x.CorrelationId, null) }, new ErrorDetails[] { new("CorrelationId", "Correlation Id cannot be null") } },
            { new[] { Set(x => x.CorrelationId, "abc") }, new ErrorDetails[] { new("CorrelationId", "Correlation Id is not a valid guid") } },
            { new[] { Set(x => x.CorrelationId, "df6ed36f-2208-495b") }, new ErrorDetails[] { new("CorrelationId", "Correlation Id is not a valid guid") } },
            { new[] { Set(x => x.EntityKey, null) }, new ErrorDetails[] { new("EntityKey", "Entity Key cannot be null") } },
            { new[] { Set(x => x.EntityKey, "abc") }, new ErrorDetails[] { new("EntityKey", "Entity Key is not a valid guid") } },
            { new[] { Set(x => x.Label, null) }, new ErrorDetails[] { new("Label", "Label must be sus.remos.establishment.update") } },
            { new[] { Set(x => x.Label, "abc") }, new ErrorDetails[] { new("Label", "Label must be sus.remos.establishment.update") } },
            { new[] { Set(x => x.MessageId, null) }, new ErrorDetails[] { new("MessageId", "Message Id cannot be null") } },
            { new[] { Set(x => x.MessageId, "abc") }, new ErrorDetails[] { new("MessageId", "Message Id is not a valid guid") } },
            { new[] { Set(x => x.OrganisationId, null) }, noErrors },
            { new[] { Set(x => x.OrganisationId, "abc") }, new ErrorDetails[] { new("OrganisationId", "Organisation Id is not a valid guid") } },
            { new[] { Set(x => x.PublisherId, null) }, new ErrorDetails[] { new("PublisherId", "Publisher Id must be SuS") } },
            { new[] { Set(x => x.PublisherId, "") }, new ErrorDetails[] { new("PublisherId", "Publisher Id must be SuS") } },
            { new[] { Set(x => x.PublisherId, "sus") }, noErrors },
            { new[] { Set(x => x.SchemaVersion, null) }, new ErrorDetails[] { new("SchemaVersion", "Schema Version must be 4") } },
            { new[] { Set(x => x.SchemaVersion, "abc") }, new ErrorDetails[] { new("SchemaVersion", "Schema Version must be 4") } },
            { new[] { Set(x => x.Status, null) }, new ErrorDetails[] { new("Status", "Status must be Completed") } },
            { new[] { Set(x => x.Status, "abc") }, new ErrorDetails[] { new("Status", "Status must be Completed") } },
            { new[] { Set(x => x.Status, "created") }, new ErrorDetails[] { new("Status", "Status must be Completed") } },
            { new[] { Set(x => x.Type, EventType.Global) }, new ErrorDetails[] { new("Type", "Type must be Internal") } },
            { new[] { Set(x => x.TimestampUtc, 0) }, new ErrorDetails[] { new("TimestampUtc", "Timestamp Utc cannot be null") } },
            { new[] { Set(x => x.TimestampUtc, 1) }, noErrors },
            { new[] { Set(x => x.UserId, null) }, noErrors },
            { new[] { Set(x => x.UserId, "abc") }, noErrors }
        };
    }

    [Theory]
    [MemberData(nameof(GetTestData))]
    public void Validate_ReturnsTheExpectedErrors(ModelSetter[] setters, ErrorDetails[] expected)
    {
        // arrange
        var header = new MessageHeader
        {
            CausationId = Guid.NewGuid().ToString(),
            ContentType = "application/json",
            CorrelationId = Guid.NewGuid().ToString(),
            EntityKey = Guid.NewGuid().ToString(),
            FullMessage = null,
            Label = "sus.remos.establishment.update",
            MessageId = Guid.NewGuid().ToString(),
            MessageSubType = null,
            OrganisationId = Guid.NewGuid().ToString(),
            PublisherId = "SuS",
            SchemaVersion = "4",
            Status = "Completed",
            TimestampUtc = DateTime.UtcNow.Ticks,
            Type = EventType.Internal,
            UserId = Guid.NewGuid().ToString()
        };
        foreach (var setter in setters)
        {
            setter.ApplyTo(header);
        }

        // act
        var result = _sut.Validate(header);

        // assert
        string.Join("\n", result.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"))
            .ShouldBe(string.Join("\n", expected.Select(e => $"{e.Property}: {e.Error}")));
    }
}
