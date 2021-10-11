using System;
namespace AnalyticsDomain.Models
{
    public class FeaturedWeedMetrics
    {
        public string ProductName { get; set; }
        public int WeedId { get; set; }
        public DateTime FeaturedDateStart { get; set; }
        public DateTime FeaturedDateEnd { get; set; }
        public int Duration { get; set; }
        public decimal Price { get; set; }
        public int Views { get; set; }
        public int WeightedViews { get; set; }
        public int Lists { get; set; }
        public int WeightedLists { get; set; }
        public int Impressions { get; set; }
        public int WeightedImpressions { get; set; }
        public bool IsBasicProduct { get; set; }
    }
}
