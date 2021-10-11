using System;
using System.Collections.Generic;

#nullable disable

namespace AnalyticsDAL.Models
{
    public partial class Province
    {
        public int Id { get; set; }
        public string ProvinceName { get; set; }
        public string ProvinceAbbr { get; set; }
        public bool Active { get; set; }
    }
}
