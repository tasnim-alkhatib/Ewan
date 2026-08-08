using Ewan.Domain.Common;
using Ewan.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Domain.Entities
{
    public class AdminUser : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public AdminRole Role { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }
}
