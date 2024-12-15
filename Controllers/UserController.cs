using AutoMapper;
using ChesnokMessengerAPI.Middleware;
using ChesnokMessengerAPI.Models;
using ChesnokMessengerAPI.Responses;
using ChesnokMessengerAPI.Services;
using ChesnokMessengerAPI.Templates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace ChesnokMessengerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly MessengerContext _context;
        private IMapper _mapper;

        public UserController(IMapper mapper)
        {
            _mapper = mapper;
            _context = new MessengerContext();
        }

        // Get a user's name
        [HttpGet("get_user")]
        [Authorize]
        public  IActionResult GetUser(int userId)
        {
            var _context = new MessengerContext();
            var user = _context.Users
                .FirstOrDefault(i => i.Id == userId);

            return Ok(user.ToJson());
        }

        [HttpGet("get_me")]
        [Authorize]
        public IActionResult GetMe()
        {
            var dbContext = new MessengerContext();
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity == null)
                return BadRequest();

            Claim? idClaim = identity.Claims.FirstOrDefault(i => i.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/authentication");

            if (idClaim == null)
                return BadRequest();

            int userId = Int32.Parse(idClaim.Value);

            var user = dbContext.Users.FirstOrDefault(i => i.Id == userId);

            if (user == null)
                return BadRequest();

            return Ok(user.ToJson());
        }


        [HttpGet("login")]
        public IActionResult Login(string login, string password)
        {
            var context = new MessengerContext();

            var loginHash = TokenService.GenerateHash(login);
            var passwordHash = TokenService.GenerateHash(password);

            var user = context.Users.FirstOrDefault(i => i.LoginHash == TokenService.GenerateHash(login)
            && i.PasswordHash == TokenService.GenerateHash(password));

            if (user == null)
                return BadRequest(new InvalidParametersResponse("Error", "Invalid credentials", new string[] { "login", "password" }));

            var claims = new List<Claim> { new Claim(ClaimTypes.Authentication, user.Id.ToString()) };
            
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromDays(1)), 
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            string token = new JwtSecurityTokenHandler().WriteToken(jwt);

            var userResponse = _mapper.Map<UserCredentials>(user);
            userResponse.Token = token;

            return Ok(userResponse.ToJson());
        }


        // Change user's name
        [HttpPut("edit_user")]
        [Authorize]
        public IActionResult EditUser(EditUserTemplate userTemplate)
        {
            var context = new MessengerContext();

            var newUser = context.Users.FirstOrDefault(i => i.Id == userTemplate.Id);
            

            if(userTemplate.Login is null)
                newUser.LoginHash = TokenService.GenerateHash(userTemplate.Login);
            if (userTemplate.Password is null)
                newUser.PasswordHash = TokenService.GenerateHash(userTemplate.Password);


            newUser = (User)_mapper.Map(userTemplate, newUser, typeof(UserTemplate), typeof(User));

            context.SaveChanges();

            return Ok(newUser.ToJson());
        }
        [HttpPost("register_user")]
        public async Task<IActionResult> RegisterUser(UserTemplate userTemplate)
        {
            using var context = new MessengerContext();

            var user = _mapper.Map<User>(userTemplate);
            user.LoginHash = TokenService.GenerateHash(userTemplate.Login);
            user.PasswordHash = TokenService.GenerateHash(userTemplate.Password);

            context.Users.Add(user);

            await context.SaveChangesAsync();

            var stringToken = TokenService.GenerateToken();
            var tokenHash = TokenService.GenerateHash(new Guid(stringToken));

            context.Tokens.Add(new Token()
            {
                UserId = user.Id,
                TokenHash = tokenHash
            });

            return Ok(user.ToJson());
        }
        [HttpGet("check_login")]
        public IActionResult CheckUser(string login)
        {
            var context = new MessengerContext();

            var user = context.Users.FirstOrDefault(i => i.LoginHash == TokenService.GenerateHash(login));

            if (user != null)
                return BadRequest();

            return Ok();
        }
        [HttpGet("search_user")]
        [Authorize]
        public IActionResult SearchUser(string username)
        {
            User[] users;
            using (var context = new MessengerContext())
            {
                users = context.Users.Where(i => i.Name.ToLower().StartsWith(username.ToLower())).ToArray();
            }

            return Ok(users.ToJson());
        }
        
    }
}


