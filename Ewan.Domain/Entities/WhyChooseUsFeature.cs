using Ewan.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Domain.Entities
{
    public class WhyChooseUsFeature : BaseEntity
    {
        public string TitleAr { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public string DescriptionAr { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        public string? IconUrl { get; set; }
        public int SortOrder { get; set; }
    }
}