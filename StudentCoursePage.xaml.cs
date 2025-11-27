using MyMauiApp.Models;
using MyMauiApp.Services;
using System.Collections.ObjectModel;
using Microsoft.Maui.ApplicationModel; // для Launcher

namespace MyMauiApp;

public partial class StudentCoursePage : ContentPage
{
    private readonly Guid _courseId;
    private Course _course;
    private Enrollment? _enrollment;

    public ObservableCollection<MaterialItem> Materials { get; } = new();

    public StudentCoursePage(Guid courseId)
    {
        InitializeComponent();
        _courseId = courseId;
        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        var data = AppData.Instance;
        var current = data.CurrentUser!;
        _course = data.Courses.First(c => c.Id == _courseId);

        // обеспечиваем наличие записи о зачислении
        _enrollment = data.EnsureEnrollment(_courseId, current.Id);

        Materials.Clear();
        foreach (var m in _course.Materials)
            Materials.Add(m);

        CourseTitleLabel.Text = _course.Title;
        LessonsInfoLabel.Text = $"Wykład {Materials.Count} z {_course.LessonsCount}";

        UpdateProgress();
    }

    private void UpdateProgress()
    {
        if (_enrollment == null) return;

        var percent = AppData.Instance.GetProgressPercent(_courseId, _enrollment.StudentId);
        ProgressPercentLabel.Text = $"{percent:0}%";
    }

    // 👉 здесь открываем материал
    private async void OnMaterialTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not MaterialItem material || _enrollment == null)
            return;

        // 1. переключаем "выполнено/не выполнено"
        AppData.Instance.ToggleMaterialCompleted(material.CourseId, material.Id, _enrollment.StudentId);
        UpdateProgress();

        // 2. пробуем открыть сам материал
        if (!string.IsNullOrWhiteSpace(material.Url))
        {
            try
            {
                // если это http/https — откроется в браузере
                await Launcher.OpenAsync(material.Url);
            }
            catch
            {
                await DisplayAlert("Błąd", "Nie można otworzyć materiału (link jest nieprawidłowy).", "OK");
            }
        }
        else
        {
            // если ссылки нет — показать инфу
            var text = string.IsNullOrWhiteSpace(material.Description)
                ? "Brak dodatkowych informacji."
                : material.Description;

            await DisplayAlert(material.Title, text, "OK");
        }
    }

    private async void OnBackTapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}