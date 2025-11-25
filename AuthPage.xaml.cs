namespace MyMauiApp;

public partial class AuthPage : ContentPage
{
    public AuthPage()
    {
        InitializeComponent();
    }

    private async void OnStudentRegisterClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(StudentRegisterPage));
    }

    private async void OnStudentLoginClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(StudentLoginPage));
    }

    // 🔹 Переход на логин учителя
    private async void OnTeacherLoginTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(TeacherLoginPage));
    }
    
    private async void OnSynergiaLoginClicked(object sender, EventArgs e)
    {
        await Launcher.OpenAsync("https://synergia.librus.pl/loguj");
    }

    private async void OnGoogleLoginClicked(object sender, EventArgs e)
    {
        await Launcher.OpenAsync("https://accounts.google.com/");
    }

    private async void OnFacebookLoginClicked(object sender, EventArgs e)
    {
        await Launcher.OpenAsync("https://www.facebook.com/login/");
    }
}