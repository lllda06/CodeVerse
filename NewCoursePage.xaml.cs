using MyMauiApp.Models;
using MyMauiApp.Services;
using System;

namespace MyMauiApp;

public partial class NewCoursePage : ContentPage
{
    public NewCoursePage()
    {
        InitializeComponent();
    }

    private async void OnCreateCourseClicked(object sender, EventArgs e)
    {
        var title = CourseTitleEntry.Text?.Trim() ?? "";
        var desc = CourseDescriptionEditor.Text?.Trim() ?? "";
        var lang = CourseLanguageEntry.Text?.Trim() ?? "";
        int lessons = int.TryParse(CourseLessonsEntry.Text, out var l) ? l : 0;

        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(lang))
        {
            await DisplayAlert("Błąd", "Proszę wypełnić wszystkie wymagane pola.", "OK");
            return;
        }

        // создание курса
        var course = AppData.Instance.AddCourse(title, desc, lang);

        // меняем иконку в зависимости от языка
        switch (lang.ToLower())
        {
            case "python":
                LanguageIcon.Source = "python_icon.png";
                break;
            case "swift":
                LanguageIcon.Source = "swift_icon.png";
                break;
            case "java":
                LanguageIcon.Source = "java_icon.png";
                break;
            case "c#":
                LanguageIcon.Source = "csharp_icon.png";
                break;
            case "javascript":
                LanguageIcon.Source = "js_icon.webp";
                break;
            case "html":
                LanguageIcon.Source = "html_icon.png";
                break;
            case "c++":
                LanguageIcon.Source = "c_icon.png";
                break;
            case "php":
                LanguageIcon.Source = "php_icon.png";
                break;
            default:
                LanguageIcon.Source = "zsm_logo.png"; // иконка по умолчанию
                break;
        }

        course.LessonsCount = lessons;

        // сохраняем курс
        await AppData.Instance.SaveAsync();

        await DisplayAlert("Курс", "Курс был добавлен ✅", "OK");
        await Navigation.PopAsync();
    }
}