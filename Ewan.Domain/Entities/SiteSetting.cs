using Ewan.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Domain.Entities
{
    public class SiteSetting : BaseEntity
    {
        /*
         * ليه key و value بدل ما نخزن كل حاجة في جدول واحد؟
         * عشان تقدري تضيفي إعداد جديد كامل من لوحة التحكم من غير أي تعديل في الـكود أو قاعدة البيانات
         * */
        public string Key { get; set; } = string.Empty;   // مثال: "phone_number", "facebook_url", "whatsapp_number"
        public string? ValueAr { get; set; }
        public string? ValueEn { get; set; }
    }
}
