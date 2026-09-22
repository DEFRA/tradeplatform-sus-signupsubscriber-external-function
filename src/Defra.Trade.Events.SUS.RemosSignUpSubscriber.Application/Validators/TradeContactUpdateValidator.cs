// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Defra.Trade.Common.Functions.Isolated.Validation;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Inbound;
using FluentValidation;

namespace Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Validators;

public sealed class TradeContactUpdateValidator : AbstractValidator<TradeContactUpdate>
{
    public TradeContactUpdateValidator()
    {
        RuleFor(m => m.PersonName)
            .NotNull().WithMessage(ValidationMessages.NullField)
            .MaximumLength(50);

        RuleFor(m => m.Position)
            .NotNull().WithMessage(ValidationMessages.NullField)
            .MaximumLength(50);

        RuleFor(m => m.Email)
            .NotNull().WithMessage(ValidationMessages.NullField)
            .MaximumLength(100);

        RuleFor(m => m.TelephoneNumber)
            .NotNull().WithMessage(ValidationMessages.NullField)
            .MaximumLength(20);
    }
}
