using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Domain.Entities
{
    public class Offer : BaseEntity
    {
        public string TitleAr { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? WhatsAppLink { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int SortOrder { get; set; }

        public ICollection<OfferPricingTier> PricingTiers { get; set; } = new List<OfferPricingTier>();
    }
}
