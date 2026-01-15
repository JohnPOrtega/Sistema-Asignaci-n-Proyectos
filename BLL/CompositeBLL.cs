using BE;
using BE.Proyectos;
using BE.Usuarios;
using DAL;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class CompositeBLL
    {
        public static void LoadStructure()
        {
            if (ProjectSingleton.Current == null)
                throw new Exception("No hay proyecto cargado en memoria.");

            int projectId = ProjectSingleton.Current.ID;

            var (modulosFunc, relaciones, empleadosFunc) = CompositeDAL.GetEstructuraProyecto(projectId);

            var modulos = modulosFunc.ToDictionary(m => m.ID);
            var empleados = empleadosFunc.ToDictionary(e => e.ID);

            // Asignar responsables y armar jerarquía
            foreach (var m in modulosFunc)
            {
                m.Responsable = empleados[m.ResponsableID_DB];

                if (m.PadreID_DB.HasValue)
                    ((Departamento)modulos[m.PadreID_DB.Value]).Modulos.Add(m);
                else
                    ProjectSingleton.Current.Estructura.Add(m);
            }

            //Asignar empleadosFunc a equipos
            foreach (var (modId, empId) in relaciones)
            {
                if (modulos[modId] is EquipoMultidisciplinario team)
                    team.Integrantes.Add(empleados[empId]);
            }
        }

        public static bool Crear(Modulo modulo, List<Asignacion> asignaciones = null)
        {
            return CompositeDAL.Create(modulo, asignaciones) == true ?
            ProjectSingleton.Current.Agregar(modulo, modulo.Padre) : false;
        }

        public static bool Modificar(Modulo modulo)
        {
            return CompositeDAL.Update(modulo) == true ?
            ProjectSingleton.Current.ModificarModulo(modulo) : false;
        }

        public static bool Eliminar(Modulo modulo)
        {
            if (modulo is Departamento dep)
            {
                if (!dep.vacio())
                    throw new Exception("Debe eliminar primero los equipos del Departamento.");
            }
            return ( CompositeDAL.Delete(modulo) && ProjectSingleton.Current.Eliminar(modulo) );
        }

        public static bool ModificarIntegrantes(int equipoID, List<Asignacion> asignaciones)
        {
            return AsignacionesDAL.ModificarEquipo(equipoID, asignaciones);
        }

        public double CalcularPresupuesto(Proyecto proyecto)
        {
            double presupuesto = 0;

            var sueldos = AsignacionesDAL.GetSueldosProyecto(proyecto.ID);
            
            foreach(var mod  in proyecto.Estructura)
            {
                presupuesto += CalcularPresupuesto(mod, sueldos);
            }

            return presupuesto;
        }

        private double CalcularPresupuesto(Modulo modulo, Dictionary<int, (double sueldoBase, double extraEspecialidad)> sueldos)
        {
            double presupuesto = 0;

            if(modulo is Departamento dpto)
            {
                foreach (var mod in dpto.Modulos)
                    presupuesto += CalcularPresupuesto(mod, sueldos);
            }
            else if(modulo is EquipoMultidisciplinario equipo)
            {
                foreach(var emp in equipo.Integrantes)
                {
                    presupuesto += SueldoTotal(emp, sueldos);
                }
            }

            return presupuesto + SueldoTotal(modulo.Responsable, sueldos);
        }
        
        public double SueldoTotal(User empleado, Dictionary<int, (double sueldoBase, double extraEspecialidad)> sueldos) => 
            sueldos[empleado.ID].sueldoBase + (sueldos[empleado.ID].sueldoBase * sueldos[empleado.ID].extraEspecialidad);
        
    }
}
