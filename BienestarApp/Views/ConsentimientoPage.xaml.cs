namespace BienestarApp.Views;

/// <summary>
/// Página de solo lectura, sin ViewModel: no tiene estado ni lógica, solo
/// muestra el texto de <see cref="Models.Consentimiento"/>.
/// </summary>
public partial class ConsentimientoPage : ContentPage
{
    public ConsentimientoPage()
    {
        InitializeComponent();
    }

    private async void OnVolverClicked(object? sender, EventArgs e) => await Shell.Current.GoToAsync("..");
}
