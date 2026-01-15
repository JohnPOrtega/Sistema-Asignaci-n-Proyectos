using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE.Proyectos
{
    public enum RolEmpleado
    {
        JefeDpto,
        JefeEquipo,
        DevSenior,
        DevSemiSenior,
        DevJunior
    }

    public class RolCliente
    {
        public int ID;
        public int ClienteID;
        public double SueldoBase;

        public RolCliente(int iD, int clienteID, double sueldoBase)
        {
            ID = iD;
            ClienteID = clienteID;
            SueldoBase = sueldoBase;
        }
    }
}
