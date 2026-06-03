using MediatR;
using Microsoft.EntityFrameworkCore;
using stud.Data;
using stud.DTOs.Student;

namespace stud.Feature.Student.Command.CreateStudent;

public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, StudentResponseDto>
{
    private readonly AppDbContext _context;

    public CreateStudentCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<StudentResponseDto> Handle(
        CreateStudentCommand request,
        CancellationToken cancellationToken)
    {
        var departmentExists = await _context.Departments
            .AnyAsync(d => d.Id == request.Dto.DepartmentId, cancellationToken);

        if (!departmentExists)
            throw new KeyNotFoundException($"Department with id {request.Dto.DepartmentId} was not found.");

        var student = new Domain.Student
        {
            Name = request.Dto.Name,
            Navname = request.Dto.Navname,
            DepartmentId = request.Dto.DepartmentId
        };

        _context.Students.Add(student);
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
