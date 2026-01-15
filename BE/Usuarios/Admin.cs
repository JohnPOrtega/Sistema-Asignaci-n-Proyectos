using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE.Usuarios
{
    public class Admin:User
    {
        public Admin(int id, string nombre, string ape, int dni, string mail, string hash, string salt) : base(id, nombre, ape, dni, mail, hash, salt, UserRole.Admin) { }
        public Admin(Admin u) : base(u.ID, u.Nombre, u.Apellido, u.DNI, u.Email, u.Hash, u.Salt, UserRole.Admin) { }

        public Admin(string nombre, string apellido, int dni, string email, string hash, string salt) : base(nombre, apellido, dni, email, hash, salt, UserRole.Admin) { }

        public Admin()
        {
        }
    }
}
