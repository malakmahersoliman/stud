using MediatR;
using Microsoft.AspNetCore.Mvc;
using stud.DTOs.Department;
using stud.Feature.Department.Command.CreateDepartment;
using stud.Feature.Department.Queries.GetAllDepartments;
using stud.Feature.Department.Queries.GetDepartmentById;

namespace stud.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentController : ControllerBase
{
    private readonly IMediator _mediator;

    public DepartmentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDepartments()
    {
        var result = await _mediator.Send(new GetAllDepartmentsQuery());
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetDepartmentById(int id)
    {
        var result = await _mediator.Send(new GetDepartmentByIdQuery(id));

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateDepartment(DepartmentRequestDto dto)
    {
        var result = await _mediator.Send(new CreateDepartmentCommand(dto));

        return CreatedAtAction(
            nameof(GetDepartmentById),
            new { id = result.Id },
            result);
    }
}
