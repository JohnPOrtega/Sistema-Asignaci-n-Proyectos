using Microsoft.VisualBasic;
using Sistema_de_asignacion_de_proyectos.Properties;
//using Sistema_de_asignacion_de_proyectos.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Resources;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BE.Usuarios;
using BLL;

namespace GUI
{
    public partial class FormAdministrarUsuarios : Form
    {
        private readonly User _actual;
        public FormAdministrarUsuarios(User logged)
        {
            InitializeComponent();
            InicializarIdioma();
            _actual = logged;
        }
        private void actualizarData()
        {
            dataGridView1.DataSource = UsuarioBLL.GetAll();
            dataGridView1.Columns["ID"].Visible = false;
            dataGridView1.Columns["Hash"].Visible = false;
            dataGridView1.Columns["Salt"].Visible = false;
        }

        private bool ValidaDatos(out int dni)
        {
            dni = 0;

            if (string.IsNullOrEmpty(txtDNI.Text) || string.IsNullOrEmpty(txtEmail.Text) || string.IsNullOrEmpty(txtNombre.Text) || string.IsNullOrEmpty(txtApellido.Text) || comboBox1.SelectedIndex == -1 ||
                txtDNI.Text == "DNI" || txtEmail.Text == Resources.Correo_Electronico || txtNombre.Text == Resources.Nombre || txtApellido.Text == Resources.Apellido || txtContraseña.Text == Resources.Contraseña)
            {
                MessageBox.Show(Resources.CompletarDatos);
                return false;
            }

            try
            {
                dni = Convert.ToInt32(txtDNI.Text);
            }
            catch (FormatException)
            {
               MessageBox.Show(Resources.ErrorDni);
               return false;
            }
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!ValidaDatos(out int dni)) return;
            try
            {
                bool ok = UsuarioBLL.Registrar(
                txtNombre.Text,
                txtApellido.Text,
                Convert.ToInt32(txtDNI.Text),
                txtEmail.Text,
                txtContraseña.Text,
                UserRole.Cliente);
                if (ok) 
                { 
                MessageBox.Show("User registrado con éxito");
                    actualizarData();
                        }

                    else
                    MessageBox.Show("No se pudo registrar el usuario");

            }


            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void FormAdministrarUsuarios_Load(object sender, EventArgs e)
        {
            actualizarData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    User seleccionado = dataGridView1.CurrentRow.DataBoundItem as User;
                    UserRole rol = seleccionado.UserRol;
                    int id = Convert.ToInt32(seleccionado.ID);
                    UsuarioBLL.BorrarUsuario(id , rol);
                    actualizarData();
                }

            }
            catch (Exception)
            {

                throw;
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {/*
            if (!ValidaDatos(out int dni) || dataGridView1.CurrentRow == null) return;

            User EquipoActual = dataGridView1.CurrentRow.DataBoundItem as User;
            int id = Convert.ToInt32(EquipoActual.ID);
            
            switch ((UserRole)Enum.Parse(typeof(UserRole), comboBox1.SelectedItem.ToString()))
            {
                case UserRole.IngenieroEnSistemas:
                    return new IngenieroEnSistemas
                        (
                            Convert.ToInt32(reader["ID"]),
                            reader["Nombre"].ToString(),
                            reader["Apellido"].ToString(),
                            Convert.ToInt32(reader["DNI"]),
                            reader["Email"].ToString(),
                            reader["Hash"].ToString(),
                            reader["Salt"].ToString()
                        );
                case UserRole.Empleado:
                    return new Empleado
                        (
                            Convert.ToInt32(reader["ID"]),
                            reader["Nombre"].ToString(),
                            reader["Apellido"].ToString(),
                            Convert.ToInt32(reader["DNI"]),
                            reader["Email"].ToString(),
                            reader["Hash"].ToString(),
                            reader["Salt"].ToString()
                        );
                case UserRole.Cliente:
                    return new Cliente
                        (
                            Convert.ToInt32(reader["ID"]),
                            reader["Nombre"].ToString(),
                            reader["Apellido"].ToString(),
                            Convert.ToInt32(reader["DNI"]),
                            reader["Email"].ToString(),
                            reader["Hash"].ToString(),
                            reader["Salt"].ToString()
                        );
            }
            }
            var usuario = new UsuarioConcreto(
                id,
                txtNombre.Text,
                txtApellido.Text,
                Convert.ToInt32(txtDNI.Text),
                txtEmail.Text,
                EquipoActual.Hash, 
                EquipoActual.Salt,
                (UserRole)Enum.Parse(typeof(UserRole),comboBox1.SelectedItem.ToString()) // Podrías usar un comboBox para elegir rol
            );

            try
            {
                bll.ModificarUsuario(usuario);
                MessageBox.Show("Usuario actualizado correctamente.");
                actualizarData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar usuario: " + ex.Message);
            }
            */
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // asegura que no se cliquee el header
            {
                User seleccionado = dataGridView1.CurrentRow.DataBoundItem as User;
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                comboBox1.SelectedItem = seleccionado.UserRol.ToString();
                txtNombre.Text = row.Cells["Nombre"].Value.ToString();
                txtApellido.Text = row.Cells["Apellido"].Value.ToString();
                txtDNI.Text = row.Cells["DNI"].Value.ToString();
                txtEmail.Text = row.Cells["Email"].Value.ToString();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //if (dataGridView1.CurrentRow == null)
            //{
            //    MessageBox.Show(Resources.ErrorSeleccionUsuario);
            //    return;
            //}

            //User EquipoActual = Archivos.listaUsuariosCSV()[dataGridView1.CurrentRow.Index];

            //// Bloquea si intenta modificar a otro admin que no sea el logeado
            //if (EquipoActual.RolSistema == UserRole.admin && EquipoActual.ID != _actual.ID)
            //{
            //    MessageBox.Show(Resources.ErrorContraAdmin);
            //    return;
            //}

            //string nuevaContraseña = Interaction.InputBox($"{Resources.IngresarContra}:", Resources.CambioContra, "", -1, -1);

            //if (string.IsNullOrWhiteSpace(nuevaContraseña)) return;

            //// Generar nuevo hash + salt
            //Seguridad.CrearHash(nuevaContraseña, out string nuevoHash, out string nuevaSalt);

            //// Reemplazar en la lista
            //EquipoActual.Hash = nuevoHash;
            //EquipoActual.Salt = nuevaSalt;

            //ServicioUsuarios.ModificarUsuario(dataGridView1.CurrentRow.Index, EquipoActual);
            //MessageBox.Show(Resources.CambioContraExitoso);
        }

        private void InicializarIdioma()
        {
            lblEmailAdministrarU.Text = Resources.Correo_Electronico;
            lblContraseñaAdminU.Text = Resources.Contraseña;
            lblNombreAdminU.Text = Resources.Nombre;
            lblApellidoAdminU.Text = Resources.Apellido;

            btnAgregarUsuarioAdminU.Text = Resources.AgregarUsuario;
            btnEliminarUAdminU.Text = Resources.EliminarUsuario;
            btnModificarUAdminU.Text = Resources.ModificarUsuario;
            btnCambiarContraseñaAdminU.Text = Resources.CambiarContraseña;
        }

       
    }
}
