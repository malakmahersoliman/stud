using stud.DTOs.Student;

namespace stud.DTOs.Department;

public class DepartmentResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<StudentResponseDto> Students { get; set; } = new();
}
