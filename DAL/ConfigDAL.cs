using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace DAL
{
    public static class ConfigDAL
    {
        public static readonly string connection =
            //ConfigurationManager.ConnectionStrings["SistemaAsignacionProyectos"].ConnectionString;
            //@"Data Source=localhost\SQLEXPRESS;Initial Catalog=SistemaAsignacionProyectos;Integrated Security=True;";
            @"Data Source=desktop-gciu8b0;Initial Catalog=SistemaAsignacionProyectos;Integrated Security=True;";
        //@"Data Source=DESKTOP-CRINK3R\SQLEXPRESS;Initial Catalog = SistemaAsignacionProyectos; Integrated Security = True";
    }
}
