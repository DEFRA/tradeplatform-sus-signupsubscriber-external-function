// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Defra.Trade.Common.Functions.Isolated.Validation;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Inbound;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Inbound.SignUp.Update;
using FluentValidation;

namespace Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Validators;

public sealed class TradePartySignUpValidator : AbstractValidator<TradeParty>
{
    public TradePartySignUpValidator(
        IValidator<TradeContactUpdate> contactUpdateValidator,
        IValidator<AuthorisedSignatory> authorisedSignatoryValidator)
    {
        RuleFor(m => m.AuthorisedSignatory)
            .SetValidator(authorisedSignatoryValidator!);

        RuleFor(m => m.OrgId)
            .NotNull().WithMessage(ValidationMessages.NullField)
            .Must(RemosValidationHelpers.BeAGuid).WithMessage(RemosValidationMessages.GuidField);

        RuleFor(m => m.SignUpRequestSubmittedBy)
            .NotNull().WithMessage(ValidationMessages.NullField)
            .Must(RemosValidationHelpers.BeAGuid).WithMessage(RemosValidationMessages.GuidField);

        RuleFor(m => m.TradeContact)
            .SetValidator(contactUpdateValidator!);
    }
}
