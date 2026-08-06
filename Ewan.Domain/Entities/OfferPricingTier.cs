using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Domain.Entities
{
    public class OfferPricingTier : BaseEntity
    {
        public int OfferId { get; set; }
        public Offer? Offer { get; set; }

        public string NationalityAr { get; set; } = string.Empty;
        public string NationalityEn { get; set; } = string.Empty;
        public string DurationAr { get; set; } = string.Empty;   // "15 يوم" / "3 شهور" / "سنة"
        public string DurationEn { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? RenewalPrice { get; set; }
        public int SortOrder { get; set; }
    }
}
