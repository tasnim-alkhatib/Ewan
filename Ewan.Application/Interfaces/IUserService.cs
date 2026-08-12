using Ewan.Application.DTOs.Common;
using Ewan.Application.DTOs.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Application.Interfaces
{
    public interface IUserService
    {
        Task<PagedResult<AdminUserDto>> GetAllAsync(PagedRequest request);
        Task<AdminUserDto?> GetByIdAsync(int id);

        // بترجع null لو الإيميل مستخدم بالفعل - الـ Controller بيقرر شكل رسالة الخطأ
        Task<AdminUserDto?> CreateAsync(CreateAdminUserRequest request);

        Task<AdminUserDto?> UpdateAsync(int id, UpdateAdminUserRequest request);
        Task<bool> DeleteAsync(int id, int currentUserId);

        // بترجع false لو كلمة المرور الحالية غلط
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request);
    }
}
