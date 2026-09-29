using BienestarApi.Data;
using BienestarApi.DTOs.EncuestasSus;
using BienestarApi.Models;
using BienestarApi.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace BienestarApi.Services;

/// <summary>
/// System Usability Scale (Brooke, 1996): 10 ítems de 1 a 5 que alternan
/// afirmaciones positivas (ítems impares) y negativas (pares). Se aplica
/// una sola vez, al cierre del piloto, después de la encuesta final — mide
/// la herramienta, no el bienestar del estudiante.
/// </summary>
public class EncuestaSusService(BienestarDbContext db) : IEncuestaSusService
{
    public async Task<EncuestaSusResponse> CrearAsync(int usuarioId, CrearEncuestaSusRequest request)
    {
        if (request.Respuestas.Count != 10 || request.Respuestas.Any(r => r is < 1 or > 5))
            throw new EncuestaSusException("La encuesta SUS debe traer exactamente 10 respuestas, cada una entre 1 y 5.");

        var hizoFinal = await db.EncuestasBasal
            .AnyAsync(e => e.UsuarioId == usuarioId && e.Fase == FaseEncuesta.Final);
        if (!hizoFinal)
            throw new EncuestaSusException("Primero debes completar la encuesta de cierre.");

        if (await db.EncuestasSUS.AnyAsync(e => e.UsuarioId == usuarioId))
            throw new EncuestaSusException("Ya respondiste la evaluación de la app. ¡Gracias!");

        var encuesta = new EncuestaSUS
        {
            UsuarioId = usuarioId,
            PuntajeSUS = CalcularPuntaje(request.Respuestas),
            RespuestasSUS = string.Join(',', request.Respuestas),
            ComentarioLoMasUtil = Limpiar(request.ComentarioLoMasUtil),
            ComentarioMejoras = Limpiar(request.ComentarioMejoras),
            FechaAplicacion = DateTime.UtcNow,
        };
        db.EncuestasSUS.Add(encuesta);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // Doble envío simultáneo: lo frena el índice único por UsuarioId.
            throw new EncuestaSusException("Ya respondiste la evaluación de la app. ¡Gracias!");
        }

        return new EncuestaSusResponse
        {
            Id = encuesta.Id,
            PuntajeSUS = encuesta.PuntajeSUS,
            FechaAplicacion = encuesta.FechaAplicacion,
        };
    }

    /// <summary>
    /// Puntuación oficial: ítems impares aportan (respuesta − 1), ítems pares
    /// aportan (5 − respuesta); la suma (0-40) se multiplica por 2.5 → 0-100.
    /// </summary>
    public static decimal CalcularPuntaje(IReadOnlyList<int> respuestas)
    {
        var suma = 0;
        for (var i = 0; i < respuestas.Count; i++)
        {
            var esImpar = i % 2 == 0; // posición 0 = ítem 1
            suma += esImpar ? respuestas[i] - 1 : 5 - respuestas[i];
        }
        return suma * 2.5m;
    }

    private static string? Limpiar(string? texto) =>
        string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
}
