using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SinoDbAPI.Models;
using SinoDbAPI.Payloads;
using SinoDbAPI.Services;
using SinoDbAPI.Utils;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SinoDbAPI.Controllers
{
    [ApiController]
    [Route("api/v1/colosseumResult")]
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
        public async Task<IActionResult> Get(int? limit)
        {
            var result = await _resultService.Get(limit);
            return Ok(result);
        }

        [HttpGet("{id:length(24)}", Name = "GetResultById")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _resultService.GetById(id);
            if (result == null)
            {
                return BadRequest(new { message = "Invalid username or password" });
            }

            return Ok(result);
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
        public ActionResult<ColosseumResult> Create([FromHeader(Name= "Authorization")][Required] string requiredHeader, ColosseumResultRequest result)
        {
            var newResult = _resultService.Create(PayloadsConverter.ToColosseumResult(result));
            return CreatedAtRoute("GetResultById", new { id = newResult.Id.ToString() }, newResult);
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
