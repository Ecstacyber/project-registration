using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ProjectRegistration.Models
{
    public partial class IDENTITYUSERContext : IdentityDbContext<User>
    {
        public IDENTITYUSERContext()
        {
        }

        public IDENTITYUSERContext(DbContextOptions<IDENTITYUSERContext> options)
            : base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.\\SQLExpress;Database=IDENTITYUSER;Trusted_Connection=True;TrustServerCertificate=True");
            }
        }

        public virtual DbSet<Class> Classes { get; set; }

        public virtual DbSet<ClassDetail> ClassDetails { get; set; }

        public virtual DbSet<Course> Courses { get; set; }

        public virtual DbSet<Department> Departments { get; set; }

        public virtual DbSet<Document> Documents { get; set; }

        public virtual DbSet<LecturerStat> LecturerStats { get; set; }

        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<ProductDetail> ProductDetails { get; set; }

        public virtual DbSet<Project> Projects { get; set; }

        public virtual DbSet<ProjectMember> ProjectMembers { get; set; }

        public virtual DbSet<StudentStat> StudentStats { get; set; }

        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Class>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Classes__3214EC07C7FBE7D2");

                entity.Property(e => e.CreatedDateTime).HasColumnType("smalldatetime");
                entity.Property(e => e.Cyear).HasColumnName("CYear");
                entity.Property(e => e.DeletedDateTime).HasColumnType("smalldatetime");

                entity.HasOne(d => d.Course).WithMany(p => p.Classes)
                    .HasForeignKey(d => d.CourseId)
                    .HasConstraintName("FK_Classes_CourseId");
            });

            modelBuilder.Entity<ClassDetail>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__ClassDet__3214EC07265C2FDC");

                entity.Property(e => e.CreatedDateTime).HasColumnType("smalldatetime");
                entity.Property(e => e.DeletedDateTime).HasColumnType("smalldatetime");

                entity.HasOne(d => d.Class).WithMany(p => p.ClassDetails)
                    .HasForeignKey(d => d.ClassId)
                    .HasConstraintName("FK_ClassDetails_ClassId");

                entity.HasOne(d => d.User).WithMany(p => p.ClassDetails)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_ClassDetails_UserId");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Courses__3214EC077A99B303");

                entity.Property(e => e.CreatedDateTime).HasColumnType("smalldatetime");
                entity.Property(e => e.DeletedDateTime).HasColumnType("smalldatetime");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Departme__3214EC07D7964779");

                entity.Property(e => e.CreatedDateTime).HasColumnType("smalldatetime");
                entity.Property(e => e.DeletedDateTime).HasColumnType("smalldatetime");
            });

            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Document__3214EC07AF051F9B");

                entity.Property(e => e.CreatedDateTime).HasColumnType("smalldatetime");
                entity.Property(e => e.DeletedDateTime).HasColumnType("smalldatetime");

                entity.HasOne(d => d.Project).WithMany(p => p.Documents)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_Documents_ProjectId");

                entity.HasOne(d => d.User).WithMany(p => p.Documents)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Documents_UserId");
            });

            modelBuilder.Entity<LecturerStat>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Lecturer__3214EC07073DD833");

                entity.Property(e => e.CreatedDateTime).HasColumnType("smalldatetime");
                entity.Property(e => e.DeletedDateTime).HasColumnType("smalldatetime");
                entity.Property(e => e.Syear).HasColumnName("SYear");

                entity.HasOne(d => d.Lecturer).WithMany(p => p.LecturerStats)
                    .HasForeignKey(d => d.LecturerId)
                    .HasConstraintName("FK_LecturerStats_LecturerId");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Products__3214EC07F35EC2C6");

                entity.Property(e => e.CreatedDateTime).HasColumnType("smalldatetime");
                entity.Property(e => e.DeletedDateTime).HasColumnType("smalldatetime");

                entity.HasOne(d => d.Project).WithMany(p => p.Products)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_Products_ProjectId");

                entity.HasOne(d => d.User).WithMany(p => p.Products)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Products_StudentId");
            });

            modelBuilder.Entity<ProductDetail>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__ProductD__3214EC07802A3FFF");

                entity.Property(e => e.CreatedDateTime).HasColumnType("smalldatetime");
                entity.Property(e => e.DeletedDateTime).HasColumnType("smalldatetime");

                entity.HasOne(d => d.Product).WithMany(p => p.ProductDetails)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_ProductDetails_ProductId");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Projects__3214EC07CFEA4E65");

                entity.Property(e => e.CreatedDateTime).HasColumnType("smalldatetime");
                entity.Property(e => e.DeletedDateTime).HasColumnType("smalldatetime");
                entity.Property(e => e.Pname).HasColumnName("PName");
                entity.Property(e => e.Pyear).HasColumnName("PYear");

                entity.HasOne(d => d.Class).WithMany(p => p.ProjectClasses)
                    .HasForeignKey(d => d.ClassId)
                    .HasConstraintName("FK_Projects_ClassId");

                entity.HasOne(d => d.ClassId2Navigation).WithMany(p => p.ProjectClassId2Navigations)
                    .HasForeignKey(d => d.ClassId2)
                    .HasConstraintName("FK_Projects_ClassId2");

                entity.HasOne(d => d.Department).WithMany(p => p.Projects)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_Projects_DepartmentId");

                entity.HasOne(d => d.GradingLecturer).WithMany(p => p.ProjectGradingLecturers)
                    .HasForeignKey(d => d.GradingLecturerId)
                    .HasConstraintName("FK_Projects_GradingLecturerId");

                entity.HasOne(d => d.GuidingLecturer).WithMany(p => p.ProjectGuidingLecturers)
                    .HasForeignKey(d => d.GuidingLecturerId)
                    .HasConstraintName("FK_Projects_GuidingLecturerId");
            });

            modelBuilder.Entity<ProjectMember>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__ProjectM__3214EC0757140A70");

                entity.Property(e => e.CreatedDateTime).HasColumnType("smalldatetime");
                entity.Property(e => e.DeletedDateTime).HasColumnType("smalldatetime");

                entity.HasOne(d => d.Project).WithMany(p => p.ProjectMembers)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_ProjectMembers_ProjectId");

                entity.HasOne(d => d.Student).WithMany(p => p.ProjectMembers)
                    .HasForeignKey(d => d.StudentId)
                    .HasConstraintName("FK_ProjectMembers_StudentId");
            });

            modelBuilder.Entity<StudentStat>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__StudentS__3214EC076466FA7E");

                entity.Property(e => e.CreatedDateTime).HasColumnType("smalldatetime");
                entity.Property(e => e.DeletedDateTime).HasColumnType("smalldatetime");
                entity.Property(e => e.Syear).HasColumnName("SYear");

                entity.HasOne(d => d.Student).WithMany(p => p.StudentStats)
                    .HasForeignKey(d => d.StudentId)
                    .HasConstraintName("FK_StudentStats_StudentId");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Users__3214EC0714467FCB");

                entity.Property(e => e.CreatedDateTime).HasColumnType("smalldatetime");
                entity.Property(e => e.DateOfBirth).HasColumnType("smalldatetime");
                entity.Property(e => e.DeletedDateTime).HasColumnType("smalldatetime");
                entity.Property(e => e.ImagePath).IsUnicode(false);

                entity.HasOne(d => d.Department).WithMany(p => p.Users)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_Users_DepartmentId");
            });



            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
