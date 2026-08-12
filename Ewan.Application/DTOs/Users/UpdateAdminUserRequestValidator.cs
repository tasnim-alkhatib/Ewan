using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Application.DTOs.Users
{
    public class UpdateAdminUserRequestValidator : AbstractValidator<UpdateAdminUserRequest>
    {
        public UpdateAdminUserRequestValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        }
    }
}
