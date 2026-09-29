using BienestarApi.Data;
using BienestarApi.DTOs.EncuestasBasal;
using BienestarApi.Models;
using BienestarApi.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace BienestarApi.Services;

/// <summary>
/// Guarda la encuesta basal y, si el ítem 9 del PHQ-9 dio positivo, crea en
/// la MISMA operación una <see cref="AlertaRiesgo"/> — el protocolo ético
/// para ese ítem. El umbral (cualquier respuesta &gt; 0) es deliberadamente
/// conservador: para un ítem de riesgo de autolesión es preferible marcar
/// de más que dejar pasar un caso. Validar con el asesor/comité de ética
/// antes del piloto real.
///
/// También aplica el orden del diseño pre-post: la fase Final exige la
/// Basal y no se habilita antes de "Piloto:FechaHabilitaEncuestaFinal"
/// (configurable en Azure sin volver a publicar ni generar otro APK).
/// </summary>
public class EncuestaBasalService(BienestarDbContext db, IConfiguration configuration, IRelojPiloto reloj) : IEncuestaBasalService
{
    private const int UmbralAlertaItem9 = 1;

    public async Task<EncuestaBasalResponse> CrearAsync(int usuarioId, CrearEncuestaBasalRequest request)
    {
        if (request.RespuestasPhq9.Count != 9 || request.RespuestasPhq9.Any(r => r is < 0 or > 3))
            throw new EncuestaBasalException("El PHQ-9 debe traer exactamente 9 respuestas, cada una entre 0 y 3.");

        var fasesHechas = await ObtenerFasesCompletadasAsync(usuarioId);
        if (fasesHechas.Contains(request.Fase))
            throw new EncuestaBasalException($"Ya existe una encuesta registrada para la fase {request.Fase}.");

        if (request.Fase == FaseEncuesta.Final)
        {
            if (!fasesHechas.Contains(FaseEncuesta.Basal))
                throw new EncuestaBasalException("Primero debes completar la encuesta inicial.");

            var fechaHabilita = FechaHabilitaFinal();
            if (fechaHabilita is not null && reloj.Hoy() < fechaHabilita)
                throw new EncuestaBasalException($"La encuesta de cierre se habilita el {fechaHabilita:dd/MM/yyyy}.");
        }

        var respuestaItem9 = request.RespuestasPhq9[8];
        var puntajePhq9 = request.RespuestasPhq9.Sum();

        var encuesta = new EncuestaBasal
        {
            UsuarioId = usuarioId,
            Fase = request.Fase,
            PuntajePHQ9 = puntajePhq9,
            RespuestaItem9 = respuestaItem9,
            PuntajeSISCO = request.PuntajeSISCO,
            PuntajeIPAQ = request.PuntajeIPAQ,
            Fecha = DateTime.UtcNow,
        };
        db.EncuestasBasal.Add(encuesta);

        var requiereAtencion = respuestaItem9 >= UmbralAlertaItem9;
        if (requiereAtencion)
        {
            // Se enlaza por navegación (no por Id) para que EF Core guarde la
            // encuesta y su alerta en un solo SaveChanges, dentro de una
            // transacción: o quedan las dos, o ninguna. Antes eran dos
            // guardados separados y una caída de conexión entre ambos dejaba
            // una encuesta con ítem 9 positivo SIN alerta registrada.
            db.AlertasRiesgo.Add(new AlertaRiesgo
            {
                UsuarioId = usuarioId,
                EncuestaBasal = encuesta,
                Fecha = DateTime.UtcNow,
                // true porque el mensaje se muestra en el mismo momento en
                // que la app recibe RequiereAtencionInmediata = true. Si
                // algún día ese aviso deja de ser automático, este campo
                // debe reflejar si REALMENTE se mostró.
                SeMostroMensajeAyuda = true,
            });
        }

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // Igual que en RegistroDiarioService: dos envíos casi
            // simultáneos pasan ambos el chequeo de arriba; el índice único
            // (UsuarioId, Fase) de la base frena al segundo.
            throw new EncuestaBasalException($"Ya existe una encuesta registrada para la fase {request.Fase}.");
        }

        return new EncuestaBasalResponse
        {
            Id = encuesta.Id,
            Fase = encuesta.Fase,
            PuntajePHQ9 = encuesta.PuntajePHQ9,
            PuntajeSISCO = encuesta.PuntajeSISCO,
            PuntajeIPAQ = encuesta.PuntajeIPAQ,
            Fecha = encuesta.Fecha,
            RequiereAtencionInmediata = requiereAtencion,
        };
    }

    public async Task<List<FaseEncuesta>> ObtenerFasesCompletadasAsync(int usuarioId) =>
        await db.EncuestasBasal
            .Where(e => e.UsuarioId == usuarioId)
            .Select(e => e.Fase)
            .ToListAsync();

    public async Task<EstadoEncuestasResponse> ObtenerEstadoAsync(int usuarioId)
    {
        var fases = await ObtenerFasesCompletadasAsync(usuarioId);
        var basal = fases.Contains(FaseEncuesta.Basal);
        var final = fases.Contains(FaseEncuesta.Final);
        var fechaHabilita = FechaHabilitaFinal();
        var sus = final && await db.EncuestasSUS.AnyAsync(e => e.UsuarioId == usuarioId);

        return new EstadoEncuestasResponse
        {
            BasalCompletada = basal,
            FinalCompletada = final,
            FechaHabilitaFinal = fechaHabilita,
            FinalDisponible = basal && !final && (fechaHabilita is null || reloj.Hoy() >= fechaHabilita),
            SusCompletada = sus,
            SusDisponible = final && !sus,
        };
    }

    /// <summary>
    /// Fecha (hora de Perú) desde la que se acepta la encuesta final. Se
    /// configura en "Piloto:FechaHabilitaEncuestaFinal" (formato AAAA-MM-DD);
    /// vacío = sin restricción, útil para probar en desarrollo.
    /// </summary>
    private DateOnly? FechaHabilitaFinal() =>
        DateOnly.TryParse(configuration["Piloto:FechaHabilitaEncuestaFinal"], out var fecha) ? fecha : null;
}
