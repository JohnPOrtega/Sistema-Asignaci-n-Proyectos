using BE;
using BE.Usuarios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE.Proyectos
{

    public class Proyecto
    {
        [Browsable(false)]
        public int ID { get; set; }
        public string nombre { get; set; }
        public DateTime fechaSuscripcion { get; set; }
        public DateTime fechaEstimada { get; set; }
        public string lenguaje { get; set; }
        public string plataforma { get; set; }

        public int IngenieroID { get; set; }

        [Browsable(false)]
        public Cliente solicitante { get; set; }

        [DisplayName("Cliente")]
        public string ClienteCompleto => $"{solicitante?.Nombre} {solicitante?.Apellido}";

        public bool Vencido
        {
            get
            {
                return DateTime.Today >= fechaEstimada;
            }
        }
        
        [System.ComponentModel.Browsable(false)]
        public List<Modulo> Estructura;

        public Proyecto(int id, string nombre, DateTime fechaestimada, string lenguaje, string plataforma, Cliente solicitante)
        {
            this.ID = id;
            this.nombre = nombre;
            this.fechaSuscripcion = DateTime.Today;
            this.fechaEstimada = fechaestimada;
            this.lenguaje = lenguaje;
            this.plataforma = plataforma;
            this.solicitante = solicitante;
            this.Estructura = new List<Modulo>();
        }

        public Proyecto(int id, string nombre, DateTime fechasub, DateTime fechaestimada, string lenguaje, string plataforma, Cliente solicitante, List<Modulo> estructura,int ingenieroID)
        {
            this.ID = id;
            this.nombre = nombre;
            this.fechaSuscripcion = fechasub;
            this.fechaEstimada = fechaestimada;
            this.lenguaje = lenguaje;
            this.plataforma = plataforma;
            this.solicitante = solicitante;
            this.IngenieroID = ingenieroID;
            this.Estructura = estructura;
        }
        public Proyecto(string nombre, DateTime fechasub, DateTime fechaestimada, string lenguaje, string plataforma, Cliente solicitante,int IngId)
        {
            this.nombre = nombre;
            this.fechaSuscripcion = fechasub;
            this.fechaEstimada = fechaestimada;
            this.lenguaje = lenguaje;
            this.plataforma = plataforma;
            this.solicitante = solicitante;
            this.IngenieroID = IngId;
            this.Estructura = new List<Modulo>(); 

        }
        public bool Agregar<T>(T item, Modulo padre)
        {
            //agregar módulo raíz
            if (padre == null && item is Modulo nuevoModulo)
            {
                Estructura.Add(nuevoModulo);
                return true;
            }

            //delegar al modulo padre
            foreach (var m in Estructura)
            {
                if (m.Agregar(item, padre))
                    return true;
            }

            return false;
        }

        public bool ModificarModulo(Modulo nuevo)
        {
            return ActualizarModulo(Estructura, nuevo);
        }

        private bool ActualizarModulo(List<Modulo> modulos, Modulo nuevo)
        {
            foreach (var modulo in modulos)
            {
                if(modulo == nuevo)
                {
                    modulo.Nombre = nuevo.Nombre;
                    modulo.Responsable = nuevo.Responsable;
                    modulo.ResponsableID_DB = nuevo.Responsable.ID;
                    return true;
                }

                if(modulo is Departamento dep && ActualizarModulo(dep.Modulos, nuevo)) return true;
            }
            return false;
        }

        public bool Eliminar<T>(T item)
        {
            if (item is Modulo modulo && Estructura.Remove(modulo)) return true;

            foreach (var m in Estructura) if (m.Eliminar(item)) return true;

            return false;
        }

    }
}
