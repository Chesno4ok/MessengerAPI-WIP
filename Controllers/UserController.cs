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
        public IActionResult GetUser(int id)
        {
            var user = _context.Users.FirstOrDefault(i => i.Id == id);

            if(user == null)
            {
                return NotFound();
            }

            return Ok(new UserResponse
            {
                Id = user.Id,
                UserName = user.UserName
            });
        }

        [HttpGet("get_token")]
        public IActionResult GetToken(int id, string login, string password)
        {

            var user = _context.Users.FirstOrDefault(i => i.Id == id && i.Login == login && i.Password == password);

            if(user == null)
            {
                return BadRequest(new Response()
                {
                    status = "Error",
                    message = "Incorrect parametrs. User not found"
                }.ToJson());
            }

            return Ok(new UserTokenResponse() { 
                Id = user.Id, 
                UserToken = user.UserToken});
        }

        [HttpGet("check_updates")]
        public IActionResult CheckUpdates(int id, string token)
        {

            var user = _context.Users.FirstOrDefault(i => i.Id == id && i.UserToken == token);

            if(user == null)
            {
                return BadRequest(new Response()
                {
                    status = "Error",
                    message = "Incorrect token"
                }.ToJson());
            }

            return Ok(new UserUpdateResponse() { Id = (int)id, HasUpdates = user.HasUpdates});
        }

        [HttpPost("register_user")]
        public IActionResult RegisterUser(string userName, string login, string password)
        {

            var user = _context.Users.FirstOrDefault(i => i.Login == login);

            if (user != null)
            {
                return BadRequest(new Response()
                {
                    status = "Error",
                    message = "login already exists"
                }.ToJson());
            }

            user = new User()
            {
                Login = login,
                UserName = userName,
                Password = password,
                UserToken = System.Guid.NewGuid().ToString()
            };
            _context.Users.Add(user);

            try
            {
                _context.SaveChanges();
                return Ok(new UserTokenResponse { Id = user.Id, UserToken = user.UserToken});
            }
            catch (Exception exc)
            {
                return BadRequest(new Response()
                {
                    status = "Error",
                    message = exc.Message
                }.ToJson());
            }
        }
    }
}


