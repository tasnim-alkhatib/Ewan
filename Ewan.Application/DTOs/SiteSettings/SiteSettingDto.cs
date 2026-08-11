using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Application.DTOs.SiteSettings
{
    public class SiteSettingDto
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string? ValueAr { get; set; }
        public string? ValueEn { get; set; }
    }

    public class UpsertSiteSettingRequest
    {
        public string Key { get; set; } = string.Empty;
        public string? ValueAr { get; set; }
        public string? ValueEn { get; set; }
    }
}
