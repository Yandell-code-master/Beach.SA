-- Este codigo solo sirve para crear la base de datos en la nube, si se intenta ejecutar esto en local daria un error si no se tiene creada la base de datos de antemano

-- No se deb utilzar este codigo para intentar crear la base de datos en local, es para la nube, como solkamente lo voy a crear una vez no necesito que funcione mas veces


DELETE FROM [dbo].[Facturas];
DELETE FROM [dbo].[Reservaciones];
DELETE FROM [dbo].[Paquetes];
DELETE FROM [dbo].[Clientes];


select * From Reservaciones

-- 2. Inserta Clientes
INSERT INTO [dbo].[Clientes] ([Cedula], [TipoCedula], [NombreCompleto], [Telefono], [Direccion], [Email])
VALUES ('118230456', 'Física', 'Juan Pérez Solano', '8888-1122', 'Paquera, Puntarenas', 'juan.perez@correo.com'),
       ('155832704534', 'Jurídica', 'Inversiones Turísticas del Pacífico S.A.', '2650-0099', 'Cóbano, Puntarenas', 'contacto@beachsa.com');

-- 3. Inserta Paquetes
INSERT INTO [dbo].[Paquetes] ([Descripcion], [PrecioPorNoche], [Prima], [Meses], [Estado])
VALUES ('Todo incluido', 450.00, 0.45, 24, 1),
       ('Alimentación', 275.00, 0.35, 18, 1),
       ('Hospedaje', 210.00, 0.15, 12, 1);


-- 4. Inserta Reservaciones 
INSERT INTO [dbo].[Reservaciones] ([Cedula], [IdPaquete], [CantidadNoches], [CantidadPersonas], [MetodoPago], [SubTotal], [Descuento], [IVA], [TotalFinal], [Prima], [Mensualidad], [TipoCambio], [TotalDolares])
VALUES ('118230456', 9, 5, 1, 'Efectivo', 2250.00, 225.00, 263.25, 2288.25, 1029.71, 52.44, 520.50, 4.40),
       ('155832704534', 10, 4, 1, 'Tarjeta', 840.00, 0.00, 109.20, 949.20, 142.38, 67.24, 520.50, 1.82);


-- 5. Inserta Facturas 
INSERT INTO [dbo].[Facturas] ([IdReservacion], [Cedula], [NombreCompleto], [CorreoElectronico], [Telefono], [NombrePaquete], [PrecioPorNoche], [CantidadNoches], [CantidadPersonas], [SubTotal], [Descuento], [PorcentajeDescuento], [MontoGravable], [IVA], [TotalFinal], [TipoCambio], [TotalDolares], [MetodoPago])
VALUES 
(6, '118230456', 'Juan Pérez Solano', 'juan.perez@correo.com', '8888-1122', 'Todo incluido', 450.00, 5, 1, 2250.00, 225.00, 10.00, 2025.00, 263.25, 2288.25, 520.50, 4.40, 'Efectivo'),
(7, '155832704534', 'Inversiones Turísticas del Pacífico S.A.', 'contacto@beachsa.com', '2650-0099', 'Hospedaje', 210.00, 4, 1, 840.00, 0.00, 0.00, 840.00, 109.20, 949.20, 520.50, 1.82, 'Tarjeta');
GO