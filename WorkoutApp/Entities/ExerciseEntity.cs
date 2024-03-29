﻿using System;
using System.Collections.Generic;
using WorkoutApp.Abstractions;

namespace WorkoutApp.Entities
{
    public class ExerciseEntity : IIdentityAwareEntity, IDeleteAwareEntity
    {
        public int Id { get; set; }

        public int WorkoutId { get; set; }

        public string Name { get; set; } = null!;

        public string? Equipment { get; set; }

        public WorkoutEntity Workout { get; set; } = null!;

        public ICollection<SetEntity> Sets { get; set; } = null!;
        
        public DateTimeOffset? DeletedOn { get; set; }
    }
}