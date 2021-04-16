using System;
using WorkoutApp.Abstractions;

namespace WorkoutApp.Entities
{
    public class SetEntity : IIdentityAwareEntity
    {
        public int Id { get; set; }

        public int ExerciseId { get; set; }

        public int Reps { get; set; }

        public double Weight { get; set; }
        
        public TimeSpan? Duration { get; set; }

        public ExerciseEntity Exercise { get; set; } = null!;
    }
}