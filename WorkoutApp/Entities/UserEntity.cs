using System;
using Microsoft.AspNetCore.Identity;
using WorkoutApp.Abstractions;

namespace WorkoutApp.Entities
{
    public class UserEntity : IIdentityAwareEntity
    {
        public int Id { get; set; }
        
        public string FullName { get; set; }
        
        public string UserName { get; set; }
        
        public string Email { get; set; }
        
        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        public int? ProfilePictureId { get; set; }

        public string About { get; set; }

        public DateTimeOffset? Birthday { get; set; }

        public bool IsAdmin { get; set; }

        public DateTimeOffset? LastSignedInOn { get; set; }

        public FileEntity? ProfilePicture { get; set; }
    }
}