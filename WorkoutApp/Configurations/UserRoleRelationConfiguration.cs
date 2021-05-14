using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutApp.Entities;

namespace WorkoutApp.Configurations
{
  public class UserRoleRelationConfiguration : IEntityTypeConfiguration<UserRoleRelationEntity>
  {
    public void Configure(EntityTypeBuilder<UserRoleRelationEntity> builder)
    {
      builder.HasOne(_ => _.User)
        .WithMany(_ => _.Roles)
        .HasForeignKey(_ => _.UserId);
      
      builder.HasOne(_ => _.Role)
        .WithMany(_ => _.Users)
        .HasForeignKey(_ => _.RoleId);
    }
  }
}