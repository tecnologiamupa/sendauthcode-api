 

namespace MicroserviceWhatsapp.Application.Template
{
    public class ConfirmacionCitas
    {
        public string Name_patient  { get; set; }
        public string Date_Cita { get; set; }
        public string Cedula { get; set; }
        public string Hora { get; set; }
        public string Ubicacion { get; set; }
        public string Especialidad { get; set; }
        public string NbrWhatsApp { get; set; }
        public string Codigo_Cita { get; set; }
        public string UsuarioMensajeEnviado { get; set; }

        public ConfirmacionCitas(string name_patient, string date_Cita, string cedula, string hora, string ubicacion, string especialidad, string nbrWhatsApp, string codigo_Cita, string usuarioMensajeEnviado)
        {
            Name_patient = name_patient;
            Date_Cita = date_Cita;
            Cedula = cedula;
            Hora = hora;
            Ubicacion = ubicacion;
            Especialidad = especialidad;
            NbrWhatsApp = nbrWhatsApp;
            Codigo_Cita = codigo_Cita;
            UsuarioMensajeEnviado = usuarioMensajeEnviado;
        }
    }
}
