using MediatR;
using Microsoft.EntityFrameworkCore;
using stud.Data;
using stud.DTOs.Student;

namespace stud.Feature.Student.Queries.GetAllStudents;

public class GetAllStudentsQueryHandler
    : IRequestHandler<GetAllStudentsQuery, List<StudentResponseDto>>
{
    private readonly AppDbContext _context;

    public GetAllStudentsQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<StudentResponseDto>> Handle(
        GetAllStudentsQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Students
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(s => new StudentResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                Navname = s.Navname,
                DepartmentId = s.DepartmentId
            })
            .ToListAsync(cancellationToken);
    }
}
