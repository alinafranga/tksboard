using System;
using System.Collections.Generic;

#nullable disable

namespace AnalyticsDAL.Models
{
    public partial class List
    {
        public int Id { get; set; }
        public int MetricCount { get; set; }
        public int WeedId { get; set; }
        public int? ProducerConnectUserId { get; set; }
        public string IpAddress { get; set; }
        public DateTime MeasuredDate { get; set; }
        public int WeightedCount { get; set; }
        public bool IsBasicProduct { get; set; }
        public int? ProvinceId { get; set; }
        public int? ProducerId { get; set; }
    }
}
