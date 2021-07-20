using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SinoDbAPI.Models;
using SinoDbAPI.Payloads;
using SinoDbAPI.Services;
using SinoDbAPI.Utils;
using System.Collections.Generic;

namespace SinoDbAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ColosseumResultController : ControllerBase
    {
        private readonly ILogger<ColosseumResultController> _logger;
        private readonly IColosseumResultService _resultService;

        public ColosseumResultController(ILogger<ColosseumResultController> logger, IColosseumResultService resultService)
        {
            _logger = logger;
            _resultService = resultService;
        }

        [HttpGet]
        public ActionResult<List<ColosseumResult>> Get()
        {
            return _resultService.Get();
        }

        [HttpGet("{id:length(24)}", Name = "GetById")]
        public ActionResult<ColosseumResult> GetById(string id)
        {
            var result = _resultService.GetById(id);
            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpGet("search", Name = "GetByGuildName")]
        public ActionResult<ColosseumResult> GetByGuildName(string guildName)
        {
            var result = _resultService.GetByGuildName(guildName);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public ActionResult<ColosseumResult> Create(ColosseumResultRequest result)
        {
            var newResult = _resultService.Create(PayloadsConverter.ToColosseumResult(result));
            return CreatedAtRoute("GetById", new { id = newResult.Id.ToString() }, newResult);
        }

        [Authorize]
        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, ColosseumResultRequest updatedResult)
        {
            var result = _resultService.GetById(id);
            if (result == null)
            {
                return NotFound();
            }
 
            _resultService.Update(id, PayloadsConverter.ToColosseumResult(updatedResult));
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var result = _resultService.GetById(id);
            if (result == null)
            {
                return NotFound();
            }

            _resultService.Remove(id);
            return NoContent();
        }
    }
}
