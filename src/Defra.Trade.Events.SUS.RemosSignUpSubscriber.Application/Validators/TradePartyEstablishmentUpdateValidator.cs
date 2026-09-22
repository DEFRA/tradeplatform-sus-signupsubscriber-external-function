// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Defra.Trade.Common.Functions.Isolated.Validation;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Inbound;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Inbound.Establishment.Update;
using FluentValidation;

namespace Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Validators;

public sealed class TradePartyEstablishmentUpdateValidator : AbstractValidator<TradeParty>
{
    public TradePartyEstablishmentUpdateValidator(
        IValidator<LogisticsLocationEstablishmentUpdate?> logisticsLocationValidator)
    {
        RuleFor(m => m.Id)
            .NotNull().WithMessage(ValidationMessages.NullField)
            .Must(RemosValidationHelpers.BeAGuid).WithMessage(RemosValidationMessages.GuidField);

        RuleFor(m => m.OrgId)
            .NotNull().WithMessage(ValidationMessages.NullField)
            .Must(RemosValidationHelpers.BeAGuid).WithMessage(RemosValidationMessages.GuidField);

        RuleFor(m => m.LogisticsLocation)
            .SetValidator(logisticsLocationValidator);
    }
}
