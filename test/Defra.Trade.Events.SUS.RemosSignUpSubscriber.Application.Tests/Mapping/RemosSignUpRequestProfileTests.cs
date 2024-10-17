// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using AutoFixture;
using AutoMapper;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Dynamics;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Inbound;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Inbound.SignUp.Create;
using Shouldly;
using Xunit;

namespace Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Mapping;

public sealed class RemosSignUpRequestProfileTests
{
    private readonly Fixture _fixture;
    private readonly IMapper _sut;

    public RemosSignUpRequestProfileTests()
    {
        _fixture = new Fixture();
        _sut = new MapperConfiguration(opt =>
        {
            opt.AddProfile<DynamicsCountryProfile>();
            opt.AddProfile<DynamicsYesNoProfile>();
            opt.AddProfile<RemosSignUpRequestProfile>();
        }).CreateMapper();
    }

    [Fact]
    public void Map_CorrectlyMapsFromRequestToDynamicsInspectionLocation()
    {
        // arrange
        var orgId = Guid.NewGuid();
        var contactId = Guid.NewGuid();
        string email = Guid.NewGuid().ToString();
        var expected = _fixture.Build<InspectionLocation>()
            .With(l => l.Id, Guid.Empty)
            .With(l => l.OrganisationId, orgId)
            .With(l => l.LastSubmittedBy, contactId)
            .With(l => l.ContactEmailAddress, email)
            .With(l => l.LocationType, LocationType.Establishment)
            .With(l => l.StatusCode, (int?)null)
            .With(l => l.StateCode, (int?)null)
            .CreateMany(3)
            .ToList();

        Request inbound = new()
        {
            TradeParty = new()
            {
                OrgId = orgId,
                SignUpRequestSubmittedBy = contactId,
                LogisticsLocations = expected.Select(l => new LogisticsLocation()
                {
                    RemosEstablishmentSchemeNumber = l.RmsEstablishmentNumber,
                    Name = l.LocationName,
                    ApprovalStatus = 434800000,
                    Email = l.ContactEmailAddress,
                    Id = new Guid?(),
                    LastModifiedDate =  DateTime.Now,
                    NI_GBFlag = "test",
                    CreatedDate = DateTime.Now,
                    TradeAddressId = new Guid?(),
                    TradePartyId = new Guid?(),
                    Address = new Address
                    {
                        LineOne = l.AddressLine1,
                        LineTwo = l.AddressLine2,
                        CityName = l.City,
                        County = l.Country,
                        PostCode = l.Postcode
                    }
                }).ToArray(),
                TradeContact = new()
                {
                    Email = email
                }
            }
        };

        // act
        var actual = _sut.Map<IEnumerable<InspectionLocation>>(inbound).ToList();

        // assert
        actual.ShouldBeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("england", true, Country.England, YesNo.Yes)]
    [InlineData("eNgLaNd", true, Country.England, YesNo.Yes)]
    [InlineData("ENGLAND", true, Country.England, YesNo.Yes)]
    [InlineData("scotland", true, Country.Scotland, YesNo.Yes)]
    [InlineData("wales", true, Country.Wales, YesNo.Yes)]
    [InlineData("ni", true, Country.NorthernIreland, YesNo.Yes)]
    [InlineData("northernireland", true, Country.NorthernIreland, YesNo.Yes)]
    [InlineData("northern ireland", true, Country.NorthernIreland, YesNo.Yes)]
    [InlineData("england", false, Country.England, YesNo.No)]
    [InlineData("scotland", false, Country.Scotland, YesNo.No)]
    [InlineData("wales", false, Country.Wales, YesNo.No)]
    [InlineData("ni", false, Country.NorthernIreland, YesNo.No)]
    public void Map_CorrectlyMapsFromRequestToDynamicsOrganisation(string country, bool submitted, Country expectedCountry, YesNo expectedTermsAndConditions)
    {
        // arrange
        var expected = _fixture.Create<OrganisationSignup>();
        expected.RmsApprovalStatus = null;
        expected.BaseCountry = expectedCountry;
        expected.RmsTAndCsAccepted = expectedTermsAndConditions;
        if (!submitted)
        {
            expected.RmsSignUpRequestSubmittedOn = null;
        }

        Request inbound = new()
        {
            TradeParty = new()
            {
                OrgId = expected.Id,
                SignUpRequestSubmittedBy = expected.LastSubmittedBy,
                CountryName = country,
                RemosBusinessSchemeNumber = expected.RmsBusinessSchemeNumber,
                FboNumber = expected.FboNumber,
                PhrNumber = expected.PhrNumber,
                TermsAndConditionsSignedDate = expected.RmsSignUpRequestSubmittedOn,
                TradeContact = new()
                {
                    PersonName = expected.RmsBusinessContactName,
                    Position = expected.RmsBusinessContactPosition,
                    Email = expected.RmsBusinessContactEmail,
                    TelephoneNumber = expected.RmsBusinessContactTelephone
                },
                AuthorisedSignatory = new()
                {
                    Id = Guid.NewGuid(),
                    TradePartyId = Guid.NewGuid(),
                    Name = expected.AuthorisedSignatoryName,
                    EmailAddress = expected.AuthorisedSignatoryEmail,
                    Position = expected.AuthorisedSignatoryPosition
                }
            }
        };

        // act
        var actual = _sut.Map<OrganisationSignup>(inbound);

        // assert
        actual.ShouldBeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(1752, true)]
    [InlineData(1753, false)]
    [InlineData(2023, false)]
    public void Map_CutsOffOldDates(int year, bool cutoff)
    {
        // arrange
        DateTimeOffset date = new(year, 01, 01, 00, 00, 00, TimeSpan.Zero);
        Request inbound = new()
        {
            TradeParty = new()
            {
                TermsAndConditionsSignedDate = date
            }
        };

        // act
        var actual = _sut.Map<OrganisationSignup>(inbound);

        // assert
        if (cutoff)
        {
            actual.RmsSignUpRequestSubmittedOn.ShouldBeNull();
        }
        else
        {
            actual.RmsSignUpRequestSubmittedOn.ShouldBe(date);
        }
    }
}
