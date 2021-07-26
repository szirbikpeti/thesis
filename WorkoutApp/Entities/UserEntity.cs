using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;
using WorkoutApp.Abstractions;

namespace WorkoutApp.Entities
{
    [SuppressMessage("CodeAnalysis", "CA2227", Justification = "Required by Entity Framework")]
    public class UserEntity : IdentityUser<int>, 
        IIdentityAwareEntity,
        IChangeAwareEntity,
        IDeleteAwareEntity
    {
        public string FullName { get; set; } = null!;

        public int ProfilePictureId { get; set; }

        public string About { get; set; } = null!;

        public DateTimeOffset? Birthday { get; set; }

        public DateTimeOffset? LastSignedInOn { get; set; }
        
        public DateTimeOffset CreatedOn { get; set; }
        
        public DateTimeOffset ModifiedOn { get; set; }
        
        public DateTimeOffset? DeletedOn { get; set; }

        public FileEntity ProfilePicture { get; set; } = null!;
        
        public ICollection<WorkoutEntity> Workouts { get; set; } = null!;
        
        public ICollection<PostEntity> Posts { get; set; } = null!;

        public ICollection<UserClaimEntity> Claims { get; set; } = null!;
        
        public ICollection<UserRoleRelationEntity> Roles { get; set; } = null!;
        
        public ICollection<FollowRequestEntity> SourceUsers { get; set; } = null!;
        
        public ICollection<FollowRequestEntity> TargetUsers { get; set; } = null!;
        
        public ICollection<FollowEntity> FollowerUsers { get; set; } = null!;
        
        public ICollection<FollowEntity> FollowedUsers { get; set; } = null!;

        public ICollection<NotificationEntity> SentNotifications { get; set; } = null!;

        public ICollection<NotificationEntity> ReceivedNotifications { get; set; } = null!;
        
        public ICollection<LikeEntity> LikedPosts { get; set; } = null!;
        
        public ICollection<CommentEntity> Comments { get; set; } = null!;
    }
}