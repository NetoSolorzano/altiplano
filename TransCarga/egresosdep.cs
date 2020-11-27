using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace TransCarga
{
    public partial class egresosdep : Form
    {
        static string nomform = "egresosdep";             // nombre del formulario
        string colback = TransCarga.Program.colbac;   // color de fondo
        string colpage = TransCarga.Program.colpag;   // color de los pageframes
        string colgrid = TransCarga.Program.colgri;   // color de las grillas
        string colfogr = TransCarga.Program.colfog;   // color fondo con grillas
        string colsfon = TransCarga.Program.colsbg;   // color fondo seleccion
        string colsfgr = TransCarga.Program.colsfc;   // color seleccion grilla
        string colstrp = TransCarga.Program.colstr;   // color del strip
        bool conectS = TransCarga.Program.vg_conSol;    // usa conector solorsoft? true=si; false=no
        static string nomtab = "cabegresos";              // cabecera de guias INDIVIDUALES

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
        string codAnul = "";            // codigo de documento anulado
        string codGene = "";            // codigo documento nuevo generado
        string MonDeft = "";            // moneda por defecto
        string v_clu = "";              // codigo del local del usuario
        string v_slu = "";              // serie del local del usuario
        string v_nbu = "";              // nombre del usuario
        string v_codc = "";             // codigo tipo documento EGRESO - pagos efectuados
        string v_noco = "";             // siglas del documento EGRESO - pagos efectuados
        string v_codd = "";             // codigo tipo documento EGRESO - deposito en cuenta propia
        string v_nodd = "";             // siglas del documento EGRESO - deposito en cuenta propia
        string vint_A0 = "";            // variable codigo anulacion interna por BD
        string v_igv = "";              // valor igv %
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
        //static string serv = ConfigurationManager.AppSettings["serv"].ToString();
        static string port = ConfigurationManager.AppSettings["port"].ToString();
        //static string usua = ConfigurationManager.AppSettings["user"].ToString();
        //static string cont = ConfigurationManager.AppSettings["pass"].ToString();
        static string data = ConfigurationManager.AppSettings["data"].ToString();
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + data + ";";

        DataTable dtm = new DataTable();        // combo moneda
        DataTable dtmpa = new DataTable();      // medio de pago del egreso
        DataTable dtcom = new DataTable();      // combo documento de compra
        DataTable dteg = new DataTable();       // combo grupos de egresos
        DataTable dtctb = new DataTable();      // cuentas bancarias propias
        public egresosdep()
        {
            InitializeComponent();
        }
        private void egresosdep_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SendKeys.Send("{TAB}");
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.N) Bt_add.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.E) Bt_edit.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.A) Bt_anul.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.O) Bt_ver.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.P) Bt_print.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.S) Bt_close.PerformClick();
        }
        private void egresosdep_Load(object sender, EventArgs e)
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
            dataGridView1.DefaultCellStyle.BackColor = Color.FromName(colgrid);
            //dataGridView1.DefaultCellStyle.BackColor = Color.FromName(colgrid);
            //dataGridView1.DefaultCellStyle.ForeColor = Color.FromName(colfogr);
            //dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromName(colsfon);
            //dataGridView1.DefaultCellStyle.SelectionForeColor = Color.FromName(colsfgr);
            //
            tx_user.Text += asd;
            tx_nomuser.Text = TransCarga.Program.vg_nuse;
            tx_locuser.Text = TransCarga.Program.vg_luse;
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
            tx_serie.MaxLength = 4;         // serie local 
            tx_numero.MaxLength = 8;        // numero correlativo 
            tx_serGR.MaxLength = 4;         // serie doc comprobante
            tx_numGR.MaxLength = 8;         // numero doc comprobante
            tx_glosa.MaxLength = 90;        // num.operacion, banco, etc.
            tx_obser1.MaxLength = 245;      // observaciones
            tx_fb.MaxLength = 1;            // F ó B
            tx_fb.CharacterCasing = CharacterCasing.Upper;
            tx_fb.Visible = false;
            // grilla
            dataGridView1.ReadOnly = true;
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;    // tipo moneda
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;     // monto egresado
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;     // monto en MN
            // todo desabilidado
            sololee();
        }
        private void initIngreso()
        {
            limpiar();
            limpia_chk();
            limpia_otros();
            limpia_combos();
            dataGridView1.Rows.Clear();
            dataGridView1.ReadOnly = true;
            tx_numero.Text = "";
            tx_serie.Text = v_slu;
            tx_numero.ReadOnly = true;
            tx_fechope.Text = DateTime.Today.ToString("dd/MM/yyyy");
            tx_digit.Text = v_nbu;
            tx_dat_estad.Text = codGene;
            tx_estado.Text = lib.nomstat(tx_dat_estad.Text);
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
                        if (row["campo"].ToString() == "estado")
                        {
                            if (row["param"].ToString() == "anulado") codAnul = row["valor"].ToString().Trim();         // codigo doc anulado
                            if (row["param"].ToString() == "generado") codGene = row["valor"].ToString().Trim();        // codigo doc generado
                        }
                    }
                    if (row["formulario"].ToString() == nomform)
                    {
                        if (row["campo"].ToString() == "documento")
                        {
                            if (row["param"].ToString() == "codpag") v_codc = row["valor"].ToString().Trim();               // codigo tipo pagos
                            if (row["param"].ToString() == "nompag") v_noco = row["valor"].ToString().Trim();               // nombre codido pagos
                            if (row["param"].ToString() == "coddep") v_codd = row["valor"].ToString().Trim();               // codigo tipo depositos
                            if (row["param"].ToString() == "nomdep") v_nodd = row["valor"].ToString().Trim();               // nombre codido depositos
                        }
                        if (row["campo"].ToString() == "moneda" && row["param"].ToString() == "default") MonDeft = row["valor"].ToString().Trim();             // moneda por defecto
                    }
                    if (row["formulario"].ToString() == "interno")              // codigo enlace interno de anulacion del cliente con en BD A0
                    {
                        if (row["campo"].ToString() == "anulado" && row["param"].ToString() == "A0") vint_A0 = row["valor"].ToString().Trim();
                        if (row["campo"].ToString() == "igv" && row["param"].ToString() == "%") v_igv = row["valor"].ToString().Trim();
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
            //try
            {
                string parte = "";
                if (campo == "tx_idcaja")
                {
                    parte = "where a.idcaja=@idcaja";
                }
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string consulta = "SELECT ??? " +
                        "FROM cabegresos a " +
                        "LEFT JOIN desc_doc b ON b.idcodice = a.cltdoco " +
                        "LEFT JOIN anag_cli c ON c.tipdoc = a.cltdoco AND c.RUC = a.dcltdoco " +
                        "LEFT JOIN desc_mon d ON d.idcodice = a.codmopa " +
                        "left join usuarios u on u.nom_user = a.userc " +
                        "left join desc_est e on e.idcodice = a.estdcob " +
                        parte;
                    MySqlCommand micon = new MySqlCommand(consulta, conn);
                    if (campo == "tx_idr") micon.Parameters.AddWithValue("@idcaja", tx_idcaja.Text);
                    MySqlDataReader dr = micon.ExecuteReader();
                    if (dr != null)
                    {
                        if (dr.Read())
                        {
                            // aca llenamos el detalle de los egresos para la caja respectiva
                        }
                        else
                        {
                            MessageBox.Show("No existe el código de egreso!", "Atención - dato incorrecto",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            tx_numero.Text = "";
                            tx_numero.Focus();
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No existe el número buscado!", "Atención - dato incorrecto",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    dr.Dispose();
                    micon.Dispose();
                    //
                }
                conn.Close();
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
                // datos para el combo de moneda
                cmb_mon.Items.Clear();
                using (MySqlCommand cmo = new MySqlCommand("select idcodice,descrizionerid from desc_mon where numero=@bloq", conn))
                {
                    cmo.Parameters.AddWithValue("@bloq", 1);
                    using (MySqlDataAdapter dacu = new MySqlDataAdapter(cmo))
                    {
                        dtm.Clear();
                        dacu.Fill(dtm);
                        cmb_mon.DataSource = dtm;
                        cmb_mon.DisplayMember = "descrizionerid";
                        cmb_mon.ValueMember = "idcodice";
                    }
                }
                // datos para el combo de medio de pago
                cmb_mpago.Items.Clear();
                using (MySqlCommand cmo = new MySqlCommand("select idcodice,descrizionerid from desc_mpa where numero=@bloq", conn))
                {
                    cmo.Parameters.AddWithValue("@bloq", 1);
                    using (MySqlDataAdapter dacu = new MySqlDataAdapter(cmo))
                    {
                        dtmpa.Clear();
                        dacu.Fill(dtmpa);
                        cmb_mpago.DataSource = dtmpa;
                        cmb_mpago.DisplayMember = "descrizionerid";
                        cmb_mpago.ValueMember = "idcodice";
                    }
                }
                // datos para el combo comprobantes de compra
                cmb_comp.Items.Clear();
                using (MySqlCommand cmo = new MySqlCommand("select idcodice,descrizionerid from desc_tdv where numero=@bloq and codigo in ('venta','compra')", conn))
                {
                    cmo.Parameters.AddWithValue("@bloq", 1);
                    using (MySqlDataAdapter dacu = new MySqlDataAdapter(cmo))
                    {
                        dtcom.Clear();
                        dacu.Fill(dtcom);
                        cmb_comp.DataSource = dtcom;
                        cmb_comp.DisplayMember = "descrizionerid";
                        cmb_comp.ValueMember = "idcodice";
                    }
                }
                // grupo de egresos del tipo pago efectuado
                cmb_grupo.Items.Clear();
                using (MySqlCommand cmo = new MySqlCommand("select idcodice,descrizionerid from desc_teg where numero=@bloq", conn))
                {
                    cmo.Parameters.AddWithValue("@bloq", 1);
                    using (MySqlDataAdapter dacu = new MySqlDataAdapter(cmo))
                    {
                        dteg.Clear();
                        dacu.Fill(dteg);
                        cmb_grupo.DataSource = dteg;
                        cmb_grupo.DisplayMember = "descrizionerid";
                        cmb_grupo.ValueMember = "idcodice";
                    }
                }
                // combo de cuentas bancarias
                cmb_ctaprop.Items.Clear();
                using (MySqlCommand cmo = new MySqlCommand("select idcodice,descrizionerid,descrizione,codigo from desc_ctb where numero=@bloq", conn))
                {
                    cmo.Parameters.AddWithValue("@bloq", 1);
                    using (MySqlDataAdapter dacu = new MySqlDataAdapter(cmo))
                    {
                        dtctb.Clear();
                        dacu.Fill(dtctb);
                        cmb_ctaprop.DataSource = dtctb;
                        cmb_ctaprop.DisplayMember = "descrizione";
                        cmb_ctaprop.ValueMember = "idcodice";
                    }
                }
            }
        }
        private bool valiVars()                 // valida existencia de datos en variables del form
        {
            bool retorna = true;
            if (codAnul == "")          // codigo de documento anulado
            {
                lib.messagebox("Código de Cobranza ANULADA");
                retorna = false;
            }
            if (codGene == "")          // codigo documento nuevo generado
            {
                lib.messagebox("Código de Cobranza GENERADA/NUEVA");
                retorna = false;
            }
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
            if (vint_A0 == "")
            {
                lib.messagebox("Código interno enlace anulación BD - A0");
                retorna = false;
            }
            // aca falta agregar resto  ...........
            return retorna;
        }
        private void tipcambio(string codmod)                // funcion para calculos con el tipo de cambio
        {
            if (codmod != MonDeft)
            {
                vtipcam vtipcam = new vtipcam(tx_PAGO.Text, codmod, DateTime.Now.Date.ToString());
                var result = vtipcam.ShowDialog();
                if (vtipcam.ReturnValue3 != null)
                {
                    tx_PAGO.Text = vtipcam.ReturnValue1;
                    tx_pagoMN.Text = vtipcam.ReturnValue2;
                    tx_tipcam.Text = vtipcam.ReturnValue3;
                    if (tx_pagoMN.Text.Trim() == "0.00" && (tx_PAGO.Text.Trim() != "" || tx_PAGO.Text.Trim() != "0"))
                    {
                        tx_pagoMN.Text = Math.Round(decimal.Parse(tx_PAGO.Text) * decimal.Parse(tx_tipcam.Text), 2).ToString();
                    }
                }
                else
                {
                    cmb_mon.SelectedValue = MonDeft;
                }
            }
        }
        private void calculos(decimal totDoc)
        {
            decimal tigv = 0;
            decimal tsub = 0;
            if (totDoc > 0)
            {
                tsub = Math.Round(totDoc / (1 + decimal.Parse(v_igv) / 100), 2);
                tigv = Math.Round(totDoc - tsub, 2);
                
            }
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
            if (tx_serie.Text.Trim() == "")
            {
                tx_serie.Focus();
                return;
            }
            if (tx_dat_mone.Text.Trim() == "")
            {
                MessageBox.Show("Seleccione el tipo de moneda de pago", " Atención ");
                cmb_mon.Focus();
                return;
            }
            if (tx_dat_mp.Text == "")
            {
                MessageBox.Show("Seleccione un tipo de pago", " Atención ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmb_mpago.Focus();
                return;
            }
            if (tx_dat_mone.Text != MonDeft && tx_tipcam.Text.Trim() == "")
            {
                MessageBox.Show("Seleccione la moneda de pago y tipo de cambio", " Atención ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmb_mon.Focus();
                return;
            }
            if (tx_PAGO.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese el monto del pago", " Atención ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tx_PAGO.Focus();
                return;
            }
            #endregion
            // grabamos, actualizamos, etc
            string modo = Tx_modo.Text;
            string iserror = "no";
            if (modo == "NUEVO")
            {
                if (tx_idr.Text.Trim() == "")
                {
                    var aa = MessageBox.Show("Confirma que desea crear el Egreso?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (aa == DialogResult.Yes)
                    {
                        if (true)
                        {
                            if (graba() == true)
                            {
                                // actualizamos la tabla seguimiento de usuarios
                                string resulta = lib.ult_mov(nomform, nomtab, asd);
                                if (resulta != "OK")
                                {
                                    MessageBox.Show(resulta, "Error en actualización de seguimiento", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                    else
                    {
                        rb_pago.Focus();
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Los datos no son nuevos en egresos", "Verifique duplicidad", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }
            }
            if (modo == "EDITAR")
            {
                if (tx_numero.Text.Trim() == "")
                {
                    tx_numero.Focus();
                    MessageBox.Show("Ingrese el número del documento", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (tx_dat_estad.Text == codAnul)
                {
                    MessageBox.Show("El documento esta ANULADO", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    tx_numero.Focus();
                    return;
                }
                if (true)
                {
                    if (tx_idr.Text.Trim() != "")
                    {
                        var aa = MessageBox.Show("Confirma que desea modificar el Egreso?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (aa == DialogResult.Yes)
                        {
                            edita();    // modificacion total
                            // actualizamos la tabla seguimiento de usuarios
                            string resulta = lib.ult_mov(nomform, nomtab, asd);
                            if (resulta != "OK")
                            {
                                MessageBox.Show(resulta, "Error en actualización de seguimiento", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            tx_serie.Focus();
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("El documento ya debe existir para editar", "Debe ser edición", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return;
                    }
                }
            }
            if (modo == "ANULAR")
            {
                if (tx_numero.Text.Trim() == "")
                {
                    tx_numero.Focus();
                    MessageBox.Show("Ingrese el número del documento", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (tx_dat_estad.Text == codAnul)
                {
                    MessageBox.Show("El documento esta ANULADO", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    tx_numero.Focus();
                    return;
                }
                // SOLO USUARIOS AUTORIZADOS DEBEN ACCEDER A ESTA OPCIÓN
                // SE ANULA EL DOCUMENTO Y LOS MOVIMIENTOS INTERNOS se hacen por B.D.
                // anulacion procede siempre y cuando sea de la fecha y del usuario
                if (asd != tx_dat_userdoc.Text || DateTime.Now.Date.ToString().Substring(0,10) != tx_fechope.Text)
                {
                    MessageBox.Show("No se puede ANULAR Egresos/Depósitos fuera de fecha" + Environment.NewLine +
                        "o que sean de otro local/usuario","Atención",MessageBoxButtons.OK,MessageBoxIcon.Hand);
                    return;
                }
                if (tx_idr.Text.Trim() != "")
                {
                    var aa = MessageBox.Show("Confirma que desea ANULAR el Egreso?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (aa == DialogResult.Yes)
                    {
                        anula();
                        string resulta = lib.ult_mov(nomform, nomtab, asd);
                        if (resulta != "OK")
                        {
                            MessageBox.Show(resulta, "Error en actualización de seguimiento", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        tx_serie.Focus();
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("El documento debe existir para poder anular!", "No esta el Id del registro", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }
            }
            if (iserror == "no")
            {
                string resulta = lib.ult_mov(nomform, nomtab, asd);
                if (resulta != "OK")                                        // actualizamos la tabla usuarios
                {
                    MessageBox.Show(resulta, "Error en actualización de tabla usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                // debe limpiar los campos y actualizar la grilla
                initIngreso();          // limpiamos todo para volver a empesar

            }
        }
        private bool graba()
        {
            bool retorna = false;
            MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
            conn.Open();
            if(conn.State == ConnectionState.Open)
            {
                string inserta = "insert into cabegresos (" +
                    " , , " +
                    "verApp,userc,fechc,diriplan4,diripwan4,netbname) values (" +
                    " , ," +
                    "@verApp,@asd,now(),@iplan,@ipwan,@nbnam)";
                using (MySqlCommand micon = new MySqlCommand(inserta, conn))
                {
                    micon.Parameters.AddWithValue("@fechop", tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2));
                    micon.Parameters.AddWithValue("@ctdvta", tx_dat_tdv.Text);
                    micon.Parameters.AddWithValue("@serdv", tx_serie.Text);
                    //micon.Parameters.AddWithValue("@numdv", tx_numero.Text);  // numero NO porque se autogenera en la BD
                    micon.Parameters.AddWithValue("@ldcpgr", TransCarga.Program.almuser);         // local origen
                    micon.Parameters.AddWithValue("@estado", tx_dat_estad.Text);
                    micon.Parameters.AddWithValue("@martdv", tx_fb.Text);
                    micon.Parameters.AddWithValue("@serdoc", tx_serGR.Text);
                    micon.Parameters.AddWithValue("@numdoc", tx_numGR.Text);
                    micon.Parameters.AddWithValue("@timepa", tx_dat_mp.Text);
                    micon.Parameters.AddWithValue("@refpag", tx_glosa.Text);
                    micon.Parameters.AddWithValue("@obsprg", tx_obser1.Text);
                    micon.Parameters.AddWithValue("@monppr", tx_dat_mone.Text);
                    micon.Parameters.AddWithValue("@totpag", tx_PAGO.Text);
                    micon.Parameters.AddWithValue("@tcoper", (tx_tipcam.Text == "")? "0" : tx_tipcam.Text);                   // TIPO DE CAMBIO
                    micon.Parameters.AddWithValue("@porcig", v_igv);                            // porcentaje en numeros de IGV
                    micon.Parameters.AddWithValue("@totMN", tx_pagoMN.Text);
                    micon.Parameters.AddWithValue("@codMN", MonDeft);                           // codigo moneda local
                    micon.Parameters.AddWithValue("@verApp", verapp);
                    micon.Parameters.AddWithValue("@asd", asd);
                    micon.Parameters.AddWithValue("@iplan", lib.iplan());
                    micon.Parameters.AddWithValue("@ipwan", lib.ipwan());
                    micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                    micon.ExecuteNonQuery();
                }
                using (MySqlCommand micon = new MySqlCommand("select last_insert_id()", conn))
                {
                    using (MySqlDataReader dr = micon.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            tx_idr.Text = dr.GetString(0);
                            tx_numero.Text = lib.Right(tx_idr.Text, 8);
                        }
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
        private void edita()
        {
            MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
            conn.Open();
            if (conn.State == ConnectionState.Open)
            {
                try
                {
                    if (true)     // EDICION DE CABECERA
                    {
                        string actua = "update cabegresos a set a.obscobc=@obsprg," +
                            "a.verApp=@verApp,a.userm=@asd,a.fechm=now(),a.diriplan4=@iplan,a.diripwan4=@ipwan,a.netbname=@nbnam " +
                            "where a.id=@idr";
                        MySqlCommand micon = new MySqlCommand(actua, conn);
                        micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                        micon.Parameters.AddWithValue("@obsprg", tx_obser1.Text);
                        micon.Parameters.AddWithValue("@verApp", verapp);
                        micon.Parameters.AddWithValue("@asd", asd);
                        micon.Parameters.AddWithValue("@iplan", lib.iplan());
                        micon.Parameters.AddWithValue("@ipwan", lib.ipwan());
                        micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                        micon.ExecuteNonQuery();
                        //
                        // EDICION DEL DETALLE .... no hay detalle
                        micon.Dispose();
                    }
                    conn.Close();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Error en modificar el documento");
                    Application.Exit();
                    return;
                }
            }
            else
            {
                MessageBox.Show("No fue posible conectarse al servidor de datos");
                Application.Exit();
                return;
            }
        }
        private void anula()
        {
            // en este caso solo hay ANULACION FISICA
            // Anulacion fisica se "anula" el numero del documento en sistema y
            // se borran todos los enlaces mediante triggers en la B.D.
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string canul = "update cabegresos set estdcob=@estser,obscobc=@obse,usera=@asd,fecha=now()," +
                        "verApp=@veap,diriplan4=@dil4,diripwan4=@diw4,netbname=@nbnp,estintreg=@eiar " +
                        "where id=@idr";
                    using (MySqlCommand micon = new MySqlCommand(canul, conn))
                    {
                        micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                        micon.Parameters.AddWithValue("@estser", codAnul);
                        micon.Parameters.AddWithValue("@obse", tx_obser1.Text);
                        micon.Parameters.AddWithValue("@asd", asd);
                        micon.Parameters.AddWithValue("@dil4", lib.iplan());
                        micon.Parameters.AddWithValue("@diw4", lib.ipwan());
                        micon.Parameters.AddWithValue("@nbnp", Environment.MachineName);
                        micon.Parameters.AddWithValue("@veap", verapp);
                        micon.Parameters.AddWithValue("@eiar", (vint_A0 == codAnul) ? "A0" : "");  // codigo anulacion interna en DB A0
                        micon.ExecuteNonQuery();
                    }
                }
            }
        }
        #endregion boton_form;

        #region leaves y checks
        private void tx_idr_Leave(object sender, EventArgs e)
        {
            if (Tx_modo.Text != "NUEVO" && tx_idr.Text != "")
            {
                dataGridView1.Rows.Clear();
                jalaoc("tx_idcaja");
            }
        }
        private void tx_numero_Leave(object sender, EventArgs e)
        {
            if (Tx_modo.Text != "NUEVO" && tx_numero.Text.Trim() != "")
            {
                // se jala todos los egresos del Id de caja
            }
        }
        private void tx_serie_Leave(object sender, EventArgs e)
        {
            tx_serie.Text = lib.Right("0000" + tx_serie.Text, 4);
            if (Tx_modo.Text == "NUEVO") tx_serGR.Focus();
        }
        private void tx_serGR_Leave(object sender, EventArgs e)
        {
            tx_serGR.Text = lib.Right("0000" + tx_serGR.Text, 4);
            tx_numGR.Focus();
        }
        private void tx_numGR_Leave(object sender, EventArgs e)
        {
            if (Tx_modo.Text == "NUEVO" && tx_serGR.Text.Trim() != "" && tx_numGR.Text.Trim() != "")
            {
                tx_numGR.Text = lib.Right("00000000" + tx_numGR.Text, 8);
            }
        }
        private void tx_pago_Leave(object sender, EventArgs e)
        {
            if (tx_PAGO.Text.Trim() != "" && Tx_modo.Text == "NUEVO")
            {
                decimal vpag = decimal.Parse(tx_PAGO.Text);
                if (vpag <= 0)
                {
                    MessageBox.Show("El monto a pagar debe ser mayor a cero", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    tx_PAGO.Text = "";
                    tx_PAGO.Focus();
                    return;
                }
            }
        }
        private void tx_fb_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (tx_fb.Text != "B" && tx_fb.Text != "F")
            {
                e.Cancel = true;
            }
        }
        private void tx_fb_Leave(object sender, EventArgs e)
        {
            //if (tx_fb.Text == "B") tx_dat_tidoor.Text = v_tip3;
            //else tx_dat_tidoor.Text = v_tip4;
            //tx_serGR.Focus();
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
            button1.Image = Image.FromFile(img_grab);
            escribe();
            // 
            Bt_ini.Enabled = false;
            Bt_sig.Enabled = false;
            Bt_ret.Enabled = false;
            Bt_fin.Enabled = false;
            //
            initIngreso();
            tx_numero.Text = "";
            tx_numero.ReadOnly = true;
            tx_noco.ReadOnly = true;
            tx_serie.ReadOnly = true;
            rb_pago.Focus();
        }
        private void Bt_edit_Click(object sender, EventArgs e)
        {
            sololee();          
            Tx_modo.Text = "EDITAR";                    // solo puede editarse la observacion 28/10/2020
            button1.Image = Image.FromFile(img_grab);
            initIngreso();
            tx_obser1.Enabled = true;
            tx_obser1.ReadOnly = false;
            tx_numero.Text = "";
            tx_numero.ReadOnly = false;
            rb_pago.Focus();
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
            sololee();
            Tx_modo.Text = "ANULAR";
            button1.Image = Image.FromFile(img_anul);
            initIngreso();
            tx_serie.ReadOnly = false;
            tx_numero.ReadOnly = false;
            tx_obser1.Enabled = true;
            tx_obser1.ReadOnly = false;
            rb_pago.Focus();
            //
            Bt_ini.Enabled = true;
            Bt_sig.Enabled = true;
            Bt_ret.Enabled = true;
            Bt_fin.Enabled = true;
        }
        private void Bt_ver_Click(object sender, EventArgs e)
        {
            sololee();
            Tx_modo.Text = "VISUALIZAR";
            button1.Image = Image.FromFile(img_ver);
            initIngreso();
            tx_serie.ReadOnly = false;
            tx_numero.ReadOnly = false;
            tx_obser1.Focus();
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

        #region comboboxes
        private void cmb_mon_SelectedIndexChanged(object sender, EventArgs e)
        {
            // lo moví a change commitment
        }
        private void cmb_mon_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (Tx_modo.Text == "NUEVO")
            {
                if (cmb_mon.SelectedIndex > -1)
                {
                    tx_dat_mone.Text = cmb_mon.SelectedValue.ToString();
                    if (true)
                    {
                        tipcambio(tx_dat_mone.Text);
                    }
                }
            }
        }
        private void cmb_mpago_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_mpago.SelectedIndex > -1)
            {
                tx_dat_mp.Text = cmb_mpago.SelectedValue.ToString();
            }
        }
        private void cmb_comp_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void cmb_grupo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void cmb_ctaprop_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        #endregion comboboxes
    }
}
