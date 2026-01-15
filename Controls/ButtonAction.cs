using BLL;
using GUI;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_de_asignacion_de_proyectos.Controls
{
    internal class ButtonAction : ButtonUI
    {
        public AccionProyecto AccionProyecto { get; set; }

        public ButtonAction() : base()
        {
        }
    }
}
