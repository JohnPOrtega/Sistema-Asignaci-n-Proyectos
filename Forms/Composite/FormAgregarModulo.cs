using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BE;
using BE.Proyectos;
using BE.Usuarios;
using BLL;
using GUI;
using Service;
namespace Sistema_de_asignacion_de_proyectos.Forms
{
    public partial class FormAgregarModulo : Form
    {
        private BindingList<Empleado> empleadosDisponibles;
        private readonly Modulo padre;
        private readonly bool addDpto;
        private readonly bool modificando;

        public FormAgregarModulo(Modulo modulo, bool addDpto, bool modificando = false)
        {
            InitializeComponent();
            padre = modulo;
            this.addDpto = addDpto;
            this.modificando = modificando;
            CargarGrid();
        }
        public void CargarGrid()
        {
            empleadosDisponibles = new BindingList<Empleado>(ProyectoBLL.GetEmpleadosDisponibles(ProjectSingleton.Current.ID).ToList());
            dgvEmpleados.DataSource = null;
            dgvEmpleados.DataSource = empleadosDisponibles;

            if (modificando)
            {
                txtNombreDepto.Text = padre.Nombre;
                empleadosDisponibles.Insert(0, padre.Responsable);
                dgvEmpleados.ClearSelection();

                dgvEmpleados.Rows[0].Selected = true;
            }

            dgvEmpleados.Columns["Rol"].Visible = false;
        }
        public bool CamposValidos()
        {
            return dgvEmpleados.CurrentRow != null && !String.IsNullOrEmpty(txtNombreDepto.Text);
        }
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (!CamposValidos())
            {
                MessageBox.Show("Debe seleccionar un empleado para responsable y completar el nombre del modulo.");
                return;
            }

            Empleado emp = dgvEmpleados.CurrentRow.DataBoundItem as Empleado;
            if (emp == null)
            {
                MessageBox.Show("Debe seleccionar un empleado válido.");
                return;
            }

            if(modificando) Modificar(emp);

            else Agregar(emp);
            
        }

        public void Agregar(Empleado emp)
        {
            Modulo modulo;
            var nombre = txtNombreDepto.Text;
            // Crear DEPARTAMENTO
            if (addDpto)
            {
                emp.Rol = RolEmpleado.JefeDpto;
                modulo = new Departamento(ProjectSingleton.Current.ID, padre, emp, nombre);
                Guardar(modulo);
            }
            // Crear EQUIPO MULTIDISCIPLINARIO
            else
            {
                emp.Rol = RolEmpleado.JefeEquipo;
                modulo = new EquipoMultidisciplinario(ProjectSingleton.Current.ID, padre, emp, nombre);

                MessageBox.Show("Ahora agregue los integrantes.");

                var uc = new FormAsignarEmpleado(modulo as EquipoMultidisciplinario) { Dock = DockStyle.Fill };
                FormMDI.Mainform.panelForm.Controls.Add(uc);
                uc.BringToFront();

                uc.Guardado += (asignaciones) =>
                {
                    ((EquipoMultidisciplinario)modulo).Integrantes = asignaciones.Select(a => a.Empleado).ToList();
                    Guardar(modulo, asignaciones);

                    FormMDI.Mainform.panelForm.Controls.Remove(uc);
                    uc.Dispose();
                };

                uc.Cancelado += () =>
                {
                    FormMDI.Mainform.panelForm.Controls.Remove(uc);
                    uc.Dispose();
                    MessageBox.Show("Debe asignar al menos un empleado.");
                };
            }
        }

        public void Modificar(Empleado emp)
        {
            if (padre.Nombre == txtNombreDepto.Text && padre.Responsable == emp)
            {
                MessageBox.Show("No ha realizado cambios");
                return;
            }
            padre.Nombre = txtNombreDepto.Text;
            padre.Responsable = emp;
            CompositeBLL.Modificar(padre);
            MessageBox.Show("Se ha modificado correctamente");
            this.Close();

        }

        public void Guardar(Modulo modulo, List<Asignacion> asignaciones = null)
        {
            CompositeBLL.Crear(modulo, asignaciones);
            MessageBox.Show("Se ha agregado el modulo correctamente");
            this.Close();
        }
    }
}
