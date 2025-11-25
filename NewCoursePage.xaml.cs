using MyMauiApp.Services;

namespace MyMauiApp;

public partial class NewCoursePage : ContentPage
{
    public NewCoursePage()
    {
        InitializeComponent();
    }

    private async void OnCreateCourseClicked(object sender, EventArgs e)
    {
        var title = CourseTitleEntry.Text ?? "";
        var desc  = CourseDescriptionEditor.Text ?? "";
        var lang = CourseLanguageEntry.Text ?? "";
        int lessons = int.TryParse(CourseLessonsEntry.Text, out var l) ? l : 0;

        var course = AppData.Instance.AddCourseForCurrentTeacher(title, desc, lang, lessons);

        await DisplayAlert("Kurs", "Kurs został dodany ✅", "OK");
        await Navigation.PopAsync(); // вернуться в TeacherDashboard
    }
}