using BusinessLogic.Configurations;
using Common.DbModels;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<RequestEntity> Requests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RequestConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }   
}