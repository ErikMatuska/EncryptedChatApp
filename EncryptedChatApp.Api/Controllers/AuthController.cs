using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using EncryptedChatApp.Api.Data;
using EncryptedChatApp.Api.Models;

namespace EncryptedChatApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ChatUser> userManager;
        private readonly IConfiguration configuration;

        public AuthController(UserManager<ChatUser> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Missing fields");

            if (model == null)
                throw new NullReferenceException("Reigster Model is null");

            if (model.Password != model.ConfirmPassword)
            {
                return BadRequest("Passwords dont match!");
            }

            var userResult = await userManager.FindByEmailAsync(model.Email);

            if (userResult != null)
            {
                return BadRequest("Email already used!");
            }

            var identityUser = new ChatUser
            {
                Email = model.Email,
                UserName = model.UserName,
                Key = model.PublicKey
            };

            var result = await userManager.CreateAsync(identityUser, model.Password);

            if (result.Succeeded)
            {
                return Ok("Success");
            }

            return BadRequest("User was not created!");
        }

        [HttpPost, Route("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.UserName);

                if (user == null)
                {
                    return BadRequest("There is no user with that Email address");
                }

                var result = await userManager.CheckPasswordAsync(user, model.Password);

                if (!result)
                {
                    return BadRequest("Invalid password");
                }

                var claims = new[]
                {
                    new Claim("Id", user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AuthSettings:Key"]));

                var token = new JwtSecurityToken(
                    issuer: configuration["AuthSettings:Issuer"],
                    audience: configuration["AuthSettings:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddDays(14),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

                string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

                Console.WriteLine(tokenAsString);

                return Ok(new AuthDetails
                {
                    Token = tokenAsString,
                    Message = "OK",
                    IsSuccess = true,
                    ExpireDate = token.ValidTo,
                    UserId = user.Id
                });
            }

            return BadRequest("Some properties are not valid");
        }

    }

    public static class UserExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var value = user.Claims.FirstOrDefault(c => c.Type == "Id");

            if (value == null)
            {
                return -1;
            }

            return int.Parse(value.Value);
        }

        public static string GetSenderUserName(this ClaimsPrincipal user)
        {
            return user.Identity.Name;
        }
    }
}
