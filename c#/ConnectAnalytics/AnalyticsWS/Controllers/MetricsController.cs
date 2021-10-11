using AnalyticsDAL.Repositories;
using AnalyticsDomain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AnalyticsWS.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class MetricsController : ControllerBase
    {
        private readonly IMetricsRepository _metricsRepository;
        private readonly ILogger<MetricsController> _logger;
        public MetricsController(IMetricsRepository metricsRepository, ILogger<MetricsController> logger)
        {
            _metricsRepository = metricsRepository;
            _logger = logger;
        }

        [HttpPost]
        [Route("productsMetrics")]
        public IActionResult ProductsMetrics([FromBody]ProductsMetricsQuery productsMetricsQuery)
        {
            try
            {
                return Ok(_metricsRepository.GetProductMetrics(productsMetricsQuery));
            }
            catch (Exception ex)
            {
                _logger.LogError("MetricsController ProductsMetrics " + ex.Message, ex);
                return StatusCode(500, JsonConvert.SerializeObject("MetricsController ProductsMetrics" + ex.Message));
            }
        }

        [HttpPost]
        [Route("productsMetricsWithoutImpressions")]
        public IActionResult ProductsMetricsWithoutImpressions([FromBody] ProductsMetricsQuery productsMetricsQuery)
        {
            try
            {
                return Ok(_metricsRepository.GetProductMetricsWithoutImpressions(productsMetricsQuery));
            }
            catch (Exception ex)
            {
                _logger.LogError("MetricsController ProductsMetricsWithoutImpressions " + ex.Message, ex);
                return StatusCode(500, JsonConvert.SerializeObject("MetricsController ProductsMetricsWithoutImpressions" + ex.Message));
            }
        }

        [HttpPost]
        [Route("featuredProductsMetrics")]
        public IActionResult FeatureProductsMetrics([FromBody] FeaturedWeedMetricQuery featuredWeedMetrics)
        {
            try
            {
                return Ok(_metricsRepository.GetFeaturedProductMetrics(featuredWeedMetrics));
            }
            catch (Exception ex)
            {
                _logger.LogError("MetricsController FeatureProductsMetrics " + ex.Message, ex);
                return StatusCode(500, JsonConvert.SerializeObject("MetricsController FeatureProductsMetrics" + ex.Message));
            }
        }

        [HttpPost]
        [Route("productsImpressions")]
        public IActionResult ProductsImpressions([FromBody] ProductsMetricsQuery productsMetricsQuery)
        {
            try
            {
                return Ok(_metricsRepository.GetProductsImpressions(productsMetricsQuery));
            }
            catch (Exception ex)
            {
                _logger.LogError("MetricsController ProductsImpressions " + ex.Message, ex);
                return StatusCode(500, JsonConvert.SerializeObject("MetricsController ProductsImpressions" + ex.Message));
            }
        }

        [HttpPost]
        [Route("productsViews")]
        public IActionResult ProductsViews([FromBody] ProductsMetricsQuery productsMetricsQuery)
        {
            try
            {
                return Ok(_metricsRepository.GetProductsViews(productsMetricsQuery));
            }
            catch (Exception ex)
            {
                _logger.LogError("MetricsController ProductsViews " + ex.Message, ex);
                return StatusCode(500, JsonConvert.SerializeObject("MetricsController ProductsViews" + ex.Message));
            }
        }

        [HttpPost]
        [Route("productsLists")]
        public IActionResult ProductsLists([FromBody] ProductsMetricsQuery productsMetricsQuery)
        {
            try
            {
                return Ok(_metricsRepository.GetProductsLists(productsMetricsQuery));
            }
            catch (Exception ex)
            {
                _logger.LogError("MetricsController ProductsLists " + ex.Message, ex);
                return StatusCode(500, JsonConvert.SerializeObject("MetricsController ProductsLists" + ex.Message));
            }
        }

        [HttpPost]
        [Route("getMostViewedProduct")]
        public IActionResult GetMostViewedProduct([FromBody] List<int> weedIds)
        {
            try
            {
                return Ok(_metricsRepository.GetMostViewedProduct(weedIds));
            }
            catch (Exception ex)
            {
                _logger.LogError("MetricsController getMostViewedProduct " + ex.Message, ex);
                return StatusCode(500, JsonConvert.SerializeObject("MetricsController getMostViewedProduct" + ex.Message));
            }
        }
    }
}
