using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutApp.Entities;

namespace WorkoutApp.Configurations
{
  public class PostFileRelationConfiguration : IEntityTypeConfiguration<PostFileRelationEntity>
  {
    public void Configure(EntityTypeBuilder<PostFileRelationEntity> builder)
    {
      if (builder == null) {
        throw new ArgumentNullException(nameof(builder));
      }

      builder.ToTable("PostFileRelations")
        .HasKey(_ => new { _.PostId, _.FileId });

      builder.Property(_ => _.PostId)
        .ValueGeneratedNever();

      builder.Property(_ => _.FileId)
        .ValueGeneratedNever();

      builder.HasOne(_ => _.Post)
        .WithMany(_ => _.FileRelationEntities)
        .HasForeignKey(_ => _.PostId)
        .OnDelete(DeleteBehavior.Restrict);

      builder.HasOne(_ => _.File)
        .WithMany(_ => _.PostRelationEntities)
        .HasForeignKey(_ => _.FileId)
        .OnDelete(DeleteBehavior.Restrict);
    }
  }
}