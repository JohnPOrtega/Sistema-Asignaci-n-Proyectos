using BE;
using BE.Proyectos;
using BE.Usuarios;
using BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_de_asignacion_de_proyectos.Forms
{
    public partial class FormAsignarEmpleado : UserControl
    {
        private readonly EquipoMultidisciplinario EquipoActual;
        private BindingList<Empleado> Disponibles = new BindingList<Empleado>();
        private BindingList<Empleado> Asignados = new BindingList<Empleado>();
        public List<Asignacion> asignaciones = new List<Asignacion>();

        public event Action<List<Asignacion>> Guardado;
        public event Action Cancelado;
        public FormAsignarEmpleado(EquipoMultidisciplinario team)
        {
            InitializeComponent();
            EquipoActual = team;
            CargarGrids();
        }

        private void CargarGrids()
        {
            Asignados = new BindingList<Empleado>( EquipoActual.Integrantes?.ToList() ?? new List<Empleado>() );
            Disponibles = new BindingList<Empleado>( ProyectoBLL.GetEmpleadosDisponibles(EquipoActual.ProyectoID).ToList() );

            if(Disponibles.Contains(EquipoActual.Responsable)) Disponibles.Remove(EquipoActual.Responsable);

            var roles = Enum.GetValues(typeof(RolEmpleado))
                .Cast<RolEmpleado>()
                .Where(r => r != RolEmpleado.JefeDpto && r != RolEmpleado.JefeEquipo)   //oculto los de jefe
                .ToList();

            dgvEmpleadosAsg.Columns.Add
            ( new DataGridViewComboBoxColumn()
                  {
                     Name = "RolCombobox",
                     HeaderText = "Rol",
                     DataPropertyName = "Rol",      // Se vincula con Empleado.Rol
                     DataSource = roles,            
                     ValueType = typeof(RolEmpleado?),
                     ValueMember = null,
                     DisplayMember = null
                  }
            );

            dgvEmpleadosAsg.DataSource = Asignados;
            dgvEmpleadosDisp.DataSource = Disponibles;
            dgvEmpleadosDisp.Columns["Rol"].Visible = false;

            dgvEmpleadosAsg.DataError += (s, e) => { e.ThrowException = false; };

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(dgvEmpleadosDisp.SelectedRows.Count == 0)
            {
                MessageBox.Show("Debe seleccionar al menos una fila","Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                return;
            }

            var seleccionados = dgvEmpleadosDisp.SelectedRows
                                .Cast<DataGridViewRow>()
                                .Select(r => (Empleado)r.DataBoundItem)
                                .ToList();

            foreach (var emp in seleccionados)
            {
                Disponibles.Remove(emp);
                Asignados.Add(emp);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dgvEmpleadosAsg.SelectedRows.Count == 0)
            {
                MessageBox.Show("Debe seleccionar al menos una fila", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var seleccionados = dgvEmpleadosAsg.SelectedRows
                                .Cast<DataGridViewRow>()
                                .Select(r => (Empleado)r.DataBoundItem)
                                .ToList();

            foreach (var emp in seleccionados)
            {
                Asignados.Remove(emp);
                Disponibles.Add(emp);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Cancelado?.Invoke();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if(dgvEmpleadosAsg.Rows.Count == 0)
            {
                MessageBox.Show("No se puede guardar un equipo vacio. Asigne empleados en la grilla");
                return;
            }
            
            foreach(DataGridViewRow row in dgvEmpleadosAsg.Rows)
            {
                Empleado emp = row.DataBoundItem as Empleado;
                if (emp == null) continue;

                if (emp.Rol == null)
                {
                    MessageBox.Show("Falta seleccionar el rol de un empleado.");
                    return;
                }

                asignaciones.Add( new Asignacion(EquipoActual.ProyectoID, emp, emp.Rol.Value) );
            }
            Guardado?.Invoke(asignaciones);
        }

        
    }
}
