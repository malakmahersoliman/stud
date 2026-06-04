using MediatR;
using Microsoft.EntityFrameworkCore;
using stud.Data;

namespace stud.Feature.Student.Command.DeleteStudent;

public class DeleteStudentCommandHandler : IRequestHandler<DeleteStudentCommand, bool>
{
    private readonly AppDbContext _context;

    public DeleteStudentCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(
        DeleteStudentCommand request,
        CancellationToken cancellationToken)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (student == null)
            return false;

        _context.Students.Remove(student);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
