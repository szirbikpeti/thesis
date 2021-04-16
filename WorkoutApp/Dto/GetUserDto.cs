using System;

namespace WorkoutApp.Dto
{
    public class GetUserDto : BaseUserDto
    {
        public int Id { get; set; }
        
        public string About { get; set; }
        
        public bool IsAdmin { get; set; }
        
        // public GetFileDto ProfilePicture { get; set; }
        
        public DateTimeOffset LastSignedInOn { get; set; }
    }
}