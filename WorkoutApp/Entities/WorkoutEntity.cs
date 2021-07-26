using System;
using System.Collections.Generic;
using WorkoutApp.Abstractions;

namespace WorkoutApp.Entities
{
    public class WorkoutEntity : IIdentityAwareEntity, IChangeAwareEntity, IDeleteAwareEntity
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }

        public DateTimeOffset Date { get; set; }

        public string Type { get; set; } = null!;
        
        public DateTimeOffset CreatedOn { get; set; }
        
        public DateTimeOffset ModifiedOn { get; set; }
        
        public DateTimeOffset? DeletedOn { get; set; }

        public UserEntity User { get; set; } = null!;
        
        public PostEntity? Post { get; set; }
        
        public ICollection<ExerciseEntity> Exercises { get; set; } = null!;
        
        public ICollection<WorkoutFileRelationEntity> FileRelationEntities { get; set; } = null!;
    }
}