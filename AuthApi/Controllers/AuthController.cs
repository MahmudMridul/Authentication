using AuthApi.Db;
using AuthApi.Models;
using AuthApi.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;

        public AuthController(AuthContext context, UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }

        [HttpPost("signup")]
        public async Task<ActionResult<ApiResponse>> Signup([FromBody] SignupDto model)
        {
            ApiResponse res;
            try
            {
                if (!ModelState.IsValid)
                {
                    List<string> errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    res = ApiResponse.Create(HttpStatusCode.BadRequest, msg: "Validation failed", errors: errors);
                    return BadRequest(res);
                }

                var existing = await _userManager.FindByNameAsync(model.Username);
                if (existing != null)
                {
                    res = ApiResponse.Create(HttpStatusCode.Conflict, msg: "User already exists");
                    return Conflict(res);
                }

                User user = new User
                {
                    UserName = model.Username,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow,
                };

                IdentityResult result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    List<string> errors = result.Errors.Select(e => e.Description).ToList();
                    res = ApiResponse.Create(HttpStatusCode.BadRequest, msg: "Signup failed", errors: errors);
                    return BadRequest(res);
                }

                await _userManager.AddToRoleAsync(user, "User");
                res = ApiResponse.Create(HttpStatusCode.OK, model, true, "Signup successfull", null);
                return Ok(res);
            }
            catch (Exception e)
            {
                List<string> errs = new List<string> { e.Message, e.StackTrace };
                res = ApiResponse.Create(HttpStatusCode.InternalServerError, msg: "An unexpected error occurred", errors: errs);
                return StatusCode((int)HttpStatusCode.InternalServerError, res);
            }
        }
    }
}
