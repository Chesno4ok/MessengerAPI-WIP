using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ChesnokMessengerAPI.Templates
{
    public class EditMessageTemplate : IValidatableObject
    {
        [Required]
        public int? Id { get; set; }

        [Required]
        [StringLength(maximumLength:100, MinimumLength = 1)]
        public string? Text { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            using var dbContext = new MessengerContext();

            if (dbContext.Messages.FirstOrDefault(i => i.Id == Id) == null && Id != null)
                yield return new ValidationResult("Message not found", new string[] {"Id"});
        }
    }
}
