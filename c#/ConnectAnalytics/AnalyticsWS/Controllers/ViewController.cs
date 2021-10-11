using AnalyticsDAL.Models;
using AnalyticsDAL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnalyticsWS.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class ViewController : Controller
    {
        private readonly IViewRepository _viewRepository;
        private readonly ILogger<ViewController> _logger;
        public ViewController(IViewRepository viewRepository, ILogger<ViewController> logger)
        {
            _viewRepository = viewRepository;
            _logger = logger;
        }

        [HttpPost]
        [Route("saveView")]
        public async Task<IActionResult> SaveView([FromBody]View view)
        {
            try
            {
                await _viewRepository.SaveViewAsync(view);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError("ViewController SaveView " + ex.Message, ex);
                return StatusCode(500, JsonConvert.SerializeObject("ViewController SaveView " + ex.Message));
            }
        }
    }
}
