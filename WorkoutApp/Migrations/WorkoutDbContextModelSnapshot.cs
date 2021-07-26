﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WorkoutApp.Data;

namespace WorkoutApp.Migrations
{
    [DbContext(typeof(WorkoutDbContext))]
    partial class WorkoutDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "6.0.0-preview.3.21201.2")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("text");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("WorkoutApp.Entities.CommentEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("CommentedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset?>("DeletedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("PostId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("WorkoutApp.Entities.ExerciseEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTimeOffset?>("DeletedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Equipment")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("WorkoutId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("WorkoutId");

                    b.ToTable("Exercises");
                });

            modelBuilder.Entity("WorkoutApp.Entities.FileEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<byte[]>("Data")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<string>("Format")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Size")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("UploadedOn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("WorkoutApp.Entities.FollowEntity", b =>
                {
                    b.Property<int>("FollowerId")
                        .HasColumnType("integer");

                    b.Property<int>("FollowedId")
                        .HasColumnType("integer");

                    b.HasKey("FollowerId", "FollowedId");

                    b.HasIndex("FollowedId");

                    b.ToTable("Follows");
                });

            modelBuilder.Entity("WorkoutApp.Entities.FollowRequestEntity", b =>
                {
                    b.Property<int>("SourceId")
                        .HasColumnType("integer");

                    b.Property<int>("TargetId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsBlocked")
                        .HasColumnType("boolean");

                    b.HasKey("SourceId", "TargetId");

                    b.HasIndex("TargetId");

                    b.ToTable("FollowRequests");
                });

            modelBuilder.Entity("WorkoutApp.Entities.LikeEntity", b =>
                {
                    b.Property<int>("PostId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("PostId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("Likes");
                });

            modelBuilder.Entity("WorkoutApp.Entities.NotificationEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTimeOffset?>("DeletedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("ReceivedUserId")
                        .HasColumnType("integer");

                    b.Property<int>("SentByUserId")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("TriggeredOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ReceivedUserId");

                    b.HasIndex("SentByUserId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("WorkoutApp.Entities.PostCommentRelationEntity", b =>
                {
                    b.Property<int>("PostId")
                        .HasColumnType("integer");

                    b.Property<int>("CommentId")
                        .HasColumnType("integer");

                    b.HasKey("PostId", "CommentId");

                    b.HasIndex("CommentId");

                    b.ToTable("PostCommentRelations");
                });

            modelBuilder.Entity("WorkoutApp.Entities.PostEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTimeOffset?>("DeletedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("PostedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<int>("WorkoutId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("WorkoutId")
                        .IsUnique();

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("WorkoutApp.Entities.PostFileRelationEntity", b =>
                {
                    b.Property<int>("PostId")
                        .HasColumnType("integer");

                    b.Property<int>("FileId")
                        .HasColumnType("integer");

                    b.HasKey("PostId", "FileId");

                    b.HasIndex("FileId");

                    b.ToTable("PostFileRelations");
                });

            modelBuilder.Entity("WorkoutApp.Entities.RoleClaimEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("WorkoutApp.Entities.RoleEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("WorkoutApp.Entities.SetEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTimeOffset?>("DeletedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<TimeSpan?>("Duration")
                        .HasColumnType("interval");

                    b.Property<int>("ExerciseId")
                        .HasColumnType("integer");

                    b.Property<int>("Reps")
                        .HasColumnType("integer");

                    b.Property<double>("Weight")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.HasIndex("ExerciseId");

                    b.ToTable("Sets");
                });

            modelBuilder.Entity("WorkoutApp.Entities.UserClaimEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("WorkoutApp.Entities.UserEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("About")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset?>("Birthday")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset?>("DeletedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset?>("LastSignedInOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<int>("ProfilePictureId")
                        .HasColumnType("integer");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.HasIndex("ProfilePictureId")
                        .IsUnique();

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("WorkoutApp.Entities.UserRoleRelationEntity", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("WorkoutApp.Entities.WorkoutEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTimeOffset>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset?>("DeletedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Workouts");
                });

            modelBuilder.Entity("WorkoutApp.Entities.WorkoutFileRelationEntity", b =>
                {
                    b.Property<int>("WorkoutId")
                        .HasColumnType("integer");

                    b.Property<int>("FileId")
                        .HasColumnType("integer");

                    b.HasKey("WorkoutId", "FileId");

                    b.HasIndex("FileId");

                    b.ToTable("WorkoutFileRelations");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.HasOne("WorkoutApp.Entities.UserEntity", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.HasOne("WorkoutApp.Entities.UserEntity", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WorkoutApp.Entities.ExerciseEntity", b =>
                {
                    b.HasOne("WorkoutApp.Entities.WorkoutEntity", "Workout")
                        .WithMany("Exercises")
                        .HasForeignKey("WorkoutId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Workout");
                });

            modelBuilder.Entity("WorkoutApp.Entities.FollowEntity", b =>
                {
                    b.HasOne("WorkoutApp.Entities.UserEntity", "FollowedUser")
                        .WithMany("FollowerUsers")
                        .HasForeignKey("FollowedId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("WorkoutApp.Entities.UserEntity", "FollowerUser")
                        .WithMany("FollowedUsers")
                        .HasForeignKey("FollowerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("FollowedUser");

                    b.Navigation("FollowerUser");
                });

            modelBuilder.Entity("WorkoutApp.Entities.FollowRequestEntity", b =>
                {
                    b.HasOne("WorkoutApp.Entities.UserEntity", "SourceUser")
                        .WithMany("TargetUsers")
                        .HasForeignKey("SourceId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("WorkoutApp.Entities.UserEntity", "TargetUser")
                        .WithMany("SourceUsers")
                        .HasForeignKey("TargetId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("SourceUser");

                    b.Navigation("TargetUser");
                });

            modelBuilder.Entity("WorkoutApp.Entities.LikeEntity", b =>
                {
                    b.HasOne("WorkoutApp.Entities.PostEntity", "Post")
                        .WithMany("LikingUsers")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("WorkoutApp.Entities.UserEntity", "User")
                        .WithMany("LikedPosts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Post");

                    b.Navigation("User");
                });

            modelBuilder.Entity("WorkoutApp.Entities.NotificationEntity", b =>
                {
                    b.HasOne("WorkoutApp.Entities.UserEntity", "ReceivedUser")
                        .WithMany("ReceivedNotifications")
                        .HasForeignKey("ReceivedUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("WorkoutApp.Entities.UserEntity", "SentByUser")
                        .WithMany("SentNotifications")
                        .HasForeignKey("SentByUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("ReceivedUser");

                    b.Navigation("SentByUser");
                });

            modelBuilder.Entity("WorkoutApp.Entities.PostCommentRelationEntity", b =>
                {
                    b.HasOne("WorkoutApp.Entities.CommentEntity", "Comment")
                        .WithMany("PostRelationEntities")
                        .HasForeignKey("CommentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("WorkoutApp.Entities.PostEntity", "Post")
                        .WithMany("CommentRelationEntities")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Comment");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("WorkoutApp.Entities.PostEntity", b =>
                {
                    b.HasOne("WorkoutApp.Entities.UserEntity", "User")
                        .WithMany("Posts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("WorkoutApp.Entities.WorkoutEntity", "Workout")
                        .WithOne("Post")
                        .HasForeignKey("WorkoutApp.Entities.PostEntity", "WorkoutId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");

                    b.Navigation("Workout");
                });

            modelBuilder.Entity("WorkoutApp.Entities.PostFileRelationEntity", b =>
                {
                    b.HasOne("WorkoutApp.Entities.FileEntity", "File")
                        .WithMany("PostRelationEntities")
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("WorkoutApp.Entities.PostEntity", "Post")
                        .WithMany("FileRelationEntities")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("File");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("WorkoutApp.Entities.RoleClaimEntity", b =>
                {
                    b.HasOne("WorkoutApp.Entities.RoleEntity", "Role")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("WorkoutApp.Entities.SetEntity", b =>
                {
                    b.HasOne("WorkoutApp.Entities.ExerciseEntity", "Exercise")
                        .WithMany("Sets")
                        .HasForeignKey("ExerciseId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Exercise");
                });

            modelBuilder.Entity("WorkoutApp.Entities.UserClaimEntity", b =>
                {
                    b.HasOne("WorkoutApp.Entities.UserEntity", "User")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("WorkoutApp.Entities.UserEntity", b =>
                {
                    b.HasOne("WorkoutApp.Entities.FileEntity", "ProfilePicture")
                        .WithOne("ProfilePictureOfUser")
                        .HasForeignKey("WorkoutApp.Entities.UserEntity", "ProfilePictureId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("ProfilePicture");
                });

            modelBuilder.Entity("WorkoutApp.Entities.UserRoleRelationEntity", b =>
                {
                    b.HasOne("WorkoutApp.Entities.RoleEntity", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WorkoutApp.Entities.UserEntity", "User")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("WorkoutApp.Entities.WorkoutEntity", b =>
                {
                    b.HasOne("WorkoutApp.Entities.UserEntity", "User")
                        .WithMany("Workouts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("WorkoutApp.Entities.WorkoutFileRelationEntity", b =>
                {
                    b.HasOne("WorkoutApp.Entities.FileEntity", "File")
                        .WithMany("WorkoutRelationEntities")
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("WorkoutApp.Entities.WorkoutEntity", "Workout")
                        .WithMany("FileRelationEntities")
                        .HasForeignKey("WorkoutId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("File");

                    b.Navigation("Workout");
                });

            modelBuilder.Entity("WorkoutApp.Entities.CommentEntity", b =>
                {
                    b.Navigation("PostRelationEntities");
                });

            modelBuilder.Entity("WorkoutApp.Entities.ExerciseEntity", b =>
                {
                    b.Navigation("Sets");
                });

            modelBuilder.Entity("WorkoutApp.Entities.FileEntity", b =>
                {
                    b.Navigation("PostRelationEntities");

                    b.Navigation("ProfilePictureOfUser");

                    b.Navigation("WorkoutRelationEntities");
                });

            modelBuilder.Entity("WorkoutApp.Entities.PostEntity", b =>
                {
                    b.Navigation("CommentRelationEntities");

                    b.Navigation("FileRelationEntities");

                    b.Navigation("LikingUsers");
                });

            modelBuilder.Entity("WorkoutApp.Entities.RoleEntity", b =>
                {
                    b.Navigation("Claims");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("WorkoutApp.Entities.UserEntity", b =>
                {
                    b.Navigation("Claims");

                    b.Navigation("FollowedUsers");

                    b.Navigation("FollowerUsers");

                    b.Navigation("LikedPosts");

                    b.Navigation("Posts");

                    b.Navigation("ReceivedNotifications");

                    b.Navigation("Roles");

                    b.Navigation("SentNotifications");

                    b.Navigation("SourceUsers");

                    b.Navigation("TargetUsers");

                    b.Navigation("Workouts");
                });

            modelBuilder.Entity("WorkoutApp.Entities.WorkoutEntity", b =>
                {
                    b.Navigation("Exercises");

                    b.Navigation("FileRelationEntities");

                    b.Navigation("Post");
                });
#pragma warning restore 612, 618
        }
    }
}
