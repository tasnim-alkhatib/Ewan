using Ewan.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Domain.Entities
{
    public class Location : BaseEntity
    {
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string AddressAr { get; set; } = string.Empty;
        public string AddressEn { get; set; } = string.Empty;

        // إحداثيات دقيقة لعرض الفرع على خريطة تفاعلية في الموقع
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        
        public string? Phone { get; set; }
        public string? WorkingHoursAr { get; set; }
        public string? WorkingHoursEn { get; set; }
    }
}
