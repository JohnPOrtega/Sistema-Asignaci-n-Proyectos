using BE;
using DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BLL
{
    public class BitacoraEventoBLL
    {
        private BitacoraEventoDAL dal = new BitacoraEventoDAL();

        public bool RegistrarEvento(DateTime fechaHora, string descripcion, string usuario, TipoEvento tipo, string origen)
        {
            BitacoraEvento nuevoEvento = new BitacoraEvento(fechaHora, descripcion, usuario, tipo, origen);
            return dal.RegistrarEvento(nuevoEvento);
        }

        public List<BitacoraEvento> GetAll()
        {
            return dal.ObtenerTodos();
        }

        public bool ExportarUnEvento(BitacoraEvento evento, string ruta)
        {
            XmlSerializer serializador = new XmlSerializer(typeof(BitacoraEvento));

            using (FileStream fs = new FileStream(ruta, FileMode.Create))
            {
                serializador.Serialize(fs, evento);
                return true;
            }
        }


        public bool ExportarMuchos(List<BitacoraEvento> eventos, string ruta)
        {
            XmlSerializer serializador = new XmlSerializer(typeof(List<BitacoraEvento>));

            using (FileStream fs = new FileStream(ruta, FileMode.Create))
            {
                serializador.Serialize(fs, eventos);
                return true;
            }
        }
    }
}
