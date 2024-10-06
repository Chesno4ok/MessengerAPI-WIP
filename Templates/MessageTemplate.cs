using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ChesnokMessengerAPI.Templates
{
    public class MessageTemplate : IValidatableObject
    {
        [Required]
        public int? UserId { get; set; }

        [Required]
        public int? ChatId { get; set; }

        [Required]
        [StringLength(maximumLength:100, MinimumLength = 1)]
        public string? Text { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            using var dbContext = new MessengerContext();

            if (dbContext.Users.FirstOrDefault(i => i.Id == UserId) == null)
                yield return new ValidationResult("User not found", new string[] { "User" });
            if (dbContext.Chats.FirstOrDefault(i => i.Id == ChatId) == null)
                yield return new ValidationResult("Chat not found", new string[] { "ChatId" });
            if (dbContext.ChatUsers.FirstOrDefault(i => i.UserId == UserId && i.ChatId == ChatId) == null)
                yield return new ValidationResult("User is not participant of the chat", new string[] {"ChatId" });

        }
    }
}
