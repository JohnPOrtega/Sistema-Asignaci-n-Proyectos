using BE.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE.Proyectos
{
    public class Departamento : Modulo
    {
        public List<Modulo> Modulos { get; set; }

        public Departamento(int proyectoID, Modulo padre, Empleado responsable, string nombre) : base(proyectoID, padre, responsable, nombre)
        {
            this.Modulos = new List<Modulo>();
        }

        public Departamento(int proyectoID, Modulo padre, Empleado responsable, string nombre, List<Modulo> modulos) : base(proyectoID, padre, responsable, nombre)
        {
            this.Modulos = modulos;
        }

        public override bool vacio()
        {
            return (Modulos.Count == 0 || Modulos == null);
        }

        public override bool Agregar<T>(T item, Modulo padre)
        {
            if (this == padre)
            {
                if (item is Modulo nuevoModulo)
                {
                    Modulos.Add(nuevoModulo);
                    nuevoModulo.Padre = this;
                    return true;
                }

                // no se pueden agregar empleados
                return false;
            }

            foreach (var m in Modulos.ToList())
            {
                if (m.Agregar(item, padre))
                    return true;
            }

            return false;
        }

        public override bool Eliminar<T>(T item)
        {
            if (item is Modulo modulo)
                if ( Modulos.Remove(modulo) ) return true;

            foreach (var m in Modulos.ToList())
                if ( m.Eliminar(item) ) return true;

            return false;
        }
    }
}
