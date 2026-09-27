using System.Globalization;
using BienestarApi.Data;
using BienestarApi.DTOs.RegistrosDiarios;
using BienestarApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BienestarApi.Services;

/// <summary>
/// Lógica del check-in diario: decide qué día es "hoy", asegura que exista
/// su fila en la dimensión Tiempo, y aplica la regla de un check-in por
/// estudiante por día (que además está garantizada por el índice único
/// (UsuarioId, TiempoId) en la base).
/// </summary>
public class RegistroDiarioService(BienestarDbContext db, IConfiguration configuration) : IRegistroDiarioService
{
    public async Task<RegistroDiarioResponse> CrearAsync(int usuarioId, CrearRegistroDiarioRequest request)
    {
        var hoy = FechaDeHoy();

        var yaRegistro = await db.RegistrosDiarios
            .AnyAsync(r => r.UsuarioId == usuarioId && r.Tiempo.Fecha == hoy);
        if (yaRegistro)
            throw new RegistroDiarioException("Ya registraste tu check-in de hoy. Vuelve mañana.");

        var tiempo = await ObtenerOCrearTiempoAsync(hoy);

        var registro = new RegistroDiario
        {
            UsuarioId = usuarioId,
            Tiempo = tiempo,
            NivelEstres = request.NivelEstres,
            CalidadSueno = request.CalidadSueno,
            MinutosActividadFisica = request.MinutosActividadFisica,
            EstadoAnimo = request.EstadoAnimo,
            FechaCreacion = DateTime.UtcNow,
        };
        db.RegistrosDiarios.Add(registro);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // Dos envíos casi simultáneos (doble toque en "Guardar") pasan
            // ambos el chequeo de arriba; el índice único de la base frena
            // al segundo y aquí se traduce a un mensaje legible.
            throw new RegistroDiarioException("Ya registraste tu check-in de hoy. Vuelve mañana.");
        }

        return ARespuesta(registro, hoy);
    }

    public async Task<RegistroDiarioResponse?> ObtenerDeHoyAsync(int usuarioId)
    {
        var hoy = FechaDeHoy();

        var registro = await db.RegistrosDiarios
            .FirstOrDefaultAsync(r => r.UsuarioId == usuarioId && r.Tiempo.Fecha == hoy);

        return registro is null ? null : ARespuesta(registro, hoy);
    }

    /// <summary>
    /// "Hoy" según la zona horaria del estudio (Perú), no la del servidor ni
    /// UTC: a las 9 p. m. en Lima ya es el día siguiente en UTC, y el
    /// check-in quedaría en la fecha equivocada.
    /// </summary>
    private DateOnly FechaDeHoy()
    {
        var zonaId = configuration["ZonaHoraria"] ?? "America/Lima";
        var zona = TimeZoneInfo.FindSystemTimeZoneById(zonaId);
        return DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zona));
    }

    /// <summary>
    /// La dimensión Tiempo se llena "a demanda": la primera vez que alguien
    /// registra algo en una fecha, se crea su fila con año/semana/día.
    /// </summary>
    private async Task<Tiempo> ObtenerOCrearTiempoAsync(DateOnly fecha)
    {
        var tiempo = await db.Tiempos.FirstOrDefaultAsync(t => t.Fecha == fecha);
        if (tiempo is not null)
            return tiempo;

        var fechaHora = fecha.ToDateTime(TimeOnly.MinValue);
        tiempo = new Tiempo
        {
            Fecha = fecha,
            // Año ISO (no calendario) para que vaya de la mano con la semana
            // ISO: el 29/12/2026 es "semana 53 de 2026", pero el 01/01/2027
            // puede seguir siendo semana 53 "de 2026" en ISO-8601.
            Anio = ISOWeek.GetYear(fechaHora),
            Semana = ISOWeek.GetWeekOfYear(fechaHora),
            DiaSemana = fecha.DayOfWeek,
        };
        db.Tiempos.Add(tiempo);
        return tiempo;
    }

    public async Task<List<RegistroDiarioResponse>> ObtenerHistorialAsync(int usuarioId, int dias)
    {
        var desde = FechaDeHoy().AddDays(-dias);

        var registros = await db.RegistrosDiarios
            .Include(r => r.Tiempo)
            .Where(r => r.UsuarioId == usuarioId && r.Tiempo.Fecha >= desde)
            .OrderByDescending(r => r.Tiempo.Fecha)
            .ToListAsync();

        return [.. registros.Select(r => ARespuesta(r, r.Tiempo.Fecha))];
    }

    private static RegistroDiarioResponse ARespuesta(RegistroDiario registro, DateOnly fecha) => new()
    {
        Id = registro.Id,
        Fecha = fecha,
        NivelEstres = registro.NivelEstres,
        CalidadSueno = registro.CalidadSueno,
        MinutosActividadFisica = registro.MinutosActividadFisica,
        EstadoAnimo = registro.EstadoAnimo,
    };
}
