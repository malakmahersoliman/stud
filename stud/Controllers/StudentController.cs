using MediatR;
using Microsoft.AspNetCore.Mvc;
using stud.DTOs.Student;
using stud.Feature.Student.Command.CreateStudent;
using stud.Feature.Student.Queries.GetAllStudents;
using stud.Feature.Student.Queries.GetStudentById;

namespace stud.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentController : ControllerBase
{
    private readonly IMediator _mediator;

    public StudentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllStudents()
    {
        var result = await _mediator.Send(new GetAllStudentsQuery());
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetStudentById(int id)
    {
        var result = await _mediator.Send(new GetStudentByIdQuery(id));

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateStudent(StudentRequestDto dto)
    {
        var result = await _mediator.Send(new CreateStudentCommand(dto));

        return CreatedAtAction(
            nameof(GetStudentById),
            new { id = result.Id },
            result);
    }
}
