using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Domain.Entities
{
    internal class Banner : BaseEntity
    {
        public string TitleAr { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public string? SubtitleAr { get; set; }
        public string? SubtitleEn { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? LinkUrl { get; set; }         // ممكن يكون رابط داخلي أو خارجي (واتساب مثلا)
        //public BannerLocation Location { get; set; }
        public int SortOrder { get; set; }
        public DateTime? StartDate { get; set; }      // لو عايز البانر يظهر في فترة محددة فقط
        public DateTime? EndDate { get; set; }
    }
}
