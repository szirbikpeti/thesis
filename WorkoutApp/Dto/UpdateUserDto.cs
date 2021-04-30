using System;
using System.ComponentModel.DataAnnotations;

namespace WorkoutApp.Dto
{
  public class UpdateUserDto
  {
  
    [Required]
    public string FullName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string About { get; set; }
    
    public DateTimeOffset? Birthday { get; set; }
  }
}