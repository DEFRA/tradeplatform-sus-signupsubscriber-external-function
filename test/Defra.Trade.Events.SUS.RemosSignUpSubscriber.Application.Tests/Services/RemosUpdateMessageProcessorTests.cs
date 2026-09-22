// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using AutoMapper;
using Defra.Trade.Common.Functions.Isolated.Models;
using Defra.Trade.Crm;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Dynamics;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Inbound.SignUp.Update;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Shouldly;
using Xunit;

namespace Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Services;

public sealed class RemosUpdateMessageProcessorTests
{
    private readonly ICrmClient _client;
    private readonly ILogger<RemosUpdateMessageProcessor> _logger;
    private readonly IMapper _mapper;
    private readonly RemosUpdateMessageProcessor _sut;

    public RemosUpdateMessageProcessorTests()
    {
        _client = A.Fake<ICrmClient>(opt => opt.Strict());
        _mapper = A.Fake<IMapper>(opt => opt.Strict());
        _logger = A.Fake<ILogger<RemosUpdateMessageProcessor>>();
        _sut = new RemosUpdateMessageProcessor(_client, _mapper, _logger);
    }

    [Fact]
    public async Task BuildCustomMessageHeaderAsync_ReturnsAnEmptyHeader()
    {
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
    public async Task ProcessAsync_UpdatesDynamicsAndReturnsAStatusResponseWhichDoesntForwardTheMessage()
    {
        // arrange
        var request = new Request();
        var header = new MessageHeader();
        var organisation = new OrganisationUpdate();
        var orgId = Guid.NewGuid();

        organisation.Id = orgId;

        var updateResultTask = Task.FromResult(organisation);

        A.CallTo(() => _mapper.Map<OrganisationUpdate>(request)).Returns(organisation);
        A.CallTo(() => _client.UpdateAsync(organisation, default)).Returns(updateResultTask);

        // act
        var result = _sut.ProcessAsync(request, header);

        // assert
        if (result.IsFaulted)
        {
            await result;
        }

        result.IsCompleted.ShouldBe(true);

        var actualResult = await result;

        actualResult.ForwardMessage.ShouldBeFalse();
        actualResult.Response.ShouldBe(request);

        A.CallTo(() => _client.UpdateAsync(organisation, default))
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

    [Fact]
    public async Task ProcessAsync_WithCrmClientError_ThrowsException()
    {
        // arrange
        var request = new Request();
        var header = new MessageHeader();
        var organisation = new OrganisationUpdate();
        var orgId = Guid.NewGuid();

        organisation.Id = orgId;

        A.CallTo(() => _mapper.Map<OrganisationUpdate>(request)).Returns(organisation);
        A.CallTo(() => _client.UpdateAsync(organisation, default)).Throws(new ArithmeticException());

        // act && assert
        await Assert.ThrowsAsync<ArithmeticException>(
            async () => await _sut.ProcessAsync(request, header));

        A.CallTo(() => _mapper.Map<OrganisationUpdate>(request))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _client.UpdateAsync(organisation, default))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task MapToDynamicsModels_WithMappingError_ThrowsException()
    {
        // arrange
        var request = new Request();
        var header = new MessageHeader();

        A.CallTo(() => _mapper.Map<OrganisationUpdate>(request)).Throws(new ArithmeticException());

        // act && assert
        await Assert.ThrowsAsync<ArithmeticException>(
            async () => await _sut.ProcessAsync(request, header));

        A.CallTo(() => _mapper.Map<OrganisationUpdate>(request))
            .MustHaveHappenedOnceExactly();
    }
}