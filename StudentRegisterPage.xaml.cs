using MyMauiApp.Models;
using MyMauiApp.Services;

namespace MyMauiApp;

public partial class StudentRegisterPage : ContentPage
{
    public StudentRegisterPage()
    {
        InitializeComponent();
    }

    private async void OnStudentRegisterClicked(object sender, EventArgs e)
    {
        var name = StudentNameEntry.Text ?? "";
        var email = StudentEmailEntry.Text ?? "";
        var password = StudentPasswordEntry.Text ?? "";

        var (ok, error) = AppData.Instance.Register(name, email, password, UserRole.Student);

        if (!ok)
        {
            await DisplayAlert("Rejestracja", error, "OK");
            return;
        }

        await Navigation.PushAsync(new StudentDashboardPage());
    }

    private async void OnGoToLoginTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new StudentLoginPage());
    }
}