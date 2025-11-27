using MyMauiApp.Models;
using MyMauiApp.Services;
using System.Collections.ObjectModel;

namespace MyMauiApp;

// Модель строки для панели ученика
public class StudentCourseRow
{
    public Course Course { get; set; } = null!;
    public bool IsEnrolled { get; set; }

    public string StatusText =>
        IsEnrolled ? "Zapisany na kurs" : "Nie zapisany";

    public string ActionText =>
        IsEnrolled ? "Otwórz" : "Zapisz się";
}

public partial class StudentDashboardPage : ContentPage
{
    public ObservableCollection<StudentCourseRow> AllCourses { get; } = new();

    public StudentDashboardPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadData();
    }

    private void LoadData()
    {
        var data    = AppData.Instance;
        var current = data.CurrentUser;

        if (current == null || current.Role != UserRole.Student)
            return;

        // имя ученика
        StudentNameLabel.Text = current.Name;

        AllCourses.Clear();

        foreach (var c in data.Courses)
        {
            bool enrolled = data.IsStudentEnrolled(current.Id, c.Id);

            AllCourses.Add(new StudentCourseRow
            {
                Course     = c,
                IsEnrolled = enrolled
            });
        }
    }

    // Кнопка "Zapisz się" / "Otwórz"
    private async void OnCourseActionClicked(object sender, EventArgs e)
    {
        var data    = AppData.Instance;
        var current = data.CurrentUser;

        if (current == null || current.Role != UserRole.Student)
            return;

        if (sender is not Button btn || btn.CommandParameter is not StudentCourseRow row)
            return;

        if (!row.IsEnrolled)
        {
            // Подписываем студента на курс
            data.EnrollStudent(current.Id, row.Course.Id);
            await data.SaveAsync();

            // Перезагружаем список, чтобы обновить статусы и кнопки
            LoadData();
        }
        else
        {
            // Уже записан — открываем курс
            await Navigation.PushAsync(new StudentCoursePage(row.Course.Id));
        }
    }
}