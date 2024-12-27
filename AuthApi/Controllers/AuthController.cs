using AuthApi.Db;
using AuthApi.Models;
using AuthApi.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using AuthApi.Helpers;

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

                var existing = await _userManager.FindByNameAsync(model.UserName);
                if (existing != null)
                {
                    res = ApiResponse.Create(HttpStatusCode.Conflict, msg: "User already exists");
                    return Conflict(res);
                }

                User user = new User
                {
                    UserName = model.UserName,
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

        [HttpPost("signin")]
        public async Task<ActionResult<ApiResponse>> Signin([FromBody] SigninDto model)
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

                User? user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null)
                {
                    res = ApiResponse.Create(HttpStatusCode.NotFound, msg: "No user found with this username");
                    return NotFound(res);
                }

                SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

                if (!result.Succeeded)
                {
                    res = ApiResponse.Create(HttpStatusCode.Unauthorized, msg: "Invalid username or password");
                    return Unauthorized(res);
                }

                string accessToken = await TokenHelper.GenerateJwtToken(user, _userManager, _config);
                (string refreshToken, DateTime expiration) = TokenHelper.GenerateRefreshToken();

                //user.RefreshToken = refreshToken;
                //user.RefreshTokenExpiryTime = expiration;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                var accessTokenOps = new CookieOptions
                {
                    HttpOnly = true,       // Prevents access via JavaScript
                    Secure = true,         // Ensures the cookie is only sent over HTTPS
                    SameSite = SameSiteMode.None,  // Prevents CSRF attacks
                    Expires = DateTime.UtcNow.AddHours(1)  // Set the expiration of the token (optional)
                };

                // Append the token to the response as a cookie
                Response.Cookies.Append("AccessToken", accessToken, accessTokenOps);

                var refreshTokenOps = new CookieOptions
                {
                    HttpOnly = true,       // Prevents access via JavaScript
                    Secure = true,         // Ensures the cookie is only sent over HTTPS
                    SameSite = SameSiteMode.None,  // Prevents CSRF attacks
                    Expires = DateTime.UtcNow.AddDays(7)  // Set the expiration of the token (optional)
                };

                // Append the token to the response as a cookie
                Response.Cookies.Append("RefreshToken", refreshToken, refreshTokenOps);

                res = ApiResponse.Create(
                    HttpStatusCode.OK,
                    new
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                    },
                    true,
                    "Sign in successfull"
                );
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
