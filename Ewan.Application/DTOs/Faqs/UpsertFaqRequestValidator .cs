using Ewan.Application.DTOs.Faqs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Application.DTOs.Faqs
{
    public class UpsertFaqRequestValidator : AbstractValidator<UpsertFaqRequest>
    {
        public UpsertFaqRequestValidator()
        {
            RuleFor(x => x.QuestionAr).NotEmpty().MaximumLength(500);
            RuleFor(x => x.QuestionEn).NotEmpty().MaximumLength(500);
            RuleFor(x => x.AnswerAr).NotEmpty();
            RuleFor(x => x.AnswerEn).NotEmpty();
            RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
        }
    }
}
