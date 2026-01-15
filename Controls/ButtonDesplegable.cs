using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_de_asignacion_de_proyectos.Controls
{
    internal class ButtonDesplegable : ButtonUI
    {
        private FlowLayoutPanel _panelAsociado;
        public FlowLayoutPanel PanelAsociado
        {
            get => _panelAsociado;
            set
            {
                _panelAsociado = value;

                // Evitar ejecutar lógica en el diseñador
                if (!DesignMode && _panelAsociado != null)
                    _panelAsociado.Visible = false;
            }
        }
        public ButtonDesplegable() : base()
        {
            this.VisibleChanged += ButtonDesplegable_VisibleChanged;
        }

        private void ButtonDesplegable_VisibleChanged(object sender, EventArgs e)
        {
            if (DesignMode) return;

            if (PanelAsociado != null && !Visible)
                PanelAsociado.Visible = false;
        }
    }
}
