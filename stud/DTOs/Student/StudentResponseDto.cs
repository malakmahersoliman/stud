namespace stud.DTOs.Student;

public class StudentResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Navname { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
}
