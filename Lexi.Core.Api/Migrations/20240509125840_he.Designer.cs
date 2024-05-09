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
    [Migration("20240509125840_he")]
    partial class he
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("Lexi.Core.Api.Models.Foundations.Feedbacks.Feedback", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Accuracy")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Complenteness")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Fluency")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Pronunciation")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Prosody")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("SpeechId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("SpeechId")
                        .IsUnique();

                    b.ToTable("Feedbacks");
                });

            modelBuilder.Entity("Lexi.Core.Api.Models.Foundations.Questions.Question", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Content")
                        .HasColumnType("TEXT");

                    b.Property<int>("Number")
                        .HasColumnType("INTEGER");

                    b.Property<string>("QuestionType")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("Lexi.Core.Api.Models.Foundations.Reviews.Review", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<long>("TelegramId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TelegramUserName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Reviews");
                });

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

                    b.Property<string>("ImprovedSpeech")
                        .HasColumnType("TEXT");

                    b.Property<string>("Level")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<decimal?>("Overall")
                        .HasColumnType("TEXT");

                    b.Property<int>("State")
                        .HasColumnType("INTEGER");

                    b.Property<long>("TelegramId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TelegramName")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Lexi.Core.Api.Models.Foundations.Feedbacks.Feedback", b =>
                {
                    b.HasOne("Lexi.Core.Api.Models.Foundations.Speeches.Speech", "Speech")
                        .WithOne("Feedbacks")
                        .HasForeignKey("Lexi.Core.Api.Models.Foundations.Feedbacks.Feedback", "SpeechId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Speech");
                });

            modelBuilder.Entity("Lexi.Core.Api.Models.Foundations.Speeches.Speech", b =>
                {
                    b.HasOne("Lexi.Core.Api.Models.Foundations.Users.User", "User")
                        .WithMany("Speeches")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Lexi.Core.Api.Models.Foundations.Speeches.Speech", b =>
                {
                    b.Navigation("Feedbacks");
                });

            modelBuilder.Entity("Lexi.Core.Api.Models.Foundations.Users.User", b =>
                {
                    b.Navigation("Speeches");
                });
#pragma warning restore 612, 618
        }
    }
}
