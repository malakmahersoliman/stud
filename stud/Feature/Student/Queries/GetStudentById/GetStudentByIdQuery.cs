using MediatR;
using stud.DTOs.Student;

namespace stud.Feature.Student.Queries.GetStudentById;

public class GetStudentByIdQuery : IRequest<StudentResponseDto?>
{
    public int Id { get; set; }

    public GetStudentByIdQuery(int id)
    {
        Id = id;
    }
}
