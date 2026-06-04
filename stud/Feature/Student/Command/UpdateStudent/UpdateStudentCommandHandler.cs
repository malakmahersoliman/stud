using MediatR;
using Microsoft.EntityFrameworkCore;
using stud.Data;
using stud.DTOs.Student;

namespace stud.Feature.Student.Command.UpdateStudent;

public class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentCommand, StudentResponseDto?>
{
    private readonly AppDbContext _context;

    public UpdateStudentCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<StudentResponseDto?> Handle(
        UpdateStudentCommand request,
        CancellationToken cancellationToken)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (student == null)
            return null;

        var departmentExists = await _context.Departments
            .AnyAsync(d => d.Id == request.Dto.DepartmentId, cancellationToken);

        if (!departmentExists)
            throw new KeyNotFoundException($"Department with id {request.Dto.DepartmentId} was not found.");

        student.Name = request.Dto.Name.Trim();
        student.Navname = request.Dto.Navname.Trim();
        student.DepartmentId = request.Dto.DepartmentId;

        await _context.SaveChangesAsync(cancellationToken);

        return new StudentResponseDto
        {
            Id = student.Id,
            Name = student.Name,
            Navname = student.Navname,
            DepartmentId = student.DepartmentId
        };
    }
}
