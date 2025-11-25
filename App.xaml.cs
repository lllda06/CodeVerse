using MyMauiApp.Services;

namespace MyMauiApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Загружаем "базу" при старте
        _ = AppData.Instance.LoadAsync();

        // Стартовое окно
        MainPage = new AppShell();
    }
}