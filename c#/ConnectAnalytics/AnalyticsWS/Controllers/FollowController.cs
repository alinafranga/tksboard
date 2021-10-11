using AnalyticsDAL.Models;
using AnalyticsDAL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AnalyticsWS.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class FollowController : Controller
    {
        private readonly IFollowRepository _followRepository;
        private readonly ILogger<FollowController> _logger;
        public FollowController(IFollowRepository followRepository, ILogger<FollowController> logger)
        {
            _followRepository = followRepository;
            _logger = logger;
        }
        [HttpPost]
        [Route("saveFollow")]
        public async Task<IActionResult> SaveFollow([FromBody] Follow follow)
        {
            try
            {
                await _followRepository.SaveFollowAsync(follow);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError("FollowController SaveFollow " + ex.Message, ex);
                return StatusCode(500, JsonConvert.SerializeObject("FollowController SaveFollow " + ex.Message));
            }
        }

        [HttpPost]
        [Route("saveFollows")]
        public IActionResult SaveFollows([FromBody] List<Follow> follows)
        {
            try
            {
                _followRepository.SaveFollows(follows);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FollowController SaveFollows " + ex.Message);
                return StatusCode(500, JsonConvert.SerializeObject("FollowController SaveFollows " + ex.Message));
            }
        }
    }
}
