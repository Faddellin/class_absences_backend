using Common.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessLogic.Configurations;

public class RequestConfiguration : IEntityTypeConfiguration<RequestEntity>
{
    public void Configure(EntityTypeBuilder<RequestEntity> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Status).HasConversion<string>();
    }
}