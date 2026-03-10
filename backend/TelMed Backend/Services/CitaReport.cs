using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TelMedAPI.Models;

namespace TelMedAPI.Services
{
    public class CitaReport : IDocument
    {
        public Cita Model { get; }

        public CitaReport(Cita model)
        {
            Model = model;
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(1, Unit.Centimetre);

                page.Header()
                    .Text("Comprobante de Cita Médica")
                    .FontSize(20)
                    .SemiBold()
                    .FontColor(Colors.Blue.Medium);

                page.Content().Column(col =>
                {
                    col.Spacing(10);

                    col.Item().Text($"Paciente: {Model.Paciente?.Nombre} {Model.Paciente?.Apellido}");

                    col.Item().Text($"Fecha Inicio: {Model.FechaInicio:dd/MM/yyyy HH:mm}");

                    col.Item().Text($"Fecha Fin: {Model.FechaFin:dd/MM/yyyy HH:mm}");

                    col.Item().Text($"Estado: {Model.Estado}");

                    col.Item().Text($"Motivo: {Model.Motivo}");
                });

                page.Footer()
                    .AlignCenter()
                    .Text(txt =>
                    {
                        txt.Span("TelMedAPI - Generado el ");
                        txt.Span($"{DateTime.Now:dd/MM/yyyy HH:mm}");
                    });
            });
        }
    }
}