using System.Text.Json.Serialization;

namespace BienestarApp.Models;

/// <summary>Forma del error que devuelve BienestarApi: <c>{ "message": "..." }</c>.</summary>
public class ErrorResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}
