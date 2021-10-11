using System;
using System.Collections.Generic;

#nullable disable

namespace AnalyticsDAL.Models
{
    public partial class TempRp
    {
        public int Id { get; set; }
        public int RetailerId { get; set; }
        public int ProvinceId { get; set; }
    }
}
