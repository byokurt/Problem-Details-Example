using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProblemDetailsExample.Data.Entities;

namespace ProblemDetailsExample.Data.Mapping;

public class UserMap : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(p => p.Id);
        builder.HasQueryFilter(p => !p.IsDeleted);

        builder.Property(p => p.Title).HasColumnType("nvarchar(256)");
    }
}