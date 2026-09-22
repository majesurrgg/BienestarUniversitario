using BienestarApp.Models;
using BienestarApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BienestarApp.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IAuthService authService;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string mensajeError = string.Empty;

    public LoginViewModel(IAuthService authService)
    {
        this.authService = authService;
        Title = "Iniciar sesión";
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy) return;

        MensajeError = string.Empty;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            MensajeError = "Ingresa tu email y contraseña.";
            return;
        }

        try
        {
            IsBusy = true;
            await authService.LoginAsync(new LoginRequest { Email = Email, Password = Password });
            await Shell.Current.GoToAsync("//MainPage");
        }
        catch (ApiException ex)
        {
            MensajeError = ex.Message;
        }
        catch (Exception)
        {
            MensajeError = "No se pudo conectar con el servidor. Revisa tu conexión y la URL de la API.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private static async Task IrARegistroAsync() => await Shell.Current.GoToAsync(nameof(Views.RegisterPage));
}
