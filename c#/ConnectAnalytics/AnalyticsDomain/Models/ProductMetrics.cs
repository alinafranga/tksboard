namespace AnalyticsDomain.Models
{
    public class ProductMetrics
    {
        public string Name { get; set; }
        public int WeedId { get; set; }
        public int Impressions { get; set; }
        public int WeightedImpressions { get; set; }
        public int Views { get; set; }
        public int WeightedViews { get; set; }
        public int RetailerLists { get; set; }
        public int WeightedRetailerLists { get; set; }
        public bool IsBasicProduct { get; set; }
    }
}
