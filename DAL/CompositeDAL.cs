using BE;
using BE.Proyectos;
using BE.Usuarios;
using MPP;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DAL
{
    public static class CompositeDAL
    {

        public static (List<Modulo> modulos, List<(int ModuloID, int EmpleadoID)> relaciones, List<Empleado> empleados)
        GetEstructuraProyecto(int proyectoId)
        {
            var modulos = new List<Modulo>();
            var relaciones = new List<(int, int)>();
            var empleados = new List<Empleado>();

            using (SqlConnection conn = new SqlConnection(ConfigDAL.connection))
            using (SqlCommand cmd = new SqlCommand("GetProjectStructure", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", proyectoId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // Modulos
                    while (reader.Read())
                    {
                        string nombre = reader.GetString(reader.GetOrdinal("Nombre"));
                        Modulo modulo;
                        
                        if ((TipoModulo)Enum.Parse(typeof(TipoModulo), reader["Tipo"].ToString()) == TipoModulo.Departamento)
                             { modulo = new Departamento(proyectoId, null, null, nombre); }
                        
                        else { modulo = new EquipoMultidisciplinario(proyectoId, null, null, nombre); }

                        modulo.ID = reader.GetInt32(reader.GetOrdinal("ID"));
                        modulo.ProyectoID = proyectoId;
                        modulo.ResponsableID_DB = reader.GetInt32(reader.GetOrdinal("ResponsableID"));

                        if (!reader.IsDBNull(reader.GetOrdinal("PadreID")))
                            modulo.PadreID_DB = reader.GetInt32(reader.GetOrdinal("PadreID"));

                        modulos.Add(modulo);
                    }

                    // Equipos
                    reader.NextResult();
                    while (reader.Read())
                        relaciones.Add((
                            reader.GetInt32(reader.GetOrdinal("ModuloId")),
                            reader.GetInt32(reader.GetOrdinal("EmpleadoId"))
                        ));

                    //Asignaciones
                    reader.NextResult();

                    Dictionary<int, RolEmpleado> asignaciones = new Dictionary<int, RolEmpleado>();

                    while (reader.Read())
                    {
                        int empId = Convert.ToInt32(reader["EmpleadoID"]);
                        RolEmpleado rol = (RolEmpleado)Enum.Parse(
                            typeof(RolEmpleado),
                            reader["Tipo"].ToString()
                        );

                        asignaciones[empId] = rol;
                    }

                    // Empleados
                    reader.NextResult();
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["IdEmp"]);
                        asignaciones.TryGetValue(id, out RolEmpleado rol);

                        empleados.Add(new Empleado
                                    (
                                        id,
                                        reader["Nombre"].ToString(),
                                        reader["Apellido"].ToString(),
                                        Convert.ToInt32(reader["DNI"]),
                                        reader["Email"].ToString(),
                                        reader["Hash"].ToString(),
                                        reader["Salt"].ToString(),
                                        rol
                                    )
                        );
                    }
                }
            }
            return (modulos, relaciones, empleados);
        }
        
        public static bool Create(Modulo modulo, List<Asignacion> asignaciones = null)
        {
            string query = @"INSERT INTO Modulos (ProyectoID, PadreID, ResponsableID, Tipo, Nombre)
                           OUTPUT INSERTED.ID
                           VALUES (@ProyectoID, @PadreID, @ResponsableID, @Tipo, @Nombre);

                           INSERT INTO Asignaciones (ProyectoID, EmpleadoID, RolSistemaID)
                           VALUES (@ProyectoID, @ResponsableID, @RolResponsable);";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(  "@ProyectoID", modulo.ProyectoID                                                                       ),
                new SqlParameter(  "@PadreID", (object)modulo.Padre?.ID ?? DBNull.Value                                                   ),
                new SqlParameter(  "@ResponsableID", modulo.Responsable.ID                                                                ),
                new SqlParameter(  "@Tipo", (modulo is Departamento)? TipoModulo.Departamento.ToString() : TipoModulo.Equipo.ToString()   ),
                new SqlParameter(  "@Nombre", modulo.Nombre                                                                               ),
                new SqlParameter(  "@RolResponsable", (int)(modulo.Responsable.Rol ?? throw new Exception("El Rol fue null") )            )
            };

            using (SqlConnection conn = new SqlConnection(ConfigDAL.connection))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    int ModuloID;
                    using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                    {
                        cmd.Parameters.AddRange(parameters);

                        ModuloID = (int)cmd.ExecuteScalar();
                    }

                    if (modulo is EquipoMultidisciplinario equipo)
                    {
                        DataTable dataTable = new DataTable();
                        dataTable.Columns.Add("ProyectoID", typeof(int));
                        dataTable.Columns.Add("EmpleadoID", typeof(int));
                        dataTable.Columns.Add("RolSistemaID", typeof(int));

                        foreach (var asig in asignaciones)
                            dataTable.Rows.Add( asig.ProyectoID, asig.Empleado.ID, (int)asig.rol );

                        using (SqlCommand cmd = new SqlCommand("InsertEquipo", conn, transaction) { CommandType = CommandType.StoredProcedure })
                        {
                            cmd.Parameters.AddWithValue("@ModuloID", ModuloID);
                            SqlParameter tvpParam = cmd.Parameters.AddWithValue("@Asignaciones", dataTable);
                            tvpParam.SqlDbType = SqlDbType.Structured;
                            tvpParam.TypeName = "AsignacionTableType";

                            cmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show(ex.Message);
                    return false;
                }
            }
            
        }

        public static bool Update(Modulo modulo)
        {
            string query = @"UPDATE Modulos SET Nombre = @Nombre, ResponsableID = @ResponsableID
                             WHERE ID = @ID;";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Nombre", modulo.Nombre),
                new SqlParameter("@ResponsableID", modulo.Responsable.ID),
                new SqlParameter("@ID", modulo.ID)
            };

            using (SqlConnection conn = new SqlConnection(ConfigDAL.connection))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddRange(parameters);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }            
        }

        public static bool Delete(Modulo modulo)
        {
            try
            {
                string query = @"DELETE FROM Modulos WHERE Id = @ID";

                using (SqlConnection conn = new SqlConnection(ConfigDAL.connection))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", modulo.ID);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
    }
}
