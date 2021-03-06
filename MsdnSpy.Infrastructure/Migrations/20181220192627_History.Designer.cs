﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MsdnSpy.Infrastructure;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MsdnSpy.Infrastructure.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20181220192627_History")]
    partial class History
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.0-preview3-35497")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("MsdnSpy.Core.HistoryEntry", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("ChatId");

                    b.Property<DateTime>("DateTime");

                    b.Property<string>("RequestQuery");

                    b.HasKey("Id");

                    b.ToTable("History");
                });

            modelBuilder.Entity("MsdnSpy.Core.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Preferences");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
