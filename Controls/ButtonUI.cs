using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_de_asignacion_de_proyectos.Controls
{
    internal class ButtonUI : Button
    {
        public AccionUI AccionUI { get; set; }

        public ButtonUI() : base()
        {
        }
    }
}
