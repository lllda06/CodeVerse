using MyMauiApp.Models;
using MyMauiApp.Services;

namespace MyMauiApp;

public partial class TeacherLoginPage : ContentPage
{
    public TeacherLoginPage()
    {
        InitializeComponent();
    }

    // Основной логин учителя
    private async void OnTeacherLoginClicked(object sender, EventArgs e)
    {
        var email = TeacherLoginEmailEntry.Text ?? "";
        var password = TeacherLoginPasswordEntry.Text ?? "";

        var (ok, error) = AppData.Instance.Login(email, password, UserRole.Teacher);

        if (!ok)
        {
            await DisplayAlert("Logowanie nauczyciela", error, "OK");
            return;
        }

        await Navigation.PushAsync(new TeacherDashboardPage());
    }

    // Переход на регистрацию учителя
    private async void OnGoToRegisterTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new TeacherRegisterPage());
    }

    // Соц-логины — пока демо, просто алерт + переход в панель
    private async void OnSynergiaLoginClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Synergia", "Demo: logowanie przez Librus Synergia ✅", "OK");
        await Navigation.PushAsync(new TeacherDashboardPage());
    }

    private async void OnGoogleLoginClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Google", "Demo: logowanie przez Google ✅", "OK");
        await Navigation.PushAsync(new TeacherDashboardPage());
    }

    private async void OnFacebookLoginClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Facebook", "Demo: logowanie przez Facebook ✅", "OK");
        await Navigation.PushAsync(new TeacherDashboardPage());
    }
}