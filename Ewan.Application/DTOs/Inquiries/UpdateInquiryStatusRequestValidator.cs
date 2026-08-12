using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Application.DTOs.Inquiries
{
    public class UpdateInquiryStatusRequestValidator : AbstractValidator<UpdateInquiryStatusRequest>
    {
        public UpdateInquiryStatusRequestValidator()
        {
            RuleFor(x => x.Notes).MaximumLength(1000);
        }
    }
}
