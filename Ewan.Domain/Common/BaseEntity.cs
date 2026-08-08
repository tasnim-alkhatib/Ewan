using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Domain.Common
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int CreatedByUserId { get; set; }
        public int? UpdatedByUserId { get; set; }
    }
}