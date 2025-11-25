using MyMauiApp.Models;
using MyMauiApp.Services;

namespace MyMauiApp;

public partial class TeacherRegisterPage : ContentPage
{
    public TeacherRegisterPage()
    {
        InitializeComponent();
    }

    // Регистрация учителя
    private async void OnTeacherRegisterClicked(object sender, EventArgs e)
    {
        var name = TeacherNameEntry?.Text?.Trim() ?? "";
        var email = TeacherEmailEntry?.Text?.Trim() ?? "";
        var password = TeacherPasswordEntry?.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Błąd", "Uzupełnij wszystkie pola.", "OK");
            return;
        }

        // здесь НИКАКОГО Teacher / FakeDatabase — используем общий AppData
        var (ok, error) = AppData.Instance.Register(name, email, password, UserRole.Teacher);

        if (!ok)
        {
            await DisplayAlert("Rejestracja nauczyciela", error, "OK");
            return;
        }

        await DisplayAlert("Sukces", "Konto nauczyciela zostało utworzone 🎉", "OK");

        // после регистрации — на панель учителя
        await Navigation.PushAsync(new TeacherDashboardPage());
    }

    // Переход на логин учителя
    private async void OnGoToLoginTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new TeacherLoginPage());
    }
}