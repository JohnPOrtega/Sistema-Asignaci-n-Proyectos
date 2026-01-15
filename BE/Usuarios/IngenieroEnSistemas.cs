using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE.Usuarios
{
    public class IngenieroEnSistemas : User
    {
        public IngenieroEnSistemas(int id, string nombre, string ape, int dni, string email, string hash, string salt) : base(id, nombre, ape, dni, email, hash, salt, UserRole.IngenieroEnSistemas)
        {
        }

        public IngenieroEnSistemas(string nombre, string ape, int dni, string email, string hash, string salt) : base(nombre, ape, dni, email, hash, salt, UserRole.IngenieroEnSistemas)
        {
            
        }

        public IngenieroEnSistemas()
        {
        }
    }
}
