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
    }
  }
}