-- ============================================================
-- Script: CreateTablas_Seguridad.sql
-- Proyecto: Beach.SA - Hotel & Resort
-- Base de datos: BeachDB
-- Descripcion: Tablas del esquema de seguridad JWT
--              (Roles, Funciones, RolFunciones, Usuarios, Auditoria).
-- NOTA: Pegar este bloque al FINAL de CreateDatabase.sql para que
--       el script reconstruya toda la base sin depender de las
--       migraciones de Entity Framework.
-- ============================================================

USE [BeachDB];
GO

-- ============================================================
-- TABLA: Roles
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Roles]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[Roles] (
        [IdRol]        INT            IDENTITY(1,1) NOT NULL,
        [Nombre]       NVARCHAR(50)   NOT NULL,
        [Descripcion]  NVARCHAR(200)  NULL,
        [Estado]       BIT            NOT NULL CONSTRAINT [DF_Roles_Estado] DEFAULT (1),

        CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([IdRol]),
        CONSTRAINT [UQ_Roles_Nombre] UNIQUE ([Nombre])
    );
END
GO

-- ============================================================
-- TABLA: Funciones
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Funciones]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[Funciones] (
        [IdFuncion]  INT            IDENTITY(1,1) NOT NULL,
        [Codigo]     NVARCHAR(50)   NOT NULL,
        [Nombre]     NVARCHAR(100)  NOT NULL,
        [Url]        NVARCHAR(100)  NULL,
        [Orden]      INT            NOT NULL CONSTRAINT [DF_Funciones_Orden] DEFAULT (0),
        [Estado]     BIT            NOT NULL CONSTRAINT [DF_Funciones_Estado] DEFAULT (1),

        CONSTRAINT [PK_Funciones] PRIMARY KEY CLUSTERED ([IdFuncion]),
        CONSTRAINT [UQ_Funciones_Codigo] UNIQUE ([Codigo])
    );
END
GO

-- ============================================================
-- TABLA: Usuarios
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Usuarios]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[Usuarios] (
        [IdUsuario]      INT            IDENTITY(1,1) NOT NULL,
        [Email]          NVARCHAR(100)  NOT NULL,
        [Password]       NVARCHAR(256)  NOT NULL,
        [IdRol]          INT            NOT NULL,
        [FechaCreacion]  DATETIME2      NOT NULL CONSTRAINT [DF_Usuarios_FechaCreacion] DEFAULT (GETDATE()),
        [Estado]         BIT            NOT NULL CONSTRAINT [DF_Usuarios_Estado] DEFAULT (1),

        CONSTRAINT [PK_Usuarios] PRIMARY KEY CLUSTERED ([IdUsuario]),
        CONSTRAINT [UQ_Usuarios_Email] UNIQUE ([Email]),
        CONSTRAINT [FK_Usuarios_Roles] FOREIGN KEY ([IdRol])
            REFERENCES [dbo].[Roles] ([IdRol])
            ON DELETE NO ACTION
    );

    CREATE NONCLUSTERED INDEX [IX_Usuarios_IdRol]
        ON [dbo].[Usuarios] ([IdRol]);
END
GO

-- ============================================================
-- TABLA: RolFunciones (tabla puente Rol <-> Funcion)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RolFunciones]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[RolFunciones] (
        [IdRolFuncion]  INT  IDENTITY(1,1) NOT NULL,
        [IdRol]         INT  NOT NULL,
        [IdFuncion]     INT  NOT NULL,

        CONSTRAINT [PK_RolFunciones] PRIMARY KEY CLUSTERED ([IdRolFuncion]),
        CONSTRAINT [FK_RolFunciones_Roles] FOREIGN KEY ([IdRol])
            REFERENCES [dbo].[Roles] ([IdRol])
            ON DELETE CASCADE,
        CONSTRAINT [FK_RolFunciones_Funciones] FOREIGN KEY ([IdFuncion])
            REFERENCES [dbo].[Funciones] ([IdFuncion])
            ON DELETE CASCADE,
        CONSTRAINT [UQ_RolFunciones_Rol_Funcion] UNIQUE ([IdRol], [IdFuncion])
    );

    CREATE NONCLUSTERED INDEX [IX_RolFunciones_IdFuncion]
        ON [dbo].[RolFunciones] ([IdFuncion]);
END
GO

-- ============================================================
-- TABLA: Auditoria
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Auditoria]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[Auditoria] (
        [IdAuditoria]  INT            IDENTITY(1,1) NOT NULL,
        [Usuario]      NVARCHAR(100)  NULL,
        [Accion]       NVARCHAR(50)   NULL,
        [Entidad]      NVARCHAR(100)  NULL,
        [Detalle]      NVARCHAR(500)  NULL,
        [Ip]           NVARCHAR(50)   NULL,
        [Fecha]        DATETIME2      NOT NULL CONSTRAINT [DF_Auditoria_Fecha] DEFAULT (GETDATE()),

        CONSTRAINT [PK_Auditoria] PRIMARY KEY CLUSTERED ([IdAuditoria])
    );
END
GO
