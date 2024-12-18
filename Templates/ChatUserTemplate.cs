﻿using System.ComponentModel.DataAnnotations;

namespace ChesnokMessengerAPI.Templates
{
    public class ChatUserTemplate : IValidatableObject
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int ChatId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            using var dbContext = new MessengerContext();

            if (dbContext.Users.FirstOrDefault(i => i.Id == UserId) == null)
                yield return new ValidationResult("User not found", new string[] { "UserId" });
            if (dbContext.Chats.FirstOrDefault(i => i.Id == ChatId) == null)
                yield return new ValidationResult("Chat not found", new string[] { "Chat" });

        }
    }
}
