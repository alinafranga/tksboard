using System;
using System.Collections.Generic;

namespace AnalyticsDomain.Models
{
    public class ProductsMetricsQuery
    {
        public List<BasicWeed> ProductList { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<BasicRetailer> RetailersList { get; set; }
    }
}
