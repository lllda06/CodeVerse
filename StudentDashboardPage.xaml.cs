using MyMauiApp.Models;
using MyMauiApp.Services;
using System.Collections.ObjectModel;

namespace MyMauiApp;

public partial class StudentDashboardPage : ContentPage
{
    public ObservableCollection<Course> StudentCourses { get; } = new();

    public StudentDashboardPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        StudentCourses.Clear();

        var current = AppData.Instance.CurrentUser;
        if (current == null || current.Role != UserRole.Student)
            return;

        var courses = AppData.Instance.GetCoursesForCurrentStudent();
        foreach (var c in courses)
            StudentCourses.Add(c);

        // имя в шапке
        if (StudentNameLabel != null)
            StudentNameLabel.Text = current.Name;

        // инфо по количеству курсов
        if (CoursesInfoLabel != null)
            CoursesInfoLabel.Text =
                $"{StudentCourses.Count} kursów, na które jesteś zapisany";
    }

    private async void OnCourseTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is Course course)
        {
            await Navigation.PushAsync(new StudentCoursePage(course.Id));
        }
    }
}