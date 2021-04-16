using Microsoft.EntityFrameworkCore;
using WorkoutApp.Entities;

namespace WorkoutApp.Data
{
    public class WorkoutDbContext : DbContext
    {
        public WorkoutDbContext(DbContextOptions<WorkoutDbContext> options) : base(options) {}
        
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<FileEntity> Files { get; set; }
        public DbSet<SetEntity> Sets { get; set; }
        public DbSet<ExerciseEntity> Exercises { get; set; }
        public DbSet<WorkoutEntity> Workouts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}