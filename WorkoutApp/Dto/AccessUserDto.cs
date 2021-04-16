using System.ComponentModel.DataAnnotations;

namespace WorkoutApp.Dto
{
    public class AccessUserDto
    {
        [Required]
        [MinLength(3)]
        public string UserName { get; set; }
        
        [Required]
        [MinLength(6)]
        [MaxLength(50)]
        public string Password { get; set; }
    }
}