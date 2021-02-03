using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace TransCarga
{
    public partial class ayccaja : Form
    {
        static string nomform = "ayccaja";             // nombre del formulario
        string colback = TransCarga.Program.colbac;   // color de fondo
        string colpage = TransCarga.Program.colpag;   // color de los pageframes
        string colgrid = TransCarga.Program.colgri;   // color de las grillas
        string colfogr = TransCarga.Program.colfog;   // color fondo con grillas
        string colsfon = TransCarga.Program.colsbg;   // color fondo seleccion
        string colsfgr = TransCarga.Program.colsfc;   // color seleccion grilla
        string colstrp = TransCarga.Program.colstr;   // color del strip
        bool conectS = TransCarga.Program.vg_conSol;    // usa conector solorsoft? true=si; false=no
        static string nomtab = "cabccaja";

        #region variables
        string img_btN = "";
        string img_btE = "";
        string img_btA = "";            // anula = bloquea
        string img_btP = "";            // imprime
        string img_btV = "";            // visualiza
        string img_bti = "";            // imagen boton inicio
        string img_bts = "";            // imagen boton siguiente
        string img_btr = "";            // imagen boton regresa
        string img_btf = "";            // imagen boton final
        string img_btq = "";
        string img_grab = "";
        string img_anul = "";
        string img_ver = "";
        string MonDeft = "";            // monde por defecto
        string v_clu = "";              // codigo del local del usuario
        string v_slu = "";              // serie del local del usuario
        string v_nbu = "";              // nombre del usuario
        string codAbie = "";            // codigo caja abierta
        string codCier = "";            // codigo caja cerrada
        decimal vsali = 0;              // saldo inicial del cuadre
        decimal vsalf = 0;              // saldo final del cuadre
        //
        static libreria lib = new libreria();   // libreria de procedimientos
        publico lp = new publico();             // libreria de clases
        string verapp = System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion;
        string nomclie = Program.cliente;           // cliente usuario del sistema
        string rucclie = Program.ruc;               // ruc del cliente usuario del sistema
        string asd = TransCarga.Program.vg_user;    // usuario conectado al sistema
        string dirloc = TransCarga.Program.vg_duse; // direccion completa del local usuario conectado
        string ubiloc = TransCarga.Program.vg_uuse; // ubigeo local del usuario conectado
        #endregion

        // string de conexion
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + login.data + ";";
        DataTable dtcuad = new DataTable();

        public ayccaja()
        {
            InitializeComponent();
        }
        private void ayccaja_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SendKeys.Send("{TAB}");
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.N) Bt_add.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.E) Bt_edit.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.A) Bt_anul.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.O) Bt_ver.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.P) Bt_print.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.S) Bt_close.PerformClick();
        }
        private void ayccaja_Load(object sender, EventArgs e)
        {
            /*
            ToolTip toolTipNombre = new ToolTip();           // Create the ToolTip and associate with the Form container.
            // Set up the delays for the ToolTip.
            toolTipNombre.AutoPopDelay = 5000;
            toolTipNombre.InitialDelay = 1000;
            toolTipNombre.ReshowDelay = 500;
            toolTipNombre.ShowAlways = true;                 // Force the ToolTip text to be displayed whether or not the form is active.
            toolTipNombre.SetToolTip(toolStrip1, nomform);   // Set up the ToolTip text for the object
            */
            this.Focus();
            jalainfo();
            init();
            dataload();
            toolboton();
            this.KeyPreview = true;
            if (valiVars() == false)
            {
                //Application.Exit();
                //return;
            }
        }
        private void init()
        {
            this.BackColor = Color.FromName(colback);
            toolStrip1.BackColor = Color.FromName(colstrp);
            //
            tx_user.Text += asd;
            tx_nomuser.Text = TransCarga.Program.vg_nuse;
            tx_locuser.Text = tx_locuser.Text + " " + TransCarga.Program.vg_nlus;    //TransCarga.Program.vg_luse;
            tx_fechact.Text = DateTime.Today.ToString();
            //
            Bt_add.Image = Image.FromFile(img_btN);
            Bt_edit.Image = Image.FromFile(img_btE);
            Bt_anul.Image = Image.FromFile(img_btA);
            Bt_ver.Image = Image.FromFile(img_btV);
            Bt_print.Image = Image.FromFile(img_btP);
            Bt_close.Image = Image.FromFile(img_btq);
            Bt_ini.Image = Image.FromFile(img_bti);
            Bt_sig.Image = Image.FromFile(img_bts);
            Bt_ret.Image = Image.FromFile(img_btr);
            Bt_fin.Image = Image.FromFile(img_btf);
            // longitudes maximas de campos
            sololee();
        }
        private void initIngreso()
        {
            limpiar();
            limpia_chk();
            limpia_otros();
            limpia_combos();
            tx_fechope.Text = DateTime.Today.ToString("dd/MM/yyyy");
            tx_dat_userdoc.Text = asd;
            tx_digit.Text = v_nbu;
            //tx_dat_estad.Text = codAbie;
        }
        private void jalainfo()                 // obtiene datos de imagenes y variables
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                string consulta = "select formulario,campo,param,valor from enlaces where formulario in (@nofo,@nfin,@nofa,@nofi)";
                MySqlCommand micon = new MySqlCommand(consulta, conn);
                micon.Parameters.AddWithValue("@nofo", "main");
                micon.Parameters.AddWithValue("@nfin", "interno");
                micon.Parameters.AddWithValue("@nofi", "xxx");      // libre
                micon.Parameters.AddWithValue("@nofa", nomform);
                MySqlDataAdapter da = new MySqlDataAdapter(micon);
                DataTable dt = new DataTable();
                da.Fill(dt);
                for (int t = 0; t < dt.Rows.Count; t++)
                {
                    DataRow row = dt.Rows[t];
                    if (row["formulario"].ToString() == "main")
                    {
                        if (row["campo"].ToString() == "imagenes")
                        {
                            if (row["param"].ToString() == "img_btN") img_btN = row["valor"].ToString().Trim();         // imagen del boton de accion NUEVO
                            if (row["param"].ToString() == "img_btE") img_btE = row["valor"].ToString().Trim();         // imagen del boton de accion EDITAR
                            if (row["param"].ToString() == "img_btA") img_btA = row["valor"].ToString().Trim();         // imagen del boton de accion ANULAR/BORRAR
                            if (row["param"].ToString() == "img_btQ") img_btq = row["valor"].ToString().Trim();         // imagen del boton de accion SALIR
                            if (row["param"].ToString() == "img_btP") img_btP = row["valor"].ToString().Trim();         // imagen del boton de accion IMPRIMIR
                            if (row["param"].ToString() == "img_btV") img_btV = row["valor"].ToString().Trim();         // imagen del boton de accion visualizar
                            if (row["param"].ToString() == "img_bti") img_bti = row["valor"].ToString().Trim();         // imagen del boton de accion IR AL INICIO
                            if (row["param"].ToString() == "img_bts") img_bts = row["valor"].ToString().Trim();         // imagen del boton de accion SIGUIENTE
                            if (row["param"].ToString() == "img_btr") img_btr = row["valor"].ToString().Trim();         // imagen del boton de accion RETROCEDE
                            if (row["param"].ToString() == "img_btf") img_btf = row["valor"].ToString().Trim();         // imagen del boton de accion IR AL FINAL
                            if (row["param"].ToString() == "img_gra") img_grab = row["valor"].ToString().Trim();         // imagen del boton grabar nuevo
                            if (row["param"].ToString() == "img_anu") img_anul = row["valor"].ToString().Trim();         // imagen del boton grabar anular
                            if (row["param"].ToString() == "img_preview") img_ver = row["valor"].ToString().Trim();         // imagen del boton grabar visualizar
                        }
                    }
                    if (row["formulario"].ToString() == nomform)
                    {
                        if (row["campo"].ToString() == "estado")
                        {
                            if (row["param"].ToString() == "abierto") codAbie = row["valor"].ToString().Trim();         // codigo caja abierta
                            if (row["param"].ToString() == "cerrado") codCier = row["valor"].ToString().Trim();         // codigo caja cerrada
                        }
                        if (row["campo"].ToString() == "documento")
                        {
                            //if (row["param"].ToString() == "codingef") v_ctpe = row["valor"].ToString().Trim();             // codigo ingreso efectivo
                        }
                        if (row["campo"].ToString() == "moneda" && row["param"].ToString() == "default") MonDeft = row["valor"].ToString().Trim();             // moneda por defecto
                    }
                }
                da.Dispose();
                dt.Dispose();
                // jalamos datos del usuario y local
                v_clu = TransCarga.Program.vg_luse;                // codigo local usuario
                v_slu = lib.serlocs(v_clu);                        // serie local usuario
                v_nbu = TransCarga.Program.vg_nuse;                // nombre del usuario
                conn.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Error de conexión");
                Application.Exit();
                return;
            }
        }
        private void jalaoc(string campo)        // jala egresos
        {
            string parte = "";
            if (campo == "tx_idcaja")
            {
                parte = "where loccaja=@loca order by id desc limit 1";   // a.id=@idcaja
            }
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                if (lib.procConn(conn) == true)
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        string consulta = "select a.id,a.userabr,a.usercie,a.fechope,a.fechcie,a.loccaja,a.obscobc,a.codmoMN,a.cantcob,a.cantinv,a.cantegr,a.saldoan," +
                            "a.cobranz,a.ingvari,a.egresos,a.saldofi,a.statusc,b.descrizionerid as ESTADO,u.nombre as CAJERO " +
                            "from cabccaja a " +
                            "left join desc_est b on b.idcodice=a.statusc " +
                            "left join usuarios u on u.nom_user=a.userabr " +
                            parte;
                        using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                        {
                            micon.Parameters.AddWithValue("@loca", v_clu);
                            using (MySqlDataReader dr = micon.ExecuteReader())
                            {
                                if (dr.HasRows == false)
                                {
                                    // primera caja que se abre en el local
                                    button1.Text = "ABRE CAJA";
                                }
                                else
                                {
                                    if (dr.Read())
                                    {
                                        if (dr.GetString("statusc") == codCier)    // estado cerrado
                                        {
                                            // ultima caja esta cerrada
                                            button1.Text = "ABRE CAJA";
                                            vsali = dr.GetDecimal("saldofi");
                                            dataGridView1.Rows.Add("SALDO ANTERIOR", "", vsali.ToString("#0.00"));
                                            dataGridView1.Rows.Add("COBRANZAS", "0", "0");
                                            dataGridView1.Rows.Add("ING.VARIOS", "0", "0");
                                            dataGridView1.Rows.Add("EGRESOS/DEP", "0", "0");
                                            dataGridView1.Rows.Add("SALDO AL CIERRE", "", vsali.ToString("#0.00"));
                                        }
                                        if (dr.GetString("statusc") == codAbie)     // la caja esta abierta
                                        {
                                            button1.Text = "CIERRA CAJA";
                                            // mostramos los datos
                                            tx_dat_estad.Text = dr.GetString("statusc");
                                            tx_estado.Text = dr.GetString("ESTADO");
                                            tx_dat_userdoc.Text = dr.GetString("userabr");
                                            tx_digit.Text = dr.GetString("CAJERO");
                                            tx_fechope.Text = dr.GetString("fechope").Substring(0,10);
                                            tx_idr.Text = dr.GetString("id");
                                            // solo se computan documentos validos NO ANULADOS
                                            decimal salan = dr.GetDecimal("saldoan");
                                            decimal vcobr = dr.GetDecimal("cobranz");
                                            decimal vingv = dr.GetDecimal("ingvari");
                                            decimal vegre = dr.GetDecimal("egresos");
                                            vsalf = salan + vcobr + vingv - vegre;
                                            dataGridView1.Rows.Add("SALDO ANTERIOR", "", dr.GetString("saldoan"));
                                            dataGridView1.Rows.Add("COBRANZAS", dr.GetString("cantcob"), dr.GetString("cobranz"));
                                            dataGridView1.Rows.Add("ING.VARIOS", dr.GetString("cantinv"),dr.GetString("ingvari"));
                                            dataGridView1.Rows.Add("EGRESOS/DEP", dr.GetString("cantegr"),dr.GetString("egresos"));
                                            dataGridView1.Rows.Add("SALDO AL CIERRE", "", vsalf.ToString("#0.00"));
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Application.Exit();
                    return;
                }
            }
        }
        public void dataload()                  // jala datos para los combos 
        {
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                while (true)
                {
                    try
                    {
                        conn.Open();
                        break;
                    }
                    catch (MySqlException ex)
                    {
                        var aa = MessageBox.Show(ex.Message + Environment.NewLine + "No se pudo conectar con el servidor" + Environment.NewLine +
                            "Desea volver a intentarlo?", "Error de conexión", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (aa == DialogResult.No)
                        {
                            Application.Exit();
                            return;
                        }
                    }
                }
                // datos para combos

            }
        }
        private bool valiVars()                 // valida existencia de datos en variables del form
        {
            bool retorna = true;
            if (MonDeft == "")          // moneda por defecto
            {
                lib.messagebox("Moneda por defecto");
                retorna = false;
            }
            if (v_slu == "")            // serie del local del usuario
            {
                lib.messagebox("Serie general local del usuario");
                retorna = false;
            }
            // aca falta agregar resto  ...........
            return retorna;
        }
        private void calculos(decimal totDoc)
        {
            /*
            decimal tigv = 0;
            decimal tsub = 0;
            if (totDoc > 0)
            {
                //tsub = Math.Round(totDoc / (1 + decimal.Parse(v_igv) / 100), 2);
                //tigv = Math.Round(totDoc - tsub, 2);
                
            }
            */
        }
        private void sumdet()                   // totalizamos detalle
        {
            /*
            tx_tfil.Text = "";
            tx_totcant.Text = "";
            decimal tp = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["valorMN"].Value != null && row.Cells["status"].Value.ToString() != codAnul)
                {
                    tp = tp + decimal.Parse(row.Cells["valorMN"].Value.ToString());  // row["valorMN"].ToString()
                }
            }
            tx_tfil.Text = (dataGridView1.Rows.Count - 1).ToString();
            tx_totcant.Text = tp.ToString();
            */
        }

        #region limpiadores_modos
        private void sololee()
        {
            lp.sololee(this);
        }
        private void escribe()
        {
            lp.escribe(this);
        }
        private void limpiar()
        {
            lp.limpiar(this);
        }
        private void limpia_chk()    
        {
            lp.limpia_chk(this);
        }
        private void limpia_otros()
        {
            //
        }
        private void limpia_combos()
        {
            lp.limpia_cmb(this);
        }
        #endregion limpiadores_modos;

        #region boton_form GRABA EDITA ANULA
        private void button1_Click(object sender, EventArgs e)
        {
            #region validaciones
            // 
            #endregion
            // grabamos, actualizamos, etc
            string modo = Tx_modo.Text;
            string iserror = "no";
            if (modo == "NUEVO" || modo == "EDITAR")
            {
                string keta = "";
                // validaciones de ingresos
                if (button1.Text == "ABRE CAJA")
                {
                    keta = "APERTURAR";
                }
                if (button1.Text == "CIERRA CAJA")
                {
                    keta = "CERRAR";
                }
                // vamos con todo
                if (tx_idr.Text.Trim() == "")
                {
                    var aa = MessageBox.Show("Confirma que desea " + keta + " la caja?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (aa == DialogResult.Yes)
                    {
                        if (true)
                        {
                            if (graba(keta) == true)
                            {
                                // actualizamos la tabla seguimiento de usuarios
                                string resulta = lib.ult_mov(nomform, nomtab, asd);
                                if (resulta != "OK")
                                {
                                    MessageBox.Show(resulta, "Error en actualización de seguimiento", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                this.Close();
                                if (keta == "CERRAR")
                                {
                                    var aaa = MessageBox.Show("Desea imprimir el cuadre de caja?", "Confirme", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    if (aaa == DialogResult.Yes )
                                    {
                                        using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
                                        {
                                            if (lib.procConn(conn) == true)
                                            {
                                                using (MySqlCommand micon = new MySqlCommand("rep_cuadre_sede", conn))
                                                {
                                                    micon.CommandType = CommandType.StoredProcedure;
                                                    micon.Parameters.AddWithValue("@idc", tx_idr.Text);
                                                    using (MySqlDataAdapter da = new MySqlDataAdapter(micon))
                                                    {
                                                        dtcuad.Rows.Clear();
                                                        da.Fill(dtcuad);
                                                        setParaCrystal("cuadre_caja");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //rb_pago.Focus();
                        return;
                    }
                }
                if (tx_idr.Text.Trim() != "")
                {
                    var aa = MessageBox.Show("Confirma que desea " + keta + " la caja?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (aa == DialogResult.Yes)
                    {
                        if (graba(keta) == true)
                        {
                            // actualizamos la tabla seguimiento de usuarios
                            string resulta = lib.ult_mov(nomform, nomtab, asd);
                            if (resulta != "OK")
                            {
                                MessageBox.Show(resulta, "Error en actualización de seguimiento", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            //jalaoc("tx_idcaja");
                            this.Close();
                        }
                    }
                    else
                    {
                        //rb_pago.Focus();
                        return;
                    }
                }
            }
            if (iserror == "no")
            {
                string resulta = lib.ult_mov(nomform, nomtab, asd);
                if (resulta != "OK")                                        // actualizamos la tabla usuarios
                {
                    MessageBox.Show(resulta, "Error en actualización de tabla usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private bool graba(string accion)
        {
            bool retorna = false;
            MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
            conn.Open();
            if(conn.State == ConnectionState.Open)
            {
                if (accion == "APERTURAR")
                {
                    string inserta = "insert into cabccaja (" +
                        "userabr,fechope,loccaja,obscobc,codmoMN,statusc,saldoan," +
                        "verApp,userc,fechc,diriplan4,diripwan4,netbname) values (" +
                        "@asd,@fechop,@ldcpgr,@obsprg,@codMN,@estado,@salan," +
                        "@verApp,@asd,now(),@iplan,@ipwan,@nbnam)";
                    using (MySqlCommand micon = new MySqlCommand(inserta, conn))
                    {
                        micon.Parameters.AddWithValue("@fechop", tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2));
                        micon.Parameters.AddWithValue("@ldcpgr", TransCarga.Program.almuser);         // local origen
                        micon.Parameters.AddWithValue("@obsprg", "");
                        micon.Parameters.AddWithValue("@salan", vsali);
                        micon.Parameters.AddWithValue("@codMN", MonDeft);
                        micon.Parameters.AddWithValue("@estado", codAbie);
                        micon.Parameters.AddWithValue("@verApp", verapp);
                        micon.Parameters.AddWithValue("@asd", asd);
                        micon.Parameters.AddWithValue("@iplan", lib.iplan());
                        micon.Parameters.AddWithValue("@ipwan", TransCarga.Program.vg_ipwan);
                        micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                        micon.ExecuteNonQuery();
                        retorna = true;
                    }
                }
                if (accion == "CERRAR")
                {
                    string actua = "update cabccaja set usercie=@asd,fechcie=DATE(NOW()),obscobc=@obs,statusc=@newst,saldofi=@salf," +
                        "verApp=@verApp,userm=@asd,fechm=now(),diriplan4=@iplan,diripwan4=@ipwan,netbname=@nbnam " +
                        "where id=@idr";
                    using (MySqlCommand micon = new MySqlCommand(actua, conn))
                    {
                        micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                        micon.Parameters.AddWithValue("@newst", codCier);
                        micon.Parameters.AddWithValue("@salf", vsalf);
                        micon.Parameters.AddWithValue("@obs", "");
                        micon.Parameters.AddWithValue("@asd", asd);
                        micon.Parameters.AddWithValue("@verApp", verapp);
                        micon.Parameters.AddWithValue("@iplan", lib.iplan());
                        micon.Parameters.AddWithValue("@ipwan", TransCarga.Program.vg_ipwan);
                        micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                        micon.ExecuteNonQuery();
                        retorna = true;
                    }
                }
            }
            else
            {
                MessageBox.Show("No fue posible conectarse al servidor de datos");
                Application.Exit();
                return retorna;
            }
            conn.Close();
            return retorna;
        }
        #endregion boton_form;

        #region leaves y checks
        private void tx_idr_Leave(object sender, EventArgs e)
        {
            if (Tx_modo.Text != "NUEVO" && tx_idr.Text != "")
            {
                //dataGridView1.Rows.Clear();
                jalaoc("tx_idcaja");
            }
        }
        #endregion

        #region botones_de_comando
        public void toolboton()
        {
            DataTable mdtb = new DataTable();
            const string consbot = "select * from permisos where formulario=@nomform and usuario=@use";
            MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
            conn.Open();
            if (conn.State == ConnectionState.Open)
            {
                try
                {
                    MySqlCommand consulb = new MySqlCommand(consbot, conn);
                    consulb.Parameters.AddWithValue("@nomform", nomform);
                    consulb.Parameters.AddWithValue("@use", asd);
                    MySqlDataAdapter mab = new MySqlDataAdapter(consulb);
                    mab.Fill(mdtb);
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, " Error ");
                    return;
                }
                finally { conn.Close(); }
            }
            else
            {
                MessageBox.Show("No se pudo conectar con el servidor", "Error de conexión");
                Application.Exit();
                return;
            }
            if (mdtb.Rows.Count > 0)
            {
                DataRow row = mdtb.Rows[0];
                if (Convert.ToString(row["btn1"]) == "S")
                {
                    this.Bt_add.Visible = true;
                }
                else { this.Bt_add.Visible = false; }
                if (Convert.ToString(row["btn2"]) == "S")
                {
                    this.Bt_edit.Visible = true;
                }
                else { this.Bt_edit.Visible = false; }
                //if (Convert.ToString(row["btn5"]) == "S")
                //{
                //    this.Bt_print.Visible = true;
                //}
                //else { this.Bt_print.Visible = false; }
                if (Convert.ToString(row["btn3"]) == "S")
                {
                    this.Bt_anul.Visible = true;
                }
                else { this.Bt_anul.Visible = false; }
                //if (Convert.ToString(row["btn4"]) == "S")
                //{
                //    this.Bt_ver.Visible = true;
                //}
                //else { this.Bt_ver.Visible = false; }
                if (Convert.ToString(row["btn6"]) == "S")
                {
                    this.Bt_close.Visible = true;
                }
                else { this.Bt_close.Visible = false; }
            }
        }
        #region botones
        private void Bt_add_Click(object sender, EventArgs e)
        {
            Tx_modo.Text = "NUEVO";
            //button1.Image = Image.FromFile(img_grab);
            escribe();
            // 
            Bt_ini.Enabled = false;
            Bt_sig.Enabled = false;
            Bt_ret.Enabled = false;
            Bt_fin.Enabled = false;
            //
            initIngreso();
            jalaoc("tx_idcaja");
        }
        private void Bt_edit_Click(object sender, EventArgs e)
        {
            sololee();          
            Tx_modo.Text = "EDITAR";
            //button1.Image = Image.FromFile(img_grab);
            initIngreso();
            jalaoc("tx_idcaja");
            //
            Bt_ini.Enabled = true;
            Bt_sig.Enabled = true;
            Bt_ret.Enabled = true;
            Bt_fin.Enabled = true;
        }
        private void Bt_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void Bt_print_Click(object sender, EventArgs e)
        {
            // Impresion ó Re-impresion ??
        }
        private void Bt_anul_Click(object sender, EventArgs e)
        {
            //
        }
        private void Bt_ver_Click(object sender, EventArgs e)
        {
            sololee();
            Tx_modo.Text = "VISUALIZAR";
            button1.Image = Image.FromFile(img_ver);
            initIngreso();
            // valida existencia de caja abierta en fecha y sede
            // aca debe ir el verdadero id de la caja abierta
            jalaoc("tx_idcaja");
            //dataGridView1.Focus();
            //
            Bt_ini.Enabled = true;
            Bt_sig.Enabled = true;
            Bt_ret.Enabled = true;
            Bt_fin.Enabled = true;
        }
        private void Bt_first_Click(object sender, EventArgs e)
        {
            limpiar();
            limpia_chk();
            limpia_combos();
            limpia_otros();
            limpia_chk();
            tx_idr.Text = lib.gofirts(nomtab);
            tx_idr_Leave(null, null);
        }
        private void Bt_back_Click(object sender, EventArgs e)
        {
            if(tx_idr.Text.Trim() != "")
            {
                int aca = int.Parse(tx_idr.Text) - 1;
                limpiar();
                limpia_chk();
                limpia_combos();
                limpia_otros();
                tx_idr.Text = aca.ToString();
                tx_idr_Leave(null, null);
            }
        }
        private void Bt_next_Click(object sender, EventArgs e)
        {
            int aca = int.Parse(tx_idr.Text) + 1;
            limpiar();
            limpia_chk();
            limpia_combos();
            limpia_otros();
            tx_idr.Text = aca.ToString();
            tx_idr_Leave(null, null);
        }
        private void Bt_last_Click(object sender, EventArgs e)
        {
            limpiar();
            limpia_chk();
            limpia_combos();
            limpia_otros();
            tx_idr.Text = lib.golast(nomtab);
            tx_idr_Leave(null, null);
        }
        #endregion botones;
        // proveed para habilitar los botones de comando
        #endregion botones_de_comando  ;

        #region crystal
        private void setParaCrystal(string repo)                    // genera el set para el reporte de crystal
        {
            if (repo == "cuadre_caja")
            {
                conClie datos = generacuadre();                        // conClie = dataset de impresion de contrato   
                frmvizoper visualizador = new frmvizoper(datos);        // POR ESO SE CREO ESTE FORM frmvizcont PARA MOSTRAR AHI. ES MEJOR ASI.  
                visualizador.Show();
            }
        }
        private conClie generacuadre()                              // genera cuadre de caja
        {
            conClie cuadre = new conClie();                                    // dataset
            conClie.cuadreCaja_cabRow rowcabeza = cuadre.cuadreCaja_cab.NewcuadreCaja_cabRow(); // rescont.rescont_cab.Newrescont_cabRow();
            //
            rowcabeza.rucEmisor = Program.ruc;
            rowcabeza.nomEmisor = Program.cliente;
            rowcabeza.dirEmisor = Program.dirfisc;
            rowcabeza.id = dtcuad.Rows[0].ItemArray[3].ToString();
            rowcabeza.serie = dtcuad.Rows[0].ItemArray[8].ToString();
            rowcabeza.corre = dtcuad.Rows[0].ItemArray[9].ToString();
            rowcabeza.cajeroA = dtcuad.Rows[0].ItemArray[28].ToString();
            rowcabeza.cajeroC = dtcuad.Rows[0].ItemArray[30].ToString();
            rowcabeza.codloc = dtcuad.Rows[0].ItemArray[1].ToString();
            rowcabeza.corre = dtcuad.Rows[0].ItemArray[9].ToString();
            rowcabeza.dircloc = ""; // dtcuad.Rows[0].ItemArray[].ToString();
            rowcabeza.estado = dtcuad.Rows[0].ItemArray[6].ToString();
            rowcabeza.fechAbier = dtcuad.Rows[0].ItemArray[4].ToString().Substring(0, 10);
            rowcabeza.fechCierr = (dtcuad.Rows[0].ItemArray[5].ToString().Trim() == "") ? "" : dtcuad.Rows[0].ItemArray[5].ToString().Substring(0, 10);
            rowcabeza.nomCajA = dtcuad.Rows[0].ItemArray[29].ToString();
            rowcabeza.nomCajC = dtcuad.Rows[0].ItemArray[31].ToString();
            rowcabeza.nomloc = dtcuad.Rows[0].ItemArray[2].ToString();
            rowcabeza.cobranzas = double.Parse(dtcuad.Rows[0].ItemArray[21].ToString());
            rowcabeza.ingvarios = double.Parse(dtcuad.Rows[0].ItemArray[22].ToString());
            rowcabeza.egresos = double.Parse(dtcuad.Rows[0].ItemArray[23].ToString());
            rowcabeza.saldoAnt = double.Parse(dtcuad.Rows[0].ItemArray[26].ToString());
            rowcabeza.saldofinal = double.Parse(dtcuad.Rows[0].ItemArray[27].ToString());
            rowcabeza.serie = dtcuad.Rows[0].ItemArray[8].ToString();
            cuadre.cuadreCaja_cab.AddcuadreCaja_cabRow(rowcabeza);    //rescont.rescont_cab.Addrescont_cabRow(rowcabeza);
            // detalle
            foreach (DataRow row in dtcuad.Rows)
            {
                if (true)
                {
                    conClie.cuadreCaja_detRow rowdetalle = cuadre.cuadreCaja_det.NewcuadreCaja_detRow();
                    rowdetalle.segmento = row.ItemArray[0].ToString();       // nombre del segmento
                    rowdetalle.id = row.ItemArray[3].ToString();             // id de la caja
                    rowdetalle.fecha = row.ItemArray[4].ToString().Substring(0, 10);          // fecha del doc del segmento
                    rowdetalle.estado = row.ItemArray[6].ToString();         // estado del doc del segmento
                    rowdetalle.nomEst = row.ItemArray[7].ToString();         // nombre del estado
                    rowdetalle.serSeg = row.ItemArray[8].ToString();         // serie del doc del segmento
                    rowdetalle.numSeg = row.ItemArray[9].ToString();         // numero del doc del segmento
                    rowdetalle.tipDoc = row.ItemArray[10].ToString();        // tipo del documento
                    rowdetalle.nomTdoc = row.ItemArray[11].ToString();       // nombre del tipo de doc
                    rowdetalle.serDoc = row.ItemArray[12].ToString();        // serie del documento
                    rowdetalle.numDoc = row.ItemArray[13].ToString();        // numero del documento
                    rowdetalle.tmepag = row.ItemArray[14].ToString();        // codigo moneda del documento
                    rowdetalle.nomMond = row.ItemArray[15].ToString();       // nombre moneda del documento
                    rowdetalle.codTipg = row.ItemArray[16].ToString();       // codigo tipo pago/cobranza
                    rowdetalle.nomTipg = row.ItemArray[17].ToString();       // nombre del tipo
                    rowdetalle.codCtag = row.ItemArray[18].ToString();       // codigo cuenta depositos
                    rowdetalle.nomCtag = row.ItemArray[19].ToString();       // nombre de cuenta
                    rowdetalle.refpago = row.ItemArray[20].ToString();       // referencia de pago/deposito/ingreso
                    rowdetalle.totdoco = double.Parse(row.ItemArray[21].ToString());       // total del documento
                    rowdetalle.totpags = double.Parse(row.ItemArray[22].ToString());       // total pagado
                    rowdetalle.saldvta = double.Parse(row.ItemArray[23].ToString());       // saldo del doc
                    rowdetalle.codmopa = row.ItemArray[24].ToString();       // codigo moneda de pago
                    rowdetalle.nomMonp = row.ItemArray[25].ToString();       // nombre de la moneda de pago
                    rowdetalle.totpago = double.Parse(row.ItemArray[26].ToString());       // total pagado/cobrado
                    rowdetalle.totpaMN = double.Parse(row.ItemArray[27].ToString());       // total pagado/cobrado en MN
                    cuadre.cuadreCaja_det.AddcuadreCaja_detRow(rowdetalle);
                }
            }
            return cuadre;
        }
        #endregion
    }
}
