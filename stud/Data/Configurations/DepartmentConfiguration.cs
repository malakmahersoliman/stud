using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using stud.Domain;

namespace stud.Data.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasMany(d => d.Students)
            .WithOne(s => s.Department)
            .HasForeignKey(s => s.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
