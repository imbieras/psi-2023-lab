﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using StudyBuddy.Data;

#nullable disable

namespace StudyBuddy.Migrations
{
    [DbContext(typeof(StudyBuddyDbContext))]
    [Migration("20231101120436_AddArrayColumns")]
    partial class AddArrayColumns
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("StudyBuddy.Models.Match", b =>
                {
                    b.Property<int>("MatchId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("MatchId"));

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("MatchDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("User1Id")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("User2Id")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("MatchId");

                    b.HasIndex("User1Id");

                    b.HasIndex("User2Id");

                    b.ToTable("Matches");
                });

            modelBuilder.Entity("StudyBuddy.Models.MatchRequest", b =>
                {
                    b.Property<int>("RequestId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("RequestId"));

                    b.Property<string>("RequestedId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RequesterId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("RequestId");

                    b.ToTable("MatchRequests");
                });

            modelBuilder.Entity("StudyBuddy.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<byte>("Flags")
                        .HasColumnType("smallint");

                    b.Property<string[]>("HobbiesArray")
                        .IsRequired()
                        .HasColumnType("text[]")
                        .HasColumnName("HobbiesArray");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int[]>("UsedIndexesArray")
                        .IsRequired()
                        .HasColumnType("integer[]")
                        .HasColumnName("UsedIndexesArray");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("StudyBuddy.Models.Match", b =>
                {
                    b.HasOne("StudyBuddy.Models.User", "User1")
                        .WithMany()
                        .HasForeignKey("User1Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StudyBuddy.Models.User", "User2")
                        .WithMany()
                        .HasForeignKey("User2Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User1");

                    b.Navigation("User2");
                });

            modelBuilder.Entity("StudyBuddy.Models.User", b =>
                {
                    b.OwnsOne("StudyBuddy.Models.UserTraits", "Traits", b1 =>
                        {
                            b1.Property<string>("UserId")
                                .HasColumnType("text");

                            b1.Property<string>("AvatarPath")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<DateTime>("Birthdate")
                                .HasColumnType("timestamp with time zone");

                            b1.Property<string>("Description")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<double>("Latitude")
                                .HasColumnType("double precision");

                            b1.Property<double>("Longitude")
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
