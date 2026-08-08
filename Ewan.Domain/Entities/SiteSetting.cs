using Ewan.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Domain.Entities
{
    public class SiteSetting : BaseEntity
    {
        // Key/Value عشان اللوحة تقدر تضيف إعدادات جديدة من غير ما تعمل Migration كل مرة
        public string Key { get; set; } = string.Empty;   // مثال: "phone_number", "facebook_url", "whatsapp_number"
        public string? ValueAr { get; set; }
        public string? ValueEn { get; set; }
    }
}
