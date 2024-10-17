// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Inbound;
using FakeItEasy;
using FluentValidation;
using Shouldly;
using Xunit;

namespace Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Validators;

public sealed class LogisticsLocationEstablishmentCreateValidatorTests : ValidatorTestBase<LogisticsLocationEstablishmentCreate>
{
    private readonly IValidator<Address?> _addressValidator;
    private readonly LogisticsLocationEstablishmentValidator _sut;

    public LogisticsLocationEstablishmentCreateValidatorTests()
    {
        _addressValidator = A.Fake<IValidator<Address?>>(opt => opt.Strict());
        _sut = new LogisticsLocationEstablishmentValidator(_addressValidator);
    }

    public static TheoryData<ModelSetter[], ErrorDetails[]> GetTestData()
    {
        var noErrors = Array.Empty<ErrorDetails>();
        return new()
        {
            { Array.Empty<ModelSetter>(), noErrors },
            { new[]{ Set(x => x.Name, null) }, noErrors },
            { new[]{ Set(x => x.Name, "abc") }, noErrors },
            { new[]{ Set(x => x.Name, new string('a', 100)) }, noErrors },
            { new[]{ Set(x => x.Name, new string('a', 101)) }, new ErrorDetails[]{ new("Name", "The length of 'Name' must be 100 characters or fewer. You entered 101 characters.") } },
            { new[]{ Set(x => x.RemosEstablishmentSchemeNumber, null) }, noErrors },
            { new[]{ Set(x => x.RemosEstablishmentSchemeNumber, "abc") }, noErrors },
            { new[]{ Set(x => x.RemosEstablishmentSchemeNumber, new string('a', 25)) }, noErrors },
            { new[]{ Set(x => x.RemosEstablishmentSchemeNumber, new string('a', 26)) }, new ErrorDetails[]{ new("RemosEstablishmentSchemeNumber", "The length of 'Remos Establishment Scheme Number' must be 25 characters or fewer. You entered 26 characters.") } },
        };
    }

    [Theory]
    [MemberData(nameof(GetTestData))]
    public void Validate_ReturnsTheExpectedErrors(ModelSetter[] setters, ErrorDetails[] expected)
    {
        // arrange
        var address = new Address();
        string email = Guid.NewGuid().ToString();

        var location = new LogisticsLocationEstablishmentCreate()
        {
            Id = Guid.NewGuid(),
            TradePartyId = Guid.NewGuid(),
            Name = Guid.NewGuid().ToString(),
            RemosEstablishmentSchemeNumber = Guid.NewGuid().ToString()[..10],
            Address = address,
            Email = email
        };

        var validateAddress = A.CallTo(() => _addressValidator.Validate(A<ValidationContext<Address>>.That.Matches(ctx => ctx.InstanceToValidate == address)));
        validateAddress.Returns(new());

        foreach (var setter in setters)
        {
            setter.ApplyTo(location);
        }

        // act
        var result = _sut.Validate(location);

        // assert
        string.Join("\n", result.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"))
            .ShouldBe(string.Join("\n", expected.Select(e => $"{e.Property}: {e.Error}")));
        validateAddress.MustHaveHappenedOnceExactly();
    }
}
