using Microsoft.EntityFrameworkCore;
using TenantService.Domain.Entities;
using TenantService.Domain.Enums;

namespace TenantService.Infrastructure;


public class TenantDbContext : DbContext
{
     public TenantDbContext(DbContextOptions<TenantDbContext> options) : base(options) { }

     public DbSet<User> Users => Set<User>();

     protected override void OnModelCreating(ModelBuilder modelBuilder)
     {
          modelBuilder.Entity<User>()
               .ToTable("user", "public") 
               .HasKey(u => u.Id).HasName("user_pkey"); 

          modelBuilder.Entity<User>()
               .Property(u => u.Id)
               .HasColumnName("id")
               .HasColumnType("uuid")
               .IsRequired();

          modelBuilder.Entity<User>()
               .Property(u => u.Username)
               .HasColumnName("username") 
               .HasColumnType("varchar(32)") 
               .IsRequired();

          modelBuilder.Entity<User>()
               .Property(u => u.FullName)
               .HasColumnName("fullName")
               .HasColumnType("varchar(256)")
               .IsRequired(); 

          modelBuilder.Entity<User>()
               .Property(u => u.Email)
               .HasColumnName("email")
               .HasColumnType("varchar(256)")
               .IsRequired();

          modelBuilder.Entity<User>().Property(u => u.PasswordHash)
               .HasColumnName("passwordHash")
               .HasColumnType("varchar(256)");

          modelBuilder.Entity<User>()
               .Property(u => u.Admin)
               .HasColumnName("admin")
               .HasColumnType("boolean")
               .IsRequired().HasDefaultValue(false);

          modelBuilder.Entity<User>()
               .Property(u => u.Status)
               .HasColumnName("status")
               .HasColumnType("smallint")
               .IsRequired().HasDefaultValue(UserStatus.Active);
     }

}
