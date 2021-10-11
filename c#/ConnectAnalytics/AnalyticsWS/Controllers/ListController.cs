using AnalyticsDAL.Models;
using AnalyticsDAL.Repositories;
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
    public class ListController: Controller
    {
        private readonly IListRepository _listRepository;
        private readonly ILogger<ListController> _logger;
        public ListController(IListRepository listRepository, ILogger<ListController> logger)
        {
            _listRepository = listRepository;
            _logger = logger;
        }

        [HttpPost]
        [Route("saveList")]
        public async Task<IActionResult> SaveList([FromBody] List list)
        {
            try
            {
                await _listRepository.SaveListAsync(list);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError("ListController SaveList " + ex.Message, ex);
                return StatusCode(500, JsonConvert.SerializeObject("ListController SaveList " + ex.Message));
            }
        }
    }
}
