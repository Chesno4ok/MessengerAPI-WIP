using AutoMapper;
using ChesnokMessengerAPI.Responses;
using ChesnokMessengerAPI.Services;
using ChesnokMessengerAPI.Templates;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using System.Collections;

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
        public  IActionResult GetUser(int userId)
        {
            var _context = new MessengerContext();
            var user = _context.Users
                .FirstOrDefault(i => i.Id == userId);

            return Ok(user.ToJson());
        }

        // Get a user's token
        [HttpPost("get_token")]
        public IActionResult GetToken(UserTemplate userTemplate)
        {
            // Client validation
            var context = new MessengerContext();

            if (userTemplate.Id != null)
                return BadRequest(new InvalidParametersResponse("Error", "Id must be null", new string[] { "Id" }));

            var user = context.Users.FirstOrDefault(i => i.Login == userTemplate.Login
            && i.Password == userTemplate.Password);

            if (user == null)
                return BadRequest(new InvalidParametersResponse("Error", "Invalid Credentials", new string[] { "Login", "Password" }));


            // Generating new token

            var tokens = context.Tokens.Where(i => i.UserId == userTemplate.Id);

            if(tokens.Count() != 0)
            {
                context.Tokens.RemoveRange(tokens);
            }

            var stringToken = TokenService.GenerateToken();
            var tokenHash = TokenService.GenerateHash(new Guid(stringToken));

            context.Tokens.Add(new Token()
            {
                UserId = (int)userTemplate.Id,
                TokenHash = tokenHash
            });

            var userResponse = GetUser((int)userTemplate.Id);

            return Ok(user.ToJson());
        }

        // Change user's name
        [HttpPut("edit_user")]
        public IActionResult EditUser( UserTemplate userTemplate)
        {
            var context = new MessengerContext();

            if (userTemplate.Id == null)
                return BadRequest(new InvalidParametersResponse("Error", "Id cannot be null", new string[] {"Id"}));

            var newUser = context.Users.FirstOrDefault(i => i.Id == userTemplate.Id);

            newUser = (User)_mapper.Map(userTemplate, newUser, typeof(UserTemplate), typeof(User));

            context.SaveChanges();

            return Ok(newUser.ToJson());
        }
        [HttpPost("register_user")]
        public async Task<IActionResult> RegisterUser(UserTemplate userTemplate)
        {
            using var context = new MessengerContext();

            if(userTemplate.Id != null)
                return BadRequest(new InvalidParametersResponse("Error", "Id must be null", new string[] { "Id" }));

            var user = _mapper.Map<User>(userTemplate);

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
            var _context = new MessengerContext();
            var user = _context.Users.FirstOrDefault(i => i.Login == login);

            if (user != null)
                return BadRequest();

            return Ok();
        }
        [HttpGet("search_user")]
        public IActionResult SearchUser(string username)
        {
            User[] users;
            using (var context = new MessengerContext())
            {
                users = context.Users.Where(i => i.Name.ToLower().StartsWith(username)).ToArray();
            }

            return Ok(users.ToJson());
        }
        
    }
}


