using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ChesnokMessengerAPI.Templates
{
    public class EditChatTemplate : IValidatableObject
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string ChatName { get; set; } = null!;

        [Required]
        public virtual ICollection<ChatUserTemplate> ChatUsers { get; set; } = new List<ChatUserTemplate>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            using var dbContext = new MessengerContext();
            var valResList = new List<ValidationResult>();

            if (dbContext.Chats.FirstOrDefault(i => i.Id == Id) == null)
                valResList.Add(new ValidationResult("Id not found", new string[] { "Id" }));

            foreach (var chatUser in ChatUsers)
            {
                valResList.AddRange(chatUser.Validate(validationContext));
            }

            if (dbContext.Chats.FirstOrDefault(i => i.Id == Id) == null)
                valResList.Add(new ValidationResult("Chat not found"));

            return valResList;
        }
    }
}
