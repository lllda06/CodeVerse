using System.Text.Json;
using MyMauiApp.Models;

namespace MyMauiApp.Services;

public class AppData
{
    private static readonly string FilePath =
        Path.Combine(FileSystem.AppDataDirectory, "appdata.json");

    public static AppData Instance { get; } = new AppData();

    // ------------------------------
    // ХРАНЕНИЕ ДАННЫХ В ПАМЯТИ
    // ------------------------------
    public List<User> Users { get; set; } = new();
    public List<Course> Courses { get; set; } = new();
    public List<MaterialItem> Materials { get; set; } = new();
    public List<Enrollment> Enrollments { get; set; } = new();

    public User? CurrentUser { get; set; }

    private AppData() { }

    // ------------------------------
    // ЗАГРУЗКА / СОХРАНЕНИЕ
    // ------------------------------
    public async Task LoadAsync()
    {
        if (!File.Exists(FilePath))
        {
            await SaveAsync();
            return;
        }

        try
        {
            var json = File.ReadAllText(FilePath);
            var loaded = JsonSerializer.Deserialize<AppData>(json);

            if (loaded != null)
            {
                Users = loaded.Users;
                Courses = loaded.Courses;
                Materials = loaded.Materials;
                Enrollments = loaded.Enrollments;
            }
        }
        catch
        {
            // если файл битый — создаём новый
            await SaveAsync();
        }
    }

    public async Task SaveAsync()
    {
        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(FilePath, json);
        await Task.CompletedTask;
    }

    // ------------------------------
    // РЕГИСТРАЦИЯ
    // ------------------------------
    public (bool ok, string error) Register(string name, string email, string password, UserRole role)
    {
        if (Users.Any(u => u.Email == email))
            return (false, "Użytkownik z takim emailem już istnieje.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            Password = password,
            Role = role
        };

        Users.Add(user);
        _ = SaveAsync();

        CurrentUser = user;

        return (true, "");
    }

    // ------------------------------
    // ЛОГИН
    // ------------------------------
    public (bool ok, string error) Login(string email, string password, UserRole role)
    {
        var user = Users.FirstOrDefault(u => u.Email == email && u.Role == role);

        if (user == null)
            return (false, "Nie znaleziono użytkownika.");

        if (user.Password != password)
            return (false, "Niepoprawne hasło.");

        CurrentUser = user;
        return (true, "");
    }

    // ------------------------------
    // КУРСЫ УЧИТЕЛЯ
    // ------------------------------
    public List<Course> GetCoursesForCurrentTeacher()
    {
        if (CurrentUser == null) return new();

        return Courses.Where(c => c.TeacherId == CurrentUser.Id).ToList();
    }

    public Course AddCourseForCurrentTeacher(string title, string desc, string lang, int lessons)
    {
        if (CurrentUser == null || CurrentUser.Role != UserRole.Teacher)
            throw new Exception("Brak uprawnień.");

        var course = new Course
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = desc,
            Language = lang,
            LessonsCount = lessons,
            TeacherId = CurrentUser.Id
        };

        Courses.Add(course);
        _ = SaveAsync();

        return course;
    }

    // ------------------------------
    // СТУДЕНТЫ: ЗАПИСЬ НА КУРС
    // ------------------------------
    public List<Course> GetCoursesForCurrentStudent()
    {
        if (CurrentUser == null) return new();

        var ids = Enrollments
            .Where(e => e.StudentId == CurrentUser.Id)
            .Select(e => e.CourseId)
            .ToList();

        return Courses.Where(c => ids.Contains(c.Id)).ToList();
    }

    public Enrollment EnrollStudent(Guid courseId, Guid studentId)
    {
        var e = new Enrollment
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            StudentId = studentId
        };

        Enrollments.Add(e);
        _ = SaveAsync();

        return e;
    }

    public Enrollment? GetEnrollment(Guid courseId, Guid studentId)
    {
        return Enrollments
            .FirstOrDefault(e => e.CourseId == courseId && e.StudentId == studentId);
    }

    // ------------------------------
    // МАТЕРИАЛЫ
    // ------------------------------
    public void AddMaterialToCourse(Guid courseId, string title, string type, string desc, string url)
    {
        var material = new MaterialItem
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            Title = title,
            Type = type,
            Description = desc,
            Url = url
        };

        Materials.Add(material);

        // кинуть материал внутрь курса
        var course = Courses.First(c => c.Id == courseId);
        course.Materials.Add(material);

        _ = SaveAsync();
    }

    // ------------------------------
    // ПРОГРЕСС
    // ------------------------------
    public void ToggleMaterialCompleted(Guid courseId, Guid materialId, Guid studentId)
    {
        var enrollment = GetEnrollment(courseId, studentId);
        if (enrollment == null) return;

        if (enrollment.CompletedMaterialIds.Contains(materialId))
            enrollment.CompletedMaterialIds.Remove(materialId);
        else
            enrollment.CompletedMaterialIds.Add(materialId);

        _ = SaveAsync();
    }

    public double GetProgressPercent(Guid courseId, Guid studentId)
    {
        var course = Courses.First(c => c.Id == courseId);
        var enrollment = GetEnrollment(courseId, studentId);

        if (enrollment == null) return 0;
        if (course.Materials.Count == 0) return 0;

        double completed = enrollment.CompletedMaterialIds.Count;
        double total = course.Materials.Count;

        return (completed / total) * 100.0;
    }

    // ------------------------------
    // ПРОГРЕСС ВСЕХ СТУДЕНТОВ КУРСА
    // ------------------------------
    public List<(User Student, double Percent)> GetStudentsProgressForCourse(Guid courseId)
    {
        var result = new List<(User, double)>();

        var course = Courses.FirstOrDefault(c => c.Id == courseId);
        if (course == null) return result;

        int total = course.Materials.Count;

        var enrolls = Enrollments.Where(e => e.CourseId == courseId);

        foreach (var e in enrolls)
        {
            var student = Users.First(u => u.Id == e.StudentId);

            double percent = total == 0
                ? 0
                : (double)e.CompletedMaterialIds.Count / total * 100.0;

            result.Add((student, percent));
        }

        return result;
    }
}