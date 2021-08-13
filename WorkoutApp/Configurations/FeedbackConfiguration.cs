using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutApp.Entities;

namespace WorkoutApp.Configurations
{
  public class FeedbackConfiguration : IEntityTypeConfiguration<FeedbackEntity>
  {
    public void Configure(EntityTypeBuilder<FeedbackEntity> builder)
    {
      builder.HasKey(_ => _.Id);
      
      builder.Property(_ => _.Id)
        .ValueGeneratedOnAdd();

      builder.HasOne(_ => _.User)
        .WithMany(_ => _.Feedbacks)
        .HasForeignKey(_ => _.UserId)
        .OnDelete(DeleteBehavior.Restrict); // TODO - no action
    }
  }
}