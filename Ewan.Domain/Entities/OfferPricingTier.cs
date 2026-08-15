using Ewan.Domain.Common;
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
        public string DurationAr { get; set; } = string.Empty;   // مدة العقد 
        public string DurationEn { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? RenewalPrice { get; set; } //  سعر التجديد لو مختلف عن سعر التعاقد الأول 
        public int SortOrder { get; set; }
    }
}
