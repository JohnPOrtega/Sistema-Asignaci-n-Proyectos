using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE.Usuarios
{
    public class Cliente : User
    {
      
        public Cliente(int id, string nombre, string ape, int dni, string mail, string hash, string salt) : base(id, nombre, ape, dni, mail, hash, salt, UserRole.Cliente) { }
        public Cliente(User u) : base(u.ID, u.Nombre, u.Apellido, u.DNI, u.Email, u.Hash, u.Salt, UserRole.Cliente) { }

        public Cliente(string nombre, string apellido, int dni, string email, string hash, string salt) : base(nombre, apellido, dni, email, hash, salt, UserRole.Cliente) { }

        public Cliente()
        {
        }
    }
}
