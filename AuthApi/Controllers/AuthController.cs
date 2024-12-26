using AuthApi.Db;
using AuthApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration config, ILogger<AuthController> logger)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _logger = logger;
        }

        //[HttpPost("signup")]
        //public async Task<ActionResult<ApiResponse>> Signup(SignupModel model)
        //{

        //}
    }
}
