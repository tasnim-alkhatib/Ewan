using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Domain.Entities
{
    public class Inquiry : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Message { get; set; }

        //public Sector? Sector { get; set; }
        //public InquirySource Source { get; set; }
        //public InquiryStatus Status { get; set; } = InquiryStatus.New;

        public int? ServiceItemId { get; set; }
        public ServiceItem? ServiceItem { get; set; }

        public int? OfferId { get; set; }
        public Offer? Offer { get; set; }

        public string? Notes { get; set; }   // ملاحظات المتابعة من فريق المبيعات في اللوحة
    }
}
