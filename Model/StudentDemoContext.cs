using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace StudentAPI.Model;

public partial class StudentDemoContext : DbContext
{
    public StudentDemoContext()
    {
    }

    public StudentDemoContext(DbContextOptions<StudentDemoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<LopHoc> LopHocs { get; set; }

    public virtual DbSet<SinhVien> SinhViens { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LopHoc>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LopHoc__3214EC27FF6FBF21");

            entity.ToTable("LopHoc");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
        });

        modelBuilder.Entity<SinhVien>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__SinhVien__32C52A79D5F44A13");

            entity.ToTable("SinhVien");

            entity.Property(e => e.StudentId)
                .HasMaxLength(10)
                .HasColumnName("StudentID");
            entity.Property(e => e.BirthDate).HasColumnType("datetime");
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.StudentName).HasMaxLength(30);

            entity.HasOne(d => d.Class).WithMany(p => p.SinhViens)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__SinhVien__ClassI__398D8EEE");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC27B8F60613");

            entity.ToTable("User");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.PassWord).HasMaxLength(50);
            entity.Property(e => e.Role).HasMaxLength(5);
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
