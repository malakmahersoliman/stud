using MediatR;
using Microsoft.EntityFrameworkCore;
using stud.Data;
using stud.DTOs.Department;
using stud.DTOs.Student;

namespace stud.Feature.Department.Queries.GetDepartmentById;

public class GetDepartmentByIdQueryHandler
    : IRequestHandler<GetDepartmentByIdQuery, DepartmentResponseDto?>
{
    private readonly AppDbContext _context;

    public GetDepartmentByIdQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DepartmentResponseDto?> Handle(
        GetDepartmentByIdQuery request,
        CancellationToken cancellationToken)
    {
        var department = await _context.Departments
            .AsNoTracking()
            .Include(d => d.Students)
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (department == null)
            return null;

        return new DepartmentResponseDto
        {
            Id = department.Id,
            Name = department.Name,
            Students = department.Students
                .Select(s => new StudentResponseDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Navname = s.Navname,
                    DepartmentId = s.DepartmentId
                })
                .ToList()
        };
    }
}
