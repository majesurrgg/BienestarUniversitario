using BienestarApp.ViewModels;

namespace BienestarApp.Views;

public partial class ProgresoPage : ContentPage
{
    private readonly ProgresoViewModel viewModel;

    public ProgresoPage(ProgresoViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = viewModel.InicializarAsync();
    }
}
