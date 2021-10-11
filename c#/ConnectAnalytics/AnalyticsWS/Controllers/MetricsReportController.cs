using AnalyticsDAL.Helpers.Enums;
using AnalyticsDAL.Repositories;
using AnalyticsDomain.Models;
using AnalyticsDomain.Models.Graphics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace AnalyticsWS.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class MetricsReportController : Controller
    {
        private readonly IMetricsReportRepository _metricsReportRepository;
        private readonly ILogger<MetricsReportController> _logger;
        private readonly IMemoryCache _cache;
        public MetricsReportController(IMetricsReportRepository metricsReportRepository, ILogger<MetricsReportController> logger, IMemoryCache cache)
        {
            _metricsReportRepository = metricsReportRepository;
            _logger = logger;
            _cache = cache;
        }

        [HttpPost]
        [Route("viewsReport")]
        public IActionResult ViewsReport([FromBody] ProductsMetricsQuery productsMetricsQuery)
        {
            try
            {
                var result = _metricsReportRepository.GetProductsViewsReport(productsMetricsQuery);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("MetricsReportController ViewsReport " + ex.Message, ex);
                return StatusCode(500, JsonConvert.SerializeObject("MetricsReportController ViewsReport " + ex.Message));
            }
        }

        [HttpGet]
        [Route("getOverviewProducerAnalytics/{producerId}/{provinceId}/{month}/{year}")]
        public string GetOverviewProducerAnalytics(int producerId, int provinceId, int month, int year)
        {
            try
            {
                OverviewMetricsInfo sim = null;
                if (!_cache.TryGetValue(string.Format("getOverviewProducerAnalytics/{0}/{1}/{2}/{3}", producerId, provinceId, month, year), out sim))
                {
                    MemoryCacheEntryOptions options = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24),
                        SlidingExpiration = TimeSpan.FromHours(6)
                    };
                    sim = _metricsReportRepository.GetOverviewProducerAnalytics(producerId, provinceId, month, year);

                    if (sim != null)
                        _cache.Set(string.Format("getOverviewProducerAnalytics/{0}/{1}/{2}/{3}", producerId, provinceId, month, year), sim, options);
                }

                return JsonConvert.SerializeObject(sim);
            }
            catch (Exception ex)
            {
                _logger.LogError("MetricsReportController GetOverviewProducerAnalytics " + ex.Message, ex);
                return ex.Message + " - " + ex.InnerException;
            }
        }

        [HttpGet]
        [Route("getOverviewByProductAnalytics/{weedId}/{provinceId}/{month}/{year}")]
        public string GetOverviewByProductAnalytics(int weedId, int provinceId, int month, int year)
        {
            try
            {
                OverviewMetricsInfo sim = null;
                if (!_cache.TryGetValue(string.Format("getOverviewByProductAnalytics/{0}/{1}/{2}/{3}", weedId, provinceId, month, year), out sim))
                {
                    MemoryCacheEntryOptions options = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24),
                        SlidingExpiration = TimeSpan.FromHours(6)
                    };
                    sim = _metricsReportRepository.GetOverviewByProductAnalytics(weedId, provinceId, month, year);

                    if (sim != null)
                        _cache.Set(string.Format("getOverviewByProductAnalytics/{0}/{1}/{2}/{3}", weedId, provinceId, month, year), sim, options);
                }

                return JsonConvert.SerializeObject(sim);
            }
            catch (Exception ex)
            {
                _logger.LogError("MetricsReportController GetOverviewByProductAnalytics " + ex.Message, ex);
                return ex.Message + " - " + ex.InnerException;
            }
        }

        [HttpGet]
        [Route("getProducerImpressions/{producerId}/{provinceId}/{month}/{year}")]
        public string GetProducerImpressions(int producerId, int provinceId, int month, int year)
        {
            try
            {
                var result = _metricsReportRepository.GetProducerMetrics(producerId, provinceId, month, year, MetricTypeEnum.Impressions);
                return JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("MetricsReportController getProducerImpressions " + ex.Message, ex);
                return ex.Message + " - " + ex.InnerException;
            }
        }

        [HttpGet]
        [Route("getProductImpressions/{weedId}/{provinceId}/{month}/{year}")]
        public string GetProductImpressions(int weedId, int provinceId, int month, int year)
        {
            try
            {
                var result = _metricsReportRepository.GetProductMetrics(weedId, provinceId, month, year, MetricTypeEnum.Impressions);
                return JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("MetricsReportController getProducerImpressions " + ex.Message, ex);
                return ex.Message + " - " + ex.InnerException;
            }
        }

        [HttpGet]
        [Route("getProducerViews/{producerId}/{provinceId}/{month}/{year}")]
        public string GetProducerViews(int producerId, int provinceId, int month, int year)
        {
            try
            {
                var result = _metricsReportRepository.GetProducerMetrics(producerId, provinceId, month, year, MetricTypeEnum.Views);
                return JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("MetricsReportController GetProducerViews " + ex.Message, ex);
                return ex.Message + " - " + ex.InnerException;
            }
        }

        [HttpGet]
        [Route("getProductViews/{weedId}/{provinceId}/{month}/{year}")]
        public string GetProductViews(int weedId, int provinceId, int month, int year)
        {
            try
            {
                var result = _metricsReportRepository.GetProductMetrics(weedId, provinceId, month, year, MetricTypeEnum.Views);
                return JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("MetricsReportController GetProducerViews " + ex.Message, ex);
                return ex.Message + " - " + ex.InnerException;
            }
        }

        [HttpGet]
        [Route("getProducerLists/{producerId}/{provinceId}/{month}/{year}")]
        public string GetProducerLists(int producerId, int provinceId, int month, int year)
        {
            try
            {
                var result = _metricsReportRepository.GetProducerMetrics(producerId, provinceId, month, year, MetricTypeEnum.Lists);
                return JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("MetricsReportController GetProducerLists " + ex.Message, ex);
                return ex.Message + " - " + ex.InnerException;
            }
        }

        [HttpGet]
        [Route("getProductLists/{weedId}/{provinceId}/{month}/{year}")]
        public string GetProductLists(int weedId, int provinceId, int month, int year)
        {
            try
            {
                var result = _metricsReportRepository.GetProductMetrics(weedId, provinceId, month, year, MetricTypeEnum.Lists);
                return JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MetricsReportController GetProductLists " + ex.Message);
                return ex.Message + " - " + ex.InnerException;
            }
        }

        [HttpGet]
        [Route("getProducerFollows/{producerId}/{provinceId}/{month}/{year}")]
        public string GetProducerFollows(int producerId, int provinceId, int month, int year)
        {
            try
            {
                var result = _metricsReportRepository.GetProducerMetrics(producerId, provinceId, month, year, MetricTypeEnum.Follows);
                return JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("MetricsReportController GetProducerLists " + ex.Message, ex);
                return ex.Message + " - " + ex.InnerException;
            }
        }

        [HttpGet]
        [Route("getProductFollows/{weedId}/{provinceId}/{month}/{year}")]
        public string GetProductFollows(int weedId, int provinceId, int month, int year)
        {
            try
            {
                var result = _metricsReportRepository.GetProductMetrics(weedId, provinceId, month, year, MetricTypeEnum.Follows);
                return JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("MetricsReportController GetProductFollows " + ex.Message, ex);
                return ex.Message + " - " + ex.InnerException;
            }
        }

        [HttpGet]
        [Route("getAvailableMonths/{producerId}/{provinceId}")]
        public string GetAvailableMonths(int producerId, int provinceId)
        {
            try
            {
                List<GraphicData> sim = new List<GraphicData>();
                if (!_cache.TryGetValue(string.Format("getAvailableMonths/{0}/{1}", producerId, provinceId), out sim))
                {
                    MemoryCacheEntryOptions options = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24),
                        SlidingExpiration = TimeSpan.FromHours(24)
                    };
                    sim = _metricsReportRepository.GetAvailableMonths(producerId, provinceId);

                    if (sim != null && sim.Count > 0)
                        _cache.Set(string.Format("getAvailableMonths/{0}/{1}", producerId, provinceId), sim, options);
                }

                return JsonConvert.SerializeObject(sim);
            }
            catch (Exception ex)
            {
                _logger.LogError("MetricsReportController GetAvailableMonths" + ex.Message, ex);
                return ex.Message + " - " + ex.InnerException;
            }
        }

        [HttpGet]
        [Route("getProductAvailableMonths/{productId}/{provinceId}")]
        public string GetProductAvailableMonths(int productId, int provinceId)
        {
            try
            {
                List<GraphicData> sim = new List<GraphicData>();
                if (!_cache.TryGetValue(string.Format("getProductAvailableMonths/{0}/{1}", productId, provinceId), out sim))
                {
                    MemoryCacheEntryOptions options = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24),
                        SlidingExpiration = TimeSpan.FromHours(24)
                    };
                    sim = _metricsReportRepository.GetProductAvailableMonths(productId, provinceId);

                    if (sim != null && sim.Count > 0)
                        _cache.Set(string.Format("getProductAvailableMonths/{0}/{1}", productId, provinceId), sim, options);
                }

                return JsonConvert.SerializeObject(sim);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MetricsReportController GetProductAvailableMonths" + ex.Message);
                return ex.Message + " - " + ex.InnerException;
            }
        }
    }
}
