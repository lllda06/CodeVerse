using System;
using System.Linq;
using MyMauiApp.Models;
using MyMauiApp.Services;

namespace MyMauiApp;

public partial class TeacherDashboardPage : ContentPage
{
    public TeacherDashboardPage()
    {
        InitializeComponent();
    }

    // Кнопка "DODAJ KURS"
    private async void OnAddCourseClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new NewCoursePage());
    }

    // Кнопка "Dodaj materiał"
    private async void OnAddMaterialClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string title)
        {
            var course = AppData.Instance.Courses.FirstOrDefault(c => c.Title == title);

            if (course == null)
            {
                await DisplayAlert("Kurs", "Nie znaleziono kursu.", "OK");
                return;
            }

            await Navigation.PushAsync(new AddMaterialPage(course.Id));
        }
    }

    // Кнопка "Postępy uczniów"
    private async void OnStudentsProgressClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string title)
        {
            var course = AppData.Instance.Courses.FirstOrDefault(c => c.Title == title);

            if (course == null)
            {
                await DisplayAlert("Kurs", "Nie znaleziono kursu.", "OK");
                return;
            }

            await Navigation.PushAsync(new StudentsProgressPage(course.Id));
        }
    }
}