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
    [Migration("20230407164449_CousesLanguages")]
    partial class CousesLanguages
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

                    b.Property<int>("TotalTakenLectures")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AssistantId");

                    b.HasIndex("ProfessorId");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("AttendanceWithQrCodes.Models.CourseLanguage", b =>
                {
                    b.Property<int?>("CourseId")
                        .HasColumnType("int");

                    b.Property<int?>("StudyLanguageId")
                        .HasColumnType("int");

                    b.HasIndex("CourseId");

                    b.HasIndex("StudyLanguageId");

                    b.ToTable("CoursesLanguages");
                });

            modelBuilder.Entity("AttendanceWithQrCodes.Models.CourseStudyProfile", b =>
                {
                    b.Property<int?>("CourseId")
                        .HasColumnType("int");

                    b.Property<int?>("StudyProfileId")
                        .HasColumnType("int");

                    b.HasIndex("CourseId");

                    b.HasIndex("StudyProfileId");

                    b.ToTable("CoursesStudyProfiles");
                });

            modelBuilder.Entity("AttendanceWithQrCodes.Models.Lecture", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CourseId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("LecturerId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("QrCodeId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.HasIndex("LecturerId");

                    b.HasIndex("QrCodeId");

                    b.ToTable("Lectures");
                });

            modelBuilder.Entity("AttendanceWithQrCodes.Models.QrCode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("ImageName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("QrCodes");
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

            modelBuilder.Entity("AttendanceWithQrCodes.Models.StudentAttendance", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LectureId")
                        .HasColumnType("int");

                    b.Property<bool>("Present")
                        .HasColumnType("bit");

                    b.Property<int?>("StudentIndex")
                        .HasColumnType("int");

                    b.Property<int?>("StudentInformationIndex")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("LectureId");

                    b.HasIndex("StudentInformationIndex");

                    b.ToTable("StudentAttendances");
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

                    b.Navigation("Assistant");

                    b.Navigation("Professor");
                });

            modelBuilder.Entity("AttendanceWithQrCodes.Models.CourseLanguage", b =>
                {
                    b.HasOne("AttendanceWithQrCodes.Models.Course", "Course")
                        .WithMany()
                        .HasForeignKey("CourseId");

                    b.HasOne("AttendanceWithQrCodes.Models.StudyLanguage", "StudyLanguage")
                        .WithMany()
                        .HasForeignKey("StudyLanguageId");

                    b.Navigation("Course");

                    b.Navigation("StudyLanguage");
                });

            modelBuilder.Entity("AttendanceWithQrCodes.Models.CourseStudyProfile", b =>
                {
                    b.HasOne("AttendanceWithQrCodes.Models.Course", "Course")
                        .WithMany()
                        .HasForeignKey("CourseId");

                    b.HasOne("AttendanceWithQrCodes.Models.StudyProfile", "StudyProfile")
                        .WithMany()
                        .HasForeignKey("StudyProfileId");

                    b.Navigation("Course");

                    b.Navigation("StudyProfile");
                });

            modelBuilder.Entity("AttendanceWithQrCodes.Models.Lecture", b =>
                {
                    b.HasOne("AttendanceWithQrCodes.Models.Course", "Course")
                        .WithMany()
                        .HasForeignKey("CourseId");

                    b.HasOne("AttendanceWithQrCodes.Models.User", "Lecturer")
                        .WithMany()
                        .HasForeignKey("LecturerId");

                    b.HasOne("AttendanceWithQrCodes.Models.QrCode", "QrCode")
                        .WithMany()
                        .HasForeignKey("QrCodeId");

                    b.Navigation("Course");

                    b.Navigation("Lecturer");

                    b.Navigation("QrCode");
                });

            modelBuilder.Entity("AttendanceWithQrCodes.Models.StudentAttendance", b =>
                {
                    b.HasOne("AttendanceWithQrCodes.Models.Lecture", "Lecture")
                        .WithMany()
                        .HasForeignKey("LectureId");

                    b.HasOne("AttendanceWithQrCodes.Models.StudentInformation", "StudentInformation")
                        .WithMany()
                        .HasForeignKey("StudentInformationIndex");

                    b.Navigation("Lecture");

                    b.Navigation("StudentInformation");
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
