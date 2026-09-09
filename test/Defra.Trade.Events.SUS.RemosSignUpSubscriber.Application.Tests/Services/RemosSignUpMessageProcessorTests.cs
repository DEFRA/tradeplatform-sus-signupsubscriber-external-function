// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using AutoMapper;
using Defra.Trade.Common.Functions.Isolated.Models;
using Defra.Trade.Crm;
using Defra.Trade.Crm.Clients;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Dynamics;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Inbound.SignUp.Create;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Shouldly;
using Xunit;

namespace Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Services;

public sealed class RemosSignUpMessageProcessorTests
{
    private readonly ICrmClient _client;
    private readonly ILogger<RemosSignUpMessageProcessor> _logger;
    private readonly IMapper _mapper;
    private readonly RemosSignUpMessageProcessor _sut;

    public RemosSignUpMessageProcessorTests()
    {
        _client = A.Fake<ICrmClient>(opt => opt.Strict());
        _mapper = A.Fake<IMapper>(opt => opt.Strict());
        _logger = A.Fake<ILogger<RemosSignUpMessageProcessor>>();
        _sut = new RemosSignUpMessageProcessor(_client, _mapper, _logger);
    }

    [Fact]
    public async Task BuildCustomMessageHeaderAsync_ReturnsAnEmptyHeader()
    {
        // arrange

        // act
        var result = await _sut.BuildCustomMessageHeaderAsync();

        // assert
        result.ShouldBeEquivalentTo(new CustomMessageHeader());
    }

    [Fact]
    public async Task GetSchemaAsync_ReturnsAnEmptyString()
    {
        // arrange
        var header = new MessageHeader();

        // act
        string result = await _sut.GetSchemaAsync(header);

        // assert
        result.ShouldBe("");
    }

    [Fact]
    public async Task MapToDynamicsModels_WithMappingError_ThrowsException()
    {
        // arrange
        var request = new Request();
        var header = new MessageHeader();

        A.CallTo(() => _mapper.Map<OrganisationSignup>(request)).Throws(new ArithmeticException());

        // act && assert
        await Assert.ThrowsAsync<ArithmeticException>(
            async () => await _sut.ProcessAsync(request, header));

        A.CallTo(() => _mapper.Map<OrganisationSignup>(request))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ProcessAsync_UpsertsTheDataIntoDynamicsAndReturnsAStatusResponseWhichDoesntForwardTheMessage()
    {
        // arrange
        var request = new Request();
        var header = new MessageHeader();
        var changeset = A.Fake<IBoundCrmChangesetBuilder>(opt => opt.Strict());
        var organisation = new OrganisationSignup();
        var orgId = Guid.NewGuid();
        var mappedLocation1 = new InspectionLocation() { RmsEstablishmentNumber = Guid.NewGuid().ToString() };
        var mappedLocation2 = new InspectionLocation() { RmsEstablishmentNumber = Guid.NewGuid().ToString() };
        var mappedLocation3 = new InspectionLocation() { RmsEstablishmentNumber = Guid.NewGuid().ToString() };

        organisation.Id = orgId;

        var organisationTcs = new TaskCompletionSource<OrganisationSignup>();
        var locationTcs1 = new TaskCompletionSource<InspectionLocation>();
        var locationTcs2 = new TaskCompletionSource<InspectionLocation>();
        var locationTcs3 = new TaskCompletionSource<InspectionLocation>();
        var organisationTask = organisationTcs.Task;
        var locationTask1 = locationTcs1.Task;
        var locationTask2 = locationTcs2.Task;
        var locationTask3 = locationTcs3.Task;
        var sendTcs = new TaskCompletionSource();

        A.CallTo(() => _mapper.Map<OrganisationSignup>(request)).Returns(organisation);
        A.CallTo(() => _mapper.Map<IEnumerable<InspectionLocation>>(request)).Returns(new[] { mappedLocation1, mappedLocation2, mappedLocation3 });
        A.CallTo(() => _client.Changeset()).Returns(changeset);
        A.CallTo(() => changeset.AddUpsert(organisation, out organisationTask, default)).Returns(changeset);
        A.CallTo(() => changeset.AddCreate(mappedLocation1, out locationTask1, default)).Returns(changeset);
        A.CallTo(() => changeset.AddCreate(mappedLocation2, out locationTask2, default)).Returns(changeset);
        A.CallTo(() => changeset.AddCreate(mappedLocation3, out locationTask3, default)).Returns(changeset);
        A.CallTo(() => changeset.SendAsync(default)).Returns(sendTcs.Task);

        // act
        var result = _sut.ProcessAsync(request, header);

        // assert
        if (result.IsFaulted)
        {
            await result;
        }

        result.IsCompleted.ShouldBe(false);
        sendTcs.SetResult();
        result.IsCompleted.ShouldBe(false);
        organisationTcs.SetResult(new());
        result.IsCompleted.ShouldBe(false);
        locationTcs1.SetResult(new());
        result.IsCompleted.ShouldBe(false);
        locationTcs2.SetResult(new());
        result.IsCompleted.ShouldBe(false);
        locationTcs3.SetResult(new());
        result.IsCompleted.ShouldBe(true);
        var actual = await result;
        actual.ShouldNotBeNull();
        actual.ForwardMessage.ShouldBeFalse();
        actual.Response.ShouldBe(request);

        A.CallTo(() => changeset.AddUpsert(organisation, out organisationTask, default))
            .MustHaveHappenedOnceExactly()
            .Then(A.CallTo(() => changeset.SendAsync(default)).MustHaveHappenedOnceExactly());

        A.CallTo(() => changeset.AddCreate(mappedLocation1, out locationTask1, default))
            .MustHaveHappenedOnceExactly()
            .Then(A.CallTo(() => changeset.SendAsync(default)).MustHaveHappenedOnceExactly());

        A.CallTo(() => changeset.AddCreate(mappedLocation2, out locationTask2, default))
            .MustHaveHappenedOnceExactly()
            .Then(A.CallTo(() => changeset.SendAsync(default)).MustHaveHappenedOnceExactly());

        A.CallTo(() => changeset.AddCreate(mappedLocation3, out locationTask3, default))
            .MustHaveHappenedOnceExactly()
            .Then(A.CallTo(() => changeset.SendAsync(default)).MustHaveHappenedOnceExactly());
    }

    [Fact]
    public async Task SendToDynamics_UpsertLocationWithCrmClientError_ThrowsException()
    {
        // arrange
        var request = new Request();
        var header = new MessageHeader();
        var changeset = A.Fake<IBoundCrmChangesetBuilder>(opt => opt.Strict());
        var organisation = new OrganisationSignup();
        var orgId = Guid.NewGuid();
        var mappedLocation = new InspectionLocation() { RmsEstablishmentNumber = Guid.NewGuid().ToString() };

        organisation.Id = orgId;

        var organisationTcs = new TaskCompletionSource<OrganisationSignup>();
        var locationTcs = new TaskCompletionSource<InspectionLocation>();
        var organisationTask = organisationTcs.Task;
        var locationTask = locationTcs.Task;
        var sendTcs = new TaskCompletionSource();

        A.CallTo(() => _mapper.Map<OrganisationSignup>(request)).Returns(organisation);
        A.CallTo(() => _mapper.Map<IEnumerable<InspectionLocation>>(request)).Returns(new[] { mappedLocation });
        A.CallTo(() => _client.Changeset()).Returns(changeset);
        A.CallTo(() => changeset.AddUpsert(organisation, out organisationTask, default)).Returns(changeset);
        A.CallTo(() => changeset.AddCreate(mappedLocation, out locationTask, default)).Throws(new ArithmeticException());
        A.CallTo(() => changeset.SendAsync(default)).Returns(sendTcs.Task);

        // act && assert
        var result = _sut.ProcessAsync(request, header);

        if (result.IsFaulted)
        {
            await result;
        }

        result.IsCompleted.ShouldBe(false);
        sendTcs.SetResult();
        result.IsCompleted.ShouldBe(false);
        organisationTcs.SetResult(new());
        result.IsCompleted.ShouldBe(true);

        await Assert.ThrowsAsync<ArithmeticException>(
            async () => await result);

        A.CallTo(() => changeset.AddUpsert(organisation, out organisationTask, default))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => changeset.AddCreate(mappedLocation, out locationTask, default))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task SendToDynamics_UpsertOrgWithCrmClientError_ThrowsException()
    {
        // arrange
        var request = new Request();
        var header = new MessageHeader();
        var changeset = A.Fake<IBoundCrmChangesetBuilder>(opt => opt.Strict());
        var organisation = new OrganisationSignup();
        var orgId = Guid.NewGuid();
        var mappedLocation = new InspectionLocation() { RmsEstablishmentNumber = Guid.NewGuid().ToString() };

        organisation.Id = orgId;

        var organisationTcs = new TaskCompletionSource<OrganisationSignup>();
        var locationTcs = new TaskCompletionSource<InspectionLocation>();
        var organisationTask = organisationTcs.Task;
        var locationTask = locationTcs.Task;
        var sendTcs = new TaskCompletionSource();

        A.CallTo(() => _mapper.Map<OrganisationSignup>(request)).Returns(organisation);
        A.CallTo(() => _mapper.Map<IEnumerable<InspectionLocation>>(request)).Returns(new[] { mappedLocation });
        A.CallTo(() => _client.Changeset()).Returns(changeset);
        A.CallTo(() => changeset.AddUpsert(organisation, out organisationTask, default)).Throws(new ArithmeticException());
        A.CallTo(() => changeset.AddCreate(mappedLocation, out locationTask, default)).Returns(changeset);
        A.CallTo(() => changeset.SendAsync(default)).Returns(sendTcs.Task);

        // act && assert
        var result = _sut.ProcessAsync(request, header);

        if (result.IsFaulted)
        {
            await result;
        }

        result.IsCompleted.ShouldBe(false);
        sendTcs.SetResult();
        result.IsCompleted.ShouldBe(false);
        organisationTcs.SetResult(new());
        result.IsCompleted.ShouldBe(false);
        locationTcs.SetResult(new());
        result.IsCompleted.ShouldBe(true);

        await Assert.ThrowsAsync<ArithmeticException>(
            async () => await result);

        A.CallTo(() => changeset.AddUpsert(organisation, out organisationTask, default))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ValidateMessageLabelAsync_ReturnsTrue()
    {
        // arrange
        var header = new MessageHeader();

        // act
        bool result = await _sut.ValidateMessageLabelAsync(header);

        // assert
        result.ShouldBe(true);
    }
}