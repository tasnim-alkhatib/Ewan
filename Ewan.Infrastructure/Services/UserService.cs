using Ewan.Application.DTOs.Common;
using Ewan.Application.DTOs.Users;
using Ewan.Application.Interfaces;
using Ewan.Domain.Entities;
using Ewan.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ewan.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly EwanDbContext _db;
        private readonly PasswordHasher<AdminUser> _passwordHasher = new();

        public UserService(EwanDbContext db)
        {
            _db = db;
        }

        public async Task<PagedResult<AdminUserDto>> GetAllAsync(PagedRequest request)
        {
            var query = _db.AdminUsers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(u => u.FullName.Contains(request.Search) || u.Email.Contains(request.Search));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(u => u.FullName)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(u => ToDto(u))
                .ToListAsync();

            return new PagedResult<AdminUserDto>
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<AdminUserDto?> GetByIdAsync(int id)
        {
            var user = await _db.AdminUsers.FindAsync(id);
            return user is null ? null : ToDto(user);
        }

        public async Task<AdminUserDto?> CreateAsync(CreateAdminUserRequest request)
        {
            var emailExists = await _db.AdminUsers.AnyAsync(u => u.Email == request.Email.ToLower());
            if (emailExists) return null;

            var user = new AdminUser
            {
                FullName = request.FullName,
                Email = request.Email.ToLower(),
                Role = request.Role
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            _db.AdminUsers.Add(user);
            await _db.SaveChangesAsync();
            return ToDto(user);
        }

        public async Task<AdminUserDto?> UpdateAsync(int id, UpdateAdminUserRequest request)
        {
            var user = await _db.AdminUsers.FindAsync(id);
            if (user is null) return null;

            user.FullName = request.FullName;
            user.Role = request.Role;
            user.IsActive = request.IsActive;

            await _db.SaveChangesAsync();
            return ToDto(user);
        }

        public async Task<bool> DeleteAsync(int id, int currentUserId)
        {
            // منع أي مستخدم من حذف حسابه هو نفسه بالغلط، وده كان يسيب اللوحة من غير SuperAdmin
            if (id == currentUserId) return false;

            var user = await _db.AdminUsers.FindAsync(id);
            if (user is null) return false;

            user.IsDeleted = true;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            var user = await _db.AdminUsers.FindAsync(userId);
            if (user is null) return false;

            var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);
            if (verifyResult == PasswordVerificationResult.Failed) return false;

            user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
            await _db.SaveChangesAsync();
            return true;
        }

        private static AdminUserDto ToDto(AdminUser u) => new()
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            Role = u.Role,
            IsActive = u.IsActive,
            LastLoginAt = u.LastLoginAt
        };
    }
}
