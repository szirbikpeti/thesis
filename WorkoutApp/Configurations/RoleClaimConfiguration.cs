using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutApp.Entities;

namespace WorkoutApp.Configurations
{
  public class RoleClaimConfiguration : IEntityTypeConfiguration<RoleClaimEntity>
  {
    public void Configure(EntityTypeBuilder<RoleClaimEntity> builder)
    {
      builder.HasOne(_ => _.Role)
        .WithMany(_ => _.Claims)
        .HasForeignKey(_ => _.RoleId);
    }
  }
}