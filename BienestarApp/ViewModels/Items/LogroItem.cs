namespace BienestarApp.ViewModels.Items;

/// <summary>Una insignia de "Mi progreso". Se recalcula al abrir la pantalla, por eso no es ObservableObject.</summary>
public class LogroItem
{
    public required string Emoji { get; init; }
    public required string Titulo { get; init; }
    public required string Descripcion { get; init; }
    public required bool Desbloqueado { get; init; }
}
