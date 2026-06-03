using MediatR;
using Microsoft.EntityFrameworkCore;
using stud.Data;
using stud.DTOs.Student;

namespace stud.Feature.Student.Queries.GetStudentById;

public class GetStudentByIdQueryHandler : IRequestHandler<GetStudentByIdQuery, StudentResponseDto?>
{
    private readonly AppDbContext _context;

    public GetStudentByIdQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<StudentResponseDto?> Handle(
        GetStudentByIdQuery request,
        CancellationToken cancellationToken)
    {
        var student = await _context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (student == null)
            return null;

        return new StudentResponseDto
        {
            Id = student.Id,
            Name = student.Name,
            Navname = student.Navname,
            DepartmentId = student.DepartmentId
        };
    }
}
