USE [BeachDB];
GO

SET IDENTITY_INSERT [dbo].[Roles] ON;
INSERT INTO [dbo].[Roles] ([IdRol],[Nombre],[Descripcion],[Estado]) VALUES
 (1, N'Administrador', N'Acceso total al sistema', 1),
 (2, N'Vendedor', N'Gestiona clientes y reservaciones', 1);
SET IDENTITY_INSERT [dbo].[Roles] OFF;
GO

SET IDENTITY_INSERT [dbo].[Funciones] ON;
INSERT INTO [dbo].[Funciones] ([IdFuncion],[Codigo],[Nombre],[Url],[Orden],[Estado]) VALUES
 (1, N'CLIENTES',      N'Gestión de Clientes',   N'/Cliente/Index',     1, 1),
 (2, N'PAQUETES',      N'Gestión de Paquetes',   N'/Paquete/Index',     2, 1),
 (3, N'RESERVACIONES', N'Reservaciones',         N'/Reservacion/Index', 3, 1),
 (4, N'FACTURAS',      N'Facturas',              N'/Factura/Index',     4, 1),
 (5, N'USUARIOS',      N'Gestión de Usuarios',   N'/Usuario/Index',     5, 1),
 (6, N'ROLES',         N'Gestión de Roles',      N'/Rol/Index',         6, 1);
SET IDENTITY_INSERT [dbo].[Funciones] OFF;
GO

-- Administrador: todas las funciones
INSERT INTO [dbo].[RolFunciones] ([IdRol],[IdFuncion])
SELECT 1, IdFuncion FROM [dbo].[Funciones];

-- Vendedor: Clientes, Reservaciones, Facturas
INSERT INTO [dbo].[RolFunciones] ([IdRol],[IdFuncion]) VALUES (2,1),(2,3),(2,4);
GO

-- Usuario administrador inicial (contraseña en texto plano, igual que el ejemplo de clase)
INSERT INTO [dbo].[Usuarios] ([Email],[Password],[IdRol],[FechaCreacion],[Estado])
VALUES (N'admin@beach.sa', N'Admin123', 1, GETDATE(), 1);
GO