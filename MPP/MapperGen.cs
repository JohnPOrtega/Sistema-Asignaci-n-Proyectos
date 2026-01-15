using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Timers;


namespace MPP
{
    public static class MapperGen
    {
        public static SqlParameter[] CrearObjeto ( object entidad ) 
        {

            var listaParametros = new List<SqlParameter>();

            PropertyInfo[] datos = entidad.GetType().GetProperties();


            foreach (PropertyInfo prop in datos) 
            {
                if (prop.Name.ToLower().EndsWith("id")) continue;

                object valor = prop.GetValue(entidad) ?? DBNull.Value;
                string ConcatenaiondeParametros = "@" + prop.Name.ToLower();

                if (prop.PropertyType.IsEnum) valor = valor.ToString();
                
                listaParametros.Add(new SqlParameter(ConcatenaiondeParametros, valor));
            }

            return listaParametros.ToArray();
        }


        public static object MapearObjeto(DataRow row , object entidad)
        {

            Type tipoentidad = entidad.GetType();
            PropertyInfo[] propiedad = tipoentidad.GetProperties();


            foreach (PropertyInfo prop in propiedad) 
            {
                string nombreColumna = prop.Name;

                if (row.Table.Columns.Contains(nombreColumna) && row[nombreColumna] != DBNull.Value) 
                {
                    object valorBD = row[nombreColumna];

                    try
                    {
                        if (prop.PropertyType.IsEnum)
                        {
                            object ValorEnum = Enum.Parse(prop.PropertyType, valorBD.ToString());
                            prop.SetValue(entidad, ValorEnum);

                        }
                        else 
                        {
                            object valorConvertido = Convert.ChangeType(valorBD, prop.PropertyType);
                            prop.SetValue(entidad, valorConvertido);
                        }

                    }
                    catch (Exception)
                    {
                        continue; // Si hay un error, simplemente continúa con la siguiente propiedad
                    }
                }
            
            }

            return entidad;
        }




           
        //                 try
        //                {
        //                    int Index = reader.GetOrdinal(nombreColumna);

        //                    if (!reader.IsDBNull(Index))
        //                    {
        //                        object valorBD = reader.GetValue(Index);

        //                        if (prop.PropertyType.IsEnum)
        //                        {

        //                            object ValorEnum = Enum.Parse(prop.PropertyType, valorBD.ToString());
        //        prop.SetValue(entidad, ValorEnum);
        //                        }
        //                        else if (valorBD.GetType() != prop.PropertyType)
        //                        {
        //                            // Usa Convert.ChangeType para manejar conversiones como int->long, etc.
        //                            object valorConvertido = Convert.ChangeType(valorBD, prop.PropertyType);
        //    prop.SetValue(entidad, valorConvertido);

        //                        }
        //                        else
        //{
        //    prop.SetValue(entidad, valorBD);

        //}
        //                    }
        //                }

        //                catch (IndexOutOfRangeException)
        //                {
        //    //     throw new Exception("Error al mapear el objeto: " + ex.Message);
        //    continue;
        //}
            
        //            }


    }
}


