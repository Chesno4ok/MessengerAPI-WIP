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
        private readonly MessengerApiContext _context;

        public UserController()
        {
            _context = new MessengerApiContext();
        }

        // Get a user's name
        [HttpGet("get_user")]
        public  IActionResult GetUser(int userId)
        {
            var _context = new MessengerApiContext();
            var user = _context.Users.FirstOrDefault(i => i.Id == userId);

            return Ok(user.ToJson());
        }

        // Get a user's token
        [HttpGet("get_token")]
        public IActionResult GetToken(string login, string password)
        {
            var _context = new MessengerApiContext();
            var user = _context.Users.FirstOrDefault(i => i.Login == login && i.Password == password);

            return Ok(new Response("Token", user.Token).ToJson());
        }

        // Change user's name
        [HttpGet("change_username")]
        public IActionResult ChangeUsername(int userId, string token, string username)
        {
            var _context = new MessengerApiContext();
            var user = _context.Users.FirstOrDefault(i => i.Id == userId);
            user.Name = username;

             _context.SaveChanges();
            return Ok(user.ToJson());
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
            var _context = new MessengerApiContext();
            var user = _context.Users.FirstOrDefault(i => i.Login == login);

            if (user != null)
                return BadRequest();

            return Ok();
        }
        [HttpPost("search_user")]
        public IActionResult SearchUser(string username)
        {
            User[] users;
            using (var context = new MessengerApiContext())
            {
                users = context.Users.Where(i => i.Name.StartsWith(username)).ToArray();
            }

            return Ok(users.ToJson());
        }
        
    }
}


