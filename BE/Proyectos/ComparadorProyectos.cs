using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE.Proyectos;

namespace Sistema_de_asignacion_de_proyectos.Domain
{
    public class ComparadorVencimiento : IComparer<Proyecto>
    {
        public int Compare(Proyecto x, Proyecto y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return 1;
            if (y == null) return -1;

            var ahora = DateTime.Now;

            bool xVencido = x.fechaEstimada < ahora;
            bool yVencido = y.fechaEstimada < ahora;

            if (!xVencido && !yVencido)
            {
                // Ambos sin vencer: ordenar más cercanos primero
                return x.fechaEstimada.CompareTo(y.fechaEstimada);
            }
            else if (!xVencido && yVencido)
            {
                // x sin vencer, y vencido
                return -1;
            }
            else if (xVencido && !yVencido)
            {
                // x vencido, y no
                return 1;
            }
            else
            {
                // Ambos vencidos: ordenar del que venció hace más tiempo al más reciente
                return y.fechaEstimada.CompareTo(x.fechaEstimada);
            }
        }
    }

    public class ComparadorNombre : IComparer<Proyecto>
    {
        public int Compare(Proyecto x, Proyecto y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return 1;
            if (y == null) return -1;
            return string.Compare(x?.nombre, y?.nombre, StringComparison.OrdinalIgnoreCase);
        }
    }

}
