using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Application.DTOs.Users
{
    public class CreateAdminUserRequestValidator : AbstractValidator<CreateAdminUserRequest>
    {
        public CreateAdminUserRequestValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();

            // 8 أحرف على الأقل، فيها حرف ورقم - حد أدنى معقول لأمان لوحة التحكم
            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8)
                .Matches(@"[A-Za-z]").WithMessage("كلمة المرور لازم تحتوي على حرف واحد على الأقل")
                .Matches(@"\d").WithMessage("كلمة المرور لازم تحتوي على رقم واحد على الأقل");
        }
    }
}
