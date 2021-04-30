using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using WorkoutApp.Abstractions;

namespace WorkoutApp.Entities
{
    public class UserEntity : 
        IdentityUser<int>, 
        IIdentityAwareEntity,
        IChangeAwareEntity,
        IDeleteAwareEntity
    {
        public string FullName { get; set; }

        public byte[] PasswordSalt { get; set; }

        public int? ProfilePictureId { get; set; }

        public string About { get; set; }

        public DateTimeOffset? Birthday { get; set; }

        public bool IsAdmin { get; set; }

        public DateTimeOffset? LastSignedInOn { get; set; }
        
        public DateTimeOffset CreatedOn { get; set; }
        
        public DateTimeOffset ModifiedOn { get; set; }
        
        public DateTimeOffset? DeletedOn { get; set; }

        public FileEntity ProfilePicture { get; set; }
        
        public ICollection<UserRoleRelationEntity> Roles { get; set; }

        public ICollection<UserUserRelationEntity> RequestingUsers { get; set; }
        
        public ICollection<UserUserRelationEntity> RequestedUsers { get; set; }
    }
}