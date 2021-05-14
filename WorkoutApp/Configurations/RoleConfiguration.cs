using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutApp.Entities;

namespace WorkoutApp.Configurations
{
  public class RoleConfiguration : IEntityTypeConfiguration<RoleEntity>
  {
    public void Configure(EntityTypeBuilder<RoleEntity> builder)
    {
      builder.HasKey(_ => _.Id);
      
      builder.Property(_ => _.Id)
        .ValueGeneratedOnAdd();
    }
  }
}