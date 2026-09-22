// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Defra.Trade.Common.Functions.Isolated.Validation;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Inbound;
using FluentValidation;

namespace Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Validators;

public sealed class LogisticsLocationEstablishmentUpdateValidator : AbstractValidator<LogisticsLocationEstablishmentUpdate>
{
    public LogisticsLocationEstablishmentUpdateValidator()
    {
        RuleFor(m => m.InspectionLocationId)
            .NotNull().WithMessage(ValidationMessages.NullField)
            .Must(RemosValidationHelpers.BeAGuid).WithMessage(RemosValidationMessages.GuidField);

        RuleFor(m => m.Status)
            .NotNull().WithMessage(ValidationMessages.NullField)
            .Equal("Removed", StringComparer.OrdinalIgnoreCase).WithMessage(RemosValidationMessages.EqualField);

        RuleFor(m => m.TradePartyId)
            .NotNull().WithMessage(ValidationMessages.NullField)
            .Must(RemosValidationHelpers.BeAGuid).WithMessage(RemosValidationMessages.GuidField);
    }
}
