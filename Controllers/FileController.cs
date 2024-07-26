using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;

namespace ChesnokMessengerAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        [HttpPost("upload_file")]
        public IActionResult UploadFile(int messageId, string type, byte[] content)
        {
            using var db = new MessengerContext();
            db.Files.Add(new ChesnokMessengerAPI.File()
            {
                Message = messageId,
                Type = type,
                Content = content
            });

            return Ok();
        }
        [HttpGet("get_files")]
        public IActionResult GetFiles(int messageId)
        {
            using var db = new MessengerContext();

            var messages = db.Files.Where(i => i.Message == messageId);

            return Ok(messages.ToJson());
        }
    }
}
