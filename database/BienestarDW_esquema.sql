IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824012206_InicialDW'
)
BEGIN
    CREATE TABLE [DimTiempo] (
        [Id] int NOT NULL IDENTITY,
        [Fecha] date NOT NULL,
        [Anio] int NOT NULL,
        [Semana] int NOT NULL,
        [DiaSemana] int NOT NULL,
        CONSTRAINT [PK_DimTiempo] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824012206_InicialDW'
)
BEGIN
    CREATE TABLE [DimUsuario] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(max) NOT NULL,
        [CodigoUniversitario] nvarchar(450) NOT NULL,
        [Carrera] nvarchar(max) NOT NULL,
        [FechaRegistro] datetime2 NOT NULL,
        CONSTRAINT [PK_DimUsuario] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824012206_InicialDW'
)
BEGIN
    CREATE TABLE [Cuentas] (
        [Id] int NOT NULL IDENTITY,
        [UsuarioId] int NOT NULL,
        [Email] nvarchar(450) NOT NULL,
        [PasswordHash] nvarchar(max) NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        CONSTRAINT [PK_Cuentas] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Cuentas_DimUsuario_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [DimUsuario] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824012206_InicialDW'
)
BEGIN
    CREATE TABLE [EncuestasBasal] (
        [Id] int NOT NULL IDENTITY,
        [UsuarioId] int NOT NULL,
        [PuntajePHQ9] int NOT NULL,
        [PuntajeSISCO] int NOT NULL,
        [PuntajeIPAQ] int NOT NULL,
        [Fecha] datetime2 NOT NULL,
        [Fase] int NOT NULL,
        CONSTRAINT [PK_EncuestasBasal] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_EncuestasBasal_DimUsuario_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [DimUsuario] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824012206_InicialDW'
)
BEGIN
    CREATE TABLE [EncuestasSUS] (
        [Id] int NOT NULL IDENTITY,
        [UsuarioId] int NOT NULL,
        [PuntajeSUS] decimal(5,2) NOT NULL,
        [FechaAplicacion] datetime2 NOT NULL,
        CONSTRAINT [PK_EncuestasSUS] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_EncuestasSUS_DimUsuario_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [DimUsuario] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824012206_InicialDW'
)
BEGIN
    CREATE TABLE [RegistrosDiarios] (
        [Id] int NOT NULL IDENTITY,
        [UsuarioId] int NOT NULL,
        [TiempoId] int NOT NULL,
        [NivelEstres] int NOT NULL,
        [CalidadSueno] int NOT NULL,
        [MinutosActividadFisica] int NOT NULL,
        [EstadoAnimo] int NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        CONSTRAINT [PK_RegistrosDiarios] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RegistrosDiarios_DimTiempo_TiempoId] FOREIGN KEY ([TiempoId]) REFERENCES [DimTiempo] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_RegistrosDiarios_DimUsuario_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [DimUsuario] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824012206_InicialDW'
)
BEGIN
    CREATE TABLE [AlertasRiesgo] (
        [Id] int NOT NULL IDENTITY,
        [UsuarioId] int NOT NULL,
        [EncuestaBasalId] int NULL,
        [Fecha] datetime2 NOT NULL,
        [SeMostroMensajeAyuda] bit NOT NULL,
        CONSTRAINT [PK_AlertasRiesgo] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AlertasRiesgo_DimUsuario_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [DimUsuario] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AlertasRiesgo_EncuestasBasal_EncuestaBasalId] FOREIGN KEY ([EncuestaBasalId]) REFERENCES [EncuestasBasal] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824012206_InicialDW'
)
BEGIN
    CREATE INDEX [IX_AlertasRiesgo_EncuestaBasalId] ON [AlertasRiesgo] ([EncuestaBasalId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824012206_InicialDW'
)
BEGIN
    CREATE INDEX [IX_AlertasRiesgo_UsuarioId] ON [AlertasRiesgo] ([UsuarioId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824012206_InicialDW'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Cuentas_Email] ON [Cuentas] ([Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824012206_InicialDW'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Cuentas_UsuarioId] ON [Cuentas] ([UsuarioId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824012206_InicialDW'
)
BEGIN
    CREATE UNIQUE INDEX [IX_DimTiempo_Fecha] ON [DimTiempo] ([Fecha]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824012206_InicialDW'
)
BEGIN
    CREATE UNIQUE INDEX [IX_DimUsuario_CodigoUniversitario] ON [DimUsuario] ([CodigoUniversitario]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824012206_InicialDW'
)
BEGIN
    CREATE UNIQUE INDEX [IX_EncuestasBasal_UsuarioId_Fase] ON [EncuestasBasal] ([UsuarioId], [Fase]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824012206_InicialDW'
)
BEGIN
    CREATE INDEX [IX_EncuestasSUS_UsuarioId] ON [EncuestasSUS] ([UsuarioId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824012206_InicialDW'
)
BEGIN
    CREATE INDEX [IX_RegistrosDiarios_TiempoId] ON [RegistrosDiarios] ([TiempoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824012206_InicialDW'
)
BEGIN
    CREATE UNIQUE INDEX [IX_RegistrosDiarios_UsuarioId_TiempoId] ON [RegistrosDiarios] ([UsuarioId], [TiempoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824012206_InicialDW'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260824012206_InicialDW', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260927202639_AgregaRespuestaItem9Phq9'
)
BEGIN
    ALTER TABLE [EncuestasBasal] ADD [RespuestaItem9] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260927202639_AgregaRespuestaItem9Phq9'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260927202639_AgregaRespuestaItem9Phq9', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928231510_AgregaConsentimientoYSusDetalle'
)
BEGIN
    DROP INDEX [IX_EncuestasSUS_UsuarioId] ON [EncuestasSUS];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928231510_AgregaConsentimientoYSusDetalle'
)
BEGIN
    ALTER TABLE [EncuestasSUS] ADD [ComentarioLoMasUtil] nvarchar(1000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928231510_AgregaConsentimientoYSusDetalle'
)
BEGIN
    ALTER TABLE [EncuestasSUS] ADD [ComentarioMejoras] nvarchar(1000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928231510_AgregaConsentimientoYSusDetalle'
)
BEGIN
    ALTER TABLE [EncuestasSUS] ADD [RespuestasSUS] nvarchar(40) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928231510_AgregaConsentimientoYSusDetalle'
)
BEGIN
    ALTER TABLE [DimUsuario] ADD [FechaAceptaConsentimiento] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928231510_AgregaConsentimientoYSusDetalle'
)
BEGIN
    ALTER TABLE [DimUsuario] ADD [VersionConsentimiento] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928231510_AgregaConsentimientoYSusDetalle'
)
BEGIN
    CREATE UNIQUE INDEX [IX_EncuestasSUS_UsuarioId] ON [EncuestasSUS] ([UsuarioId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928231510_AgregaConsentimientoYSusDetalle'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260928231510_AgregaConsentimientoYSusDetalle', N'10.0.11');
END;

COMMIT;
GO

