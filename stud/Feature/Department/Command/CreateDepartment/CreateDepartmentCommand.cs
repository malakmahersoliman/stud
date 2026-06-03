using MediatR;
using stud.DTOs.Department;

namespace stud.Feature.Department.Command.CreateDepartment;

public class CreateDepartmentCommand : IRequest<DepartmentResponseDto>
{
    public DepartmentRequestDto Dto { get; set; }

    public CreateDepartmentCommand(DepartmentRequestDto dto)
    {
        Dto = dto;
    }
}
