using System.ComponentModel.DataAnnotations;

namespace ChesnokMessengerAPI.Templates
{
    public class MessageTemplate : IValidatableObject
    {
        [Display]
        public int? Id { get; set; }

        [Required]
        public int User { get; set; }

        [Required]
        public int ChatId { get; set; }

        [Required]
        [StringLength(maximumLength:100, MinimumLength = 1)]
        public string? Text { get; set; }

        public virtual User UserNavigation { get; set; } = null!;
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            using var dbContext = new MessengerContext();

            if (dbContext.Messages.FirstOrDefault(i => i.Id == Id) == null && Id != null)
                yield return new ValidationResult("Message not found");
            if (dbContext.Users.FirstOrDefault(i => i.Id == User) == null)
                yield return new ValidationResult("User not found", new string[] { "User" });
            if (dbContext.Chats.FirstOrDefault(i => i.Id == ChatId) == null && Id != null)
                yield return new ValidationResult("Id not found", new string[] { "Id" });

        }
    }
}
