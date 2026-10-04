using BienestarApp.ViewModels;

namespace BienestarApp.Views;

public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel viewModel;

    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = viewModel;
    }

    // Login es la primera pantalla de Shell: al aparecer, si ya hay sesión
    // guardada, el ViewModel manda directo a la pantalla principal.
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = viewModel.EntrarSiHaySesionAsync();
    }
}
