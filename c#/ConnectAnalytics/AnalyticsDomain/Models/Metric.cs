using System;
using System.Collections.Generic;
using System.Text;

namespace AnalyticsDomain.Models
{
    public class Metric
    {
        public DateTime MeasuredDate { get; set; }
        public int Count { get; set; }
        public int WeightedCount { get; set; }
    }
}
