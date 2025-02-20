using Common.DbModels;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }

}