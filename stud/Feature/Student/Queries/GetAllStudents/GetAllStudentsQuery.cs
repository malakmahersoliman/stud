using MediatR;
using stud.DTOs.Student;

namespace stud.Feature.Student.Queries.GetAllStudents;

public class GetAllStudentsQuery : IRequest<List<StudentResponseDto>>
{
}
