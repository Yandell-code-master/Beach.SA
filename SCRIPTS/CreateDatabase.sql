-- ============================================================
-- Script: CreateDatabase.sql
-- Proyecto: Beach.SA - Hotel & Resort
-- Base de datos: BeachDB
-- Motor: SQL Server
-- Descripcion: Creacion completa de la base de datos con
--              tablas, PKs, FKs, DEFAULTs, CHECKs e indices.
-- ============================================================

-- Crear la base de datos si no existe
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'BeachDB')
BEGIN
    CREATE DATABASE [BeachDB];
END
GO

USE [BeachDB];
GO

-- ============================================================
-- TABLA: Clientes
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Clientes]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[Clientes] (
        [Cedula]          NVARCHAR(20)  NOT NULL,
        [TipoCedula]      NVARCHAR(10)  NULL,
        [NombreCompleto]  NVARCHAR(100) NULL,
        [Telefono]        NVARCHAR(20)  NULL,
        [Direccion]       NVARCHAR(200) NULL,
        [Email]           NVARCHAR(100) NOT NULL,

        CONSTRAINT [PK_Clientes] PRIMARY KEY CLUSTERED ([Cedula]),
        CONSTRAINT [UQ_Clientes_Email] UNIQUE ([Email]),
        CONSTRAINT [CK_Clientes_Cedula] CHECK ([Cedula] != '')
    );
END
GO

-- ============================================================
-- TABLA: Paquetes
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Paquetes]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[Paquetes] (
        [IdPaquete]       INT            IDENTITY(1,1) NOT NULL,
        [Descripcion]     NVARCHAR(100)  NOT NULL,
        [PrecioPorNoche]  DECIMAL(18,2)  NOT NULL,
        [Prima]           DECIMAL(18,2)  NOT NULL,
        [Meses]           INT            NOT NULL,
        [Estado]          BIT            NOT NULL CONSTRAINT [DF_Paquetes_Estado] DEFAULT (1),

        CONSTRAINT [PK_Paquetes] PRIMARY KEY CLUSTERED ([IdPaquete]),
        CONSTRAINT [CK_Paquetes_PrecioPorNoche] CHECK ([PrecioPorNoche] > 0),
        CONSTRAINT [CK_Paquetes_Prima] CHECK ([Prima] >= 0 AND [Prima] <= 1),
        CONSTRAINT [CK_Paquetes_Meses] CHECK ([Meses] > 0)
    );
END
GO

-- ============================================================
-- TABLA: Reservaciones
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Reservaciones]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[Reservaciones] (
        [IdReservacion]    INT            IDENTITY(1,1) NOT NULL,
        [Cedula]           NVARCHAR(20)   NOT NULL,
        [IdPaquete]        INT            NOT NULL,
        [CantidadNoches]   INT            NOT NULL,
        [CantidadPersonas] INT            NOT NULL,
        [MetodoPago]       NVARCHAR(20)   NOT NULL,
        [NumeroCheque]     NVARCHAR(50)   NULL,
        [BancoCheque]      NVARCHAR(100)  NULL,
        [SubTotal]         DECIMAL(18,2)  NOT NULL CONSTRAINT [DF_Reservaciones_SubTotal] DEFAULT (0),
        [Descuento]        DECIMAL(18,2)  NOT NULL CONSTRAINT [DF_Reservaciones_Descuento] DEFAULT (0),
        [IVA]              DECIMAL(18,2)  NOT NULL CONSTRAINT [DF_Reservaciones_IVA] DEFAULT (0),
        [TotalFinal]       DECIMAL(18,2)  NOT NULL CONSTRAINT [DF_Reservaciones_TotalFinal] DEFAULT (0),
        [Prima]            DECIMAL(18,2)  NOT NULL CONSTRAINT [DF_Reservaciones_Prima] DEFAULT (0),
        [Mensualidad]      DECIMAL(18,2)  NOT NULL CONSTRAINT [DF_Reservaciones_Mensualidad] DEFAULT (0),
        [TipoCambio]       DECIMAL(18,2)  NOT NULL CONSTRAINT [DF_Reservaciones_TipoCambio] DEFAULT (0),
        [TotalDolares]     DECIMAL(18,2)  NOT NULL CONSTRAINT [DF_Reservaciones_TotalDolares] DEFAULT (0),
        [FechaReservacion] DATETIME2      NOT NULL CONSTRAINT [DF_Reservaciones_FechaReservacion] DEFAULT (GETDATE()),

        CONSTRAINT [PK_Reservaciones] PRIMARY KEY CLUSTERED ([IdReservacion]),
        CONSTRAINT [FK_Reservaciones_Clientes] FOREIGN KEY ([Cedula])
            REFERENCES [dbo].[Clientes] ([Cedula])
            ON DELETE CASCADE,
        CONSTRAINT [FK_Reservaciones_Paquetes] FOREIGN KEY ([IdPaquete])
            REFERENCES [dbo].[Paquetes] ([IdPaquete])
            ON DELETE CASCADE,
        CONSTRAINT [CK_Reservaciones_CantidadNoches] CHECK ([CantidadNoches] > 0),
        CONSTRAINT [CK_Reservaciones_CantidadPersonas] CHECK ([CantidadPersonas] > 0),
        CONSTRAINT [CK_Reservaciones_MetodoPago] CHECK ([MetodoPago] IN ('Efectivo', 'Tarjeta', 'Cheque'))
    );

    CREATE NONCLUSTERED INDEX [IX_Reservaciones_Cedula]
        ON [dbo].[Reservaciones] ([Cedula]);

    CREATE NONCLUSTERED INDEX [IX_Reservaciones_IdPaquete]
        ON [dbo].[Reservaciones] ([IdPaquete]);
END
GO

-- ============================================================
-- TABLA: Facturas
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Facturas]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[Facturas] (
        [IdFactura]          INT            IDENTITY(1,1) NOT NULL,
        [IdReservacion]      INT            NOT NULL,
        [Cedula]             NVARCHAR(20)   NOT NULL,
        [NombreCompleto]     NVARCHAR(100)  NULL,
        [CorreoElectronico]  NVARCHAR(100)  NULL,
        [Telefono]           NVARCHAR(20)   NULL,
        [NombrePaquete]      NVARCHAR(100)  NULL,
        [PrecioPorNoche]     DECIMAL(18,2)  NOT NULL CONSTRAINT [DF_Facturas_PrecioPorNoche] DEFAULT (0),
        [CantidadNoches]     INT            NOT NULL CONSTRAINT [DF_Facturas_CantidadNoches] DEFAULT (0),
        [CantidadPersonas]   INT            NOT NULL CONSTRAINT [DF_Facturas_CantidadPersonas] DEFAULT (0),
        [SubTotal]           DECIMAL(18,2)  NOT NULL CONSTRAINT [DF_Facturas_SubTotal] DEFAULT (0),
        [Descuento]          DECIMAL(18,2)  NOT NULL CONSTRAINT [DF_Facturas_Descuento] DEFAULT (0),
        [PorcentajeDescuento] DECIMAL(18,2) NOT NULL CONSTRAINT [DF_Facturas_PorcentajeDescuento] DEFAULT (0),
        [MontoGravable]      DECIMAL(18,2)  NOT NULL CONSTRAINT [DF_Facturas_MontoGravable] DEFAULT (0),
        [IVA]                DECIMAL(18,2)  NOT NULL CONSTRAINT [DF_Facturas_IVA] DEFAULT (0),
        [TotalFinal]         DECIMAL(18,2)  NOT NULL CONSTRAINT [DF_Facturas_TotalFinal] DEFAULT (0),
        [TipoCambio]         DECIMAL(18,4)  NOT NULL CONSTRAINT [DF_Facturas_TipoCambio] DEFAULT (0),
        [TotalDolares]       DECIMAL(18,2)  NOT NULL CONSTRAINT [DF_Facturas_TotalDolares] DEFAULT (0),
        [MetodoPago]         NVARCHAR(20)   NULL,
        [FechaEmision]       DATETIME2      NOT NULL CONSTRAINT [DF_Facturas_FechaEmision] DEFAULT (GETDATE()),

        CONSTRAINT [PK_Facturas] PRIMARY KEY CLUSTERED ([IdFactura]),
        CONSTRAINT [FK_Facturas_Reservaciones] FOREIGN KEY ([IdReservacion])
            REFERENCES [dbo].[Reservaciones] ([IdReservacion])
            ON DELETE NO ACTION,
        CONSTRAINT [CK_Facturas_CantidadNoches] CHECK ([CantidadNoches] >= 0),
        CONSTRAINT [CK_Facturas_CantidadPersonas] CHECK ([CantidadPersonas] >= 0),
        CONSTRAINT [CK_Facturas_PorcentajeDescuento] CHECK ([PorcentajeDescuento] >= 0)
    );

    CREATE NONCLUSTERED INDEX [IX_Facturas_IdReservacion]
        ON [dbo].[Facturas] ([IdReservacion]);
END
GO
