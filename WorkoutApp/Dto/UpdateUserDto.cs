using System;
using System.ComponentModel.DataAnnotations;

namespace WorkoutApp.Dto
{
  public class UpdateUserDto
  {
  
    [Required]
    public string FullName { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    public string About { get; set; } = null!;
    
    public int ProfilePictureId { get; set; }
    
    public DateTimeOffset? Birthday { get; set; }
    
    public UpdatePasswordDto PasswordChange { get; set; } = null!;
    
  }
}