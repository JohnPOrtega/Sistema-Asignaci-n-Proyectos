using BE.Proyectos;
using BE.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Asignacion
    {
        public Asignacion(int proyectoID, Empleado empleado, RolEmpleado rol)
        {
            ProyectoID = proyectoID;
            Empleado = empleado;
            this.rol = rol;
        }

        public int ProyectoID { get; set; }

        public Empleado Empleado { get; set; }

        public RolEmpleado rol {  get; set; }
    }
}
