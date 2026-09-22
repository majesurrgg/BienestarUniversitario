using System.Globalization;

namespace BienestarApp.Converters;

/// <summary>true si el string tiene contenido — usado para mostrar un Label de error solo cuando hay mensaje.</summary>
public class StringNotEmptyConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        !string.IsNullOrWhiteSpace(value as string);

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
