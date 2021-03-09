using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using WymianaKsiazek.Api.Database;
using WymianaKsiazek.Api.Database.Entities;
using WymianaKsiazek.Api.Database.Extensions;
using WymianaKsiazek.Api.Models;

namespace WymianaKsiazek.Api.Controllers
{
    [ApiController]
    [Route("")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly Context _context;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<UserEntity> _signInManager;
        private readonly UserManager<UserEntity> _userManager;

        public UserController(
            ILogger<UserController> logger,
            Context context,
            IConfiguration configuration,
            SignInManager<UserEntity> signInManager,
            UserManager<UserEntity> userManager)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
            _signInManager = signInManager;
            _userManager = userManager;
        }


        [HttpPost("users/register")]
        public async Task<IActionResult> Register([FromBody] CreateUserInput model)
        {
            return await CreateUser(model);
        }

        [HttpPost("users/token")]
        [ProducesResponseType(typeof(TokenOutput), 200)]
        public async Task<IActionResult> Token([FromBody] CreateTokenInput model)
        {
            return await CreateToken(model.Email, model.Password);
        }

        [HttpPost("users/token/refresh")]
        [ProducesResponseType(typeof(TokenOutput), 200)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenInput model)
        {
            return await RefreshToken(model.Token);
        }

        [HttpPost("users/signout")]
        public async Task<IActionResult> SignOut([FromBody] SignOutInput model)
        {
            return await RemoveRefreshToken(model.Token);
        }

        // te wszystkie metody poniżej można przerzucić do zewnętrznej klasy np. IUserService

        private async Task<IActionResult> CreateUser(CreateUserInput model)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                var existingUser = await _userManager.FindByNameAsync(model.Email);
                if (existingUser != null)
                {
                    return BadRequest();
                }

                UserEntity user = new UserEntity();
                user.Email = model.Email;
                user.UserName = model.Email;

                var passwordHash = new PasswordHasher<UserEntity>();
                var hashed = passwordHash.HashPassword(user, model.Password);

                user.PasswordHash = hashed;
                user.NormalizedEmail = user.Email.ToUpper();
                user.NormalizedUserName = user.UserName.ToUpper();
                user.SecurityStamp = Guid.NewGuid().ToString();

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    return BadRequest();
                }

                transaction.Commit();
            }

            return Ok();
        }

        private async Task<IActionResult> CreateToken(string email, string password)
        {
            var user = await _userManager.FindByNameAsync(email);
            if (user == null)
            {
                return BadRequest();
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!signInResult.Succeeded)
            {
                return BadRequest();
            }

            var accessToken = CreateAccessToken(user);

            return Ok(new TokenOutput()
            {
                AccessToken = accessToken.Item1,
                Expires = accessToken.Item2,
                RefreshToken = await CreateRefreshToken(user.Id),
                Id = user.Id,
                Email = user.UserName
            });
        }

        private async Task<IActionResult> RefreshToken(string token)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                var refreshToken = await _context.RefreshTokens
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.Token == token);

                if (refreshToken == null)
                {
                    return BadRequest();
                }

                if (!refreshToken.IsActive(DateTime.UtcNow))
                {
                    return Unauthorized();
                }

                var user = refreshToken.User;

                _context.RefreshTokens.Remove(refreshToken);
                await _context.SaveChangesAsync();
                transaction.Commit();

                var accessToken = CreateAccessToken(user);

                return Ok(new TokenOutput()
                {
                    AccessToken = accessToken.Item1,
                    Expires = accessToken.Item2,
                    RefreshToken = await CreateRefreshToken(user.Id),
                    Id = user.Id,
                    Email = user.UserName
                });
            }
        }

        private async Task<IActionResult> RemoveRefreshToken(string token)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                var refreshToken = await _context.RefreshTokens
                    .FirstOrDefaultAsync(x => x.Token == token);
                if (refreshToken == null)
                {
                    return BadRequest();
                }

                _context.RefreshTokens.Remove(refreshToken);

                await _context.SaveChangesAsync();
                transaction.Commit();
            }

            return Ok();
        }

        private (string, DateTime) CreateAccessToken(UserEntity user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var expires = DateTime.UtcNow.AddMinutes(15);

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims,
                expires: expires,
                signingCredentials: credentials);

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            return (jwtSecurityTokenHandler.WriteToken(jwtSecurityToken), expires);
        }

        private async Task<string> CreateRefreshToken(string userId)
        {
            var createdOn = DateTime.UtcNow;
            var expiresOn = createdOn.AddMonths(1);

            var refreshToken = new RefreshTokenEntity
            {
                UserId = userId,
                CreatedOn = createdOn,
                ExpiresOn = expiresOn,
                Token = Guid.NewGuid().ToString()
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken.Token;
        }
    }
}
