using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Application.DTOs.Locations
{
    public class UpsertLocationRequestValidator : AbstractValidator<UpsertLocationRequest>
    {
        public UpsertLocationRequestValidator()
        {
            RuleFor(x => x.NameAr).NotEmpty().MaximumLength(200);
            RuleFor(x => x.NameEn).NotEmpty().MaximumLength(200);
            RuleFor(x => x.AddressAr).NotEmpty();
            RuleFor(x => x.AddressEn).NotEmpty();

            // إحداثيات صحيحة على الخريطة - برة النطاق ده يبقى غلط أكيد
            RuleFor(x => x.Latitude).InclusiveBetween(-90, 90);
            RuleFor(x => x.Longitude).InclusiveBetween(-180, 180);
        }
    }
}
