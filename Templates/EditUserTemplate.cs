using ChesnokMessengerAPI.Services;
using System.ComponentModel.DataAnnotations;

namespace ChesnokMessengerAPI.Templates
{
    public class EditUserTemplate
    {
        [Required]
        public int Id { get; set; }
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

            if (dbContext.Users.FirstOrDefault(i => i.Id == Id) == null)
                yield return new ValidationResult("User not found", new string[] { "Id" });
            if (dbContext.Users.FirstOrDefault(i => i.LoginHash == TokenService.GenerateHash(Login) && i.Id != Id) != null)
                yield return new ValidationResult("Login already exists", new string[] { "Login" });
        }
    }
}
