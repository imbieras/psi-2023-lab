﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using StudyBuddy.API.Data;

#nullable disable

namespace StudyBuddy.API.Data.Migrations
{
    [DbContext(typeof(StudyBuddyDbContext))]
    partial class StudyBuddyDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("StudyBuddy.Shared.Models.ChatMessage", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ConversationId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SenderUserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("ConversationId");

                    b.ToTable("ChatMessages");
                });

            modelBuilder.Entity("StudyBuddy.Shared.Models.Event", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Color")
                        .HasColumnType("text");

                    b.Property<DateTime>("End")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("Start")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Schedules");
                });

            modelBuilder.Entity("StudyBuddy.Shared.Models.Match", b =>
                {
                    b.Property<string>("User1Id")
                        .HasColumnType("text");

                    b.Property<string>("User2Id")
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("MatchDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("User1Id", "User2Id");

                    b.HasIndex("User2Id");

                    b.HasIndex("User1Id", "User2Id")
                        .IsUnique();

                    b.ToTable("Matches");
                });

            modelBuilder.Entity("StudyBuddy.Shared.Models.MatchRequest", b =>
                {
                    b.Property<string>("RequesterId")
                        .HasColumnType("text");

                    b.Property<string>("RequestedId")
                        .HasColumnType("text");

                    b.HasKey("RequesterId", "RequestedId");

                    b.ToTable("MatchRequests");
                });

            modelBuilder.Entity("StudyBuddy.Shared.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<byte>("Flags")
                        .HasColumnType("smallint");

                    b.Property<string>("Hobbies")
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("StudyBuddy.Shared.Models.UserSeenProfile", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("SeenUserId")
                        .HasColumnType("text");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("UserId", "SeenUserId");

                    b.HasIndex("SeenUserId");

                    b.HasIndex("UserId");

                    b.ToTable("UserSeenProfiles");
                });

            modelBuilder.Entity("StudyBuddy.Shared.Models.Match", b =>
                {
                    b.HasOne("StudyBuddy.Shared.Models.User", null)
                        .WithMany()
                        .HasForeignKey("User1Id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("StudyBuddy.Shared.Models.User", null)
                        .WithMany()
                        .HasForeignKey("User2Id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("StudyBuddy.Shared.Models.User", b =>
                {
                    b.OwnsOne("StudyBuddy.Shared.Models.UserTraits", "Traits", b1 =>
                        {
                            b1.Property<string>("UserId")
                                .HasColumnType("text");

                            b1.Property<string>("AvatarPath")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<DateTime>("Birthdate")
                                .HasColumnType("timestamp with time zone");

                            b1.Property<string>("Description")
                                .HasColumnType("text");

                            b1.Property<double?>("Latitude")
                                .HasColumnType("double precision");

                            b1.Property<double?>("Longitude")
                                .HasColumnType("double precision");

                            b1.Property<string>("Subject")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("UserId");

                            b1.ToTable("Users");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.Navigation("Traits")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
