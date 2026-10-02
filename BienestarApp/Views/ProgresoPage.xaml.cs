using BienestarApp.ViewModels;
using BienestarApp.Views.Graficos;

namespace BienestarApp.Views;

public partial class ProgresoPage : ContentPage
{
    private readonly ProgresoViewModel viewModel;
    private readonly GraficoSemanalDrawable grafico = new();

    public ProgresoPage(ProgresoViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = viewModel;
        GraficoSemanal.Drawable = grafico;
    }

    // El gráfico es presentación pura: se le pasan los días que el
    // ViewModel ya cargó y se vuelve a dibujar.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await viewModel.InicializarAsync();
        grafico.Dias = [.. viewModel.Dias];
        GraficoSemanal.Invalidate();
    }
}
