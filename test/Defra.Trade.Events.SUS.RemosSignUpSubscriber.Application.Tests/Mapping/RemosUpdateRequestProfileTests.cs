// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using AutoFixture;
using AutoMapper;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Dynamics;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Inbound.SignUp.Update;
using Shouldly;
using Xunit;

namespace Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Mapping;

public class RemosUpdateRequestProfileTests
{

    private readonly Fixture _fixture;
    private readonly IMapper _sut;

    public RemosUpdateRequestProfileTests()
    {
        _fixture = new Fixture();
        _sut = new MapperConfiguration(opt =>
        {
            opt.AddProfile<RemosUpdateRequestProfile>();
        }).CreateMapper();
    }

    [Fact]
    public void RemosUpdateRequestProfile_WhenMapped_ShouldReturnAsExpected()
    {
        // arrange
        var mockedTradeParty = _fixture.Build<TradeParty>()
            .With(x => x.OrgId, new Guid().ToString)
            .With(x => x.SignUpRequestSubmittedBy, new Guid().ToString)
            .Create();
        var input = _fixture.Build<Request>()
            .With(x => x.TradeParty, mockedTradeParty)
            .Create();

        var expected = new OrganisationUpdate
        {
            Id = new Guid(input.TradeParty?.OrgId!),
            LastSubmittedBy = new Guid(input.TradeParty?.SignUpRequestSubmittedBy!),
            RmsBusinessContactName = input.TradeParty!.TradeContact!.PersonName,
            RmsBusinessContactPosition = input.TradeParty!.TradeContact!.Position,
            RmsBusinessContactEmail = input.TradeParty!.TradeContact!.Email,
            RmsBusinessContactTelephone = input.TradeParty!.TradeContact!.TelephoneNumber,
            AuthorisedSignatoryName = input.TradeParty!.AuthorisedSignatory!.Name,
            AuthorisedSignatoryPosition = input.TradeParty!.AuthorisedSignatory!.Position,
            AuthorisedSignatoryEmail = input.TradeParty!.AuthorisedSignatory!.EmailAddress
        };
        // act
        var actual = _sut.Map<OrganisationUpdate?>(input);

        // assert
        actual.ShouldBeEquivalentTo(expected);
    }
}
