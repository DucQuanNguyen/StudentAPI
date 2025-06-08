using Microsoft.EntityFrameworkCore;
using StudentAPI.Models;

namespace StudentAPI.Repository
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<SinhVien> students { get; set; }
        public DbSet<LopHoc> classes { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LopHoc>()
            .HasMany(l => l.SinhViens)
            .WithOne(s => s.LopHoc)
            .HasForeignKey(s => s.ClassId);
    }

    }
}
