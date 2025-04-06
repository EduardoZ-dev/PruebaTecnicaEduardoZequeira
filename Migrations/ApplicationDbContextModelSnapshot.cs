﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RouletteTechTest.API.Data.Context;

#nullable disable

namespace RouletteTechTest.API.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("RouletteTechTest.API.Models.Entities.Bet", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("BetValue")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Outcome")
                        .HasColumnType("int");

                    b.Property<decimal>("Prize")
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid>("RoundId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("RoundId");

                    b.HasIndex("UserId");

                    b.ToTable("Bets");
                });

            modelBuilder.Entity("RouletteTechTest.API.Models.Entities.Round", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("RoundNumber")
                        .HasColumnType("int");

                    b.Property<Guid>("SessionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("SessionId");

                    b.ToTable("Rounds");
                });

            modelBuilder.Entity("RouletteTechTest.API.Models.Entities.Session", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("RouletteTechTest.API.Models.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Balance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .UseCollation("SQL_Latin1_General_CP1_CI_AS");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SessionUser", b =>
                {
                    b.Property<Guid>("PlayersId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SessionsId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("PlayersId", "SessionsId");

                    b.HasIndex("SessionsId");

                    b.ToTable("SessionPlayers", (string)null);
                });

            modelBuilder.Entity("RouletteTechTest.API.Models.Entities.Bet", b =>
                {
                    b.HasOne("RouletteTechTest.API.Models.Entities.Round", "Round")
                        .WithMany("Bets")
                        .HasForeignKey("RoundId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RouletteTechTest.API.Models.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Round");

                    b.Navigation("User");
                });

            modelBuilder.Entity("RouletteTechTest.API.Models.Entities.Round", b =>
                {
                    b.HasOne("RouletteTechTest.API.Models.Entities.Session", "Session")
                        .WithMany("Rounds")
                        .HasForeignKey("SessionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("RouletteTechTest.API.Models.DTOs.Common.SpinResultDTO", "Result", b1 =>
                        {
                            b1.Property<Guid>("RoundId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Color")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Parity")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<int>("ResultNumber")
                                .HasColumnType("int");

                            b1.Property<DateTime>("SpinTime")
                                .HasColumnType("datetime2");

                            b1.HasKey("RoundId");

                            b1.ToTable("Rounds");

                            b1.WithOwner()
                                .HasForeignKey("RoundId");
                        });

                    b.Navigation("Result");

                    b.Navigation("Session");
                });

            modelBuilder.Entity("SessionUser", b =>
                {
                    b.HasOne("RouletteTechTest.API.Models.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("PlayersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RouletteTechTest.API.Models.Entities.Session", null)
                        .WithMany()
                        .HasForeignKey("SessionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RouletteTechTest.API.Models.Entities.Round", b =>
                {
                    b.Navigation("Bets");
                });

            modelBuilder.Entity("RouletteTechTest.API.Models.Entities.Session", b =>
                {
                    b.Navigation("Rounds");
                });
#pragma warning restore 612, 618
        }
    }
}
