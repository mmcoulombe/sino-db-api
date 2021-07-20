using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SinoDbAPI.Payloads;
using SinoDbAPI.Services;

namespace SinoDbAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
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
        public IActionResult Authenticate(AuthenticationRequest request)
        {
            var response = _usersService.Authenticate(request.Username, request.Password);
            if (response == null)
            {
                return BadRequest(new { message = "Invalid username or password" });
            }

            return Ok(response);
        }

        [HttpPost("register")]
        public IActionResult Register(AuthenticationRequest request)
        {
            var response = _usersService.Register(request.Username, request.Password);
            if (response == null)
            {
                return BadRequest();
            }
            return Ok();
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _usersService.GetAll();
            return Ok(users);
        }
    }
}
