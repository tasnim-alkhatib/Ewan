using System;
using System.Collections.Generic;
using System.Text;
using Ewan.Application.DTOs.Auth;

namespace Ewan.Application.Interfaces
{
    public interface IAuthService
    {
        // بترجع null لو الإيميل أو الباسورد غلط - الـ Controller هو اللي بيقرر شكل رسالة الخطأ
        Task<LoginResponse?> LoginAsync(LoginRequest request);
    }
}
