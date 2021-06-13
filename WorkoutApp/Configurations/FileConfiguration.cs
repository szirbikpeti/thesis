using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutApp.Entities;

namespace WorkoutApp.Configurations
{
  public class FileConfiguration: IEntityTypeConfiguration<FileEntity>
  {
    public void Configure(EntityTypeBuilder<FileEntity> builder)
    {
      builder.HasKey(_ => _.Id);
      
      builder.Property(_ => _.Id)
        .ValueGeneratedOnAdd();
    }
  }
}