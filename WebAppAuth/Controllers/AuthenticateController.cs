using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebAppAuth.Models;

namespace WebAppAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private UserManager<IdentityUser> _userManager = null;
        private IConfiguration _configuration = null;

        public AuthenticateController(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            this._userManager = userManager;
            this._configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] AuthUserDto dtoUser)
        {
            IActionResult result = BadRequest();
            var user = await _userManager.FindByEmailAsync(dtoUser.Login);

            if (user != null)
            {
                var verif = await _userManager.CheckPasswordAsync(user, dtoUser.Password);
                if (verif)
                {
                    result = Ok(new AuthUserDto()
                    {
                        Login = user.Email,
                        Name = user.UserName,
                        Token = GenerateJwtToken(user)
                    });
                }
            }
            return result;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] AuthUserDto dtoUser)
        {
            IActionResult result = BadRequest();

            var user = new IdentityUser(dtoUser.Name);
            user.Email = dtoUser.Login;

            var success = await _userManager.CreateAsync(user, dtoUser.Password);

            if (success.Succeeded)
            {
                dtoUser.Token = GenerateJwtToken(user);
                result = Ok(dtoUser);
            }
            return result;
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            // définit le jeton jwt qui sera responsable de la création de nos jetons
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            // Récupère notre clé secrete des paramètres de l'application
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            // Configuration du token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                // Définit la durée de vie du jeton
                Expires = DateTime.UtcNow.AddHours(24),
                // nous ajoutons les informations d'algorithme de cryptage qui seront utilisées pour décrypter notre token
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }

    }
}
