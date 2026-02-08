using dotnet_core_authentication.Data;
using dotnet_core_authentication.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_core_authentication.Controllers
{
    // baseurl: https://localhost:44371/api/UserAuth
    [Route("api/[controller]")]
    [ApiController]
    public class UserAuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUsers> _UserManger;
        private readonly SignInManager<ApplicationUsers> _SignInManager;
        private readonly string? _jwtKey;
        private readonly string? _JwtIssuer;
        private readonly string? _JwtAudience;
        private readonly int _JwtExpiry;

        public UserAuthController(UserManager<ApplicationUsers> userManager,
            SignInManager<ApplicationUsers> signInManager ,
            IConfiguration configuration)
        {
            _UserManger = userManager;
            _SignInManager = signInManager;
            _jwtKey = configuration["jWT:key"];
            _JwtIssuer = configuration["Jwt:Issuer"];
            _JwtAudience = configuration["Jwt:Audience"];
            _JwtExpiry = Convert.ToInt32(  configuration["Jwt:ExpiryInMinutes"]);

        }
        // basseurl/api/UserAuth/Register

        [HttpPost]
           [Route("Register")]

           public async  Task<IActionResult> Register([FromBody] RegisterModel registerModel )
        {
            if (registerModel == null
                || String.IsNullOrEmpty(registerModel.Name)
                || String.IsNullOrEmpty(registerModel.Email)
                || String.IsNullOrEmpty(registerModel.Password))

            { 
                 return BadRequest("Invalid client request");

            }

            var existingUser = await _UserManger.FindByEmailAsync(registerModel.Email);

            if (existingUser != null)
            {
                return Conflict ("User with this email already exists");
            }

            var newUser = new ApplicationUsers
            {
                UserName = registerModel.Email,
                Email = registerModel.Email,
                Name = registerModel.Name,

            };
             var  results =   await   _UserManger.CreateAsync(newUser,registerModel.Password);
            if (!results.Succeeded)
            {
               return BadRequest(results.Errors);
            }

            return Ok("User Created Successfully");
        }
    }
}
