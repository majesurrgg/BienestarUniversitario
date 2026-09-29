using BienestarApp.ViewModels;

namespace BienestarApp.Views;

public partial class EncuestaSusPage : ContentPage
{
    private readonly EncuestaSusViewModel viewModel;

    public EncuestaSusPage(EncuestaSusViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        viewModel.Inicializar();
    }
}
