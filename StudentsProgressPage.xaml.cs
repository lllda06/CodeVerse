using MyMauiApp.Models;
using MyMauiApp.Services;
using System.Collections.ObjectModel;

namespace MyMauiApp;

public class StudentProgressRow
{
    public string StudentName { get; set; } = "";
    public double Percent { get; set; }
    public string PercentText => $"{Percent:0}%";
}

public partial class StudentsProgressPage : ContentPage
{
    private readonly Guid _courseId;

    public ObservableCollection<StudentProgressRow> Students { get; } = new();

    public StudentsProgressPage(Guid courseId)
    {
        InitializeComponent();
        _courseId = courseId;
        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Students.Clear();

        var data = AppData.Instance;
        var rows = data.GetStudentsProgressForCourse(_courseId);

        foreach (var (student, percent) in rows)
        {
            Students.Add(new StudentProgressRow
            {
                StudentName = student.Name,
                Percent = percent
            });
        }
    }
}