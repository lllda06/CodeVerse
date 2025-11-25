using MyMauiApp.Services;
using MyMauiApp.Models;

namespace MyMauiApp;

public partial class AddMaterialPage : ContentPage
{
    private readonly Guid _courseId;

    public AddMaterialPage(Guid courseId)
    {
        InitializeComponent();
        _courseId = courseId;
    }

    private async void OnAddMaterialClicked(object sender, EventArgs e)
    {
        var title = MaterialTitleEntry.Text?.Trim() ?? "";
        var type = MaterialTypeEntry.SelectedItem?.ToString() ?? "";   // <- Picker, не Text
        var desc = MaterialDescriptionEntry.Text?.Trim() ?? "";
        var url  = MaterialUrlEntry.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(title))
        {
            await DisplayAlert("Błąd", "Podaj tytuł materiału.", "OK");
            return;
        }

        AppData.Instance.AddMaterialToCourse(_courseId, title, type, desc, url);

        await DisplayAlert("Materiał", "Materiał dodany ✅", "OK");
        await Navigation.PopAsync();
    }

    // Временный обработчик кнопки "Przeglądaj plik"
    private async void OnBrowseFileClicked(object sender, EventArgs e)
    {
        await DisplayAlert(
            "Plik",
            "Demo: wybór pliku nie jest jeszcze zaimplementowany.\n" +
            "Wpisz link lub ścieżkę ręcznie w polu URL.",
            "OK");
    }
}