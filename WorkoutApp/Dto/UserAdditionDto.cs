using System.ComponentModel.DataAnnotations;

namespace WorkoutApp.Dto
{
    public class UserAdditionDto : BaseUserDto
    {
        [Required]
        [MinLength(6)]
        [MaxLength(50)]
        public string Password { get; set; } = null!;
        
        public int ProfilePictureId { get; set; } 
    }
}