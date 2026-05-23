using HR_Management.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR_Management.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Department_Tbl");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Id).ValueGeneratedOnAdd();
            entity.Property(d => d.DepartmentName).HasMaxLength(200).IsRequired();
            entity.Property(d => d.DepartmentCode).HasMaxLength(50).IsRequired();
            entity.Property(d => d.Location).HasMaxLength(200).IsRequired();
            entity.Property(d => d.NumberOfPersonals).HasDefaultValue(0);

            entity.HasIndex(d => d.DepartmentCode).IsUnique();

            entity.HasMany(d => d.Employees)
                .WithOne(e => e.Department)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("Employee_Tbl");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.EmployeeName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.EmployeeCode).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Rank).HasMaxLength(100).IsRequired();

            entity.HasIndex(e => e.EmployeeCode).IsUnique();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User_Tbl");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).ValueGeneratedOnAdd();
            entity.Property(u => u.Email).HasMaxLength(256).IsRequired();
            entity.Property(u => u.PasswordHash).HasMaxLength(512).IsRequired();
            entity.Property(u => u.FullName).HasMaxLength(200).IsRequired();

            entity.HasIndex(u => u.Email).IsUnique();
        });
    }
}
