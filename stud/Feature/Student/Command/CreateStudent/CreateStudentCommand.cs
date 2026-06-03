using MediatR;
using stud.DTOs.Student;

namespace stud.Feature.Student.Command.CreateStudent;

public class CreateStudentCommand : IRequest<StudentResponseDto>
{
    public StudentRequestDto Dto { get; set; }

    public CreateStudentCommand(StudentRequestDto dto)
    {
        Dto = dto;
    }
}
