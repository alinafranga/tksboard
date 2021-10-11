using System;
using System.Collections.Generic;

namespace AnalyticsDomain.Models
{
    public class ImpressionExt
    {
        public int MetricCount { get; set; }
        public List<WeedMetric> WeedIds { get; set; }
        public int? ProducerConnectUserId { get; set; }
        public string IpAddress { get; set; }
        public DateTime? MeasuredDate { get; set; }
        public int WeightedCount { get; set; }
        public bool IsBasicProduct { get; set; }
        public int ProvinceId { get; set; }
    }
}
