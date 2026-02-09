using dotnet_core_authentication.Data;
using dotnet_core_authentication.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
            SignInManager<ApplicationUsers> signInManager,
            IConfiguration configuration)
        {
            _UserManger = userManager;
            _SignInManager = signInManager;
            _jwtKey = configuration["JWTSettings:SecurityKey"];
            _JwtIssuer = configuration["JWTSettings:Issuer"];
            _JwtAudience = configuration["Jwt:Audience"];
            _JwtExpiry = Convert.ToInt32(configuration["JWTSettings:ExpiryInMinutes"]);

        }
        // basseurl/api/UserAuth/Register

        [HttpPost]
        [Route("Register")]

        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
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
                return Conflict("User with this email already exists");
            }

            var newUser = new ApplicationUsers
            {
                UserName = registerModel.Email,
                Email = registerModel.Email,
                Name = registerModel.Name,

            };
            var results = await _UserManger.CreateAsync(newUser, registerModel.Password);
            if (!results.Succeeded)
            {
                return BadRequest(results.Errors);
            }

            return Ok("User Created Successfully");
        }
        [HttpPost("Login")]

        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var checkemail = await _UserManger.FindByEmailAsync(loginModel.Email);

            if (checkemail == null)
            {
                return Unauthorized(new { succes = false, message = "Invalid email and password" });
            }

            var checkpassword = await _SignInManager.CheckPasswordSignInAsync(checkemail, loginModel.Password, false);

            if (!checkpassword.Succeeded)
            {
                return Unauthorized(new { succes = false, message = "Invalid email and password" });
            }

            var tokken = JWTtokkenGenearte(checkemail);

            return Ok(new { succes = true, tokken });


        }

        private string JWTtokkenGenearte(ApplicationUsers users)
        {
            var Claims = new[]
            {
                   new  Claim(JwtRegisteredClaimNames.Sub, users.Id),
                     new  Claim(JwtRegisteredClaimNames.Email, users.Email),
                     new  Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

                     new Claim("name", users.Name),
             };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));

            var Credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokken = new JwtSecurityToken
            (
               claims: Claims,
               expires: DateTime.UtcNow.AddMinutes(_JwtExpiry),
               signingCredentials: Credentials

            );

            return new JwtSecurityTokenHandler().WriteToken(tokken);

        }

        [HttpPost("Logout")]

        public async Task<IActionResult> Logout()
        {
            await _SignInManager.SignOutAsync();
            return Ok("User logout Successfully");
        }

    }

    }




