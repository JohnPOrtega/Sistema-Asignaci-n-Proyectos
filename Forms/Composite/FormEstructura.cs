using BE.Proyectos;
using Service;
using Sistema_de_asignacion_de_proyectos.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_de_asignacion_de_proyectos.UI.Modulos
{
    public partial class FormEstructura : Form
    {
        public FormEstructura()
        {
            InitializeComponent();
            panelResponsable.Visible = false;
            CargarTreeView();
        }

        public TreeNode SelectedNode => treeView1.SelectedNode;
        private bool cancelarSeleccion = false;

        public void CargarTreeView()
        {
            treeView1.Nodes.Clear();

            if (ProjectSingleton.Current == null || ProjectSingleton.Current.Estructura == null)
                return;

            foreach (var moduloRaiz in ProjectSingleton.Current.Estructura)
            {
                treeView1.Nodes.Add( CrearNodoModulo(moduloRaiz) );
            }

            treeView1.ExpandAll();
        }

        private TreeNode CrearNodoModulo(Modulo modulo)
        {
            TreeNode nodo = new TreeNode(modulo.Nombre) { Tag = modulo };

            if (modulo is Departamento dpto && dpto.Modulos.Any() )
            {
                foreach (var sub in dpto.Modulos)
                {
                    nodo.Nodes.Add(CrearNodoModulo(sub));
                }
            }
            
            else if (modulo is EquipoMultidisciplinario eq && eq.Integrantes.Any())
            {
                TreeNode nodoEmpleados = new TreeNode("Empleados") { Tag = "Empleados" };

                foreach (var emp in eq.Integrantes)
                {
                    nodoEmpleados.Nodes.Add(new TreeNode($"{emp.Nombre} {emp.Apellido} ({emp.Rol})") { Tag = emp});
                }

                nodo.Nodes.Add(nodoEmpleados);
            }

            return nodo;
        }

        private void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if(e.Node.Tag is string) e.Cancel = true;

            if (cancelarSeleccion)
            {
                e.Cancel = true;
                cancelarSeleccion = false;
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (treeView1.SelectedNode == e.Node)
            {
                cancelarSeleccion = true;
                treeView1.SelectedNode = null;
                treeView1.Invalidate();

                panelResponsable.Controls.Clear();
                panelResponsable.Visible = false;
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if(treeView1.SelectedNode.Tag is Modulo mod)
            {
                var uc = new UC_Responsable(mod.Responsable);
                uc.Dock = DockStyle.Fill;
                panelResponsable.Controls.Clear();
                panelResponsable.Controls.Add(uc);
                panelResponsable.Visible = true;
            }
            else
            {
                panelResponsable.Controls.Clear();
                panelResponsable.Visible = false;
            }
        }
    }
}
