using MediatR;
using stud.DTOs.Student;

namespace stud.Feature.Student.Command.UpdateStudent;

public class UpdateStudentCommand : IRequest<StudentResponseDto?>
{
    public int Id { get; set; }
    public StudentRequestDto Dto { get; set; }

    public UpdateStudentCommand(int id, StudentRequestDto dto)
    {
        Id = id;
        Dto = dto;
    }
}
