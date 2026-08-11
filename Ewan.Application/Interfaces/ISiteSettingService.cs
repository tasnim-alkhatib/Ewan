using Ewan.Application.DTOs.Common;
using Ewan.Application.DTOs.SiteSettings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Application.Interfaces
{
    public interface ISiteSettingService
    {
        // بيستخدمه الموقع العام - كل الإعدادات كـ Dictionary (Key -> Value) عشان الفرونت
        // يقدر يجيب أي إعداد بسهولة زي settings["phone_number"]
        Task<Dictionary<string, SiteSettingDto>> GetAllAsPublicDictionaryAsync();

        Task<PagedResult<SiteSettingDto>> GetAllAsync(PagedRequest request);
        Task<SiteSettingDto?> GetByKeyAsync(string key);

        // Upsert بمعنى الكلمة: لو الـ Key مش موجود بيتعمل، لو موجود بيتحدّث
        Task<SiteSettingDto> UpsertAsync(UpsertSiteSettingRequest request, int adminUserId);

        Task<bool> DeleteAsync(string key);
    }
}
