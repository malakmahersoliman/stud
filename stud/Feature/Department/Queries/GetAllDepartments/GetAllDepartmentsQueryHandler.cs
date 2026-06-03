using MediatR;
using Microsoft.EntityFrameworkCore;
using stud.Data;
using stud.DTOs.Department;

namespace stud.Feature.Department.Queries.GetAllDepartments;

public class GetAllDepartmentsQueryHandler
    : IRequestHandler<GetAllDepartmentsQuery, List<DepartmentResponseDto>>
{
    private readonly AppDbContext _context;

    public GetAllDepartmentsQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DepartmentResponseDto>> Handle(
        GetAllDepartmentsQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Departments
            .AsNoTracking()
            .OrderBy(d => d.Name)
            .Select(d => new DepartmentResponseDto
            {
                Id = d.Id,
                Name = d.Name
            })
            .ToListAsync(cancellationToken);
    }
}
