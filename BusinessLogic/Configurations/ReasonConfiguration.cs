using Common.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessLogic.Configurations;

public class ReasonConfiguration : IEntityTypeConfiguration<ReasonEntity>
{
    public void Configure(EntityTypeBuilder<ReasonEntity> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.ReasonType).HasConversion<string>();
    }
}