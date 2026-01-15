using BE.Proyectos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE.Usuarios
{
    public class Empleado : User
    {
        public Empleado()
        {
        }

        public RolEmpleado? Rol { get; set; }

        public Empleado(string nombre, string apellido, int dni, string email, string hash, string salt) : base(nombre, apellido, dni, email, hash, salt, UserRole.Empleado) { }

        public Empleado(int id, string nombre, string ape, int dni, string email, string hash, string salt) : base(id, nombre, ape, dni, email, hash, salt, UserRole.Empleado) { }

        public Empleado(int id, string nombre, string ape, int dni, string email, string hash, string salt, RolEmpleado rol) : base(id, nombre, ape, dni, email, hash, salt, UserRole.Empleado)
        {
            this.Rol = rol;
        }
    }
}
