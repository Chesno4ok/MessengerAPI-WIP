using AutoMapper;
using ChesnokMessengerAPI.Responses;
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
        public  IActionResult GetUser(int UserId)
        {
            var _context = new MessengerContext();
            var user = _context.Users.FirstOrDefault(i => i.Id == UserId);

            return Ok(user.ToJson());
        }

        // Get a user's token
        [HttpGet("get_token")]
        public IActionResult GetToken(string login, string password)
        {
            var _context = new MessengerContext();
            var user = _context.Users.FirstOrDefault(i => i.Login == login && i.Password == password);

            var userToken = _mapper.Map<UserCredentials>(user);
            return Ok(userToken.ToJson());
        }

        // Change user's name
        [HttpPatch("change_username")]
        public IActionResult ChangeUsername(int userId, string token, string username)
        {
            var _context = new MessengerContext();
            var user = _context.Users.FirstOrDefault(i => i.Id == userId);
            user.Name = username;

             _context.SaveChanges();

            var newUser = _mapper.Map<UserInfo>(user);
            return Ok(newUser.ToJson());
        }
        [HttpPost("register_user")]
        public async Task<IActionResult> RegisterUser(string name, string login, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(i => i.Login == login);

            if (user != null)
            {
                return BadRequest(new Response("Error", "login already exists").ToJson());
            }

            user = new User()
            {
                Login = login,
                Name = name,
                Password = password,
                Token = System.Guid.NewGuid().ToString()
            };
            await _context.Users.AddAsync(user);

            await _context.SaveChangesAsync();
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


