using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement.Mvc;
using SinoDbAPI.Payloads;
using SinoDbAPI.Services;
using System.Threading.Tasks;

namespace SinoDbAPI.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger, IUsersService usersService)
        {
            _logger = logger;
            _usersService = usersService;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate(AuthenticationRequest request)
        {
            var response = await _usersService.Authenticate(request.Username, request.Password);
            if (response == null)
            {
                return BadRequest(new { message = "Invalid username or password" });
            }

            return Ok(response);
        }

        [FeatureGate(Settings.FeaturesList.RegisterAPI)]
        [HttpPost("register")]
        public async Task<IActionResult> Register(AuthenticationRequest request)
        {
            var response = await _usersService.Register(request.Username, request.Password);
            if (response == null)
            {
                return BadRequest();
            }
            return Ok();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _usersService.GetAll();
            return Ok(users);
        }
    }
}
