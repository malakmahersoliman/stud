namespace stud.Domain;

public class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Navname { get; set; } = string.Empty;
    public int DepartmentId { get; set; }

    public Department Department { get; set; } = null!;
}
