﻿using System;
using System.Collections.Generic;
using WorkoutApp.Abstractions;

namespace WorkoutApp.Entities
{
    public class WorkoutEntity : IIdentityAwareEntity
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }

        public DateTimeOffset Date { get; set; }

        public string Type { get; set; }

        public UserEntity User { get; set; }
        
        public ICollection<ExerciseEntity> Exercises { get; set; }
    }
}