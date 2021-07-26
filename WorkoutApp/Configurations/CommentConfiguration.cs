using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutApp.Entities;

namespace WorkoutApp.Configurations
{
  public class CommentConfiguration : IEntityTypeConfiguration<CommentEntity>
  {
    public void Configure(EntityTypeBuilder<CommentEntity> builder)
    {
      builder.HasKey(_ => _.Id);
      
      builder.Property(_ => _.Id)
        .ValueGeneratedOnAdd();
      
      builder.HasOne(_ => _.Post)
        .WithMany(_ => _.Comments)
        .HasForeignKey(_ => _.PostId)
        .OnDelete(DeleteBehavior.Restrict);
      
      builder.HasOne(_ => _.User)
        .WithMany(_ => _.Comments)
        .HasForeignKey(_ => _.UserId)
        .OnDelete(DeleteBehavior.Restrict);
    }
  }
}