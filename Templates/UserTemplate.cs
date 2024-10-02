using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace ChesnokMessengerAPI.Templates
{
    public class UserTemplate : IValidatableObject
    {
        [Display]
        public int? Id { get; set; }
        [Required]
        [StringLength(16, MinimumLength = 3)]
        public string Name { get; set; } = null!;
        [Required]
        [StringLength(64, MinimumLength = 5)]
        public string Login { get; set; } = null!;
        [Required]
        [StringLength(64, MinimumLength = 8)]
        public string Password { get; set; } = null!;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            using var dbContext = new MessengerContext();

            if (dbContext.Users.FirstOrDefault(i => i.Id == Id) == null && Id != null)
                yield return new ValidationResult("User not found", new string[] {"Id"});
        }
    }
}
