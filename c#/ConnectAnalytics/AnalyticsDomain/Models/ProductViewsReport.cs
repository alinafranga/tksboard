using System;
using System.Collections.Generic;
using System.Text;

namespace AnalyticsDomain.Models
{
    public class ProductViewsReport
    {
        public int WeedId { get; set; }
        public string ProductName { get; set; }
        public bool IsBasicProduct { get; set; }
        public List<Metric> Views { get; set; }
    }
}
