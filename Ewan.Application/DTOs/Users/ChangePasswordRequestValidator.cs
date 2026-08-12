using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Application.DTOs.Users
{
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.CurrentPassword).NotEmpty();
            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .MinimumLength(8)
                .Matches(@"[A-Za-z]").WithMessage("كلمة المرور الجديدة لازم تحتوي على حرف واحد على الأقل")
                .Matches(@"\d").WithMessage("كلمة المرور الجديدة لازم تحتوي على رقم واحد على الأقل");
        }
    }
}
