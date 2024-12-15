using System.ComponentModel.DataAnnotations;

namespace ChesnokMessengerAPI.Templates
{
    public class UserNameTemplate 
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [StringLength(16, MinimumLength = 3)]
        public string Name { get; set; } = null!;
    }
}
