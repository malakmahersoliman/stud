using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using stud.Domain;

namespace stud.Data.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Navname)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(s => s.Department)
            .WithMany(d => d.Students)
            .HasForeignKey(s => s.DepartmentId);
    }
}
