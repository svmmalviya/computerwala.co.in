﻿// <auto-generated />
using System;
using DBService.AppContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DBService.Migrations
{
    [DbContext(typeof(AppDBContext))]
    partial class AppDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("DBService.Models.CWAttendance", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<bool>("Active")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("AttendanceDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("AttendanceTime")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("TIMESTAMP");

                    b.Property<bool>("HasAttended")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.ToTable("CWAttendance");
                });

            modelBuilder.Entity("DBService.Models.CWCalender", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<bool>("Active")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("AttachmentId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("TIMESTAMP");

                    b.Property<DateTime>("EventDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("CWCalenders");
                });

            modelBuilder.Entity("DBService.Models.CWDays", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<bool>("Active")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("AttachmentId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("CWMonthId")
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Day")
                        .HasColumnType("int");

                    b.Property<string>("MonthId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("CWMonthId");

                    b.ToTable("CWDays");
                });

            modelBuilder.Entity("DBService.Models.CWMonth", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<bool>("Active")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("AttachmentId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("TIMESTAMP");

                    b.Property<int>("Month")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("YearId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("YearId");

                    b.ToTable("CWMonths");
                });

            modelBuilder.Entity("DBService.Models.CWSubscriptions", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("TIMESTAMP");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("CWSubscriptions");
                });

            modelBuilder.Entity("DBService.Models.CWVisiteres", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("IpAddress")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("CWVisiters");
                });

            modelBuilder.Entity("DBService.Models.CWYear", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<bool>("Active")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("AttachmentId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("TIMESTAMP");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("CWYears");
                });

            modelBuilder.Entity("DBService.Models.CWDays", b =>
                {
                    b.HasOne("DBService.Models.CWMonth", null)
                        .WithMany("cWDays")
                        .HasForeignKey("CWMonthId");
                });

            modelBuilder.Entity("DBService.Models.CWMonth", b =>
                {
                    b.HasOne("DBService.Models.CWYear", "Year")
                        .WithMany("CWMonths")
                        .HasForeignKey("YearId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Year");
                });

            modelBuilder.Entity("DBService.Models.CWMonth", b =>
                {
                    b.Navigation("cWDays");
                });

            modelBuilder.Entity("DBService.Models.CWYear", b =>
                {
                    b.Navigation("CWMonths");
                });
#pragma warning restore 612, 618
        }
    }
}
