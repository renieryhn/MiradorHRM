using PlanillaPM.Models;
using System.Data;
using System.Reflection;
using System;
using System.Collections;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Net.Http;


namespace PlanillaPM
{
    public class cGeneralFun
    {
        public class ListtoDataTableConverter
        {
            public DataTable ToDataTable<T>(List<T> items)
            {
                DataTable dataTable = new DataTable(typeof(T).Name);
                //Get all the properties
                PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo prop in Props)
                {
                    //Setting column names as Property names
                    dataTable.Columns.Add(prop.Name);
                }
                foreach (T item in items)
                {
                    var values = new object[Props.Length];
                    for (int i = 0; i < Props.Length; i++)
                    {
                        //inserting property values to datatable rows
                        values[i] = Props[i].GetValue(item, null);
                    }
                    dataTable.Rows.Add(values);
                }
                //put a breakpoint here and check datatable
                return dataTable;
            }
        }

        public List<MenuDinamico> ObtenerMenu(string sObjeto)
        {
            // Convierte el JSON en una lista de objetos MenuDinamico
            List<MenuDinamico> menuList = ListadoDeMenus().Where(menu => menu.Objeto == sObjeto).ToList();
            return menuList;
        }

        private List<MenuDinamico> ListadoDeMenus()
        {
            // Aquí puedes cargar los países desde cualquier fuente de datos, como un archivo de configuración o un servicio externo.
            // Por ejemplo, aquí se crea una lista de países en código duro para este ejemplo.
            var menu = new List<MenuDinamico>
        {
            new MenuDinamico { Objeto= "Perfil",Titulo= "Datos Personales",Area= "",Controler= "Usuario",Accion= "PersonalData" },
            new MenuDinamico { Objeto= "Perfil",Titulo= "Cambiar Correo Electrónico",Area= "",Controler= "Usuario",Accion= "Email" },
             new MenuDinamico { Objeto= "Perfil",Titulo= "Cambiar Contraseña",Area= "",Controler= "Usuario",Accion= "ChangePassword" },

             new MenuDinamico { Objeto= "Empleado",Titulo= "Contacto de Emergencia",Area= "",Controler= "EmpleadoContacto",Accion= "Index" },
             new MenuDinamico { Objeto= "Empleado",Titulo= "Contratos",Area= "",Controler= "EmpleadoContrato",Accion= "Index" },
             new MenuDinamico { Objeto= "Empleado",Titulo= "Educación",Area= "",Controler= "EmpleadoEducacion",Accion= "Index" },
             new MenuDinamico { Objeto= "Empleado",Titulo= "Experiencia",Area= "",Controler= "EmpleadoExperiencium",Accion= "Index" },
             new MenuDinamico { Objeto= "Empleado",Titulo= "Habilidades",Area= "",Controler= "EmpleadoHabilidad",Accion= "Index" },
             new MenuDinamico { Objeto= "Empleado",Titulo= "Ausencias",Area= "",Controler= "EmpleadoAusencium",Accion= "Index" },
             new MenuDinamico { Objeto= "Empleado",Titulo= "Activos",Area= "",Controler= "EmpleadoActivo",Accion= "Index" },
             //new MenuDinamico { Objeto= "Empleado",Titulo= "Vacaciones",Area= "",Controler= "EmpleadoVacaciones",Accion= "Index" },
                        
            new MenuDinamico { Objeto= "NominaEmpleado",Titulo= "Nómina",Area= "",Controler= "Nomina",Accion= "Index" },
            new MenuDinamico { Objeto= "NominaEmpleado",Titulo= "Ingresos",Area= "",Controler= "EmpleadoIngreso",Accion= "Index" },
            new MenuDinamico { Objeto= "NominaEmpleado",Titulo= "Deducciones",Area= "",Controler= "EmpleadoDeduccion",Accion= "Index" },
            new MenuDinamico { Objeto= "NominaEmpleado",Titulo= "Impuestos",Area= "",Controler= "EmpleadoImpuesto",Accion= "Index" },
            //new MenuDinamico { Objeto= "NominaEmpleado",Titulo= "Horas Extra",Area= "",Controler= "EmpleadoHorasExtra",Accion= "Index" },
            new MenuDinamico { Objeto= "NominaEmpleado",Titulo= "Cuentas por Cobrar",Area= "",Controler= "CuentaPorCobrar",Accion= "Index" },
            //new MenuDinamico { Objeto= "NominaEmpleado",Titulo= "ISR",Area= "",Controler= "EmpleadoISR",Accion= "Index" },
            new MenuDinamico { Objeto= "NominaEmpleado",Titulo= "Viáticos",Area= "",Controler= "Viatico",Accion= "Index" },
            new MenuDinamico { Objeto= "NominaEmpleado",Titulo= "Vacaciones",Area= "",Controler= "Vacacion",Accion= "Index" },

            // Agrega más países según necesites
        };
            return menu;
        }

        //public List<Ventana> ObtenerVentana()
        //{
        //    // Convierte el JSON en una lista de objetos MenuDinamico
        //    List<Ventana> menuList = ListadoVentanas().ToList();
        //    return menuList;
        //}

        //private List<Ventana> ListadoVentanas()
        //{

        //    var menu = new List<Ventana>
        //{
        //    new Ventana { Nombre= "Perfil" },
        //    new Ventana { Nombre= "Banco" },
        //    new Ventana { Nombre= "Cargo" },
        //    new Ventana { Nombre= "ClaseEmpleado" },
        //    new Ventana { Nombre= "Constancia" },
        //    new Ventana { Nombre= "CuentaPorCobrar" },
        //    new Ventana { Nombre= "Dashboard" },
        //    new Ventana { Nombre= "Deduccion" },
        //    new Ventana { Nombre= "Departamento" },
        //    new Ventana { Nombre= "DiaFestivo" },
        //    new Ventana { Nombre= "Division" },
        //    new Ventana { Nombre= "Empleado" },
        //    new Ventana { Nombre= "EmpleadoActivo" },
        //    new Ventana { Nombre= "EmpleadoAusencium" }
           

        //    // Agrega más países según necesites
        //};

        //    return menu;
        //}
    }
}
