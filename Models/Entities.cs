namespace MyMauiApp.Models;

public enum UserRole
{
    Teacher,
    Student
}

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public UserRole Role { get; set; }
}

public class Course
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TeacherId { get; set; }          // владелец
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Language { get; set; } = "";
    public int LessonsCount { get; set; }

    public string Icon { get; set; } = "swift_icon.png";
    public string IconBackground { get; set; } = "#FFFFFF";

    public List<MaterialItem> Materials { get; set; } = new();
}

public class MaterialItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CourseId { get; set; }
    public string Title { get; set; } = "";
    public string Type { get; set; } = "";
    public string Description { get; set; } = "";
    public string Url { get; set; } = "";
}


public class Enrollment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CourseId { get; set; }
    public Guid StudentId { get; set; }

    // ВАЖНО: вот это свойство нужно для AppData
    public HashSet<Guid> CompletedMaterialIds { get; set; } = new();
}