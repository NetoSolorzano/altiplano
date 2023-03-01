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
        public static string vg_tius = "";      // tipo de usuario
        public static string vg_nius = "";      // nivel del usuario
        public static string vg_luse = "";      // codigo local usuario
        public static string vg_nlus = "";      // nombre local usuario
        public static string vg_duse = "";      // dirección locar usuario
        public static string vg_uuse = "";      // ubigeo local usuario
        public static string almuser = "";      // codigo almacen del usuario
        public static string vg_fcaj = "";      // fecha de la caja abierta (vacio si no hay caja abierta)
        public static string bd = "";           // base de datos seleccionada
        // colores formularios y objetos
        public static string colbac = "";       // back color
        public static string colpag = "";       // pagaframe color
        public static string colgri = "";       // grids color fondo sin grilla
        public static string colfog = "";       // grids color fondo con grilla
        public static string colsbg = "";       // grids color seleccion fondo
        public static string colsfc = "";       // grids color seleccion contenido
        public static string colstr = "";       // strip color
        public static string colpnc = "";       // panel cabecera color
        // datos del cliente implementado
        public static string cliente = "";      // cliente del sistema
        public static string ruc = "";          // ruc del cliente
        public static string dirfisc = "";      // direccion fiscal del cliente
        public static string ubidirfis = "";    // ubigeo de la direccion fiscal
        public static string distfis = "";      // distrito 
        public static string provfis = "";      // provincia
        public static string depfisc = "";      // departamento
        public static string codlocsunat = "";  // codigo sunat del local de emisión del DV
        public static string mailclte = "";     // correo electronico del cliente del sistema
        public static string telclte1 = "";     // telefono cliente del sistema
        public static string regmtc = "";       // registro MTC 
        // datos de impuestos, detracciones, sunat
        public static string ctadetra = "";     // cuenta de detraccion
        public static string valdetra = "";     // valor flete desde donde origina la detraccion
        public static string pordetra = "";     // valor en % de la detraccion
        public static string coddetra = "";     // codigo sunat detraccion
        //
        public static string retorna1 = "";
        public static string tituloF = "SOLORSOFT - TransCarga";      // titulo del sistema
        public static string vg_ipwan = "";     // ip wan del cliente
        public static bool vg_conSol = false;   // usa conector solorsoft para ruc y dni

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new login());   // new login()
        }
    }
}
