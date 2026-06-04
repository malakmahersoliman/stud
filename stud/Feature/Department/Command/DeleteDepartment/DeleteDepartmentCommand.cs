using MediatR;

namespace stud.Feature.Department.Command.DeleteDepartment;

public class DeleteDepartmentCommand : IRequest<bool>
{
    public int Id { get; set; }

    public DeleteDepartmentCommand(int id)
    {
        Id = id;
    }
}
