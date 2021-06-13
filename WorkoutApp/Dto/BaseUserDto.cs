using System;
using System.ComponentModel.DataAnnotations;

namespace WorkoutApp.Dto
{
    public class BaseUserDto
    {
        [Required]
        public string FullName { get; set; } = null!;

        [Required]
        public string UserName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        public DateTimeOffset? Birthday { get; set; }
    }
}