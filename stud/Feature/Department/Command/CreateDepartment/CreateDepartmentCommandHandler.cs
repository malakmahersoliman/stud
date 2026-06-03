using MediatR;
using stud.Data;
using stud.DTOs.Department;

namespace stud.Feature.Department.Command.CreateDepartment;

public class CreateDepartmentCommandHandler
    : IRequestHandler<CreateDepartmentCommand, DepartmentResponseDto>
{
    private readonly AppDbContext _context;

    public CreateDepartmentCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DepartmentResponseDto> Handle(
        CreateDepartmentCommand request,
        CancellationToken cancellationToken)
    {
        var department = new Domain.Department
        {
            Name = request.Dto.Name
        };

        _context.Departments.Add(department);
        await _context.SaveChangesAsync(cancellationToken);

        return new DepartmentResponseDto
        {
            Id = department.Id,
            Name = department.Name
        };
    }
}
