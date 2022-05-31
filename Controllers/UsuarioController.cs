using DisneyAPI.IdentityAuth;
using DisneyAPI.Models;
using DisneyAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DisneyAPI.Controllers
{
    [AllowAnonymous]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private IMailService _mailService;

        public AuthenticateController(UserManager<ApplicationUser> userManager, IConfiguration configuration, IMailService mailService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _mailService = mailService;
        }

        [HttpPost]
        [Route("auth/register")]
        public async Task<ActionResult> Register([FromBody] Register register)
        {
            var userExist = await _userManager.FindByNameAsync(register.Username);
            if (userExist != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseAuth { Status = "Error", Message = "El usuario ya existe" });
            ApplicationUser user = new()
            {
                Email = register.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = register.Username,
            };

            var result = await _userManager.CreateAsync(user, register.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseAuth { Status = "Error", Message = "Error en la creación del nuevo usuario. Revisa tus datos." });

            // Enviar email:
            await _mailService.SendEmailAsync(register.Email, "Bienvenido usuario nuevo", "<h1>Bienvenido</h1><p>Gracias por registrarte</p>");

            return Ok(new ResponseAuth { Status = "Success", Message = "El usuario se ha creado con exito!" });
        }

        [AllowAnonymous]
        [HttpPost("auth/login")]
        public async Task<ActionResult> Login([FromBody] Login login)
        {
            var user = await _userManager.FindByNameAsync(login.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, login.Password))
            {
                var authClaims = new List<Claim>
            { new Claim(ClaimTypes.Name, user.UserName),
              new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())};
                var authorizationSigninKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(4),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authorizationSigninKey, SecurityAlgorithms.HmacSha256)
                    );
                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token), expiration = token.ValidTo });
            }
            return Unauthorized();
        }
    }
}