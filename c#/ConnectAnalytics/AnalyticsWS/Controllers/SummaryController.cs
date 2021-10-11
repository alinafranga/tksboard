using AnalyticsDAL.Helpers;
using AnalyticsDAL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AnalyticsWS.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class SummaryController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SummaryController> _logger;
        private readonly ISummaryRepository _summary;

        public SummaryController(ISummaryRepository summary, ILogger<SummaryController> logger, IConfiguration configuration)
        {
            _summary = summary;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("saveDay/{day}/{month}/{year}")]
        public async Task<string> SaveDay(int day, int month, int year)
        {
            try
            {
                var data = await GetProducersFromMain();
                if (data != null)
                {
                    var list = JsonConvert.DeserializeObject<List<UserData>>(data);
                    var producerIds = new List<int>();
                    list.ForEach(d => producerIds.Add(d.Id));
                    return JsonConvert.SerializeObject(_summary.SaveDay(producerIds, day, month, year));
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError("SummaryController SaveDay" + ex.Message, ex);
                return "SummaryController SaveDay" + ex.Message;
            }
        }

        private async Task<string> GetProducersFromMain()
        {
            var mainUrl = _configuration.GetSection("MainAppUrl").Value;
            var url = mainUrl + "/api/user/searchProducers";

            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage responseMessage = null;
                try
                {
                    responseMessage = await httpClient.GetAsync(url);
                    return responseMessage.Content.ReadAsStringAsync().Result;
                }
                catch (Exception ex)
                {
                    if (responseMessage == null)
                    {
                        responseMessage = new HttpResponseMessage();
                    }
                    responseMessage.StatusCode = HttpStatusCode.InternalServerError;
                    responseMessage.ReasonPhrase = string.Format("RestHttpClient.GetData failed: {0}", ex);
                }
                return null;
            }
        }

        [HttpGet]
        [Route("saveDayWithProducer/{producerId}/{day}/{month}/{year}")]
        public string SaveDayWithProducer(int producerId, int day, int month, int year)
        {
            try
            {
                return JsonConvert.SerializeObject(_summary.SaveDay(new List<int> { producerId }, day, month, year));
            }
            catch (Exception ex)
            {
                _logger.LogError("SummaryController SaveDayWithProducer" + ex.Message, ex);
                return ex.Message + " - " + ex.InnerException;
            }
        }

        [HttpGet]
        [Route("saveMonth/{month}/{year}")]
        public async Task<string> SaveMonth(int month, int year)
        {
            try
            {
                var data = await GetProducersFromMain();
                if (data != null)
                {
                    var list = JsonConvert.DeserializeObject<List<UserData>>(data);
                    var producerIds = new List<int>();
                    list.ForEach(d => producerIds.Add(d.Id));
                    return JsonConvert.SerializeObject(_summary.SaveMonth(producerIds, month, year));
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError("SummaryController SaveMonth" + ex.Message, ex);
                return "SummaryController SaveMonth" + ex.Message;
            }
        }

        [HttpGet]
        [Route("saveMonthWithProducer/{producerId}/{month}/{year}")]
        public string SaveMonthWithProducer(int producerId, int month, int year)
        {
            try
            {
                return JsonConvert.SerializeObject(_summary.SaveMonth(new List<int> { producerId }, month, year));
            }
            catch (Exception ex)
            {
                _logger.LogError("SummaryController SaveMonthWithProducer" + ex.Message, ex);
                return ex.Message + " - " + ex.InnerException;
            }
        }

        [HttpGet]
        [Route("saveYear/{year}")]
        public async Task<string> SaveYear(int year)
        {
            try
            {
                var data = await GetProducersFromMain();
                if (data != null)
                {
                    var list = JsonConvert.DeserializeObject<List<UserData>>(data);
                    var producerIds = new List<int>();
                    list.ForEach(d => producerIds.Add(d.Id));
                    return JsonConvert.SerializeObject(_summary.SaveYear(producerIds, year));
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError("SummaryController SaveYear" + ex.Message, ex);
                return "SummaryController SaveYear" + ex.Message;
            }
        }

        [HttpGet]
        [Route("saveYearWithProducer/{producerId}/{year}")]
        public string SaveYearWithProducer(int producerId, int year)
        {
            try
            {
                return JsonConvert.SerializeObject(_summary.SaveYear(new List<int> { producerId }, year));
            }
            catch (Exception ex)
            {
                _logger.LogError("SummaryController SaveYearWithProducer" + ex.Message, ex);
                return ex.Message + " - " + ex.InnerException;
            }
        }
    }
}
