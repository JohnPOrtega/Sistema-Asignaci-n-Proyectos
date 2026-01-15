using BE;
using BE.Proyectos;
using BE.Usuarios;
using Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public static class AsignacionesDAL
    {
        public static RolEmpleado GetRole(Empleado empleado)
        {
            string query = @"SELECT r.Tipo FROM Asignaciones a JOIN RolesSistema r on r.ID = a.RolSistemaID WHERE EmpleadoID = @EmpleadoID AND ProyectoID = @ProyectoID";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("EmpleadoID", empleado.ID),
                new SqlParameter("ProyectoID", ProjectSingleton.Current.ID)
            };

            using (SqlConnection conn = new SqlConnection(ConfigDAL.connection))
            using(SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                cmd.Parameters.AddRange(parameters);
                using(SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return (RolEmpleado)Enum.Parse(typeof(RolEmpleado), reader["Tipo"].ToString());
                    
                    else throw new Exception("Ha ocurrido un error en la recuperacion de datos");
                }
            }
        }

        public static Dictionary<int, (double sueldoBase, double extraEspecialidad)> GetSueldosProyecto(int proyectoId)
        {
            var sueldos = new Dictionary<int, (double, double)>();

            using (SqlConnection conn = new SqlConnection(ConfigDAL.connection))
            {
                conn.Open();

                string query = @"
                    SELECT a.EmpleadoID, r.SueldoBase, e.ValorPorcentual
                    FROM Asignaciones a
                    JOIN RolesCliente r ON a.RolID = r.RolID
                    LEFT JOIN Especialidades e ON a.EspecialidadID = e.EspecialidadID
                    WHERE a.ProyectoID = @ProyectoID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProyectoID", proyectoId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int empleadoId = (int)reader["EmpleadoID"];
                            double sueldoBase = Convert.ToDouble(reader["SueldoBase"]);
                            double extra = (reader["ValorPorcentual"] is DBNull) ? 0 : Convert.ToDouble(reader["FactorEspecialidad"]);

                            sueldos[empleadoId] = (sueldoBase, extra);
                        }
                    }
                }
            }

            return sueldos;
        }

        public static bool ModificarEquipo(int moduloId, List<Asignacion> asignaciones)
        {
            string query = @"   DELETE a FROM Asignaciones a
                                INNER JOIN Equipos e ON e.EmpleadoID = a.EmpleadoID
                                WHERE e.ModuloID = @ModuloID;";

            using (SqlConnection conn = new SqlConnection(ConfigDAL.connection))
            {
                conn.Open();
                using (SqlTransaction tx = conn.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(query, conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@ModuloID", moduloId);
                            cmd.ExecuteNonQuery();
                        }

                        using (SqlCommand cmd = new SqlCommand("DELETE FROM Equipos WHERE ModuloID = @ModuloID", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@ModuloID", moduloId);
                            cmd.ExecuteNonQuery();
                        }

                        DataTable dataTable = new DataTable();
                        dataTable.Columns.Add("ProyectoID", typeof(int));
                        dataTable.Columns.Add("EmpleadoID", typeof(int));
                        dataTable.Columns.Add("RolSistemaID", typeof(int));

                        foreach (var asig in asignaciones)
                            dataTable.Rows.Add(asig.ProyectoID, asig.Empleado.ID, (int)asig.rol);

                        using (SqlCommand insertCmd = new SqlCommand("InsertAsignaciones", conn, tx))
                        {
                            insertCmd.CommandType = CommandType.StoredProcedure;
                            insertCmd.Parameters.Add("@Asignaciones", SqlDbType.Structured).Value = dataTable;

                            insertCmd.ExecuteNonQuery();
                        }

                        using (SqlCommand insertEquip = new SqlCommand(@" INSERT INTO Equipos(ModuloId, EmpleadoId) SELECT @ModuloID, EmpleadoID FROM @Asignaciones;", conn, tx))
                        {
                            insertEquip.CommandType = CommandType.Text;

                            var param = insertEquip.Parameters.Add("@Asignaciones", SqlDbType.Structured);
                            param.Value = dataTable;

                            insertEquip.Parameters.AddWithValue("@ModuloID", moduloId);

                            insertEquip.ExecuteNonQuery();
                        }

                        tx.Commit();
                        return true;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
