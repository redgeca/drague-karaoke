﻿// <auto-generated />
using DBSetupAndDataSeed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace DBSetupAndDataSeed.Migrations
{
    [DbContext(typeof(SongDBContext))]
    partial class SongDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("KaraokeCoreObjects.Artist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Artists");
                });

            modelBuilder.Entity("KaraokeCoreObjects.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("KaraokeCoreObjects.KaraokeState", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("KaraokeStarted");

                    b.HasKey("id");

                    b.ToTable("KaraokeState");
                });

            modelBuilder.Entity("KaraokeCoreObjects.Playlist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("IsDone");

                    b.Property<int>("Order");

                    b.Property<int>("RequestId");

                    b.HasKey("Id");

                    b.HasIndex("RequestId");

                    b.ToTable("Playlists");
                });

            modelBuilder.Entity("KaraokeCoreObjects.Request", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Notes");

                    b.Property<DateTime>("RequestTime");

                    b.Property<string>("SingerName");

                    b.Property<int>("SongId");

                    b.HasKey("Id");

                    b.HasIndex("SongId");

                    b.ToTable("Requests");
                });

            modelBuilder.Entity("KaraokeCoreObjects.Song", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ArtistId");

                    b.Property<int>("CategoryId");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("ArtistId");

                    b.HasIndex("CategoryId");

                    b.ToTable("Songs");
                });

            modelBuilder.Entity("KaraokeCoreObjects.Playlist", b =>
                {
                    b.HasOne("KaraokeCoreObjects.Request", "Request")
                        .WithMany()
                        .HasForeignKey("RequestId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("KaraokeCoreObjects.Request", b =>
                {
                    b.HasOne("KaraokeCoreObjects.Song", "Song")
                        .WithMany()
                        .HasForeignKey("SongId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("KaraokeCoreObjects.Song", b =>
                {
                    b.HasOne("KaraokeCoreObjects.Artist", "Artist")
                        .WithMany("Songs")
                        .HasForeignKey("ArtistId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("KaraokeCoreObjects.Category", "Category")
                        .WithMany("Songs")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
