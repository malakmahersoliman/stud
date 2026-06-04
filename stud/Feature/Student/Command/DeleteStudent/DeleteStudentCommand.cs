using MediatR;

namespace stud.Feature.Student.Command.DeleteStudent;

public class DeleteStudentCommand : IRequest<bool>
{
    public int Id { get; set; }

    public DeleteStudentCommand(int id)
    {
        Id = id;
    }
}
