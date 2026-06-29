-- Solamente funciona para ejecutar en la nube


-- ============================================================
-- TABLA: Usuarios
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Usuarios]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[Usuarios] (
        [IdUsuario]      INT            IDENTITY(1,1) NOT NULL,
        [Email]          NVARCHAR(100)  NOT NULL,
        [Password]       NVARCHAR(256)  NOT NULL,
        [Estado]         BIT            NOT NULL CONSTRAINT [DF_Usuarios_Estado] DEFAULT (1),

    );
END
GO

