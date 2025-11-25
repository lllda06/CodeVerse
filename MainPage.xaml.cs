namespace MyMauiApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnNextClicked(object sender, EventArgs e)
    {
        // przejdź do strony rejestracji/logowania
        await Shell.Current.GoToAsync(nameof(AuthPage));
    }
}