using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TelMedAPI.Models;

namespace TelMedAPI.Services
{
    public class CitaReport : IDocument
    {
        public Cita CitaData { get; }
        public Consulta ConsultaData { get; }

      public CitaReport(Cita cita, Consulta consulta)
        {
            CitaData = cita;
            ConsultaData = consulta;
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

                    col.Item().Text($"Paciente: {CitaData.Paciente?.Nombre} {CitaData.Paciente?.Apellido}");
                    col.Item().Text($"Fecha Inicio: {CitaData.FechaInicio:dd/MM/yyyy HH:mm}");
                    col.Item().Text($"Fecha Fin: {CitaData.FechaFin:dd/MM/yyyy HH:mm}");
                    col.Item().Text($"Estado: {CitaData.Estado}");
                    col.Item().Text($"Motivo: {CitaData.Motivo}");
                    //Campos Médico
                    col.Item().Text($"Diagnóstico: {ConsultaData.Diagnostico}");
                    col.Item().Text($"Síntomas: {ConsultaData.Sintomas}");
                    col.Item().Text($"Tratamiento: {ConsultaData.Tratamiento}");
                    col.Item().Text($"Observaciones: {ConsultaData.Observaciones}");
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