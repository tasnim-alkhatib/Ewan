using Ewan.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Application.DTOs.Users
{
    public class AdminUserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public AdminRole Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }

    public class CreateAdminUserRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public AdminRole Role { get; set; }
    }

    // تعديل بيانات مستخدم - من غير الباسورد (ده Endpoint منفصل بيتعمله)
    public class UpdateAdminUserRequest
    {
        public string FullName { get; set; } = string.Empty;
        public AdminRole Role { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
