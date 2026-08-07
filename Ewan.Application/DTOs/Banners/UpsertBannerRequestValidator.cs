using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Application.DTOs.Banners
{
    public class UpsertBannerRequestValidator : AbstractValidator<UpsertBannerRequest>
    {
        public UpsertBannerRequestValidator()
        {
            RuleFor(x => x.TitleAr).NotEmpty().MaximumLength(300);
            RuleFor(x => x.TitleEn).NotEmpty().MaximumLength(300);
            RuleFor(x => x.ImageUrl).NotEmpty().MaximumLength(500);
            RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
                .WithMessage("تاريخ الانتهاء لازم يكون بعد تاريخ البداية");
        }
    }
}
