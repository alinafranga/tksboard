using System;
using System.Collections.Generic;

#nullable disable

namespace AnalyticsDAL.Models
{
    public partial class SummaryProductDay
    {
        public int Id { get; set; }
        public int? ProducerId { get; set; }
        public int WeedId { get; set; }
        public bool? IsBasicProduct { get; set; }
        public int ImpressionNumber { get; set; }
        public int ListNumber { get; set; }
        public int ViewsNumber { get; set; }
        public int? FollowsNumber { get; set; }
        public DateTime Date { get; set; }
        public int ProvinceId { get; set; }
    }
}
