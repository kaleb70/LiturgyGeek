﻿// <auto-generated />
using LiturgyGeek.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace LiturgyGeek.Data.Migrations
{
    [DbContext(typeof(LiturgyGeekContext))]
    [Migration("20230115165234_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("LiturgyGeek.Data.Calendar.Occasion", b =>
                {
                    b.Property<long>("OccasionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("OccasionId"));

                    b.Property<string>("DefaultName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OccasionCode")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("OccasionId");

                    b.HasIndex("OccasionCode")
                        .IsUnique();

                    b.ToTable("Occasions", "calendar");
                });
#pragma warning restore 612, 618
        }
    }
}
