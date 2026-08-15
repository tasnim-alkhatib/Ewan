using System;
using System.Collections.Generic;
using System.Text;
using Ewan.Domain.Common;
using Ewan.Domain.Enums;

namespace Ewan.Domain.Entities
{
    public class Inquiry : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Message { get; set; }

        public Sector? Sector { get; set; } // القطاع اللي العميل مهتم بيه , لو متاح
        public InquirySource Source { get; set; } // عرف الاستفسار جاي من أنهي صفحة بالظبط، مفيد لتحليل مصادر الطلبات
        public InquiryStatus Status { get; set; } = InquiryStatus.New;

        public int? ServiceItemId { get; set; } // لو الاستفسار جاي من كارت خدمة أو عرض معين بالذات
        public ServiceItem? ServiceItem { get; set; }

        public int? OfferId { get; set; } // لو الاستفسار جاي من كارت خدمة أو عرض معين بالذات
        public Offer? Offer { get; set; }

        public string? Notes { get; set; }   // ملاحظات المتابعة من فريق المبيعات في اللوحة - مش بتظهر للعميل
    }
}
