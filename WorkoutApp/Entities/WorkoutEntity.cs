using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using WorkoutApp.Abstractions;

namespace WorkoutApp.Entities
{
    public class WorkoutEntity : IIdentityAwareEntity, IChangeAwareEntity, IDeleteAwareEntity
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }

        public DateTimeOffset Date { get; set; }

        public WorkoutType Type { get; set; }
        
        public double? Distance { get; set; }
        
        public string? Duration { get; set; }
        
        public DateTimeOffset CreatedOn { get; set; }
        
        public DateTimeOffset ModifiedOn { get; set; }
        
        public DateTimeOffset? DeletedOn { get; set; }

        public UserEntity User { get; set; } = null!;
        
        public PostEntity? Post { get; set; }
        
        public ICollection<ExerciseEntity>? Exercises { get; set; }
        
        public ICollection<WorkoutFileRelationEntity> FileRelationEntities { get; set; } = null!;
    }
}