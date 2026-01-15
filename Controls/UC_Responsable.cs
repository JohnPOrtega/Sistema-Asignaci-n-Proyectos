using BE.Proyectos;
using BE.Usuarios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_de_asignacion_de_proyectos.Controls
{
    public partial class UC_Responsable : UserControl
    {
        public UC_Responsable(Empleado responsable)
        {
            InitializeComponent();
            labelNombre.Text = responsable.Nombre;
            labelApellido.Text = responsable.Apellido;
            labelDNI.Text = responsable.DNI.ToString();
            labelCorreo.Text = responsable.Email;
            labelCargo.Text = responsable.Rol.ToString();
        }
    }
}
