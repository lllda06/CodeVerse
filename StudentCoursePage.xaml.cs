using System;
using System.Collections.ObjectModel;
using System.Linq;
using MyMauiApp.Models;
using MyMauiApp.Services;

namespace MyMauiApp;

public partial class StudentCoursePage : ContentPage
{
    private readonly Guid _courseId;
    private Course _course = null!;
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
        _enrollment = data.GetEnrollment(_courseId, current.Id);

        // наполняем список материалов
        Materials.Clear();
        foreach (var m in _course.Materials)
            Materials.Add(m);

        // заполняем шапку
        CourseTitleLabel.Text = _course.Title;
        LessonsInfoLabel.Text = $"Wykład {_course.Materials.Count} z {_course.LessonsCount}";

        UpdateProgress();
    }

    private void UpdateProgress()
    {
        if (_enrollment == null) return;

        var percent = AppData.Instance.GetProgressPercent(_courseId, _enrollment.StudentId);
        ProgressPercentLabel.Text = $"{percent:0}%";
    }

    private void OnMaterialTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is MaterialItem material && _enrollment != null)
        {
            AppData.Instance.ToggleMaterialCompleted(
                material.CourseId,
                material.Id,
                _enrollment.StudentId);

            UpdateProgress();
        }
    }

    private async void OnBackTapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}