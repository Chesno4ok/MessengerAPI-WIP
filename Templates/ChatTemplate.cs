using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ChesnokMessengerAPI.Templates
{
    public class ChatTemplate : IValidatableObject
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string ChatName { get; set; } = null!;

        [Required]
        public virtual ICollection<ChatUserTemplate> ChatUsers { get; set; } = new List<ChatUserTemplate>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            using var dbContext = new MessengerContext();

            var valResList = new List<ValidationResult>();

            foreach(var chatUser in ChatUsers)
            {
                valResList.AddRange(chatUser.Validate(validationContext));
            }

            return valResList;
        }
    }
}
