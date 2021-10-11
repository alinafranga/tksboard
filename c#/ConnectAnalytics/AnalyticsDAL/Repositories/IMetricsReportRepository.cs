using AnalyticsDAL.Helpers.Enums;
using AnalyticsDomain.Models;
using AnalyticsDomain.Models.Graphics;
using System.Collections.Generic;

namespace AnalyticsDAL.Repositories
{
    public interface IMetricsReportRepository
    {
        List<ProductViewsReport> GetProductsViewsReport(ProductsMetricsQuery productsMetricsQuery);
        OverviewMetricsInfo GetOverviewProducerAnalytics(int producerId, int provinceId, int month, int year);
        List<GraphicData> GetAvailableMonths(int userId, int provinceId);
        List<GraphicData> GetProductAvailableMonths(int userId, int provinceId);
        MultiAxisGraphicData GetProducerMetrics(int producerId, int provinceId, int month, int year, MetricTypeEnum metricTypeEnum);
        MultiAxisGraphicData GetProductMetrics(int weedId, int provinceId, int month, int year, MetricTypeEnum metricTypeEnum);
        OverviewMetricsInfo GetOverviewByProductAnalytics(int weedId, int provinceId, int month, int year);
    }
}
