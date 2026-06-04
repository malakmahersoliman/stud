using MediatR;
using Microsoft.EntityFrameworkCore;
using stud.Data;

namespace stud.Feature.Department.Command.DeleteDepartment;

public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, bool>
{
    private readonly AppDbContext _context;

    public DeleteDepartmentCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(
        DeleteDepartmentCommand request,
        CancellationToken cancellationToken)
    {
        var department = await _context.Departments
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (department == null)
            return false;

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
