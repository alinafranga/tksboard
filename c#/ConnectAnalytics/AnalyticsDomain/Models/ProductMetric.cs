using System;

namespace AnalyticsDomain.Models
{
    public class ProductMetric
    {
        public string ProductName { get; set; }
        public int WeedId { get; set; }
        public int MetricCount { get; set; }
        public int WeightedMetric { get; set; }
        public string RetailerDisplayName { get; set; }
        public int? RetailerId { get; set; }
        public DateTime MeasuredDate { get; set; }
        public bool IsBasicProduct { get; set; }
    }
}
