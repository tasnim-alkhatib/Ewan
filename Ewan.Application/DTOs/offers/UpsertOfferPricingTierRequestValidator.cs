using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace Ewan.Application.DTOs.offers
{
    public class UpsertOfferPricingTierRequestValidator : AbstractValidator<UpsertOfferPricingTierRequest>
    {
        public UpsertOfferPricingTierRequestValidator()
        {
            RuleFor(x => x.NationalityAr).NotEmpty().MaximumLength(150);
            RuleFor(x => x.NationalityEn).NotEmpty().MaximumLength(150);
            RuleFor(x => x.DurationAr).NotEmpty().MaximumLength(100);
            RuleFor(x => x.DurationEn).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("السعر لازم يكون أكبر من صفر");
            RuleFor(x => x.RenewalPrice).GreaterThan(0)
                .When(x => x.RenewalPrice.HasValue)
                .WithMessage("سعر التجديد لازم يكون أكبر من صفر لو موجود");
        }
    }

    public class UpsertOfferRequestValidator : AbstractValidator<UpsertOfferRequest>
    {
        public UpsertOfferRequestValidator()
        {
            RuleFor(x => x.TitleAr).NotEmpty().MaximumLength(300);
            RuleFor(x => x.TitleEn).NotEmpty().MaximumLength(300);
            RuleFor(x => x.ImageUrl).NotEmpty().MaximumLength(500);
            RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
                .WithMessage("تاريخ الانتهاء لازم يكون بعد تاريخ البداية");

            // لازم يكون فيه صف سعر واحد على الأقل - عرض من غير أسعار مالوش معنى
            RuleFor(x => x.PricingTiers)
                .NotEmpty()
                .WithMessage("لازم يكون فيه صف سعر واحد على الأقل");

            RuleForEach(x => x.PricingTiers).SetValidator(new UpsertOfferPricingTierRequestValidator());
        }
    }
}
