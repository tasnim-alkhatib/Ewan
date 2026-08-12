using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Application.DTOs.Inquiries
{
    public class CreateInquiryRequestValidator : AbstractValidator<CreateInquiryRequest>
    {
        public CreateInquiryRequestValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);

            // رقم هاتف سعودي بسيط (05xxxxxxxx أو +9665xxxxxxxx) - كفاية لمرحلة التأسيس
            RuleFor(x => x.Phone)
                .NotEmpty()
                .Matches(@"^(\+?966|0)?5\d{8}$")
                .WithMessage("رقم الهاتف غير صحيح، لازم يكون رقم سعودي صحيح");

            RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
            RuleFor(x => x.Message).MaximumLength(1000);
        }
    }
}
