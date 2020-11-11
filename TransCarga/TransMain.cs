using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using MySql.Data.MySqlClient;

namespace TransCarga
{
    public partial class TransMain : Form
    {
        #region conexion a la base de datos
        // own database connection
        static string serv = "solorsoft.com";
        static string port = ConfigurationManager.AppSettings["port"].ToString();
        static string usua = "solorsof_rei";
        static string cont = "190969Sorol";
        static string data = ConfigurationManager.AppSettings["data"].ToString();
        static string ctl = "300"; // ConfigurationManager.AppSettings["ConnectionLifeTime"].ToString();
        string DB_CONN_STR = "server=" + serv + ";port=" + port + ";uid=" + usua + ";pwd=" + cont + ";database=" + data +
            ";ConnectionLifeTime=" + ctl + ";";
        #endregion

        #region variables publicas
        // datos generales del emisor para fact. electrónica
        string nomclie = "";                                            // nombre comercial emisor
        string rucclie = "";                                            // ruc del emisor
        string dirclie = "";                                            // direccion fiscal del emisor
        string rasclie = "";                                            // razon social emisor
        string tasaigv = "";                                            // tasa IGV
        string ubigeoe = "";                                            // ubigeo del emisor
        string direcem = "";                                            // direccion de emision
        string distemi = "";                                            // distrito
        string provemi = "";                                            // provincia
        string depaemi = "";                                            // departamento
        string urbemis = "";                                            // urbanizacion
        // ticket impresion 
        string leyen1 = "";                                             // leyenda1
        string nuausu = "";                                             // autorizsunat
        string leyen3 = "";                                             // leyenda3
        string desped = "";                                             // despedida
        string despe2 = "";                                             // despedida 2
        string provee = "";                                             // ose o pse
        string Cfactura = "";                                           // documento factura
        string Cboleta = "";                                            // documento boleta
        string iFE = "";                                                // identificador de factura electrónica
        // funcionamiento del formulario
        string urlemis = "";                                            // url de la empresa
        string nomform = "TransMain";                                   // nombre del formulario
        string asd = Program.vg_user;                                   // usuario logueado
        string img_log1 = "";                                           // ruta y nombre del logo del applicativo
        string img_sol1 = "";                                           // ruta y nombre del logo de solorsoft.com
        string img_sali = "";                                           // imagen para el boton de salir del sistema
        string img_pcon = "";                                           // imagen para el boton de panel de control
        string img_fact = "";                                           // imagen para el boton de facturacion
        string img_vent = "";                                           // imagen para el boton de ventas contratos
        string img_pedi = "";                                           // imagen para el boton de pedidos de fab.
        string img_alma = "";                                           // imagen para el boton de almacen
        string img_maes = "";                                           // imagen para el boton de maestras
        string imgF1 = "";                                              // imagen1 de menu facturacion
        string imgF2 = "";                                              // imagen2 de menu facturacion
        string imgF3 = "";                                              // imagen3 de menu facturacion
        string imgF4 = "";                                              // imagen4 de menu facturacion
        string imgF5 = "";                                              // imagen5 de menu facturacion
        string imgpc1 = "";                                             // imagen1 de menu panel de control
        string imgpc2 = "";                                             // imagen2 de menu panel de control
        string imgpc3 = "";                                             // imagen3 de menu panel de control
        string imgpc4 = "";                                             // imagen4 de menu panel de control
        string imgpc5 = "";                                             // imagen5 de menu panel de control
        string imgma1 = "";                                             // imagen 1 maestras - clientes
        string imgma2 = "";                                             // imagen 2 maestras - artículos
        string imgma3 = "";                                             // imagen 3 maestras - camiones
        string imgma4 = "";                                             // imagen 4 maestras - RR.HH.
        string imgma5 = "";                                             // imagen 5 maestras - Tipos de cambio
        string imgpe1 = "";                                             // imagen 1 administ - cuadre
        string imgpe2 = "";                                             // imagen 2 administ - cobranzas
        string imgpe3 = "";                                             // imagen 3 administ - egresos
        string imgpe4 = "";                                             // imagen 4 administ - ingresos varios
        string imgpe5 = "";                                             // imagen 5 administ - reportes
        string imgvc1 = "";                                             // imagen 1 ventas - contratos
        string imgvpc1 = "";                                            // imagen 1 operaciones 
        string imgvic1 = "";                                            // imagen 1 operaciones - 
        string imgvsc1 = "";                                            // imagen 1 operaciones - 
        string imgvtc1 = "";                                            // imagen 1 operaciones - transbordos
        string imgvre1 = "";                                            // imagen 1 operaciones - reportes
        string imgalm1 = "";                                            // imagen 1 almacen - gestion
        string imgalm2 = "";                                            // imagen 2 almacen - movimientos fiscos
        string imgalm3 = "";                                            // imagen 3 almacen - historico de ventas
        // botones de accion
        string img_btN = "";                                            // imagen del boton de accion NUEVO
        string img_btE = "";                                            // imagen del boton de accion EDITAR
        string img_btA = "";                                            // imagen del boton de accion ANULAR/BORRAR
        string img_btP = "";                                            // imagen del boton de accion IMPRIMIR
        string img_bti = "";                                            // imagen del boton de accion IR AL INICIO
        string img_bts = "";                                            // imagen del boton de accion SIGUIENTE
        string img_btr = "";                                            // imagen del boton de accion RETROCEDE
        string img_btf = "";                                            // imagen del boton de accion IR AL FINAL
        // varios
        public string nufha = "";                                       // nombre del formulario hijo activo
        #endregion

        public TransMain()
        {
            InitializeComponent();
        }

        private void TransMain_Load(object sender, EventArgs e)
        {
            jalainfo();                                         // jalamos los parametros 
            Image logo1 = Image.FromFile(img_log1);
            Image solo1 = Image.FromFile(img_sol1);
            Image salir = Image.FromFile(img_sali);
            Image factu = Image.FromFile(img_fact);
            Image venta = Image.FromFile(img_vent);
            Image pedid = Image.FromFile(img_pedi);
            Image almac = Image.FromFile(img_alma);
            Image maest = Image.FromFile(img_maes);
            Image panel = Image.FromFile(img_pcon);
            pictureBox1.Image = logo1;
            bt_solorsoft.Image = solo1;
            bt_salir.Image = salir;
            bt_facele.Image = factu;
            bt_ventas.Image = venta;
            bt_pedidos.Image = pedid;
            bt_almacen.Image = almac;
            bt_maestras.Image = maest;
            bt_pcontrol.Image = panel;
            // botones de acciones
            Image botnew = Image.FromFile(img_btN);
            Image botedi = Image.FromFile(img_btE);
            Image botanu = Image.FromFile(img_btA);
            Image botimp = Image.FromFile(img_btP);
            //Image bot     // vista preliminar
            Image botini = Image.FromFile(img_bti);     // ir al inicio
            Image botsig = Image.FromFile(img_bts);     // siguiente
            Image botret = Image.FromFile(img_btr);     // retrocede
            Image botfin = Image.FromFile(img_btf);     // ir al final
            //bt_nuevo.Image = botnew;

            cuadre();                                           // formateamos el form principal
            pn_phor.BackColor = Color.Gray;
            pn_pver.BackColor = Color.Gray;
            bt_facele.BackColor = Color.White;
            bt_salir.BackColor = Color.White;
            bt_ventas.BackColor = Color.White;
            bt_pedidos.BackColor = Color.White;
            bt_almacen.BackColor = Color.White;
            bt_maestras.BackColor = Color.White;
            bt_pcontrol.BackColor = Color.White;
            pn_user.BackColor = Color.White;
            pn_menu.BackColor = Color.White;
            //pn_acciones.BackColor = Color.White;
            //
            tx_user.Text = Program.vg_user;                     // código de usuario
            tx_nuser.Text = Program.vg_nuse;                    // nombre de usuario
            tx_empresa.Text = Program.cliente;                 // nombre de la organización
            //
            pn_phor.Controls.Add(pn_menu);
            pn_menu.Width = pn_phor.Width;  // - pn_acciones.Width;
            menuStrip1.Visible = true;
            pn_menu.Controls.Add(menuStrip1);
            menuStrip1.Dock = DockStyle.Top;
            //
            dataload();                                         // jalamos datos comunes a todo el sistema
        }

        private void jalainfo()
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                string consulta = "select * from baseconf limit 1";
                MySqlCommand micon = new MySqlCommand(consulta, conn);
                micon.CommandTimeout = 300;
                MySqlDataReader dr = micon.ExecuteReader();
                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        nomclie = dr.GetString("Cliente");                      // nombre comercial
                        rucclie = dr.GetString("Ruc");
                        dirclie = dr.GetString("direcc").Trim() + " - " + dr.GetString("distrit").Trim();
                        rasclie = dr.GetString("razonsocial");
                        tasaigv = dr.GetString("igv");
                        ubigeoe = dr.GetString("referen1");                     // ubigeo del emisor
                        direcem = dr.GetString("direcc").Trim();
                        distemi = dr.GetString("distrit").Trim();
                        provemi = dr.GetString("provin").Trim();
                        urbemis = dr.GetString("referen2").Trim();              // urbanizacion
                        depaemi = dr.GetString("depart").Trim();          // departamento
                        urlemis = dr.GetString("urlCliente").Trim();          // 
                    }
                    dr.Close();
                }
                else
                {
                    dr.Close();
                    conn.Close();
                    MessageBox.Show("No se ubica empresa", "Error fatal de config.");
                    Application.Exit();
                    return;
                }
                //
                consulta = "select campo,param,valor from enlaces where formulario=@nofo";
                micon = new MySqlCommand(consulta, conn);
                micon.Parameters.AddWithValue("@nofo", "main");
                MySqlDataAdapter da = new MySqlDataAdapter(micon);
                DataTable dt = new DataTable();
                da.Fill(dt);
                for (int t = 0; t < dt.Rows.Count; t++)
                {
                    DataRow row = dt.Rows[t];
                    if (row["campo"].ToString() == "leyendas")
                    {
                        if (row["param"].ToString() == "1") leyen1 = row["valor"].ToString();             // leyenda1
                        if (row["param"].ToString() == "2") nuausu = row["valor"].ToString();             // autorizsunat
                        if (row["param"].ToString() == "3") leyen3 = row["valor"].ToString();             // leyenda3
                        if (row["param"].ToString() == "4") desped = row["valor"].ToString();             // despedida
                        if (row["param"].ToString() == "5") despe2 = row["valor"].ToString();             // despedida 2
                        if (row["param"].ToString() == "6") provee = row["valor"].ToString();             // pag. del proveedor
                    }
                    if (row["campo"].ToString() == "docvta")
                    {
                        if (row["param"].ToString() == "factura") Cfactura = row["valor"].ToString();           // documento factura
                        if (row["param"].ToString() == "boleta") Cboleta = row["valor"].ToString();             // documento boleta
                    }
                    if (row["campo"].ToString() == "identificador")
                    {
                        if (row["param"].ToString() == "identif") iFE = row["valor"].ToString().Trim();         // identif. de fact. electrónica
                    }
                    if(row["campo"].ToString() == "imagenes")
                    {
                        if (row["param"].ToString() == "logoPrin") img_log1 = row["valor"].ToString().Trim();   // logo principal
                        if (row["param"].ToString() == "logosolChi") img_sol1 = row["valor"].ToString().Trim(); // logo solorsoft chico
                        if (row["param"].ToString() == "imgsalir") img_sali = row["valor"].ToString().Trim();   // imagen boton salida
                        if (row["param"].ToString() == "imgpcont") img_pcon = row["valor"].ToString().Trim();   // imagen boton panel de control
                        if (row["param"].ToString() == "imgfactu") img_fact = row["valor"].ToString().Trim();   // imagen boton facturacion
                        if (row["param"].ToString() == "imgventa") img_vent = row["valor"].ToString().Trim();   // imagen para el boton de ventas contratos
                        if (row["param"].ToString() == "imgpedid") img_pedi = row["valor"].ToString().Trim();   // imagen para el boton de pedidos fab.
                        if (row["param"].ToString() == "imgalmac") img_alma = row["valor"].ToString().Trim();   // imagen para el boton de almacen
                        if (row["param"].ToString() == "imgmaest") img_maes = row["valor"].ToString().Trim();   // imagen para el boton de maestras
                        if (row["param"].ToString() == "imgF1") imgF1 = row["valor"].ToString().Trim();         // imagen1 del menu de facturacion opcion1
                        if (row["param"].ToString() == "imgF2") imgF2 = row["valor"].ToString().Trim();         // imagen2 del menu de facturacion opcion2
                        if (row["param"].ToString() == "imgF3") imgF3 = row["valor"].ToString().Trim();         // imagen3 del menu de facturacion opcion3
                        if (row["param"].ToString() == "imgF4") imgF4 = row["valor"].ToString().Trim();         // imagen4 del menu de facturacion opcion4
                        if (row["param"].ToString() == "imgF5") imgF5 = row["valor"].ToString().Trim();         // imagen5 del menu de facturacion opcion5
                        if (row["param"].ToString() == "imgpc1") imgpc1 = row["valor"].ToString().Trim();         // imagen1 del menu de facturacion opcion1
                        if (row["param"].ToString() == "imgpc2") imgpc2 = row["valor"].ToString().Trim();         // imagen2 del menu de facturacion opcion2
                        if (row["param"].ToString() == "imgpc3") imgpc3 = row["valor"].ToString().Trim();         // imagen3 del menu de facturacion opcion3
                        if (row["param"].ToString() == "imgpc4") imgpc4 = row["valor"].ToString().Trim();         // imagen4 del menu de facturacion opcion4
                        if (row["param"].ToString() == "imgpc5") imgpc5 = row["valor"].ToString().Trim();         // imagen5 del menu de facturacion opcion4
                        if (row["param"].ToString() == "imgma1") imgma1 = row["valor"].ToString().Trim();         // imagen1 de maestras - clientes
                        if (row["param"].ToString() == "imgma2") imgma2 = row["valor"].ToString().Trim();         // imagen2 de maestras - proveedores 
                        if (row["param"].ToString() == "imgma3") imgma3 = row["valor"].ToString().Trim();         // imagen3 de maestras - vehiculos 
                        if (row["param"].ToString() == "imgma4") imgma4 = row["valor"].ToString().Trim();         // imagen3 de maestras - RR.HH.
                        if (row["param"].ToString() == "imgma5") imgma5 = row["valor"].ToString().Trim();         // imagen3 de maestras - Tipos de cambio
                        if (row["param"].ToString() == "imgpe1") imgpe1 = row["valor"].ToString().Trim();         // imagen1 de administ - cuadre
                        if (row["param"].ToString() == "imgpe2") imgpe2 = row["valor"].ToString().Trim();         // imagen2 de administ - cobranzas
                        if (row["param"].ToString() == "imgpe3") imgpe3 = row["valor"].ToString().Trim();         // imagen3 de administ - egresos
                        if (row["param"].ToString() == "imgpe4") imgpe4 = row["valor"].ToString().Trim();         // imagen4 de administ - ingresos extraordinarios
                        if (row["param"].ToString() == "imgpe5") imgpe5 = row["valor"].ToString().Trim();         // imagen5 de administ - reportes
                        if (row["param"].ToString() == "imgvc1") imgvc1 = row["valor"].ToString().Trim();         // imagen1 de ventas contratos
                        if (row["param"].ToString() == "imgvpc1") imgvpc1 = row["valor"].ToString().Trim();         // imagen1 de ventas contratos pedidos clientes
                        if (row["param"].ToString() == "imgvic1") imgvic1 = row["valor"].ToString().Trim();         // imagen1 de ventas ingreso pedidos clientes
                        if (row["param"].ToString() == "imgvsc1") imgvsc1 = row["valor"].ToString().Trim();         // imagen1 de ventas salidas pedidos clientes
                        if (row["param"].ToString() == "imgvtc1") imgvtc1 = row["valor"].ToString().Trim();         // imagen1 operaciones transbordos
                        if (row["param"].ToString() == "imgvre1") imgvre1 = row["valor"].ToString().Trim();         // imagen1 de ventas clientes reportes
                        if (row["param"].ToString() == "imgalm1") imgalm1 = row["valor"].ToString().Trim();         // imagen1 de almace - gestion
                        if (row["param"].ToString() == "imgalm2") imgalm2 = row["valor"].ToString().Trim();         // imagen2 de almace - movimientos fisicos
                        if (row["param"].ToString() == "imgalm3") imgalm3 = row["valor"].ToString().Trim();         // imagen3 de almace - historico de ventas
                        // .. resto de imagenes de ventas
                        if (row["param"].ToString() == "img_btN") img_btN = row["valor"].ToString().Trim();         // imagen del boton de accion NUEVO
                        if (row["param"].ToString() == "img_btE") img_btE = row["valor"].ToString().Trim();         // imagen del boton de accion EDITAR
                        if (row["param"].ToString() == "img_btA") img_btA = row["valor"].ToString().Trim();         // imagen del boton de accion ANULAR/BORRAR
                        if (row["param"].ToString() == "img_btP") img_btP = row["valor"].ToString().Trim();         // imagen del boton de accion IMPRIMIR
                        // boton de vista preliminar .... esta por verse su utlidad
                        if (row["param"].ToString() == "img_bti") img_bti = row["valor"].ToString().Trim();         // imagen del boton de accion IR AL INICIO
                        if (row["param"].ToString() == "img_bts") img_bts = row["valor"].ToString().Trim();         // imagen del boton de accion SIGUIENTE
                        if (row["param"].ToString() == "img_btr") img_btr = row["valor"].ToString().Trim();         // imagen del boton de accion RETROCEDE
                        if (row["param"].ToString() == "img_btf") img_btf = row["valor"].ToString().Trim();         // imagen del boton de accion IR AL FINAL
                    }
                }
                da.Dispose();
                dt.Dispose();
                conn.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Error de conexión");
                Application.Exit();
                return;
            }
        }
        private void dataload()
        {
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                try
                {
                    conn.Open();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("No fue posible conectarse al servidor","Error en la conexión",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    Application.Exit();
                }
                // tabla de ubigeos - departamentos, provincias, distritos
                string consulta = "select * from ubigeos";
                using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter(micon))
                    {
                        DataTable dtu = new DataTable();
                        da.Fill(dtu);
                        CacheManager.AddItem("ubigeos", dtu, 3600);
                    }
                }
            }
        }
        public string[] toolboton(string formu)
        {
            string[] retorno = new string[3];
            retorno[0] = "";
            retorno[1] = "";
            retorno[2] = "";

            DataTable mdtb = new DataTable();
            const string consbot = "select * from permisos where formulario=@nomform and usuario=@use";
            MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
            conn.Open();
            if (conn.State == ConnectionState.Open)
            {
                try
                {
                    MySqlCommand consulb = new MySqlCommand(consbot, conn);
                    consulb.Parameters.AddWithValue("@nomform", formu);
                    consulb.Parameters.AddWithValue("@use", asd);
                    MySqlDataAdapter mab = new MySqlDataAdapter(consulb);
                    mab.Fill(mdtb);
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, " Error ");
                    return retorno;
                }
                finally { conn.Close(); }
            }
            else
            {
                MessageBox.Show("No se pudo conectar con el servidor", "Error de conexión");
                Application.Exit();
                return retorno;
            }
            if (mdtb.Rows.Count > 0)
            {
                DataRow row = mdtb.Rows[0];

                if (Convert.ToString(row["btn1"]) == "S")
                {
                    retorno[0] = "true";
                }
                else { retorno[0] = "false"; }
                if (Convert.ToString(row["btn2"]) == "S")
                {
                    retorno[1] = "true";
                }
                else { retorno[1] = "false"; }
                if (Convert.ToString(row["btn5"]) == "S")
                {
                    retorno[2] = "true";
                }
                else { retorno[2] = "false"; }
            }
            return retorno;
        }

        private void cuadre()
        {
            ControlBox = true;
            MaximizeBox = true;
            MinimizeBox = true;
            FormBorderStyle = FormBorderStyle.Sizable;  // FormBorderStyle.FixedSingle
            Text = Program.tituloF;
            Left = Screen.PrimaryScreen.Bounds.Left;
            Top = Screen.PrimaryScreen.Bounds.Top;
            //Width = Screen.PrimaryScreen.Bounds.Width;
            //Height = Screen.PrimaryScreen.Bounds.Height;
            //
            bt_facele.Top = pictureBox1.Top + pictureBox1.Height + 2;
            bt_ventas.Top = bt_facele.Top + bt_facele.Height + 2;
            bt_pedidos.Top = bt_ventas.Top + bt_ventas.Height + 2;
            bt_almacen.Top = bt_pedidos.Top + bt_pedidos.Height + 2;
            bt_maestras.Top = bt_almacen.Top + bt_almacen.Height + 2;
            bt_pcontrol.Top = bt_maestras.Top + bt_maestras.Height + 2;
        }

        #region botones_verticales
        private void bt_salir_Click(object sender, EventArgs e)
        {
            var aa = MessageBox.Show("Realmente desea salir del sistema?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if(aa == DialogResult.Yes)
            {
                Application.Exit();
                return;
            }
        }
        #endregion

        #region botones_horizontales
        private void bt_solorsoft_Click(object sender, EventArgs e)
        {
            string url = "http://solorsoft.com";
            Process.Start(url);
        }
        #endregion

        #region botones_click   // menus
        private void bt_facele_Click(object sender, EventArgs e)        // facturacion electrónica
        {
            Image img_F1 = Image.FromFile(imgF1);
            Image img_F2 = Image.FromFile(imgF2);
            Image img_F3 = Image.FromFile(imgF3);
            Image img_F4 = Image.FromFile(imgF4);
            Image img_F5 = Image.FromFile(imgF5);
            //
            pic_icon_menu.Image = TransCarga.Properties.Resources.fec_elect21;
            menuStrip1.Items.Clear();
            menuStrip1.Items.Add("Boletas/Facturas",img_F1,fac_rapida_Click);           // F1
            menuStrip1.Items.Add("Anulaciones",img_F4,fac_anulac_Click);              // F4
            menuStrip1.Items.Add("Reportes",img_F5,fac_reportes_Click);               // F5
            //
            menuStrip1.Visible = true;
        }
        private void fac_rapida_Click(object sender, EventArgs e)       // factura rapida
        {
            facelect ffe1 = new TransCarga.facelect();
            ffe1.TopLevel = false;
            ffe1.Parent = this;
            ffe1.Top = pn_phor.Top + pn_phor.Height + 1;
            ffe1.Left = pn_pver.Left + pn_pver.Width + 1;
            pn_centro.Controls.Add(ffe1);
            ffe1.Show();
        }
        private void fac_anulac_Click(object sender, EventArgs e)       // anulaciones de facturas
        {
            MessageBox.Show("Form de anulaciones de facturas");
        }
        private void fac_reportes_Click(object sender, EventArgs e)     // reportes de facturas
        {
            MessageBox.Show("Form de reportes de facturas");
        }
        //
        private void bt_ventas_Click(object sender, EventArgs e)        // Operaciones
        {
            Image img_v_c = Image.FromFile(imgvc1);
            Image img_v_pc = Image.FromFile(imgvpc1);
            Image img_v_i = Image.FromFile(imgvic1);
            Image img_v_s = Image.FromFile(imgvsc1);
            Image img_v_t = Image.FromFile(imgvtc1);
            Image img_v_r = Image.FromFile(imgvre1);
            //
            pic_icon_menu.Image = TransCarga.Properties.Resources.etiq_venta32;
            menuStrip1.Items.Clear();
            menuStrip1.Items.Add("Pre-Guías",img_v_c, vc_registro_Click);
            menuStrip1.Items.Add("GR Transp.",img_v_pc, vpc_registro_Click);
            menuStrip1.Items.Add("GR Remitente",img_v_i, vic_registro_Click);
            menuStrip1.Items.Add("Plan.Carga",img_v_s, vsc_registro_Click);
            menuStrip1.Items.Add("Transbordo", img_v_t, vtc_registro_Click);
            menuStrip1.Items.Add("Reportes",img_v_r, vc_reportes_Click);
            menuStrip1.Visible = true;
        }
        private void vc_registro_Click(object sender, EventArgs e)          // Pre guías transportista
        {
            preguiat fvc = new preguiat();
            fvc.TopLevel = false;
            fvc.Parent = this;
            pn_centro.Controls.Add(fvc);
            fvc.Location = new Point((pn_centro.Width - fvc.Width) / 2, (pn_centro.Height - fvc.Height) / 2);
            fvc.Anchor = AnchorStyles.None;
            fvc.Show();
            fvc.BringToFront();
        }       
        private void vpc_registro_Click(object sender, EventArgs e)         // Guías Remisión Transportista
        {
            guiati fpc = new guiati();
            fpc.TopLevel = false;
            fpc.Parent = this;
            pn_centro.Controls.Add(fpc);
            fpc.Location = new Point((pn_centro.Width - fpc.Width) / 2, (pn_centro.Height - fpc.Height) / 2);
            fpc.Anchor = AnchorStyles.None;
            fpc.Show();
            fpc.BringToFront();
        }      
        private void vic_registro_Click(object sender, EventArgs e)         // Guías Remisión Remitente
        {
            /*
            ingpedclts fip = new ingpedclts();
            fip.TopLevel = false;
            fip.Parent = this;
            pn_centro.Controls.Add(fip);
            //fip.Location = new Point((pn_centro.Width - fip.Width) / 2, (pn_centro.Height - fip.Height) / 2);
            fip.Anchor = AnchorStyles.None;
            fip.Show();
            fip.BringToFront();
            */
        }
        private void vsc_registro_Click(object sender, EventArgs e)         // Planilla de carga
        {
            planicarga fsp = new planicarga();
            fsp.TopLevel = false;
            fsp.Parent = this;
            pn_centro.Controls.Add(fsp);
            //fsp.Location = new Point((pn_centro.Width - fsp.Width) / 2, (pn_centro.Height));
            fsp.Anchor = AnchorStyles.None;
            fsp.Show();
            fsp.BringToFront();
        }
        private void vtc_registro_Click(object sender, EventArgs e)         // transbordos
        {
            transbord fsp = new transbord();
            fsp.TopLevel = false;
            fsp.Parent = this;
            pn_centro.Controls.Add(fsp);
            //fsp.Location = new Point((pn_centro.Width - fsp.Width) / 2, (pn_centro.Height));
            fsp.Anchor = AnchorStyles.None;
            fsp.Show();
            fsp.BringToFront();
        }
        private void vc_reportes_Click(object sender, EventArgs e)          // Reportes de operaciones
        {
            repsoper frv = new repsoper();
            frv.TopLevel = false;
            frv.Parent = this;
            pn_centro.Controls.Add(frv);
            frv.Show();
            if (frv.Parent.Width < frv.Width)
            {
                //frv.Parent.Width = frv.Width + 10;
                this.Width = this.Width + (this.Width - frv.Width) + 20;
            }
            frv.BringToFront();
        }
        //
        private void bt_pedidos_Click(object sender, EventArgs e)           // Administración
        {
            pic_icon_menu.Image = TransCarga.Properties.Resources.process32;
            Image img_pe1 = Image.FromFile(imgpe1);                     // Cuadre de Caja
            Image img_pe2 = Image.FromFile(imgpe2);                     // Cobranzas
            Image img_pe3 = Image.FromFile(imgpe3);                     // Egresos (Varios, Depositos) 
            Image img_pe4 = Image.FromFile(imgpe4);                     // Ingresos varios
            Image img_pe5 = Image.FromFile(imgpe5);                     // reportes
            menuStrip1.Items.Clear();
            menuStrip1.Items.Add("Cuadre", img_pe1, pe_registro_Click);            // Cuadre, apertura y cierre
            menuStrip1.Items.Add("Cobranzas", img_pe2, pe_cobranzas_Click);            // Cobranzas de ventas
            menuStrip1.Items.Add("Egresos", img_pe3, pe_egresos_Click);            // Egresos y depositos
            menuStrip1.Items.Add("Ing. Varios", img_pe4, pe_ingresosV_Click);            // Ingresos varios
            menuStrip1.Items.Add("Reportes", img_pe5, pe_reportes_Click);            // Reportes
            menuStrip1.Visible = true;
        }
        private void pe_registro_Click(object sender, EventArgs e)
        {
            /*
            Pedsalm fpe = new Pedsalm();
            fpe.TopLevel = false;
            fpe.Parent = this;
            pn_centro.Controls.Add(fpe);
            fpe.Location = new Point((pn_centro.Width - fpe.Width) / 2, (pn_centro.Height - fpe.Height) / 2);
            fpe.Anchor = AnchorStyles.None;
            fpe.Show();
            fpe.BringToFront();
            */
        }
        private void pe_cobranzas_Click(object sender, EventArgs e)
        {

        }
        private void pe_egresos_Click(object sender, EventArgs e)
        {

        }
        private void pe_ingresosV_Click(object sender, EventArgs e)
        {

        }
        private void pe_reportes_Click(object sender, EventArgs e)
        {
            /*
            repspedalm frp = new repspedalm();
            frp.TopLevel = false;
            frp.Parent = this;
            pn_centro.Controls.Add(frp);
            frp.Location = new Point((pn_centro.Width - frp.Width) / 2, (pn_centro.Height - frp.Height) / 2);
            frp.Anchor = AnchorStyles.None;
            frp.Show();
            frp.BringToFront();
            */
        }
        //
        private void bt_pcontrol_Click(object sender, EventArgs e)      // Configuración
        {
            pic_icon_menu.Image = Properties.Resources.service_manager;
            Image img_pc1 = Image.FromFile(imgpc1);
            Image img_pc2 = Image.FromFile(imgpc2);
            Image img_pc3 = Image.FromFile(imgpc3);
            Image img_pc4 = Image.FromFile(imgpc4);
            Image img_pc5 = Image.FromFile(imgpc5);
            menuStrip1.Items.Clear();
            menuStrip1.Items.Add("Usuarios", img_pc1, pc_usuarios_Click);                    // usuarios
            menuStrip1.Items.Add("Definiciones", img_pc2, pc_definiciones_Click);            // definiciones
            menuStrip1.Items.Add("Series", img_pc3, pc_series_Click);                        // series de documentos
            menuStrip1.Items.Add("Enlaces", img_pc4, pc_enlaces_Click);                      // enlaces de datos
            menuStrip1.Items.Add("Permisos", img_pc5, pc_permisos_Click);                    // permisos
            menuStrip1.Visible = true;
        }
        //
        private void bt_maestras_Click(object sender, EventArgs e)      // Maestras
        {
            pic_icon_menu.Image = Properties.Resources.maestras48;
            Image img_ma1 = Image.FromFile(imgma1);
            Image img_ma2 = Image.FromFile(imgma2);
            Image img_ma3 = Image.FromFile(imgma3);
            Image img_ma4 = Image.FromFile(imgma4);
            Image img_ma5 = Image.FromFile(imgma5);
            menuStrip1.Items.Clear();
            menuStrip1.Items.Add("Clientes", img_ma1, ma_clientes_Click);               // clientes
            menuStrip1.Items.Add("Proveedores", img_ma2, ma_proveed_Click);             // proveedores
            menuStrip1.Items.Add("Vehículos", img_ma3, ma_camiones_Click);              // camiones propios y terceros
            menuStrip1.Items.Add("RR.HH.", img_ma4, ma_rrhh_Click);                     // recursos humanos
            menuStrip1.Items.Add("Tipo Cambio", img_ma5, ma_tipcam_Click);              // tipos de cambio
            menuStrip1.Visible = true;
        }
        private void ma_clientes_Click(object sender, EventArgs e)
        {
            clients fmc = new clients();
            fmc.TopLevel = false;
            fmc.Parent = this;
            pn_centro.Controls.Add(fmc);
            fmc.Location = new Point((pn_centro.Width - fmc.Width) / 2, (pn_centro.Height - fmc.Height) / 2);
            fmc.Anchor = AnchorStyles.None;
            fmc.Show();
            fmc.BringToFront();
        }
        private void ma_proveed_Click(object sender, EventArgs e)
        {
            proveed fpr = new proveed();
            fpr.TopLevel = false;
            fpr.Parent = this;
            pn_centro.Controls.Add(fpr);
            fpr.Location = new Point((pn_centro.Width - fpr.Width) / 2, (pn_centro.Height - fpr.Height) / 2);
            fpr.Anchor = AnchorStyles.None;
            fpr.Show();
            fpr.BringToFront();
        }
        private void ma_camiones_Click(object sender, EventArgs e)
        {
            vehiculos fma = new vehiculos();
            fma.TopLevel = false;
            fma.Parent = this;
            pn_centro.Controls.Add(fma);
            fma.Location = new Point((pn_centro.Width - fma.Width) / 2, (pn_centro.Height - fma.Height) / 2);
            fma.Anchor = AnchorStyles.None;
            fma.Show();
            fma.BringToFront();
        }
        private void ma_rrhh_Click(object sender, EventArgs e)
        {
            /*
            adicionals fad = new adicionals();
            fad.TopLevel = false;
            fad.Parent = this;
            pn_centro.Controls.Add(fad);
            fad.Location = new Point((pn_centro.Width - fad.Width) / 2, (pn_centro.Height - fad.Height) / 2);
            fad.Anchor = AnchorStyles.None;
            fad.Show();
            fad.BringToFront();
            */
        }
        private void ma_tipcam_Click(object sender, EventArgs e)
        {
            tipcamref fmc = new tipcamref();
            fmc.TopLevel = false;
            fmc.Parent = this;
            pn_centro.Controls.Add(fmc);
            fmc.Location = new Point((pn_centro.Width - fmc.Width), (pn_centro.Height - fmc.Height) / 2);
            fmc.Anchor = AnchorStyles.None;
            fmc.Show();
            fmc.BringToFront();
        }
        //
        private void pc_usuarios_Click(object sender, EventArgs e)
        {
            users fuser = new TransCarga.users();
            fuser.TopLevel = false;
            fuser.Parent = this;
            pn_centro.Controls.Add(fuser);
            fuser.Location = new Point((pn_centro.Width - fuser.Width) / 2, (pn_centro.Height - fuser.Height) / 2);
            fuser.Anchor = AnchorStyles.None;
            fuser.Show();
            fuser.BringToFront();
        }
        private void pc_definiciones_Click(object sender, EventArgs e)
        {
            defs fdefs = new defs();
            fdefs.TopLevel = false;
            fdefs.Parent = this;
            pn_centro.Controls.Add(fdefs);
            fdefs.Location = new Point((pn_centro.Width - fdefs.Width) / 2, (pn_centro.Height - fdefs.Height) / 2);
            fdefs.Anchor = AnchorStyles.None;
            fdefs.Show();
            fdefs.BringToFront();
        }
        private void pc_series_Click(object sender, EventArgs e)
        {
            sernum fsn = new sernum();
            fsn.TopLevel = false;
            fsn.Parent = this;
            pn_centro.Controls.Add(fsn);
            fsn.Location = new Point((pn_centro.Width - fsn.Width) / 2, (pn_centro.Height - fsn.Height) / 2);
            fsn.Anchor = AnchorStyles.None;
            fsn.Show();
            fsn.BringToFront();
        }
        private void pc_enlaces_Click(object sender, EventArgs e)
        {
            enlaces fenl = new enlaces();
            fenl.TopLevel = false;
            fenl.Parent = this;
            pn_centro.Controls.Add(fenl);
            fenl.Location = new Point((pn_centro.Width - fenl.Width) / 2, (pn_centro.Height - fenl.Height) / 2);
            fenl.Anchor = AnchorStyles.None;
            fenl.Show();
            fenl.BringToFront();
        }
        private void pc_permisos_Click(object sender, EventArgs e)
        {
            permisos fper = new permisos();
            fper.TopLevel = false;
            fper.Parent = this;
            pn_centro.Controls.Add(fper);
            fper.Location = new Point((pn_centro.Width - fper.Width) / 2, (pn_centro.Height - fper.Height) / 2);
            fper.Anchor = AnchorStyles.None;
            fper.Show();
            fper.BringToFront();
        }
        //
        private void bt_almacen_Click(object sender, EventArgs e)       // Almacen
        {
            pic_icon_menu.Image = Properties.Resources.almacen48;
            Image img_alm1 = Image.FromFile(imgalm1);
            Image img_alm2 = Image.FromFile(imgalm2);
            Image img_alm3 = Image.FromFile(imgalm3);
            menuStrip1.Items.Clear();
            menuStrip1.Items.Add("Gestión", img_alm1, alm_gestion_Click);                    // gestion de almacen
            menuStrip1.Items.Add("Mov.Físicos", img_alm2, alm_movfisicos_Click);             // movimientos fisicos
            menuStrip1.Items.Add("Reportes", img_alm3, alm_historicos_Click);                // 
            menuStrip1.Visible = true;
        }
        private void alm_gestion_Click(object sender, EventArgs e)
        {
            /*
            almgestion fenl = new almgestion();
            //pan_inicio fenl = new pan_inicio();
            fenl.TopLevel = false;
            fenl.Parent = this;
            pn_centro.Controls.Add(fenl);
            fenl.Location = new Point((pn_centro.Width - fenl.Width) / 2, (pn_centro.Height - fenl.Height) / 2);
            fenl.Anchor = AnchorStyles.None;
            fenl.Show();
            fenl.BringToFront();
            */
        }
        private void alm_movfisicos_Click(object sender, EventArgs e)
        {
            /*
            almovfis fmf = new almovfis();
            fmf.TopLevel = false;
            fmf.Parent = this;
            pn_centro.Controls.Add(fmf);
            fmf.Location = new Point((pn_centro.Width - fmf.Width) / 2, (pn_centro.Height - fmf.Height) / 2);
            fmf.Anchor = AnchorStyles.None;
            fmf.Show();
            fmf.BringToFront();
            */
        }
        private void alm_historicos_Click(object sender, EventArgs e)
        {
            /*
            repsalmac fra = new repsalmac();
            fra.TopLevel = false;
            fra.Parent = this;
            pn_centro.Controls.Add(fra);
            //fra.Location = new Point((pn_centro.Width - fra.Width) / 2, (pn_centro.Height - fra.Height) / 2);
            fra.Anchor = AnchorStyles.None;
            fra.Show();
            fra.BringToFront();
            */
        }
        //
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            string url = urlemis;   // hacer la pagina de Transcarga con estilo manual
            Process.Start(url);
        }
        #endregion

        private void TransMain_Activated(object sender, EventArgs e)
        {
            //bt_nuevo.Enabled = false;
        }

        private void TransMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                const string mensaje = "Desea salir del sistema?";
                const string titulo = "Confirme por favor";
                var result = MessageBox.Show(mensaje, titulo,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes) Application.Exit(); // Environment.Exit(0);
                else e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
            }
        }
    }
}
