using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE.Usuarios;

namespace BE.Proyectos
{
    public enum TipoModulo
    {
        Departamento,
        Equipo,
    }
    public abstract class Modulo
    {
        public int ID;
        public int ProyectoID;
        public Modulo Padre;
        public Empleado Responsable;
        public string Nombre;

        public int? PadreID_DB { get; set; }
        public int ResponsableID_DB { get; set; }

        protected Modulo(int proyectoID, Modulo padre, Empleado responsable, string nombre)
        {
            ProyectoID = proyectoID;
            Padre = padre;
            Responsable = responsable;
            Nombre = nombre;
        }

        protected Modulo(int proyectoID, Empleado responsable, string nombre)
        {
            ProyectoID = proyectoID;
            Responsable = responsable;
            Nombre = nombre;
        }

        public abstract bool vacio();

        public abstract bool Agregar<T>(T item, Modulo padre);

        public abstract bool Eliminar<T>(T item);

        public override bool Equals(object obj)
        {
            if (!(obj is Modulo other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ID == other.ID;
        }

        public override int GetHashCode() => ID.GetHashCode();

        public static bool operator ==(Modulo a, Modulo b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(Modulo a, Modulo b) => !(a == b);
    }
}
