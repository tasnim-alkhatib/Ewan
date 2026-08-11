using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Application.DTOs.Locations
{
    public class LocationDto
    {
        public int Id { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string AddressAr { get; set; } = string.Empty;
        public string AddressEn { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? Phone { get; set; }
        public string? WorkingHoursAr { get; set; }
        public string? WorkingHoursEn { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpsertLocationRequest
    {
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string AddressAr { get; set; } = string.Empty;
        public string AddressEn { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? Phone { get; set; }
        public string? WorkingHoursAr { get; set; }
        public string? WorkingHoursEn { get; set; }
        public bool IsActive { get; set; } = true;
    }

}
