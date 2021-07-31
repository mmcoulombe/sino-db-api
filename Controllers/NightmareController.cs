using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using SinoDbAPI.Models.Nightmares;
using SinoDbAPI.Services;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace SinoDbAPI.Controllers
{
    [ApiController]
    [Route("api/v1/nightmares")]
    public class NightmareController : ControllerBase
    {
        private readonly ILogger<NightmareController> _logger;
        private readonly INightmareService _resultService;

        public NightmareController(ILogger<NightmareController> logger, INightmareService resultService)
        {
            _logger = logger;
            _resultService = resultService;
        }

        [HttpGet]
        public Task<List<NightmareInfo>> Get()
        {
            return _resultService.Get();
        }

        [HttpGet("{id:length(24)}", Name = "GetNightmareById")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _resultService.GetById(id);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("search", Name = "Find")]
        public async Task<IActionResult> GetByGuildName(string name)
        {
            var result = await _resultService.Find(name);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(NightmareInfo result)
        {
            var newResult = await _resultService.Create(result);
            return CreatedAtRoute("GetNightmareById", new { id = newResult }, newResult);
        }

        [Authorize]
        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, NightmareInfo updatedResult)
        {
            var result = await _resultService.GetById(id);
            if (result == null)
            {
                return NotFound();
            }

            await _resultService.Update(id, updatedResult);
            return Ok();
        }

        [Authorize]
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _resultService.GetById(id);
            if (result == null)
            {
                return NotFound();
            }

            _resultService.Remove(id);
            return Ok();
        }

    }
}
