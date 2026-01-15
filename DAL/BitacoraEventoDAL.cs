using BE;
using BE.Usuarios;
using MPP;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace DAL
{
    public class BitacoraEventoDAL
    {

        public bool RegistrarEvento(BitacoraEvento evento)
        {
            using (SqlConnection conn = new SqlConnection(ConfigDAL.connection))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    string query = @"INSERT INTO Bitacora 
                            (FechaHora, Descripcion, Usuario, Tipo, Origen)
                             VALUES 
                            (@fechahora, @descripcion, @usuario, @tipo, @origen)";

                    using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                    {
                        SqlParameter[] parametros = MapperGen.CrearObjeto(evento);
                        cmd.Parameters.AddRange(parametros);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public List<BitacoraEvento> ObtenerTodos()
        {
            string query = "SELECT * FROM Bitacora";
            using(SqlConnection conn=new SqlConnection(ConfigDAL.connection))
            {
                List<BitacoraEvento> lista = new List<BitacoraEvento>();
                SqlDataAdapter da = new SqlDataAdapter(query, conn);

                DataTable dt = new DataTable();

                da.Fill(dt);

                foreach(DataRow dr in dt.Rows)
                {
                    BitacoraEvento be = new BitacoraEvento();
                    MapperGen.MapearObjeto(dr, be);
                    lista.Add(be);
                }
                return lista;
            }
        }
        

    }
}
