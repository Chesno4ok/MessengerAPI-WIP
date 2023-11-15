using ChesnokMessengerAPI.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;

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

        [HttpGet("get_user")]
        public IActionResult GetUser(int userId)
        {
            var user = _context.Users.FirstOrDefault(i => i.Id == userId);

            if(user == null)
            {
                return NotFound();
            }

            return Ok(new UserResponse
            {
                Id = user.Id,
                Name = user.Name
            });
        }

        [HttpGet("get_token")]
        public IActionResult GetToken(int userId, string login, string password)
        {

            var user = _context.Users.FirstOrDefault(i => i.Id == userId && i.Login == login && i.Password == password);

            if(user == null)
            {
                return BadRequest(new Response("Error", "Incorrect parametrs. User not found")
                .ToJson());
            }

            return Ok(new TokenResponse() { 
                Id = user.Id, 
                Token = user.Token});
        }

        [HttpGet("check_updates")]
        public IActionResult CheckUpdates(int userId, string token)
        {

            var user = _context.Users.FirstOrDefault(i => i.Id == userId && i.Token == token);

            if(user == null)
            {
                return BadRequest(new Response("Error","Incorrect token").ToJson());
            }

            return Ok(new UserUpdateResponse() { Id = (int)userId, Updates = user.Updates});
        }

        [HttpPost("register_user")]
        public IActionResult RegisterUser(string name, string login, string password)
        {

            var user = _context.Users.FirstOrDefault(i => i.Login == login);

            if (user != null)
            {
                return BadRequest(new Response( "Error", "login already exists").ToJson());
            }

            user = new User()
            {
                Login = login,
                Name = name,
                Password = password,
                Token = System.Guid.NewGuid().ToString()
            };
            _context.Users.Add(user);

            try
            {
                _context.SaveChanges();
                return Ok(new TokenResponse { Id = user.Id, Token = user.Token});
            }
            catch (Exception exc)
            {
                return BadRequest(new Response("Error",exc.Message ).ToJson());
            }
        }
    }
}


