// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using AutoFixture;
using AutoMapper;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Dynamics;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Inbound.Establishment.Create;
using Shouldly;
using Xunit;

namespace Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Mapping;

public class RemosEstablishmentCreateRequestProfileTests
{

    private readonly Fixture _fixture;
    private readonly IMapper _sut;

    public RemosEstablishmentCreateRequestProfileTests()
    {
        _fixture = new Fixture();
        _sut = new MapperConfiguration(opt =>
        {
            opt.AddProfile<RemosEstablishmentCreateRequestProfile>();
        }).CreateMapper();
    }

    [Fact]
    public void RemosUpdateRequestProfile_WhenMapped_ShouldReturnAsExpected()
    {
        // arrange
        var mockedTradeParty = _fixture.Build<TradeParty>()
            .With(x => x.OrgId, new Guid().ToString)
            .Create();
        var input = _fixture.Build<Request>()
            .With(x => x.TradeParty, mockedTradeParty)
            .Create();

        var expected = new InspectionLocation
        {
            Id = new Guid(input.TradeParty?.OrgId!),
            OrganisationId = new Guid(input.TradeParty!.OrgId!),
            RmsEstablishmentNumber = input.TradeParty!.LogisticsLocation!.RemosEstablishmentSchemeNumber,
            LocationName = input.TradeParty!.LogisticsLocation!.Name,
            AddressLine1 = input.TradeParty!.LogisticsLocation!.Address!.LineOne,
            AddressLine2 = input.TradeParty!.LogisticsLocation!.Address!.LineTwo,
            City = input.TradeParty!.LogisticsLocation!.Address!.CityName,
            Country = input.TradeParty!.LogisticsLocation!.Address!.County,
            Postcode = input.TradeParty!.LogisticsLocation!.Address!.PostCode,
            ContactEmailAddress = input.TradeParty!.LogisticsLocation!.Email,
            LocationType = LocationType.Establishment,
            StateCode = null,
            StatusCode = null
        };

        // act
        var actual = _sut.Map<InspectionLocation?>(input);

        // assert
        actual.ShouldBeEquivalentTo(expected);
    }
}
