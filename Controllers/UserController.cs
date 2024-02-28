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

            return Ok(new UserResponse
            {
                Id = user.Id,
                Name = user.Name
            });
        }

        // Get a user's token
        [HttpGet("get_token")]
        public IActionResult GetToken(string login, string password)
        {
            var _context = new MessengerApiContext();
            var user = _context.Users.FirstOrDefault(i => i.Login == login && i.Password == password);

            return Ok(new TokenResponse() { 
                Id = user.Id, 
                Token = user.Token});
        }

        // Change user's name
        [HttpGet("change_username")]
        public IActionResult ChangeUsername(int userId, string token, string username)
        {
            var _context = new MessengerApiContext();
            var user = _context.Users.FirstOrDefault(i => i.Id == userId);
            user.Name = username;

             _context.SaveChanges();
            return Ok();
        }

        // Check if a user has receieved a message
        // Depricated
        //[HttpGet("check_updates")]
        //public IActionResult CheckUpdates(int userId, string token)
        //{
        //    List<ChatUser> chats;
        //    using(var context = new MessengerApiContext())
        //    {
        //        chats = _context.ChatUsers.Where(i => i.UserId == userId && i.HasUpdates == true).ToList();
        //    }

        //    return Ok(new UserUpdateResponse() { Id = (int)userId, Updates = chats});
        //}

        // Register a new user
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
            return Ok(new TokenResponse { Id = user.Id, Token = user.Token });

        }
        [HttpPost("search_user")]
        public IActionResult SearchUser(string username)
        {
            User[] users;
            using (var context = new MessengerApiContext())
            {
                users = context.Users.Where(i => i.Name.StartsWith(username)).ToArray();
            }


            

            List<UserResponse> responses = new List<UserResponse>();

            foreach (User i in users)
            {
                responses.Add(new UserResponse() { Id = i.Id, Name = i.Name });
            }

            return Ok(responses);
        }
        
    }
}


