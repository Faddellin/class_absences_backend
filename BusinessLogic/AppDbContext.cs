using Common.DbModels;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<ReasonEntity> Reasons { get; set; }
    public DbSet<RequestEntity> Requests { get; set; }
}