using System.ComponentModel.DataAnnotations;

namespace WorkoutApp.Dto
{
    public class AdditionUserDto : BaseUserDto
    {
        [Required]
        [MinLength(6)]
        [MaxLength(50)]
        public string Password { get; set; } = null!;
    }
}