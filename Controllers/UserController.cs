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
        public IActionResult GetUser(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var user = _context.Users.FirstOrDefault(i => i.Id == id);

            if(user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost("register_user")]
        public IActionResult RegisterUser(string? userName, string? login, string? password)
        {
            if(userName == null || login == null || password == null)
            {
                return BadRequest(new Response() { status = "Error", reason = "Incorrect parametrs" }.ToJson());
            }

            var user = _context.Users.FirstOrDefault(i => i.Login == login);

            if (user != null)
            {
                return BadRequest(new Response() { status = "Error", reason = "login already exists" }.ToJson());
            }

            try
            {
                _context.Users.Add(new User { Login = login, UserName = userName, Password = password });
                _context.SaveChanges();
                return Ok();
            }
            catch(Exception exc)
            {
                return BadRequest(new Response() { status = "Error", reason = exc.Message }.ToJson());
            }
            


        }


        
    }

   
}


