using MyMauiApp.Models;
using MyMauiApp.Services;

namespace MyMauiApp;

public partial class StudentLoginPage : ContentPage
{
    public StudentLoginPage()
    {
        InitializeComponent();
    }

    // Основной логин ученика
    private async void OnStudentLoginClicked(object sender, EventArgs e)
    {
        var email = StudentLoginEmailEntry.Text ?? "";
        var password = StudentLoginPasswordEntry.Text ?? "";

        var (ok, error) = AppData.Instance.Login(email, password, UserRole.Student);

        if (!ok)
        {
            await DisplayAlert("Logowanie ucznia", error, "OK");
            return;
        }

        await Navigation.PushAsync(new StudentDashboardPage());
    }

    // Переход на регистрацию ученика
    private async void OnGoToRegisterTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new StudentRegisterPage());
    }

    // Соц-логины — пока фейковые (демо)
    private async void OnSynergiaLoginClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Synergia", "Demo: logowanie przez Librus Synergia ✅", "OK");
        await Navigation.PushAsync(new StudentDashboardPage());
    }

    private async void OnGoogleLoginClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Google", "Demo: logowanie przez Google ✅", "OK");
        await Navigation.PushAsync(new StudentDashboardPage());
    }

    private async void OnFacebookLoginClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Facebook", "Demo: logowanie przez Facebook ✅", "OK");
        await Navigation.PushAsync(new StudentDashboardPage());
    }
}