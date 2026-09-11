// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Azure.Messaging.ServiceBus;
using Defra.Trade.Common.Functions.Isolated.Interfaces;
using FakeItEasy;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Defra.Trade.Events.SUS.RemosSignUpSubscriber.Functions;

public sealed class RemosSignUpSubscriberServiceBusTriggerFunctionTests
{
    private readonly IMessageExecutorFactory _messageExecutorFactory;
    private readonly RemosSignUpSubscriberServiceBusTriggerFunction _sut;

    public RemosSignUpSubscriberServiceBusTriggerFunctionTests()
    {
        _messageExecutorFactory = A.Fake<IMessageExecutorFactory>(opt => opt.Strict());
        var serviceBusClient = A.Fake<ServiceBusClient>(opt => opt.Strict());
        var sender = A.Fake<ServiceBusSender>(opt => opt.Strict());
        A.CallTo(() => serviceBusClient.CreateSender(A<string>._)).Returns(sender);
        var logger = A.Fake<ILogger<RemosSignUpSubscriberServiceBusTriggerFunction>>();

        _sut = new RemosSignUpSubscriberServiceBusTriggerFunction(_messageExecutorFactory, serviceBusClient, logger);
    }

    [Theory]
    [InlineData("sus.remos.signup")]
    [InlineData("sus.remos.update")]
    [InlineData("sus.remos.establishment.create")]
    [InlineData("sus.remos.establishment.update")]
    public async Task RunAsync_WithKnownLabel_ProcessesMessage(string label)
    {
        // arrange
        string messageId = Guid.NewGuid().ToString();
        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(messageId: messageId, subject: label);
        var actions = A.Fake<ServiceBusMessageActions>(opt => opt.Strict());
        var functionContext = A.Fake<FunctionContext>(opt => opt.Strict());
        var functionDefinition = A.Fake<FunctionDefinition>(opt => opt.Strict());
        A.CallTo(() => functionDefinition.Name).Returns("RemosSignUpSubscriberServiceBusTriggerFunction");
        A.CallTo(() => functionContext.FunctionDefinition).Returns(functionDefinition);

        var executor = A.Fake<IMessageExecutor>();

        var executeCall = A.CallTo(() => executor.ExecuteAsync(
            message,
            actions,
            A<ServiceBusSender>._,
            functionContext,
            A<string>._,
            A<string>._,
            A<string>._,
            A<string?>._));
        var createMessageExecutorCall = A.CallTo(() => _messageExecutorFactory.CreateMessageExecutor(message));

        executeCall.Returns(Task.CompletedTask);
        createMessageExecutorCall.Returns(executor);

        // act
        await _sut.RunAsync(message, actions, functionContext);

        // assert
        createMessageExecutorCall.MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task RunAsync_WithUnknownLabel_DoesNotThrow()
    {
        // arrange
        string messageId = Guid.NewGuid().ToString();
        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(messageId: messageId, subject: "abcxyz");
        var actions = A.Fake<ServiceBusMessageActions>(opt => opt.Strict());
        var functionContext = A.Fake<FunctionContext>(opt => opt.Strict());
        var functionDefinition = A.Fake<FunctionDefinition>(opt => opt.Strict());
        A.CallTo(() => functionDefinition.Name).Returns("RemosSignUpSubscriberServiceBusTriggerFunction");
        A.CallTo(() => functionContext.FunctionDefinition).Returns(functionDefinition);

        var executor = A.Fake<IMessageExecutor>();

        var createMessageExecutorCall = A.CallTo(() => _messageExecutorFactory.CreateMessageExecutor(message));
        createMessageExecutorCall.Returns(executor);

        var executeCall = A.CallTo(() => executor.ExecuteAsync(
            A<ServiceBusReceivedMessage>._,
            A<ServiceBusMessageActions>._,
            A<ServiceBusSender>._,
            A<FunctionContext>._,
            A<string>._,
            A<string>._,
            A<string>._,
            A<string?>._));

        // act
        var exception = await Record.ExceptionAsync(() => _sut.RunAsync(message, actions, functionContext));

        // assert - unknown label is caught internally and logged, no exception should propagate
        Assert.Null(exception);
        createMessageExecutorCall.MustHaveHappenedOnceExactly();
        executeCall.MustNotHaveHappened();
    }
}
