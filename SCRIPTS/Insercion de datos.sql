USE BeachDB;

INSERT INTO [dbo].[Clientes] 
(
    [Cedula], 
    [TipoCedula], 
    [NombreCompleto], 
    [Telefono], 
    [Direccion], 
    [Email]
)
VALUES 
(
    '118230456',                               -- Cedula (PK, nvarchar)
    'Física',                                  -- TipoCedula (No NULL)
    'Juan Pérez Solano',                       -- NombreCompleto (No NULL)
    '8888-1122',                               -- Telefono (Permite NULL)
    'Paquera, Puntarenas, Costa Rica',         -- Direccion (Permite NULL)
    'juan.perez@correo.com'                    -- Email (No NULL)
),
(
    '155832704534',                             -- Cedula larga (Prueba Jurídica de GOMETA)
    'Jurídica',                                -- TipoCedula
    'Inversiones Turísticas del Pacífico S.A.',-- NombreCompleto
    '2650-0099',                               -- Telefono
    'Cóbano, Puntarenas, Costa Rica',          -- Direccion
    'contacto@beachsa.com'                     -- Email
);
GO

-- Asegúrate de estar posicionado en la base de datos correcta de tu proyecto
-- USE [TuBaseDeDatosBeach];
-- GO

-- NOTA: Si tu columna 'IdPaquetes' es IDENTITY (autonumérica), remueve el campo [IdPaquete] y sus números de la lista.
INSERT INTO [dbo].[Paquetes] 
(
    [Descripcion], 
    [PrecioPorNoche], 
    [Prima], 
    [Meses], 
    [Estado]
)
VALUES 
( 
    'Todo incluido',   -- Descripcion (nvarchar)
    450.00,            -- PrecioPorNoche ($450 por persona/noche)
    45.00,             -- Prima (45% de prima obligatoria requerido)
    24,                -- Meses (Resto financiado a 24 mensualidades)
    1                  -- Estado (1 = bit True / Activo)
),
(
    'Alimentación', 
    275.00,            -- PrecioPorNoche ($275 por persona/noche)
    35.00,             -- Prima (35% de prima obligatoria requerido)
    18,                -- Meses (Resto financiado a 18 mensualidades)
    1                  -- Estado (Activo)
),
(
    'Hospedaje', 
    210.00,            -- PrecioPorNoche ($210 por persona/noche)
    15.00,             -- Prima (15% de prima obligatoria requerido)
    12,                -- Meses (Resto financiado a 12 mensualidades)
    1                  -- Estado (Activo)
);
GO

-- Asegúrate de estar posicionado en la base de datos de tu proyecto
-- USE [TuBaseDeDatosBeach];
-- GO

-- 2. AHORA SÍ PODEMOS INSERTAR LAS RESERVACIONES SIN CONFLICTO
INSERT INTO [dbo].[Reservaciones] 
(
    [Cedula], [IdPaquete], [CantidadNoches], [CantidadPersonas], [MetodoPago], 
    [NumeroCheque], [BancoCheque], [SubTotal], [Descuento], [IVA], 
    [TotalFinal], [Prima], [Mensualidad], [TipoCambio], [TotalDolares], [FechaReservacion]
)
VALUES 
(
    '118230456', 1, 5, 1, 'Efectivo', NULL, NULL, 
    2250.00, 225.00, 263.25, 2288.25, 1029.71, 52.44, 520.50, 4.40, '2026-05-27 14:00:00'
),
(
    '155832704534', 3, 4, 1, 'Tarjeta', NULL, NULL, 
    840.00, 0.00, 109.20, 949.20, 142.38, 67.24, 520.50, 1.82, '2026-06-15 11:30:00'
);
GO
