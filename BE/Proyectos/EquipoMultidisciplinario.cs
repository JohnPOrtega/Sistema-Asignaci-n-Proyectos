using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE.Usuarios;

namespace BE.Proyectos
{
    public class EquipoMultidisciplinario : Modulo
    {
        public List<Empleado> Integrantes;

        public EquipoMultidisciplinario(int proyectoID, Modulo padre, Empleado responsable, string nombre) : base(proyectoID, padre, responsable, nombre)
        {
            this.Integrantes = new List<Empleado>();
        }

        public EquipoMultidisciplinario(int proyectoID, Modulo padre, Empleado responsable, string nombre, List<Empleado> integrantes) : base(proyectoID, padre, responsable, nombre)
        {
            this.Integrantes = integrantes;
        }

        public override bool Agregar<T>(T item, Modulo padre)
        {
            if (this == padre)
            {
                if (item is Empleado nuevoEmpleado)
                {
                    Integrantes.Add(nuevoEmpleado);
                    return true;
                }

                // No se puede agregar módulos dentro de equipos
                return false;
            }

            return false;
        }

        public override bool Eliminar<T>(T item)
        {
            if(item is Empleado emp)return Integrantes.Remove(emp);
            return false;
        }

        public override bool vacio()
        {
            return (Integrantes.Count != 0 || Integrantes == null);
        }
    }
}
