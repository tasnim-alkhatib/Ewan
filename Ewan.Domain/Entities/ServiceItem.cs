using System;
using System.Collections.Generic;
using System.Text;
using Ewan.Domain.Common;
using Ewan.Domain.Enums;

namespace Ewan.Domain.Entities
{
    public class ServiceItem : BaseEntity
    {
        public Sector Sector { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }
}
