using System;
using System.Collections.Generic;
using WorkoutApp.Abstractions;

namespace WorkoutApp.Entities
{
  public class PostEntity : IIdentityAwareEntity, IDeleteAwareEntity
  {
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    public DateTimeOffset PostedOn { get; set; }

    public string Description { get; set; } = null!;
    
    public DateTimeOffset? DeletedOn { get; set; }

    public UserEntity User { get; set; } = null!;
    
    public ICollection<PostFileRelationEntity> FileRelationEntities { get; set; } = null!;
    
    public ICollection<LikeEntity> LikingUsers { get; set; } = null!;
    
    public ICollection<PostCommentRelationEntity> CommentRelationEntities { get; set; } = null!;
  }
}