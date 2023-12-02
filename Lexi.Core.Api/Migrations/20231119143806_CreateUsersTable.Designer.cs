﻿// <auto-generated />
using System;
using Lexi.Core.Api.Brokers.Storages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Lexi.Core.Api.Migrations
{
    [DbContext(typeof(StorageBroker))]
    [Migration("20231119143806_CreateUsersTable")]
    partial class CreateUsersTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("Lexi.Core.Api.Models.Foundations.Speeches.Speech", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Sentence")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Speeches");
                });

            modelBuilder.Entity("Lexi.Core.Api.Models.Foundations.Users.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Lexi.Core.Api.Models.Foundations.Speeches.Speech", b =>
                {
                    b.HasOne("Lexi.Core.Api.Models.Foundations.Users.User", null)
                        .WithMany("Speeches")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Lexi.Core.Api.Models.Foundations.Users.User", b =>
                {
                    b.Navigation("Speeches");
                });
#pragma warning restore 612, 618
        }
    }
}