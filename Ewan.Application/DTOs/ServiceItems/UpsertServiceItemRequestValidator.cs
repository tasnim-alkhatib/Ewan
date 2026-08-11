using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Application.DTOs.ServiceItems
{
    public class UpsertServiceItemRequestValidator : AbstractValidator<UpsertServiceItemRequest>
    {
        public UpsertServiceItemRequestValidator()
        {
            RuleFor(x => x.NameAr).NotEmpty().MaximumLength(200);
            RuleFor(x => x.NameEn).NotEmpty().MaximumLength(200);
            RuleFor(x => x.ImageUrl).NotEmpty().MaximumLength(500);
            RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);

            // Slug بيتستخدم في روابط الموقع (مثلا /services/driver) فلازم يكون بصيغة موحدة:
            // حروف صغيرة وأرقام وشرطات بس، من غير مسافات أو رموز
            RuleFor(x => x.Slug)
                .NotEmpty()
                .Matches("^[a-z0-9]+(-[a-z0-9]+)*$")
                .WithMessage("الـ Slug لازم يكون حروف إنجليزية صغيرة وأرقام وشرطات بس، مثال: personal-driver");
        }
    }
}
