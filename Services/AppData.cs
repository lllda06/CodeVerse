using System.Text.Json;
using System.Linq;
using MyMauiApp.Models;

namespace MyMauiApp.Services;

public class AppData
{
    // --------------------------
    // Singleton
    // --------------------------
    private static AppData? _instance;
    public static AppData Instance => _instance ??= new AppData();

    private AppData() { }

    private const string FileName = "appdata.json";

    // --------------------------
    // State
    // --------------------------
    public List<User> Users { get; set; } = new();
    public List<Course> Courses { get; set; } = new();
    public List<Enrollment> Enrollments { get; set; } = new();

    public User? CurrentUser { get; private set; }

    // Внутренний класс для сохранения/загрузки
    private class PersistedState
    {
        public List<User>? Users { get; set; }
        public List<Course>? Courses { get; set; }
        public List<Enrollment>? Enrollments { get; set; }
    }

    // --------------------------
    // SAVE / LOAD
    // --------------------------

    public async Task SaveAsync()
    {
        var state = new PersistedState
        {
            Users = Users,
            Courses = Courses,
            Enrollments = Enrollments
        };

        var json = JsonSerializer.Serialize(
            state,
            new JsonSerializerOptions { WriteIndented = true });

        var file = Path.Combine(FileSystem.Current.AppDataDirectory, FileName);
        await File.WriteAllTextAsync(file, json);
    }

    public async Task LoadAsync()
    {
        try
        {
            var file = Path.Combine(FileSystem.Current.AppDataDirectory, FileName);
            if (!File.Exists(file))
                return;

            var json = await File.ReadAllTextAsync(file);
            var state = JsonSerializer.Deserialize<PersistedState>(json);

            if (state != null)
            {
                Users       = state.Users       ?? new();
                Courses     = state.Courses     ?? new();
                Enrollments = state.Enrollments ?? new();

                // Проставляем иконки, если их нет
                foreach (var c in Courses)
                {
                    if (string.IsNullOrWhiteSpace(c.Icon))
                        c.Icon = GetIconForLanguage(c.Language);
                }
            }
        }
        catch
        {
            // битый JSON — просто начинаем с чистого состояния
        }
    }

    // --------------------------
    // Helpers
    // --------------------------

    private string GetIconForLanguage(string? language)
    {
        var lang = (language ?? "").Trim().ToLowerInvariant();

        return lang switch
        {
            "python"                      => "python_icon.png",
            "swift"                       => "swift_icon.png",
            "java"                        => "java_icon.png",
            "c#" or "csharp"              => "csharp_icon.png",
            "c++" or "cpp"                => "c_icon.png",
            "javascript" or "js"          => "js_icon.webp",
            "html" or "html/css"          => "html_icon.png",
            "php"                         => "php_icon.png",
            _                             => "zsm_logo.png"   // дефолтная — логотип школы
        };
    }

    // --------------------------
    // REGISTER / LOGIN
    // --------------------------

    public (bool Ok, string Error) Register(string name, string email, string password, UserRole role)
    {
        if (Users.Any(u => u.Email == email))
            return (false, "Email już istnieje");

        var user = new User
        {
            Name         = name,
            Email        = email,
            PasswordHash = password,
            Role         = role
        };

        Users.Add(user);
        CurrentUser = user;

        _ = SaveAsync();
        return (true, "");
    }

    public (bool Ok, string Error) Login(string email, string password, UserRole role)
    {
        var user = Users.FirstOrDefault(u =>
            u.Email == email &&
            u.PasswordHash == password);

        if (user == null)
            return (false, "Nieprawidłowy email lub hasło");

        if (user.Role != role)
            return (false, "To konto nie jest typu: " + role);

        CurrentUser = user;
        return (true, "");
    }

    public void Logout()
    {
        CurrentUser = null;
    }

    // --------------------------
    // COURSES
    // --------------------------

    public Course AddCourse(string title, string description, string language)
    {
        if (CurrentUser == null || CurrentUser.Role != UserRole.Teacher)
            throw new InvalidOperationException("No logged-in teacher");

        var course = new Course
        {
            Title          = title,
            Description    = description,
            Language       = language,
            LessonsCount   = 0,
            OwnerTeacherId = CurrentUser.Id,
            Icon           = GetIconForLanguage(language)
        };

        Courses.Add(course);
        _ = SaveAsync();

        return course;
    }

    public List<Course> GetCoursesForTeacher(string teacherId)
    {
        return Courses
            .Where(c => c.OwnerTeacherId == teacherId)
            .ToList();
    }

    public List<Course> GetCoursesForStudent(string studentId)
    {
        var enrolledCourseIds = Enrollments
            .Where(e => e.StudentId == studentId)
            .Select(e => e.CourseId)
            .Distinct()
            .ToList();

        return Courses
            .Where(c => enrolledCourseIds.Contains(c.Id))
            .ToList();
    }

    public void DeleteCourse(Guid courseId)
    {
        var course = Courses.FirstOrDefault(c => c.Id == courseId);
        if (course == null)
            return;

        Courses.Remove(course);

        // чистим записи об участии
        Enrollments.RemoveAll(e => e.CourseId == courseId);

        _ = SaveAsync();
    }

    // --------------------------
    // MATERIALS
    // --------------------------

    public void AddMaterialToCourse(Guid courseId, string title, string type, string description, string url)
    {
        var course = Courses.FirstOrDefault(c => c.Id == courseId);
        if (course == null)
            return;

        course.Materials.Add(new MaterialItem
        {
            CourseId    = courseId,
            Title       = title,
            Type        = type,
            Description = description,
            Url         = url
        });

        course.LessonsCount = course.Materials.Count;

        _ = SaveAsync();
    }

    // --------------------------
    // ENROLLMENTS & PROGRESS
    // --------------------------

    public Enrollment? GetEnrollment(Guid courseId, string studentId)
    {
        return Enrollments.FirstOrDefault(e =>
            e.CourseId == courseId &&
            e.StudentId == studentId);
    }

    public Enrollment EnsureEnrollment(Guid courseId, string studentId)
    {
        var enroll = GetEnrollment(courseId, studentId);
        if (enroll != null)
            return enroll;

        enroll = new Enrollment
        {
            CourseId  = courseId,
            StudentId = studentId
        };

        Enrollments.Add(enroll);
        _ = SaveAsync();

        return enroll;
    }

    // ✅ Студент записан на курс?
    public bool IsStudentEnrolled(string studentId, Guid courseId)
    {
        return Enrollments.Any(e => e.StudentId == studentId && e.CourseId == courseId);
    }

    // ✅ Записать студента на курс (используется в StudentDashboardPage)
    public void EnrollStudent(string studentId, Guid courseId)
    {
        EnsureEnrollment(courseId, studentId);
    }

    // ✅ Переключить выполненность материала (используется в StudentCoursePage)
    public void ToggleMaterialCompleted(Guid courseId, Guid materialId, string studentId)
    {
        var enroll = EnsureEnrollment(courseId, studentId);

        if (enroll.CompletedMaterialIds.Contains(materialId))
            enroll.CompletedMaterialIds.Remove(materialId);
        else
            enroll.CompletedMaterialIds.Add(materialId);

        _ = SaveAsync();
    }

    public double GetProgressPercent(Guid courseId, string studentId)
    {
        var course = Courses.FirstOrDefault(c => c.Id == courseId);
        if (course == null || course.Materials.Count == 0)
            return 0;

        var enrollment = GetEnrollment(courseId, studentId);
        if (enrollment == null)
            return 0;

        var total   = course.Materials.Count;
        var done    = enrollment.CompletedMaterialIds.Count;
        var percent = (double)done / total * 100.0;

        return percent;
    }

    public List<(User Student, double Percent)> GetStudentsProgressForCourse(Guid courseId)
    {
        var result = new List<(User, double)>();

        var course = Courses.FirstOrDefault(c => c.Id == courseId);
        if (course == null || course.Materials.Count == 0)
            return result;

        var total   = course.Materials.Count;
        var enrolls = Enrollments.Where(e => e.CourseId == courseId);

        foreach (var e in enrolls)
        {
            var student = Users.FirstOrDefault(u => u.Id == e.StudentId);
            if (student == null)
                continue;

            double percent =
                total == 0
                    ? 0
                    : (double)e.CompletedMaterialIds.Count / total * 100.0;

            result.Add((student, percent));
        }

        return result;
    }
}