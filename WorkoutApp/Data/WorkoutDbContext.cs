using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkoutApp.Configurations;
using WorkoutApp.Entities;

namespace WorkoutApp.Data
{
    public class WorkoutDbContext : IdentityDbContext<
    UserEntity,
    RoleEntity,
    int,
    UserClaimEntity,
    UserRoleRelationEntity,
    IdentityUserLogin<int>,
    RoleClaimEntity,
    IdentityUserToken<int>
    >
    {
        public WorkoutDbContext(DbContextOptions<WorkoutDbContext> options) : base(options) {}
        
        public DbSet<FileEntity> Files { get; set; }
        public DbSet<SetEntity> Sets { get; set; }
        public DbSet<ExerciseEntity> Exercises { get; set; }
        public DbSet<WorkoutEntity> Workouts { get; set; }
        
        public DbSet<UserUserRelationEntity> UserUserRelations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserClaimConfiguration());
            modelBuilder.ApplyConfiguration(new RoleClaimConfiguration());
            modelBuilder.ApplyConfiguration(new UserUserRelationConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleRelationConfiguration());

            modelBuilder.Entity<UserEntity>()
                .HasQueryFilter(_ => _.DeletedOn == null);

            modelBuilder.Entity<WorkoutEntity>()
                .HasQueryFilter(_ => _.DeletedOn == null);
        }
    }
}