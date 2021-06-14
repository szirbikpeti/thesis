using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkoutApp.Configurations;
using WorkoutApp.Entities;

namespace WorkoutApp.Data
{
    public sealed class WorkoutDbContext : IdentityDbContext<
    UserEntity,
    RoleEntity,
    int,
    UserClaimEntity,
    UserRoleRelationEntity,
    IdentityUserLogin<int>,
    RoleClaimEntity,
    IdentityUserToken<int>>
    {
        public WorkoutDbContext(DbContextOptions<WorkoutDbContext> options) : base(options) {}
        
        public DbSet<FileEntity> Files { get; set; } = null!;
        public DbSet<SetEntity> Sets { get; set; } = null!;
        public DbSet<ExerciseEntity> Exercises { get; set; } = null!;
        public DbSet<WorkoutEntity> Workouts { get; set; } = null!;
        
        public DbSet<WorkoutFileRelationEntity> WorkoutFileRelations { get; set; } = null!;
        public DbSet<UserUserRelationEntity> UserUserRelations { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new FileConfiguration());
            modelBuilder.ApplyConfiguration(new WorkoutConfiguration());
            modelBuilder.ApplyConfiguration(new ExerciseConfiguration());
            modelBuilder.ApplyConfiguration(new SetConfiguration());
            modelBuilder.ApplyConfiguration(new UserClaimConfiguration());
            modelBuilder.ApplyConfiguration(new RoleClaimConfiguration());
            modelBuilder.ApplyConfiguration(new WorkoutFileRelationConfiguration());
            modelBuilder.ApplyConfiguration(new UserUserRelationConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleRelationConfiguration());

            modelBuilder.Entity<UserEntity>()
                .HasQueryFilter(_ => _.DeletedOn == null);

            modelBuilder.Entity<WorkoutEntity>()
                .HasQueryFilter(_ => _.DeletedOn == null);
        }
    }
}