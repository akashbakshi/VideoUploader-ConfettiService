﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using VideoUploader_ConfettiService.Data;

namespace VideoUploader_ConfettiService.Migrations
{
    [DbContext(typeof(VideoDbContext))]
    partial class VideoDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("VideoUploader_ConfettiService.Video", b =>
                {
                    b.Property<Guid>("VideoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("BucketURL")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Dislikes")
                        .HasColumnType("integer");

                    b.Property<int>("Likes")
                        .HasColumnType("integer");

                    b.Property<string>("LinkURL")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("PostedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Views")
                        .HasColumnType("integer");

                    b.HasKey("VideoId");

                    b.ToTable("Videos");
                });
#pragma warning restore 612, 618
        }
    }
}