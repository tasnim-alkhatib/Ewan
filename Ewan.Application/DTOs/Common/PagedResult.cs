using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Application.DTOs.Common
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }

    public class PagedRequest
    {
        private int _pageSize = 10;
        public int PageNumber { get; set; } = 1;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > 50 ? 50 : value;   // حد أقصى عشان محدش يجيب 10000 صف مرة واحدة
        }
        public string? Search { get; set; }
    }
}
