-- Vistas analíticas de BienestarDW que consume el dashboard de Power BI.
-- Exportadas de Azure SQL Database el 2026-11-12. Ejecutar después de BienestarDW_esquema.sql.

IF OBJECT_ID('dbo.vw_pbi_Alerta', 'V') IS NOT NULL DROP VIEW dbo.vw_pbi_Alerta;
GO
/* 6. Protocolo ético – alertas del ítem 9 (solo conteos en el dashboard) */
CREATE   VIEW dbo.vw_pbi_Alerta AS
SELECT
    a.Id                                         AS AlertaId,
    a.UsuarioId                                  AS ParticipanteId,
    CAST(a.Fecha AS date)                        AS Fecha,
    a.SeMostroMensajeAyuda
FROM dbo.AlertasRiesgo a;
GO

IF OBJECT_ID('dbo.vw_pbi_Encuesta', 'V') IS NOT NULL DROP VIEW dbo.vw_pbi_Encuesta;
GO
/* 3. Hechos: encuestas basal / final con categorías clínicas */
CREATE   VIEW dbo.vw_pbi_Encuesta AS
SELECT
    b.Id                                         AS EncuestaId,
    b.UsuarioId                                  AS ParticipanteId,
    CASE b.Fase WHEN 0 THEN 'Basal' ELSE 'Final' END AS Fase,
    CAST(b.Fecha AS date)                        AS Fecha,
    b.PuntajePHQ9,
    CASE
        WHEN b.PuntajePHQ9 <= 4  THEN '1. Mínima (0-4)'
        WHEN b.PuntajePHQ9 <= 9  THEN '2. Leve (5-9)'
        WHEN b.PuntajePHQ9 <= 14 THEN '3. Moderada (10-14)'
        WHEN b.PuntajePHQ9 <= 19 THEN '4. Mod. severa (15-19)'
        ELSE '5. Severa (20-27)'
    END                                          AS NivelPHQ9,
    CASE WHEN b.RespuestaItem9 >= 1 THEN 1 ELSE 0 END AS Item9Positivo,
    b.PuntajeSISCO,                              -- 0-100 (media x 20); 0 = respondió "No" al filtro
    CASE
        WHEN b.PuntajeSISCO = 0  THEN '0. Sin estrés reportado (filtro = No)'
        WHEN b.PuntajeSISCO <= 33 THEN '1. Leve (1-33)'
        WHEN b.PuntajeSISCO <= 66 THEN '2. Moderado (34-66)'
        ELSE '3. Severo (67-100)'
    END                                          AS NivelSISCO,
    b.PuntajeIPAQ,                               -- MET-min/semana
    CASE
        WHEN b.PuntajeIPAQ < 600  THEN '1. Bajo (<600)'
        WHEN b.PuntajeIPAQ < 3000 THEN '2. Moderado (600-2999)'
        ELSE '3. Alto (>=3000)'
    END                                          AS NivelIPAQ
FROM dbo.EncuestasBasal b;
GO

IF OBJECT_ID('dbo.vw_pbi_Participante', 'V') IS NOT NULL DROP VIEW dbo.vw_pbi_Participante;
GO
/* 1. Dimensión participante (anonimizada) + adherencia */
CREATE   VIEW dbo.vw_pbi_Participante AS
SELECT
    u.Id                                         AS ParticipanteId,
    CONCAT('P', RIGHT('00' + CAST(u.Id AS varchar(3)), 2)) AS Participante,
    CASE
        WHEN LOWER(u.Carrera) LIKE '%sistema%'   THEN 'Ing. de Sistemas e Informática'
        WHEN LOWER(u.Carrera) LIKE '%educaci%'   THEN 'Educación'
        WHEN LOWER(u.Carrera) LIKE '%psicolog%'  THEN 'Psicología'
        WHEN LOWER(u.Carrera) LIKE '%obstetr%'   THEN 'Obstetricia'
        WHEN LOWER(u.Carrera) LIKE '%derecho%'   THEN 'Derecho'
        ELSE u.Carrera
    END                                          AS Carrera,
    CAST(u.FechaRegistro AS date)                AS FechaRegistro,
    CASE WHEN EXISTS (SELECT 1 FROM dbo.EncuestasBasal b WHERE b.UsuarioId = u.Id AND b.Fase = 0)
         THEN 1 ELSE 0 END                       AS CompletoBasal,
    CASE WHEN EXISTS (SELECT 1 FROM dbo.EncuestasBasal b WHERE b.UsuarioId = u.Id AND b.Fase = 1)
         THEN 1 ELSE 0 END                       AS CompletoFinal,
    (SELECT COUNT(*) FROM dbo.RegistrosDiarios r WHERE r.UsuarioId = u.Id) AS DiasRegistrados,
    (SELECT MAX(t.Fecha) FROM dbo.RegistrosDiarios r JOIN dbo.DimTiempo t ON t.Id = r.TiempoId
      WHERE r.UsuarioId = u.Id)                  AS UltimoRegistro
FROM dbo.DimUsuario u;
GO

IF OBJECT_ID('dbo.vw_pbi_PrePost', 'V') IS NOT NULL DROP VIEW dbo.vw_pbi_PrePost;
GO
/* 4. Comparación pre-post por participante (se llena desde el 01/11/2026) */
CREATE   VIEW dbo.vw_pbi_PrePost AS
SELECT
    pre.UsuarioId                                AS ParticipanteId,
    pre.PuntajePHQ9                              AS PHQ9_Basal,
    post.PuntajePHQ9                             AS PHQ9_Final,
    post.PuntajePHQ9 - pre.PuntajePHQ9           AS PHQ9_Cambio,
    pre.PuntajeSISCO                             AS SISCO_Basal,
    post.PuntajeSISCO                            AS SISCO_Final,
    post.PuntajeSISCO - pre.PuntajeSISCO         AS SISCO_Cambio,
    pre.PuntajeIPAQ                              AS IPAQ_Basal,
    post.PuntajeIPAQ                             AS IPAQ_Final
FROM dbo.EncuestasBasal pre
LEFT JOIN dbo.EncuestasBasal post
       ON post.UsuarioId = pre.UsuarioId AND post.Fase = 1
WHERE pre.Fase = 0;
GO

IF OBJECT_ID('dbo.vw_pbi_RegistroDiario', 'V') IS NOT NULL DROP VIEW dbo.vw_pbi_RegistroDiario;
GO
/* 2. Hechos: registro diario con día y semana del piloto */
CREATE   VIEW dbo.vw_pbi_RegistroDiario AS
SELECT
    r.Id                                         AS RegistroId,
    r.UsuarioId                                  AS ParticipanteId,
    t.Fecha,
    DATEDIFF(day, '2026-10-05', t.Fecha) + 1     AS DiaPiloto,
    DATEDIFF(day, '2026-10-05', t.Fecha) / 7 + 1 AS SemanaPiloto,
    t.DiaSemana,
    r.NivelEstres,                               -- 1-10
    r.CalidadSueno,                              -- 1-5
    r.EstadoAnimo,                               -- 1-5
    r.MinutosActividadFisica,
    CASE WHEN r.NivelEstres >= 7 THEN 1 ELSE 0 END            AS EstresAlto,
    CASE WHEN r.MinutosActividadFisica = 0 THEN 1 ELSE 0 END  AS SinActividadFisica
FROM dbo.RegistrosDiarios r
JOIN dbo.DimTiempo t ON t.Id = r.TiempoId;
GO

IF OBJECT_ID('dbo.vw_pbi_SUS', 'V') IS NOT NULL DROP VIEW dbo.vw_pbi_SUS;
GO
/* 5. Usabilidad (SUS) – se llena al cierre del piloto */
CREATE   VIEW dbo.vw_pbi_SUS AS
SELECT
    s.UsuarioId                                  AS ParticipanteId,
    CAST(s.FechaAplicacion AS date)              AS Fecha,
    s.PuntajeSUS,
    CASE WHEN s.PuntajeSUS >= 68 THEN 'Aceptable (>=68)' ELSE 'Bajo el umbral (<68)' END AS NivelSUS,
    s.RespuestasSUS,
    s.ComentarioLoMasUtil,
    s.ComentarioMejoras
FROM dbo.EncuestasSUS s;
GO

