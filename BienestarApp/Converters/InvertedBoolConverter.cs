using System.Globalization;

namespace BienestarApp.Converters;

/// <summary>Invierte un bool en el binding (ej. deshabilitar un botón mientras IsBusy es true).</summary>
public class InvertedBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is bool b && !b;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is bool b && !b;
}
