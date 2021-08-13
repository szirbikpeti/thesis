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
        public DbSet<NotificationEntity> Notifications { get; set; } = null!;
        public DbSet<PostEntity> Posts { get; set; } = null!;
        public DbSet<CommentEntity> Comments { get; set; } = null!;
        public DbSet<FeedbackEntity> Feedbacks { get; set; } = null!;
        
        public DbSet<WorkoutFileRelationEntity> WorkoutFileRelations { get; set; } = null!;
        public DbSet<FollowRequestEntity> FollowRequests { get; set; } = null!;
        public DbSet<FollowEntity> Follows { get; set; } = null!;
        public DbSet<PostFileRelationEntity> PostFileRelations { get; set; } = null!;
        public DbSet<LikeEntity> Likes { get; set; } = null!;
        

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
            modelBuilder.ApplyConfiguration(new FollowRequestConfiguration());
            modelBuilder.ApplyConfiguration(new FollowConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleRelationConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationConfiguration());
            modelBuilder.ApplyConfiguration(new PostConfiguration());
            modelBuilder.ApplyConfiguration(new CommentConfiguration());
            modelBuilder.ApplyConfiguration(new FeedbackConfiguration());
            modelBuilder.ApplyConfiguration(new PostFileRelationConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationConfiguration());
            modelBuilder.ApplyConfiguration(new LikeConfiguration());

            modelBuilder.Entity<UserEntity>()
                .HasQueryFilter(_ => _.DeletedOn == null);

            modelBuilder.Entity<WorkoutEntity>()
                .HasQueryFilter(_ => _.DeletedOn == null);

            modelBuilder.Entity<ExerciseEntity>()
                .HasQueryFilter(_ => _.DeletedOn == null);

            modelBuilder.Entity<SetEntity>()
                .HasQueryFilter(_ => _.DeletedOn == null);
            
            modelBuilder.Entity<NotificationEntity>()
                .HasQueryFilter(_ => _.DeletedOn == null);
            
            modelBuilder.Entity<PostEntity>()
                .HasQueryFilter(_ => _.DeletedOn == null);
            
            modelBuilder.Entity<CommentEntity>()
                .HasQueryFilter(_ => _.DeletedOn == null);
        }
    }
}