using MediatR;
using Microsoft.EntityFrameworkCore;
using stud.Data;
using stud.DTOs.Department;

namespace stud.Feature.Department.Command.UpdateDepartment;

public class UpdateDepartmentCommandHandler
    : IRequestHandler<UpdateDepartmentCommand, DepartmentResponseDto?>
{
    private readonly AppDbContext _context;

    public UpdateDepartmentCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DepartmentResponseDto?> Handle(
        UpdateDepartmentCommand request,
        CancellationToken cancellationToken)
    {
        var department = await _context.Departments
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (department == null)
            return null;

        department.Name = request.Dto.Name.Trim();
        await _context.SaveChangesAsync(cancellationToken);

        return new DepartmentResponseDto
        {
            Id = department.Id,
            Name = department.Name
        };
    }
}
