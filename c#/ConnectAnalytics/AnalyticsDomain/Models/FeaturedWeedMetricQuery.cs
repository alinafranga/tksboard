using System;
using System.Collections.Generic;
namespace AnalyticsDomain.Models
{
    public class FeaturedWeedMetricQuery
    {
        public List<BasicFeaturedWeed> FeaturedWeeds { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
