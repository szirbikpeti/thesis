using System;
using System.Collections.Generic;
using WorkoutApp.Abstractions;

namespace WorkoutApp.Entities
{
  public class CommentEntity : IIdentityAwareEntity, IDeleteAwareEntity
  {
    public int Id { get; set; }
    
    public int PostId { get; set; }
    
    public int UserId { get; set; }

    public string Comment { get; set; } = null!;
    
    public DateTimeOffset CommentedOn { get; set; }
    
    public DateTimeOffset ModifiedOn { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }

    public PostEntity Post { get; set; } = null!;
    
    public UserEntity User { get; set; } = null!;
  }
}