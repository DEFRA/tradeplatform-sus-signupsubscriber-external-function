// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Defra.Trade.Common.Functions.Isolated.Models.Enum;
using Defra.Trade.Common.Functions.Isolated.Validation;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Dtos.Inbound.Interfaces;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Models;
using Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Validators;
using FluentValidation;

namespace Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Extensions;

public static class AbstractValidatorExtensions
{
    public static void AddCommonMessageHeaderValidation<T>(this AbstractValidator<T> validator)
        where T : ICommonMessageHeader
    {
        validator.RuleFor(m => m.MessageId)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(ValidationMessages.NullField)
            .Must(RemosValidationHelpers.BeAGuid).WithMessage(RemosValidationMessages.GuidField);

        validator.RuleFor(m => m.CorrelationId)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(ValidationMessages.NullField)
            .Must(RemosValidationHelpers.BeAGuid).WithMessage(RemosValidationMessages.GuidField);

        validator.RuleFor<string>(m => m.CausationId!)
            .Must(RemosValidationHelpers.BeAGuid).WithMessage(RemosValidationMessages.GuidField);

        validator.RuleFor<string>(m => m.ContentType!)
            .Equal(RemosSignUpServiceHeaderConstants.ContentType, StringComparer.OrdinalIgnoreCase).WithMessage(RemosValidationMessages.EqualField);

        validator.RuleFor(m => m.EntityKey)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(ValidationMessages.NullField)
            .Must(RemosValidationHelpers.BeAGuid).WithMessage(RemosValidationMessages.GuidField);

        validator.RuleFor<string>(m => m.PublisherId!)
            .Equal(RemosSignUpServiceHeaderConstants.PublisherId, StringComparer.OrdinalIgnoreCase).WithMessage(RemosValidationMessages.EqualField);

        validator.RuleFor<string>(m => m.OrganisationId!)
            .Must(RemosValidationHelpers.BeAGuid).WithMessage(RemosValidationMessages.GuidField);

        validator.RuleFor(m => m.Type)
            .Equal(EventType.Internal).WithMessage(RemosValidationMessages.EqualField);

        validator.RuleFor(m => m.TimestampUtc)
            .GreaterThan(0).WithMessage(ValidationMessages.NullField);
    }
}
