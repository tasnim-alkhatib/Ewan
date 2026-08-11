using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Application.DTOs.SiteSettings
{
    public class UpsertSiteSettingRequestValidator : AbstractValidator<UpsertSiteSettingRequest>
    {
        public UpsertSiteSettingRequestValidator()
        {
            // Key هو المعرّف اللي الفرونت بيستخدمه عشان يجيب القيمة (مثلا "phone_number")
            // لازم يكون بصيغة موحدة عشان محدش يكتبه بشكل مختلف كل مرة
            RuleFor(x => x.Key)
                .NotEmpty()
                .Matches("^[a-z0-9_]+$")
                .WithMessage("الـ Key لازم يكون حروف إنجليزية صغيرة وأرقام و underscore بس، مثال: phone_number");
        }
    }
}
