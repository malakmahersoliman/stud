using MediatR;
using stud.DTOs.Department;

namespace stud.Feature.Department.Queries.GetDepartmentById;

public class GetDepartmentByIdQuery : IRequest<DepartmentResponseDto?>
{
    public int Id { get; set; }

    public GetDepartmentByIdQuery(int id)
    {
        Id = id;
    }
}
