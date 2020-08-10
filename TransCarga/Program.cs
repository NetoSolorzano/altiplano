using System;
using System.Windows.Forms;

namespace TransCarga
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        public static string vg_user = "";      // codigo de usuario
        public static string vg_nuse = "";      // nombre de usuario
        public static string bd = "";           // base de datos seleccionada
        public static string colbac = "";       // back color
        public static string colpag = "";       // pagaframe color
        public static string colgri = "";       // grids color fondo sin grilla
        public static string colfog = "";       // grids color fondo con grilla
        public static string colsbg = "";       // grids color seleccion fondo
        public static string colsfc = "";       // grids color seleccion contenido
        public static string colstr = "";       // strip color
        public static string colpnc = "";       // panel cabecera color
        public static string m70 = "";          // acceso directo a modulo almacen fisico
        public static string cliente = "";      // cliente del sistema
        public static string almuser = "";     // valor almacen del usuario
        public static string retorna1 = "";
        public static string ruc = "";          // ruc del cliente
        public static string tituloF = "SOLORSOFT - TransCarga" + Environment.NewLine +
            "Solución para empresas de Transporte de Carga";      // titulo del sistema
        public static bool vg_conSol = false;   // usa conector solorsoft para ruc y dni

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new login());
        }
    }
}
