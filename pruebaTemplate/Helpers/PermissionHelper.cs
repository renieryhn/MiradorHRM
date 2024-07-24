namespace MiradorHRM.Helpers
{
    public static class PermissionHelper
    {
        public static bool HasNominaPermissions(Dictionary<string, bool> menuPermissions)
        {
            return menuPermissions != null &&
                (menuPermissions.ContainsKey("NominaEmpleado") && menuPermissions["NominaEmpleado"]) ||
                (menuPermissions.ContainsKey("TipoNomina") && menuPermissions["TipoNomina"]) ||
                (menuPermissions.ContainsKey("Ingreso") && menuPermissions["Ingreso"]) ||
                (menuPermissions.ContainsKey("Deduccion") && menuPermissions["Deduccion"]) ||
                (menuPermissions.ContainsKey("Impuesto") && menuPermissions["Impuesto"]);
        }

        public static bool HasRecursosHumanosPermissions(Dictionary<string, bool> menuPermissions)
        {
            return menuPermissions != null &&
                (menuPermissions.ContainsKey("Division") && menuPermissions["Division"]) ||
                (menuPermissions.ContainsKey("Departamento") && menuPermissions["Departamento"]) ||
                (menuPermissions.ContainsKey("Cargo") && menuPermissions["Cargo"]) ||
                (menuPermissions.ContainsKey("ClaseEmpleado") && menuPermissions["ClaseEmpleado"]) ||
                (menuPermissions.ContainsKey("TipoAusencium") && menuPermissions["TipoAusencium"]) ||
                (menuPermissions.ContainsKey("TipoContrato") && menuPermissions["TipoContrato"]);
        }

        public static bool HasEmpresaPermissions(Dictionary<string, bool> menuPermissions)
        {
            return menuPermissions != null &&
                (menuPermissions.ContainsKey("Empresa") && menuPermissions["Empresa"]) ||
                (menuPermissions.ContainsKey("Banco") && menuPermissions["Banco"]) ||
                (menuPermissions.ContainsKey("Monedum") && menuPermissions["Monedum"]) ||
                (menuPermissions.ContainsKey("TipoHorario") && menuPermissions["TipoHorario"]) ||
                (menuPermissions.ContainsKey("Horario") && menuPermissions["Horario"]) ||
                (menuPermissions.ContainsKey("DiaFestivo") && menuPermissions["DiaFestivo"]) ||
                (menuPermissions.ContainsKey("Producto") && menuPermissions["Producto"]) ||
                (menuPermissions.ContainsKey("Ubicacion") && menuPermissions["Ubicacion"]) ||
                (menuPermissions.ContainsKey("Employee") && menuPermissions["Employee"]);
        }
    }
}
