using AnalyticsDomain.Models;
using System.Collections.Generic;

namespace AnalyticsDAL.Repositories
{
    public interface IMetricsRepository
    {
        List<ProductMetrics> GetProductMetrics(ProductsMetricsQuery productsMetricsQuery);
        List<FeaturedWeedMetrics> GetFeaturedProductMetrics(FeaturedWeedMetricQuery featuredWeedMetricQuery);
        List<ProductMetrics> GetProductMetricsWithoutImpressions(ProductsMetricsQuery productsMetricsQuery);
        List<ProductMetric> GetProductsImpressions(ProductsMetricsQuery productsMetricsQuery);
        List<ProductMetric> GetProductsViews(ProductsMetricsQuery productsMetricsQuery);
        List<ProductMetric> GetProductsLists(ProductsMetricsQuery productsMetricsQuery);
        int GetMostViewedProduct(List<int> weedIds);
    }
}
