using MediatR;
using stud.DTOs.Department;

namespace stud.Feature.Department.Command.UpdateDepartment;

public class UpdateDepartmentCommand : IRequest<DepartmentResponseDto?>
{
    public int Id { get; set; }
    public DepartmentRequestDto Dto { get; set; }

    public UpdateDepartmentCommand(int id, DepartmentRequestDto dto)
    {
        Id = id;
        Dto = dto;
    }
}
