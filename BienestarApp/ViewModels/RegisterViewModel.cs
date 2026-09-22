using BienestarApp.Models;
using BienestarApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BienestarApp.ViewModels;

public partial class RegisterViewModel : BaseViewModel
{
    private readonly IAuthService authService;

    [ObservableProperty]
    private string nombre = string.Empty;

    [ObservableProperty]
    private string codigoUniversitario = string.Empty;

    [ObservableProperty]
    private string carrera = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string mensajeError = string.Empty;

    public RegisterViewModel(IAuthService authService)
    {
        this.authService = authService;
        Title = "Crear cuenta";
    }

    [RelayCommand]
    private async Task RegistrarAsync()
    {
        if (IsBusy) return;

        MensajeError = string.Empty;

        if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(CodigoUniversitario) ||
            string.IsNullOrWhiteSpace(Carrera) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            MensajeError = "Completa todos los campos.";
            return;
        }

        try
        {
            IsBusy = true;
            await authService.RegistrarAsync(new RegisterRequest
            {
                Nombre = Nombre,
                CodigoUniversitario = CodigoUniversitario,
                Carrera = Carrera,
                Email = Email,
                Password = Password,
            });
            // El registro ya deja la sesión iniciada (ver AuthService),
            // así que se pasa directo a MainPage, sin pedir login de nuevo.
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
    private static async Task VolverAsync() => await Shell.Current.GoToAsync("..");
}
