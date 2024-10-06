using ChesnokMessengerAPI.Controllers;
using ChesnokMessengerAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.ComponentModel.DataAnnotations;

namespace ChesnokMessengerAPI.Templates
{
    public class UserTemplate : IValidatableObject
    {
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
            
            if(dbContext.Users.FirstOrDefault(i => i.LoginHash == TokenService.GenerateHash(Login)) != null)
                yield return new ValidationResult("Login already exists", new string[] { "Login" });
        }
    }
}
