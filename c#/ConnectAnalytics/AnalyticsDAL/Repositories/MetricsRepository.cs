using AnalyticsDAL.Models;
using AnalyticsDomain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnalyticsDAL.Repositories
{
    public class MetricsRepository : IMetricsRepository
    {
        private readonly AnalyticsContext _analyticsContext;
        public MetricsRepository(AnalyticsContext analyticsContext)
        {
            _analyticsContext = analyticsContext;
        }

        public List<FeaturedWeedMetrics> GetFeaturedProductMetrics(FeaturedWeedMetricQuery featuredWeedMetricQuery)
        {
            List<FeaturedWeedMetrics> featuredProductMetrics = new List<FeaturedWeedMetrics>();
            ConcurrentBag<FeaturedWeedMetrics> cb = new ConcurrentBag<FeaturedWeedMetrics>();
            var weedIds = featuredWeedMetricQuery.FeaturedWeeds.Select(x => x.WeedId).ToList();

            var listMetric = (from l in _analyticsContext.List
                              where l.MeasuredDate.Date >= featuredWeedMetricQuery.StartDate.Date && l.MeasuredDate.Date <= featuredWeedMetricQuery.EndDate.Date
                              && weedIds.Contains(l.WeedId) && !l.IsBasicProduct
                              select l).AsNoTracking().ToList();

            var viewMetric = (from v in _analyticsContext.View
                              where v.MeasuredDate.Date >= featuredWeedMetricQuery.StartDate.Date && v.MeasuredDate.Date <= featuredWeedMetricQuery.EndDate.Date
                              && weedIds.Contains(v.WeedId) && !v.IsBasicProduct
                              select v).AsNoTracking().ToList();

            var impressionMetric = (from imp in _analyticsContext.Impression
                                    where imp.MeasuredDate.Date >= featuredWeedMetricQuery.StartDate.Date && imp.MeasuredDate.Date <= featuredWeedMetricQuery.EndDate.Date
                                    && weedIds.Contains(imp.WeedId) && !imp.IsBasicProduct
                                    select imp).AsNoTracking().ToList();

            Parallel.ForEach(featuredWeedMetricQuery.FeaturedWeeds, (weed) =>
            {
                var endDateToCompare = weed.FeaturedEndDate.Date > featuredWeedMetricQuery.EndDate.Date ? featuredWeedMetricQuery.EndDate.Date : weed.FeaturedEndDate.Date;
                var view = viewMetric.Where(x => x.WeedId == weed.WeedId && x.MeasuredDate.Date >= weed.FeaturedStartDate.Date && x.MeasuredDate.Date <= endDateToCompare).ToList();
                var list = listMetric.Where(x => x.WeedId == weed.WeedId && x.MeasuredDate.Date >= weed.FeaturedStartDate.Date && x.MeasuredDate.Date <= endDateToCompare).ToList();
                var impression = impressionMetric.Where(x => x.WeedId == weed.WeedId && x.MeasuredDate.Date >= weed.FeaturedStartDate.Date && x.MeasuredDate.Date <= endDateToCompare);
                var featureProductMetric = new FeaturedWeedMetrics() { WeedId = weed.WeedId };

                if (impression != null)
                {
                    featureProductMetric.Impressions = impression.Sum(x=>x.MetricCount);
                    featureProductMetric.WeightedImpressions = impression.Sum(x => x.WeightedCount);
                }

                if (view != null)
                {
                    featureProductMetric.Views = view.Sum(x=>x.MetricCount);
                    featureProductMetric.WeightedViews = view.Sum(x => x.WeightedCount);
                }
                if (list != null)
                {
                    featureProductMetric.Lists = list.Sum(x=>x.MetricCount);
                    featureProductMetric.WeightedLists = list.Sum(x => x.WeightedCount);
                }
                cb.Add(featureProductMetric);
            });
            featuredProductMetrics = cb.ToList();
            return featuredProductMetrics;
        }

        public int GetMostViewedProduct(List<int> weedIds)
        {
            var totalViews = (from v in _analyticsContext.View
                         where weedIds.Contains(v.WeedId) && !v.IsBasicProduct
                         group v by new { v.WeedId } into g
                         select new
                         {
                             Id = g.Key.WeedId,
                             TotalView = g.Sum(x => x.MetricCount)
                         }).AsNoTracking().ToList();
            int foundMaxProduct = -1;
            if (totalViews.Count > 0)
            {
                var maxViewValue = totalViews[0].TotalView;
                foundMaxProduct = totalViews[0].Id;
                for (var i = 1; i < totalViews.Count; i++)
                {
                    if(maxViewValue < totalViews[i].TotalView)
                    {
                        maxViewValue = totalViews[i].TotalView;
                        foundMaxProduct = totalViews[i].Id;
                    }
                }
            }
            
            return foundMaxProduct;
        }

        public List<ProductMetrics> GetProductMetrics(ProductsMetricsQuery productsMetricsQuery)
        {
            List<ProductMetrics> productMetrics = new List<ProductMetrics>();
            ConcurrentBag<ProductMetrics> cb = new ConcurrentBag<ProductMetrics>();
            var weedIds = productsMetricsQuery.ProductList.Select(x => x.WeedId).ToList();

            var impressionMetric = (from imp in _analyticsContext.Impression
                                    where imp.MeasuredDate.Date >= productsMetricsQuery.StartDate.Date && imp.MeasuredDate.Date <= productsMetricsQuery.EndDate.Date
                                    && weedIds.Contains(imp.WeedId) && !imp.IsBasicProduct
                                    group imp by new { imp.WeedId, imp.IsBasicProduct } into g
                                    select new
                                    {
                                        g.Key.WeedId,
                                        Impressions = g.Sum(x => x.MetricCount),
                                        WeightedImpressions = g.Sum(x => x.WeightedCount),
                                        IsBasicProduct = g.Key.IsBasicProduct
                                    }).AsNoTracking().ToList();

            var listMetric = (from imp in _analyticsContext.List
                              where imp.MeasuredDate.Date >= productsMetricsQuery.StartDate.Date && imp.MeasuredDate.Date <= productsMetricsQuery.EndDate.Date
                              && weedIds.Contains(imp.WeedId) && !imp.IsBasicProduct
                              group imp by new { imp.WeedId, imp.IsBasicProduct } into g
                              select new
                              {
                                  g.Key.WeedId,
                                  RetailerLists = g.Sum(x => x.MetricCount),
                                  WeightedRetailerLists = g.Sum(x => x.WeightedCount),
                                  IsBasicProduct = g.Key.IsBasicProduct
                              }).AsNoTracking().ToList();

            var viewMetric = (from v in _analyticsContext.View
                              where v.MeasuredDate.Date >= productsMetricsQuery.StartDate.Date && v.MeasuredDate.Date <= productsMetricsQuery.EndDate.Date
                              && weedIds.Contains(v.WeedId) && !v.IsBasicProduct
                              group v by new { v.WeedId, v.IsBasicProduct } into g
                              select new 
                              {
                                  g.Key.WeedId,
                                  Views = g.Sum(x => x.MetricCount),
                                  WeightedViews = g.Sum(x => x.WeightedCount),
                                  IsBasicProduct = g.Key.IsBasicProduct
                              }).AsNoTracking().ToList();

            Parallel.ForEach(productsMetricsQuery.ProductList, (weed) =>
            {
                var impression = impressionMetric.Where(x => x.WeedId == weed.WeedId).FirstOrDefault();
                var view = viewMetric.Where(x => x.WeedId == weed.WeedId).FirstOrDefault();
                var list = listMetric.Where(x => x.WeedId == weed.WeedId).FirstOrDefault();
                var productMetric = new ProductMetrics() { WeedId = weed.WeedId, Name = weed.Name };
                if (impression != null)
                {
                    productMetric.Impressions = impression.Impressions;
                    productMetric.WeightedImpressions = impression.WeightedImpressions;
                    productMetric.IsBasicProduct = impression.IsBasicProduct;
                }
                if (view != null)
                {
                    productMetric.Views = view.Views;
                    productMetric.WeightedViews = view.WeightedViews;
                    productMetric.IsBasicProduct = view.IsBasicProduct;
                }
                if (list != null)
                {
                    productMetric.RetailerLists = list.RetailerLists;
                    productMetric.WeightedRetailerLists = list.WeightedRetailerLists;
                    productMetric.IsBasicProduct = list.IsBasicProduct;
                }
                cb.Add(productMetric);
            });
            productMetrics = cb.ToList();
            return productMetrics;
        }

        public List<ProductMetrics> GetProductMetricsWithoutImpressions(ProductsMetricsQuery productsMetricsQuery)
        {
            List<ProductMetrics> productMetrics = new List<ProductMetrics>();
            ConcurrentBag<ProductMetrics> cb = new ConcurrentBag<ProductMetrics>();
            var weedIds = productsMetricsQuery.ProductList.Select(x => x.WeedId).ToList();

            var listMetric = (from imp in _analyticsContext.List
                              where imp.MeasuredDate.Date >= productsMetricsQuery.StartDate.Date && imp.MeasuredDate.Date <= productsMetricsQuery.EndDate.Date
                              && weedIds.Contains(imp.WeedId) && !imp.IsBasicProduct
                              group imp by imp.WeedId into g
                              select new
                              {
                                  g.Key,
                                  RetailerLists = g.Sum(x => x.MetricCount),
                                  WeightedRetailerLists = g.Sum(x => x.WeightedCount)
                              }).AsNoTracking().ToList();

            var viewMetric = (from v in _analyticsContext.View
                              where v.MeasuredDate.Date >= productsMetricsQuery.StartDate.Date && v.MeasuredDate.Date <= productsMetricsQuery.EndDate.Date
                              && weedIds.Contains(v.WeedId) && !v.IsBasicProduct
                              group v by v.WeedId into g
                              select new
                              {
                                  g.Key,
                                  Views = g.Sum(x => x.MetricCount),
                                  WeightedViews = g.Sum(x => x.WeightedCount)
                              }).AsNoTracking().ToList();

            Parallel.ForEach(productsMetricsQuery.ProductList, (weed) =>
            {
                var view = viewMetric.Where(x => x.Key == weed.WeedId).FirstOrDefault();
                var list = listMetric.Where(x => x.Key == weed.WeedId).FirstOrDefault();
                var productMetric = new ProductMetrics() { WeedId = weed.WeedId, Name = weed.Name };

                if (view != null)
                {
                    productMetric.Views = view.Views;
                    productMetric.WeightedViews = view.WeightedViews;
                }
                if (list != null)
                {
                    productMetric.RetailerLists = list.RetailerLists;
                    productMetric.WeightedRetailerLists = list.WeightedRetailerLists;
                }
                cb.Add(productMetric);
            });
            productMetrics = cb.ToList();
            return productMetrics;
        }

        public List<ProductMetric> GetProductsImpressions(ProductsMetricsQuery productsMetricsQuery)
        {
            List<ProductMetric> productMetrics = new List<ProductMetric>();
            ConcurrentBag<ProductMetric> cb = new ConcurrentBag<ProductMetric>();
            var weedIds = productsMetricsQuery.ProductList.Select(x => x.WeedId).ToList();

            var impressionMetric = (from imp in _analyticsContext.Impression
                                    where imp.MeasuredDate.Date >= productsMetricsQuery.StartDate.Date && imp.MeasuredDate.Date <= productsMetricsQuery.EndDate.Date
                                    && weedIds.Contains(imp.WeedId) && !imp.IsBasicProduct
                                    select new ProductMetric()
                                    {
                                        WeedId = imp.WeedId,
                                        MetricCount = imp.MetricCount,
                                        WeightedMetric = imp.WeightedCount,
                                        MeasuredDate = imp.MeasuredDate,
                                        RetailerId = imp.ProducerConnectUserId
                                    }).AsNoTracking().ToList();

            Parallel.ForEach(productsMetricsQuery.ProductList, (weed) =>
            {
                var impressions = impressionMetric.Where(x => x.WeedId == weed.WeedId).ToList();
                if (impressions != null && impressions.Count > 0)
                {
                    foreach (var impression in impressions)
                    {
                        var productMetric = new ProductMetric() { WeedId = weed.WeedId, ProductName = weed.Name };
                        productMetric.MeasuredDate = impression.MeasuredDate;
                        productMetric.MetricCount = impression.MetricCount;
                        productMetric.WeightedMetric = impression.WeightedMetric;

                        var retailer = productsMetricsQuery.RetailersList.Where(x => x.RetailerId == impression.RetailerId).FirstOrDefault();
                        productMetric.RetailerDisplayName = retailer != null ? retailer.DisplayName : "";
                        cb.Add(productMetric);
                    }
                }
            });
            productMetrics = cb.ToList();
            return productMetrics;
        }

        public List<ProductMetric> GetProductsLists(ProductsMetricsQuery productsMetricsQuery)
        {
            List<ProductMetric> productMetrics = new List<ProductMetric>();
            ConcurrentBag<ProductMetric> cb = new ConcurrentBag<ProductMetric>();
            var weedIds = productsMetricsQuery.ProductList.Select(x => x.WeedId).ToList();

            var listMetrics = (from l in _analyticsContext.List
                                    where l.MeasuredDate.Date >= productsMetricsQuery.StartDate.Date && l.MeasuredDate.Date <= productsMetricsQuery.EndDate.Date
                                    && weedIds.Contains(l.WeedId) && !l.IsBasicProduct
                                    select new ProductMetric()
                                    {
                                        WeedId = l.WeedId,
                                        MetricCount = l.MetricCount,
                                        WeightedMetric = l.WeightedCount,
                                        MeasuredDate = l.MeasuredDate,
                                        RetailerId = l.ProducerConnectUserId,
                                        IsBasicProduct = l.IsBasicProduct
                                    }).AsNoTracking().ToList();

            Parallel.ForEach(productsMetricsQuery.ProductList, (weed) =>
            {
                var lists = listMetrics.Where(x => x.WeedId == weed.WeedId).ToList();
                if (lists != null && lists.Count>0)
                {
                    foreach (var list in lists)
                    {
                        var productMetric = new ProductMetric() { WeedId = weed.WeedId, ProductName = weed.Name };
                        productMetric.MeasuredDate = list.MeasuredDate;
                        productMetric.MetricCount = list.MetricCount;
                        productMetric.WeightedMetric = list.WeightedMetric;

                        var retailer = productsMetricsQuery.RetailersList.Where(x => x.RetailerId == list.RetailerId).FirstOrDefault();
                        productMetric.RetailerDisplayName = retailer != null ? retailer.DisplayName : "";
                        cb.Add(productMetric);
                    }
                }
            });
            productMetrics = cb.ToList();
            return productMetrics;
        }

        public List<ProductMetric> GetProductsViews(ProductsMetricsQuery productsMetricsQuery)
        {
            List<ProductMetric> productMetrics = new List<ProductMetric>();
            ConcurrentBag<ProductMetric> cb = new ConcurrentBag<ProductMetric>();
            var weedIds = productsMetricsQuery.ProductList.Select(x => x.WeedId).ToList();

            var viewMetrics = (from v in _analyticsContext.View
                               where v.MeasuredDate.Date >= productsMetricsQuery.StartDate.Date && v.MeasuredDate.Date <= productsMetricsQuery.EndDate.Date
                               && weedIds.Contains(v.WeedId) && !v.IsBasicProduct
                               select new ProductMetric()
                               {
                                   WeedId = v.WeedId,
                                   MetricCount = v.MetricCount,
                                   WeightedMetric = v.WeightedCount,
                                   MeasuredDate = v.MeasuredDate,
                                   RetailerId = v.ProducerConnectUserId
                               }).AsNoTracking().ToList();

            Parallel.ForEach(productsMetricsQuery.ProductList, (weed) =>
            {
                var views = viewMetrics.Where(x => x.WeedId == weed.WeedId).ToList();
                
                if (views != null && views.Count>0)
                {
                    foreach(var view in views)
                    {
                        var productMetric = new ProductMetric() { WeedId = weed.WeedId, ProductName = weed.Name };
                        productMetric.MeasuredDate = view.MeasuredDate;
                        productMetric.MetricCount = view.MetricCount;
                        productMetric.WeightedMetric = view.WeightedMetric;
                        var retailer = productsMetricsQuery.RetailersList.Where(x => x.RetailerId == view.RetailerId).FirstOrDefault();
                        productMetric.RetailerDisplayName = retailer != null ? retailer.DisplayName : "";
                        cb.Add(productMetric);
                    }
                }
            });
            productMetrics = cb.ToList();
            return productMetrics;
        }
    }
}
