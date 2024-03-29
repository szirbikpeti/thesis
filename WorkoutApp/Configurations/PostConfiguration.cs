﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutApp.Entities;

namespace WorkoutApp.Configurations
{
  public class PostConfiguration : IEntityTypeConfiguration<PostEntity>
  {
    public void Configure(EntityTypeBuilder<PostEntity> builder)
    {
      if (builder == null) {
        throw new ArgumentNullException(nameof(builder));
      }

      builder.HasKey(_ => _.Id);

      builder.Property(_ => _.Id)
        .ValueGeneratedOnAdd();

      builder.HasOne(_ => _.Workout)
        .WithOne(_ => _!.Post!)
        .HasForeignKey<PostEntity>(_ => _.WorkoutId)
        .OnDelete(DeleteBehavior.Restrict);
      
      builder.HasOne(_ => _.User)
        .WithMany(_ => _.Posts)
        .HasForeignKey(_ => _.UserId)
        .OnDelete(DeleteBehavior.Restrict);
    }
  }
}