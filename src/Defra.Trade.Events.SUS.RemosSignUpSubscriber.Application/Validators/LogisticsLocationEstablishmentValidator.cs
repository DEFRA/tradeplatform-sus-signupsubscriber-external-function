﻿// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Inbound;
using FluentValidation;

namespace Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Validators;

public sealed class LogisticsLocationEstablishmentValidator : AbstractValidator<LogisticsLocationEstablishmentCreate>
{
    public LogisticsLocationEstablishmentValidator(
        IValidator<Address> addressValidator)
    {
        RuleFor(m => m.Name)
            .MaximumLength(100);

        RuleFor(m => m.Email)
            .MaximumLength(100);

        RuleFor(m => m.RemosEstablishmentSchemeNumber)
            .MaximumLength(25);

        RuleFor(m => m.Address)
            .SetValidator(addressValidator!);
    }
}