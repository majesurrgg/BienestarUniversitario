using BienestarApp.ViewModels;

namespace BienestarApp.Views;

public partial class EncuestaBasalPage : ContentPage
{
    public EncuestaBasalPage(EncuestaBasalViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
