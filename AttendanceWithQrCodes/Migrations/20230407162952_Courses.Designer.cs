﻿// <auto-generated />
using System;
using AttendanceWithQrCodes.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AttendanceWithQrCodes.Migrations
{
    [DbContext(typeof(Context))]
    [Migration("20230407162952_Courses")]
    partial class Courses
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AttendanceWithQrCodes.Models.Course", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("AssistantId")
                        .HasColumnType("int");

                    b.Property<int?>("LecturesNumForAssistent")
                        .HasColumnType("int");

                    b.Property<int>("LecturesNumForProfessor")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ProfessorId")
                        .HasColumnType("int");

                    b.Property<int?>("StudyLanguageId")
                        .HasColumnType("int");

                    b.Property<int>("TotalTakenLectures")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AssistantId");

                    b.HasIndex("ProfessorId");

                    b.HasIndex("StudyLanguageId");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("AttendanceWithQrCodes.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("AttendanceWithQrCodes.Models.StudentInformation", b =>
                {
                    b.Property<int>("Index")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Index"));

                    b.Property<string>("MacAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("StudyLanguageId")
                        .HasColumnType("int");

                    b.Property<int?>("StudyProfileId")
                        .HasColumnType("int");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Index");

                    b.HasIndex("StudyLanguageId");

                    b.HasIndex("StudyProfileId");

                    b.HasIndex("UserId");

                    b.ToTable("StudentInformations");
                });

            modelBuilder.Entity("AttendanceWithQrCodes.Models.StudyLanguage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("StudyLanguages");
                });

            modelBuilder.Entity("AttendanceWithQrCodes.Models.StudyProfile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("StudyProfiles");
                });

            modelBuilder.Entity("AttendanceWithQrCodes.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("AttendanceWithQrCodes.Models.Course", b =>
                {
                    b.HasOne("AttendanceWithQrCodes.Models.User", "Assistant")
                        .WithMany()
                        .HasForeignKey("AssistantId");

                    b.HasOne("AttendanceWithQrCodes.Models.User", "Professor")
                        .WithMany()
                        .HasForeignKey("ProfessorId");

                    b.HasOne("AttendanceWithQrCodes.Models.StudyLanguage", "StudyLanguage")
                        .WithMany()
                        .HasForeignKey("StudyLanguageId");

                    b.Navigation("Assistant");

                    b.Navigation("Professor");

                    b.Navigation("StudyLanguage");
                });

            modelBuilder.Entity("AttendanceWithQrCodes.Models.StudentInformation", b =>
                {
                    b.HasOne("AttendanceWithQrCodes.Models.StudyLanguage", "StudyLanguage")
                        .WithMany()
                        .HasForeignKey("StudyLanguageId");

                    b.HasOne("AttendanceWithQrCodes.Models.StudyProfile", "StudyProfile")
                        .WithMany()
                        .HasForeignKey("StudyProfileId");

                    b.HasOne("AttendanceWithQrCodes.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("StudyLanguage");

                    b.Navigation("StudyProfile");

                    b.Navigation("User");
                });

            modelBuilder.Entity("AttendanceWithQrCodes.Models.User", b =>
                {
                    b.HasOne("AttendanceWithQrCodes.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId");

                    b.Navigation("Role");
                });
#pragma warning restore 612, 618
        }
    }
}
