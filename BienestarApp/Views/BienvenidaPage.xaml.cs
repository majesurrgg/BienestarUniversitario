using BienestarApp.ViewModels;

namespace BienestarApp.Views;

public partial class BienvenidaPage : ContentPage
{
    public BienvenidaPage(BienvenidaViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        BienvenidaViewModel.MarcarVista();
    }
}
