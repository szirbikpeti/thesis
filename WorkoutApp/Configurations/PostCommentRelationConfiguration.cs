using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutApp.Entities;

namespace WorkoutApp.Configurations
{
  public class PostCommentRelationConfiguration : IEntityTypeConfiguration<PostCommentRelationEntity>
  {
    public void Configure(EntityTypeBuilder<PostCommentRelationEntity> builder)
    {
      if (builder == null) {
        throw new ArgumentNullException(nameof(builder));
      }

      builder.ToTable("PostCommentRelations")
        .HasKey(_ => new { _.PostId, _.CommentId });

      builder.Property(_ => _.PostId)
        .ValueGeneratedNever();

      builder.Property(_ => _.CommentId)
        .ValueGeneratedNever();

      builder.HasOne(_ => _.Post)
        .WithMany(_ => _.CommentRelationEntities)
        .HasForeignKey(_ => _.PostId)
        .OnDelete(DeleteBehavior.Restrict);

      builder.HasOne(_ => _.Comment)
        .WithMany(_ => _.PostRelationEntities)
        .HasForeignKey(_ => _.CommentId)
        .OnDelete(DeleteBehavior.Restrict);
    }
  }
}