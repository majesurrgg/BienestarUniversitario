using BienestarApi.Data;
using BienestarApi.DTOs.EncuestasBasal;
using BienestarApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BienestarApi.Services;

/// <summary>
/// Guarda la encuesta basal y, si el ítem 9 del PHQ-9 dio positivo, crea de
/// inmediato una <see cref="AlertaRiesgo"/> — el protocolo ético para ese
/// ítem. El umbral (cualquier respuesta &gt; 0) es deliberadamente
/// conservador: para un ítem de riesgo de autolesión es preferible marcar
/// de más que dejar pasar un caso. Validar con el asesor/comité de ética
/// antes del piloto real.
/// </summary>
public class EncuestaBasalService(BienestarDbContext db) : IEncuestaBasalService
{
    private const int UmbralAlertaItem9 = 1;

    public async Task<EncuestaBasalResponse> CrearAsync(int usuarioId, CrearEncuestaBasalRequest request)
    {
        if (request.RespuestasPhq9.Count != 9 || request.RespuestasPhq9.Any(r => r is < 0 or > 3))
            throw new EncuestaBasalException("El PHQ-9 debe traer exactamente 9 respuestas, cada una entre 0 y 3.");

        var yaExiste = await db.EncuestasBasal
            .AnyAsync(e => e.UsuarioId == usuarioId && e.Fase == request.Fase);
        if (yaExiste)
            throw new EncuestaBasalException($"Ya existe una encuesta registrada para la fase {request.Fase}.");

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

        var requiereAtencion = respuestaItem9 >= UmbralAlertaItem9;
        if (requiereAtencion)
        {
            db.AlertasRiesgo.Add(new AlertaRiesgo
            {
                UsuarioId = usuarioId,
                EncuestaBasalId = encuesta.Id,
                Fecha = DateTime.UtcNow,
                // true porque el mensaje se muestra en el mismo momento en
                // que la app recibe RequiereAtencionInmediata = true. Si
                // algún día ese aviso deja de ser automático, este campo
                // debe reflejar si REALMENTE se mostró.
                SeMostroMensajeAyuda = true,
            });
            await db.SaveChangesAsync();
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
}
