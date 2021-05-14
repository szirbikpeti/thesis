using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutApp.Entities;

namespace WorkoutApp.Configurations
{
  public class UserClaimConfiguration : IEntityTypeConfiguration<UserClaimEntity>
  {
    public void Configure(EntityTypeBuilder<UserClaimEntity> builder)
    {
      builder.HasOne(_ => _.User)
        .WithMany(_ => _.Claims)
        .HasForeignKey(_ => _.UserId);
    }
  }
}