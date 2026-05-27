using API.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace API.Services
{
    public class PdfService : IPdfService
    {
        static PdfService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GenerarPdfReservacion(
            Reservacion reservacion,
            Cliente cliente,
            Paquete paquete,
            decimal tipoCambio,
            decimal porcentajeDescuento)
        {
            return Document.Create(documento =>
            {
                documento.Page(pagina =>
                {
                    pagina.Size(PageSizes.A4);
                    pagina.Margin(40);
                    pagina.DefaultTextStyle(x => x.FontFamily("Calibri").FontSize(11));

                    pagina.Header().Element(h => ComponerEncabezado(h, reservacion));
                    pagina.Content().Element(c => ComponerContenido(c, reservacion, cliente, paquete, tipoCambio, porcentajeDescuento));
                    pagina.Footer().Element(ComponerPie);
                });
            }).GeneratePdf();
        }

        private void ComponerEncabezado(IContainer container, Reservacion reservacion)
        {
            container.Column(columna =>
            {
                columna.Item().Row(fila =>
                {
                    fila.RelativeItem().Column(c =>
                    {
                        c.Item().Text("BEACH.SA - HOTEL & RESORT")
                            .FontSize(22).Bold().FontColor(Colors.Blue.Darken3);
                        c.Item().Text("Factura / Comprobante de Reservación")
                            .FontSize(14).FontColor(Colors.Grey.Darken2);
                        c.Item().Text($"N° FAC-{reservacion.IdReservacion:D4}")
                            .FontSize(12).Bold();
                    });

                    fila.ConstantItem(180).Column(c =>
                    {
                        c.Item().AlignRight().Text($"Fecha: {reservacion.FechaReservacion:dd/MM/yyyy}")
                            .FontSize(10).FontColor(Colors.Grey.Darken2);
                        c.Item().AlignRight().Text($"Hora: {reservacion.FechaReservacion:HH:mm:ss}")
                            .FontSize(10).FontColor(Colors.Grey.Darken2);
                        c.Item().AlignRight().Text($"Reservación N°: {reservacion.IdReservacion}")
                            .FontSize(10).FontColor(Colors.Grey.Darken2);
                    });
                });

                columna.Item().PaddingVertical(8)
                    .LineHorizontal(1).LineColor(Colors.Blue.Darken3);
            });
        }

        private void ComponerContenido(
            IContainer container,
            Reservacion reservacion,
            Cliente cliente,
            Paquete paquete,
            decimal tipoCambio,
            decimal porcentajeDescuento)
        {
            container.Column(columna =>
            {
                // ---- DATOS DEL CLIENTE ----
                columna.Item().PaddingBottom(4)
                    .Background(Colors.Blue.Lighten5)
                    .Padding(8)
                    .Text("DATOS DEL CLIENTE")
                    .FontSize(13).Bold().FontColor(Colors.Blue.Darken3);

                columna.Item().PaddingBottom(12).Row(fila =>
                {
                    fila.RelativeItem().Column(c =>
                    {
                        c.Item().Text($"Cédula: {cliente.TipoCedula ?? ""}-{cliente.Cedula}").FontSize(10);
                        c.Item().Text($"Nombre: {cliente.NombreCompleto}").FontSize(10);
                        c.Item().Text($"Teléfono: {cliente.Telefono ?? "---"}").FontSize(10);
                    });
                    fila.RelativeItem().Column(c =>
                    {
                        c.Item().Text($"Dirección: {cliente.Direccion ?? "---"}").FontSize(10);
                        c.Item().Text($"Email: {cliente.Email}").FontSize(10);
                    });
                });

                // ---- DETALLE DE LA RESERVACIÓN ----
                columna.Item().PaddingBottom(4)
                    .Background(Colors.Blue.Lighten5)
                    .Padding(8)
                    .Text("DETALLE DE LA RESERVACIÓN")
                    .FontSize(13).Bold().FontColor(Colors.Blue.Darken3);

                columna.Item().PaddingBottom(12).Row(fila =>
                {
                    fila.RelativeItem().Column(c =>
                    {
                        c.Item().Text($"Paquete: {paquete.Descripcion}").FontSize(10);
                        c.Item().Text($"Precio por noche: ₡{paquete.PrecioPorNoche:N2}").FontSize(10);
                        c.Item().Text($"Cantidad de noches: {reservacion.CantidadNoches}").FontSize(10);
                    });
                    fila.RelativeItem().Column(c =>
                    {
                        c.Item().Text($"Cantidad de personas: {reservacion.CantidadPersonas}").FontSize(10);
                        c.Item().Text($"Método de pago: {reservacion.MetodoPago}").FontSize(10);

                        if (reservacion.MetodoPago == "Cheque")
                        {
                            c.Item().Text($"No. Cheque: {reservacion.NumeroCheque ?? "---"}").FontSize(10);
                            c.Item().Text($"Banco: {reservacion.BancoCheque ?? "---"}").FontSize(10);
                        }
                    });
                });

                // ---- DESGLOSE ECONÓMICO ----
                columna.Item().PaddingBottom(4)
                    .Background(Colors.Blue.Lighten5)
                    .Padding(8)
                    .Text("DESGLOSE ECONÓMICO")
                    .FontSize(13).Bold().FontColor(Colors.Blue.Darken3);

                decimal montoBruto = paquete.PrecioPorNoche * reservacion.CantidadNoches;
                decimal descuento = reservacion.Descuento;
                decimal montoGravable = montoBruto - descuento;
                decimal iva = reservacion.IVA;
                decimal totalFinal = reservacion.TotalFinal;

                columna.Item().PaddingBottom(8).Table(tabla =>
                {
                    tabla.ColumnsDefinition(columnas =>
                    {
                        columnas.RelativeColumn(3);
                        columnas.RelativeColumn(2);
                        columnas.RelativeColumn(2);
                    });

                    tabla.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5)
                            .Text("Concepto").Bold().FontSize(10);
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5)
                            .Text("Detalle").Bold().FontSize(10);
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5)
                            .AlignRight().Text("Monto (CRC)").Bold().FontSize(10);
                    });

                    void Fila(string concepto, string detalle, decimal monto, bool negrita = false)
                    {
                        var estilo = NegritaIf(negrita);
                        tabla.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Lighten3)
                            .Text(concepto).FontSize(10).Style(estilo);
                        tabla.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Lighten3)
                            .Text(detalle).FontSize(10).Style(estilo);
                        tabla.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Lighten3)
                            .AlignRight().Text($"₡{monto:N2}").FontSize(10).Style(estilo);
                    }

                    Fila("Monto bruto inicial", $"{reservacion.CantidadNoches} noches x ₡{paquete.PrecioPorNoche:N2}", montoBruto);

                    if (porcentajeDescuento > 0)
                    {
                        Fila("Descuento por pronto pago", $"Efectivo - {porcentajeDescuento:F0}% desc.", -descuento);
                    }
                    else
                    {
                        Fila("Descuento aplicado", "Sin descuento", 0);
                    }

                    Fila("Subtotal gravable", $"{montoGravable:N2}", montoGravable);
                    Fila("IVA (13%)", "Impuesto al valor agregado", iva);
                    Fila("TOTAL FINAL", "Monto total en colones", totalFinal, negrita: true);
                });

                // ---- FINANCIAMIENTO ----
                if (paquete.Meses > 0)
                {
                    columna.Item().PaddingBottom(4).PaddingTop(8)
                        .Background(Colors.Blue.Lighten5)
                        .Padding(8)
                        .Text("FINANCIAMIENTO")
                        .FontSize(13).Bold().FontColor(Colors.Blue.Darken3);

                    columna.Item().PaddingBottom(8).Row(fila =>
                    {
                        fila.RelativeItem().Column(c =>
                        {
                            c.Item().Text($"Prima ({paquete.Prima:P0}): ₡{totalFinal * paquete.Prima:N2}").FontSize(10);
                            c.Item().Text($"Meses a financiar: {paquete.Meses}").FontSize(10);
                        });
                        fila.RelativeItem().Column(c =>
                        {
                            c.Item().Text($"Mensualidad: ₡{reservacion.Mensualidad:N2}").FontSize(10);
                        });
                    });
                }

                // ---- TOTALIZACIÓN DUAL ----
                decimal totalUsd = totalFinal / tipoCambio;

                columna.Item().PaddingBottom(4).PaddingTop(8)
                    .Background(Colors.Blue.Lighten5)
                    .Padding(8)
                    .Text("TOTALIZACIÓN DUAL (CRC / USD)")
                    .FontSize(13).Bold().FontColor(Colors.Blue.Darken3);

                columna.Item().Row(fila =>
                {
                    fila.RelativeItem().Padding(4).Border(1).BorderColor(Colors.Blue.Darken3).Padding(10).Column(c =>
                    {
                        c.Item().AlignCenter().Text("COLONES (CRC)").Bold().FontSize(11);
                        c.Item().PaddingTop(6).AlignCenter().Text($"₡{totalFinal:N2}")
                            .FontSize(20).Bold().FontColor(Colors.Blue.Darken3);
                    });
                    fila.RelativeItem().Padding(4).Border(1).BorderColor(Colors.Blue.Darken3).Padding(10).Column(c =>
                    {
                        c.Item().AlignCenter().Text("DÓLARES (USD)").Bold().FontSize(11);
                        c.Item().PaddingTop(6).AlignCenter().Text($"${totalUsd:N2}")
                            .FontSize(20).Bold().FontColor(Colors.Green.Darken3);
                        c.Item().PaddingTop(4).AlignCenter().Text($"T.C. ₡{tipoCambio:N2}")
                            .FontSize(9).FontColor(Colors.Grey.Darken2);
                    });
                });

                columna.Item().PaddingTop(6)
                    .Text("Tipo de cambio obtenido del Banco Central de Costa Rica (BCCR)")
                    .FontSize(8).FontColor(Colors.Grey.Darken2).Italic();
            });
        }

        private void ComponerPie(IContainer container)
        {
            container.Column(columna =>
            {
                columna.Item().PaddingVertical(8)
                    .LineHorizontal(1).LineColor(Colors.Blue.Darken3);

                columna.Item().AlignCenter()
                    .Text("¡Gracias por preferir Beach.SA - Hotel & Resort! • Tel: (506) 2222-0000")
                    .FontSize(9).FontColor(Colors.Grey.Darken2);

                columna.Item().AlignCenter()
                    .Text("Este documento es un comprobante de reservación. Conservar para efectos de garantía.")
                    .FontSize(8).FontColor(Colors.Grey.Lighten1);
            });
        }

        private static TextStyle NegritaIf(bool condicion)
        {
            return condicion ? TextStyle.Default.Bold() : TextStyle.Default;
        }
    }
}
