using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{

    public enum TipoEvento
    {
        Info,
        Advertencia,
        Error,
        Critico
    }
    public class BitacoraEvento
    {
       
        public DateTime FechaHora { get; set; }
        public string Descripcion { get; set; }
        public string Usuario { get; set; }
        public TipoEvento Tipo { get; set; }
        public string Origen { get; set; }

        public BitacoraEvento(DateTime fechaHora, string descripcion, string usuario, TipoEvento tipo, string origen)
        {
            
            FechaHora = fechaHora;
            Descripcion = descripcion;
            Usuario = usuario;
            Tipo = tipo;
            Origen = origen;
        }

        public BitacoraEvento()
        {
                
        }
    }
}
