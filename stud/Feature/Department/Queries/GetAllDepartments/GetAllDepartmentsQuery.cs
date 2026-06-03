using MediatR;
using stud.DTOs.Department;

namespace stud.Feature.Department.Queries.GetAllDepartments;

public class GetAllDepartmentsQuery : IRequest<List<DepartmentResponseDto>>
{
}
