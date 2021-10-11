using AnalyticsDAL.Repositories;
using AnalyticsDomain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace AnalyticsWS.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class ImpresionController : Controller
    {
        private readonly IImpresionsRepository _impresionsRepository;
        private readonly ILogger<ImpresionController> _logger;
        public ImpresionController(IImpresionsRepository impresionsRepository, ILogger<ImpresionController> logger)
        {
            _impresionsRepository = impresionsRepository;
            _logger = logger;
        }

        [HttpPost]
        [Route("saveImpresions")]
        public async Task<IActionResult> SaveImpresion([FromBody]ImpressionExt impresions)
        {
            try
            {
                await _impresionsRepository.SaveImpresionsAsync(impresions);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError("ImpresionController SaveImpresions " + ex.Message, ex);
                return StatusCode(500, JsonConvert.SerializeObject("ImpresionController SaveImpresions " + ex.Message));
            }
        }
    }
}
