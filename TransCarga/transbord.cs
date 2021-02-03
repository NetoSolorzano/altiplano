using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace TransCarga
{
    public partial class transbord : Form
    {
        static string nomform = "transbord";           // nombre del formulario
        string colback = TransCarga.Program.colbac;     // color de fondo
        string colpage = TransCarga.Program.colpag;     // color de los pageframes
        string colgrid = TransCarga.Program.colgri;     // color de las grillas
        string colfogr = TransCarga.Program.colfog;     // color fondo con grillas
        string colsfon = TransCarga.Program.colsbg;     // color fondo seleccion
        string colsfgr = TransCarga.Program.colsfc;     // color seleccion grilla
        string colstrp = TransCarga.Program.colstr;     // color del strip
        bool conectS = TransCarga.Program.vg_conSol;    // usa conector solorsoft? true=si; false=no
        static string nomtab = "";

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
        string codCrrdo = "";           // codigo de documento cerrado
        string codRecep = "";           // codigo doc recepcionado
        string v_clu = "";              // codigo del local del usuario
        string v_slu = "";              // serie del local del usuario
        string v_nbu = "";              // nombre del usuario
        string v_esta = "";             // codigo estado transbordado
        string v_plazo = "";            // dias de plazo para transbordos entre planillas
        string vint_A0 = "";            // variable INTERNA para amarrar el codigo anulacion cliente con A0
        //
        static libreria lib = new libreria();   // libreria de procedimientos
        publico lp = new publico();             // libreria de clases
        string verapp = System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion;
        string nomclie = Program.cliente;           // cliente usuario del sistema
        string rucclie = Program.ruc;               // ruc del cliente usuario del sistema
        string asd = TransCarga.Program.vg_user;    // usuario conectado al sistema
        #endregion

        // string de conexion
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + login.data + ";";
        public transbord()
        {
            InitializeComponent();
        }
        private void transbord_Load(object sender, EventArgs e)
        {
            this.Focus();
            jalainfo();
            init();
            //dataload();
            toolboton();
            this.KeyPreview = true;
            if (valiVars() == false)
            {
                Application.Exit();
                return;
            }
            lp.sololee(this);
        }
        private void transbord_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SendKeys.Send("{TAB}");
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.N) Bt_add.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.E) Bt_edit.PerformClick();
            //if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.P) Bt_print.PerformClick();
            //if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.A) Bt_anul.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.O) Bt_ver.PerformClick();
            //if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.S) Bt_close.PerformClick();
        }
        private void jalainfo()                 // obtiene datos de imagenes y variables
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                string consulta = "select formulario,campo,param,valor from enlaces where formulario in (@nofo,@nfin,@nofa)";
                MySqlCommand micon = new MySqlCommand(consulta, conn);
                micon.Parameters.AddWithValue("@nofo", "main");
                micon.Parameters.AddWithValue("@nfin", "interno");
                //micon.Parameters.AddWithValue("@nofi", "clients");
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
                            if (row["param"].ToString() == "cerrado") codCrrdo = row["valor"].ToString().Trim();        // codigo doc cerrado
                            if (row["param"].ToString() == "recepcionado") codRecep = row["valor"].ToString().Trim();   // codigo doc recepcionado
                        }
                    }
                    if (row["formulario"].ToString() == nomform)
                    {
                        if (row["campo"].ToString() == "plazo" && row["param"].ToString() == "maximo") v_plazo = row["valor"].ToString().Trim();           // dias maximo plazo entre planillas
                        if (row["campo"].ToString() == "documento" && row["param"].ToString() == "estado") v_esta = row["valor"].ToString().Trim();        // estado transbordado
                        if (row["campo"].ToString() == "impresion")
                        {
                            //if (row["param"].ToString() == "formato") vi_formato = row["valor"].ToString().Trim();
                        }
                        //if (row["campo"].ToString() == "moneda" && row["param"].ToString() == "default") v_mondef = row["valor"].ToString().Trim();
                    }
                    if (row["formulario"].ToString() == "interno")  // variables configuracion interna, campos especiales de base de datos
                    {
                        if (row["campo"].ToString() == "anulado" && row["param"].ToString() == "A0") vint_A0 = row["valor"].ToString().Trim();
                    }
                }
                da.Dispose();
                dt.Dispose();
                // jalamos datos del usuario y local
                v_clu = lib.codloc(asd);                // codigo local usuario
                v_slu = lib.serlocs(v_clu);             // serie local usuario
                v_nbu = lib.nomuser(asd);               // nombre del usuario
                conn.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Error de conexión");
                Application.Exit();
                return;
            }
        }
        private void init()
        {
            this.BackColor = Color.FromName(colback);
            toolStrip1.BackColor = Color.FromName(colstrp);
            //
            tx_user.Text += asd;
            tx_nomuser.Text = lib.nomuser(asd);
            tx_locuser.Text += lib.locuser(asd);
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
            tx_serie.MaxLength = 4;
            tx_ser_pla_des.MaxLength = 4;
            tx_numero.MaxLength = 8;
            tx_num_pla_des.MaxLength = 8;
            // campos en mayusculas

            // grilla
        }
        private void dataload()                  // jala datos para los combos 
        {
            MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
            conn.Open();
            if (conn.State != ConnectionState.Open)
            {
                MessageBox.Show("No se pudo conectar con el servidor", "Error de conexión");
                Application.Exit();
                return;
            }
            //  datos para los combos de locales origen y destino
            conn.Close();
        }
        private bool valiVars()                 // valida existencia de datos en variables del form
        {
            bool retorna = true;
            if (codAnul == "")          // codigo de documento anulado
            {
                lib.messagebox("Código de planilla indivual ANULADA");
                retorna = false;
            }
            if (codGene == "")          // codigo documento nuevo generado
            {
                lib.messagebox("Código de planilla indivual GENERADA/NUEVA");
                retorna = false;
            }
            if (codCrrdo == "")          // codigo documento cerrado
            {
                lib.messagebox("Código de planilla indivual CERRADA");
                retorna = false;
            }
            if (v_clu == "")            // codigo del local del usuario
            {
                lib.messagebox("Código local del usuario");
                retorna = false;
            }
            if (v_slu == "")            // serie del local del usuario
            {
                lib.messagebox("Serie general local del usuario");
                retorna = false;
            }
            if (v_nbu == "")            // nombre del usuario
            {
                lib.messagebox("Nombre del usuario");
                retorna = false;
            }
            if (vint_A0 == "")
            {
                lib.messagebox("Cód. Interno enlace Anulado: A0");
                retorna = false;
            }
            return retorna;
        }
        private void armagrilla()
        {
            //dataGridView1.Font = tiplg;
            //dataGridView1.DefaultCellStyle.Font = tiplg;
            dataGridView1.RowTemplate.Height = 16;
            dataGridView1.DefaultCellStyle.BackColor = Color.FromName(colback);
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.Width = this.Parent.Width - 50; // 1015;
            if (dataGridView1.DataSource == null) dataGridView1.ColumnCount = 5;
            if (dataGridView1.Rows.Count > 0)
            {
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    dataGridView1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    _ = decimal.TryParse(dataGridView1.Rows[0].Cells[i].Value.ToString(), out decimal vd);
                    if (vd != 0) dataGridView1.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                int b = 0;
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    int a = dataGridView1.Columns[i].Width;
                    b += a;
                    dataGridView1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    dataGridView1.Columns[i].Width = a;
                }
                if (b < dataGridView1.Width) dataGridView1.Width = b + 60;
                dataGridView1.ReadOnly = true;
            }
        }
        private bool valids_leave(string tipo, string valor, NumericTextBox con)
        {
            bool retorna = false;
            if (tipo == "serie")
            {
                con.Text = lib.Right("000" + valor, 4);
                retorna = true;
            }
            if (tipo == "numero")
            {
                con.Text = lib.Right("0000000" + valor, 8);
                if (jalaoc(con.Name.ToString()) == false) retorna = false;
                else
                {
                    if (con.Name.ToString() == "tx_numero")
                    {
                        jaladet(tx_serie.Text, tx_numero.Text);
                        if (tx_estado.Text == codRecep || tx_estado.Text == codAnul)
                        {
                            retorna = false;
                            MessageBox.Show("La planilla origen esta Anulada o Recepcionada", "No puede transbordar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else retorna = true;
                    }
                    else
                    {
                        // validamos que ambas planillas:
                        // - tengan el mismo destino/origen
                        // - la planilla destino tenga estado generado
                        // - que no sean iguales
                        // - alguna validacion de fecha ???
                        retorna = true;
                        if (tx_estad_des.Text != codGene)
                        {
                            retorna = false;
                            MessageBox.Show("La planilla destino debe estar abierta!", "No puede transbordar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        if (tx_origen.Text != tx_orig_des.Text)
                        {
                            retorna = false;
                            MessageBox.Show("Las planillas no tienen el mismo origen", "No puede transbordar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        if (tx_destino.Text != tx_dest_des.Text)
                        {
                            retorna = false;
                            MessageBox.Show("Las planillas no tienen el mismo destino", "No puede transbordar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        if (tx_numero.Text == tx_num_pla_des.Text)
                        {
                            retorna = false;
                            MessageBox.Show("Las planillas no pueden ser las mismas", "No puede transbordar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        DateTime odate = Convert.ToDateTime(tx_pla_fech.Text);
                        //if ()
                        //{

                        //}
                    }
                }
            }
            return retorna;
        }
        private bool jalaoc(string tipo)        // jala datos de las planillas
        {
            bool retorna = false;
            string consulta = "SELECT a.estadoser,a.fechope,lo.Descrizione,ld.Descrizione,a.brevchofe,a.nomchofe,a.brevayuda,a.nomayuda," +
                "a.rucpropie,af.RazonSocial,a.platracto,a.placarret,a.confvehic,a.autorizac," +
                "a.cantfilas,a.cantotpla,a.pestotpla,a.totplacar,a.totpagado,a.salxpagar,a.id " +
                "from cabplacar a " +
                "LEFT JOIN desc_loc lo on lo.idcodice = a.locorigen " +
                "LEFT JOIN desc_loc ld on ld.IDCodice = a.locdestin " +
                "LEFT JOIN anag_for af on af.RUC = a.rucpropie " +
                "WHERE a.serplacar = @ser AND a.numplacar = @num";
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                {
                    micon.Parameters.AddWithValue("@ser", (tipo == "tx_numero") ? tx_serie.Text : tx_ser_pla_des.Text);
                    micon.Parameters.AddWithValue("@num", (tipo == "tx_numero") ? tx_numero.Text : tx_num_pla_des.Text);
                    using (MySqlDataReader dr = micon.ExecuteReader())
                    {
                        if (dr.HasRows && dr.Read())
                        {
                            if (tipo == "tx_numero")        // jalamos datos de planilla origen
                            {
                                tx_estado.Text = dr.GetString(0);
                                tx_pla_fech.Text = dr.GetString(1).Substring(0, 10);
                                tx_origen.Text = dr.GetString(2);
                                tx_destino.Text = dr.GetString(3);
                                tx_pla_brevet.Text = dr.GetString(4);
                                tx_pla_nomcho.Text = dr.GetString(5);
                                tx_pla_ayud.Text = dr.GetString(6);
                                tx_pla_nomayu.Text = dr.GetString(7);
                                tx_pla_ruc.Text = dr.GetString(8);
                                tx_pla_propiet.Text = dr.GetString(9);
                                tx_pla_placa.Text = dr.GetString(10);
                                tx_pla_carret.Text = dr.GetString(11);
                                tx_pla_confv.Text = dr.GetString(12);
                                tx_pla_autor.Text = dr.GetString(13);
                                //
                                tx_dat_o_tfil.Text = dr.GetString("cantfilas");
                                tx_dat_o_tcan.Text = dr.GetString("cantotpla");
                                tx_dat_o_tpes.Text = dr.GetString("pestotpla");
                                tx_dat_o_totf.Text = dr.GetString("totplacar");
                                tx_dat_o_tpag.Text = dr.GetString("totpagado");
                                tx_dat_o_tsal.Text = dr.GetString("salxpagar");
                                retorna = true;
                                jaladet(tx_serie.Text, tx_numero.Text);
                            }
                            if (tipo == "tx_num_pla_des")   // jalamos datos de planilla destino
                            {
                                tx_estad_des.Text = dr.GetString(0);
                                tx_fech_des.Text = dr.GetString(1).Substring(0, 10); ;
                                tx_orig_des.Text = dr.GetString(2);
                                tx_dest_des.Text = dr.GetString(3);
                                tx_chof_des.Text = dr.GetString(4);
                                tx_nomChof_des.Text = dr.GetString(5);
                                tx_ayu_des.Text = dr.GetString(6);
                                tx_nomAyu_des.Text = dr.GetString(7);
                                tx_prop_des.Text = dr.GetString(8);
                                tx_nomProp_des.Text = dr.GetString(9);
                                tx_plac_des.Text = dr.GetString(10);
                                tx_carret_des.Text = dr.GetString(11);
                                tx_confv_des.Text = dr.GetString(12);
                                tx_aut_des.Text = dr.GetString(13);
                                tx_idr_des.Text = dr.GetString("id");
                                retorna = true;
                            }
                        }
                    }
                }
            }
            return retorna;
        }
        private void jaladet(string ser, string num)                  // jala datos a la grilla
        {
            dataGridView1.DataSource = null;
            dataGridView1.ReadOnly = true;
            string consulta = "select a.fila as Fila,a.serguia as Serie,a.numguia as Número,a.totcant as Bultos,a.totpeso as Kgs,b.descrizionerid as 'Mon',a.totflet as Flete," +
                "a.pagado as Pagado,a.salxcob as Saldo,a.codmone " +
                "from detplacar a left join desc_mon b on b.idcodice=a.codmone " +
                "WHERE a.serplacar = @ser AND a.numplacar = @num";
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                if (lib.procConn(conn) == true)
                {
                    using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                    {
                        micon.Parameters.AddWithValue("@ser", ser);
                        micon.Parameters.AddWithValue("@num", num);
                        using (MySqlDataAdapter da = new MySqlDataAdapter(micon))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            dataGridView1.DataSource = dt;
                            armagrilla();
                        }
                    }
                }
            }
        }

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
                if (Convert.ToString(row["btn1"]) == "S") this.Bt_add.Visible = true;
                else this.Bt_add.Visible = false; 
                if (Convert.ToString(row["btn2"]) == "S") this.Bt_edit.Visible = true;
                else this.Bt_edit.Visible = false;
                if (Convert.ToString(row["btn3"]) == "S") this.Bt_anul.Visible = true;
                else this.Bt_anul.Visible = false;
                if (Convert.ToString(row["btn4"]) == "S") this.Bt_ver.Visible = true;
                else this.Bt_ver.Visible = false;
                if (Convert.ToString(row["btn5"]) == "S") this.Bt_print.Visible = true;
                else this.Bt_print.Visible = false;
                if (Convert.ToString(row["btn6"]) == "S") this.Bt_close.Visible = true;
                else this.Bt_close.Visible = false;
            }
        }
        #region botones
        private void Bt_add_Click(object sender, EventArgs e)
        {
            lp.escribe(this);
            lp.limpiagbox(gbox_serie);
            lp.limpiagbox(gbox_dest);
            dataGridView1.Rows.Clear();
            Tx_modo.Text = "NUEVO";
            button1.Image = Image.FromFile(img_grab);
            tx_serie.Focus();
        }
        private void Bt_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion botones;

        #endregion botones_de_comando  ;

        #region leaves y validating
        private void tx_serie_Validating(object sender, CancelEventArgs e)
        {
            if (tx_serie.Text.Trim() != "")
            {
                if (valids_leave("serie", tx_serie.Text.Trim(), tx_serie) == false)
                {
                    e.Cancel = true;
                }
            }
        }
        private void tx_numero_Validating(object sender, CancelEventArgs e)
        {
            if (tx_numero.Text.Trim() != "")
            {
                if (valids_leave("numero",tx_numero.Text.Trim(),tx_numero) == false)
                {
                    tx_numero.Text = "";
                    e.Cancel = true;
                }
                else
                {
                    tx_ser_pla_des.Focus();
                }
            }
        }
        private void tx_ser_pla_des_Validating(object sender, CancelEventArgs e)
        {
            if (tx_ser_pla_des.Text.Trim() != "")
            {
                if (valids_leave("serie", tx_ser_pla_des.Text.Trim(), tx_ser_pla_des) == false)
                {
                    e.Cancel = true;
                }
            }
        }
        private void tx_num_pla_des_Validating(object sender, CancelEventArgs e)
        {
            if (tx_num_pla_des.Text.Trim() != "")
            {
                if (valids_leave("numero", tx_num_pla_des.Text.Trim(), tx_num_pla_des) == false)
                {
                    tx_num_pla_des.Text = "";
                    tx_orig_des.Text = "";
                    tx_dest_des.Text = "";
                    tx_fech_des.Text = "";
                    tx_estad_des.Text = "";
                    tx_chof_des.Text = "";
                    tx_nomChof_des.Text = "";
                    tx_ayu_des.Text = "";
                    tx_nomAyu_des.Text = "";
                    tx_prop_des.Text = "";
                    tx_nomProp_des.Text = "";
                    tx_plac_des.Text = "";
                    tx_carret_des.Text = "";
                    tx_confv_des.Text = "";
                    tx_aut_des.Text = "";
                    e.Cancel = true;
                }
                else
                {
                    button1.Focus();
                }
            }
        }
        #endregion
        
        private void button1_Click(object sender, EventArgs e)
        {
            if (tx_numero.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese el número de la planilla origen","Atención - complete",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            if (tx_num_pla_des.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese el número de la planilla destino", "Atención - complete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (dataGridView1.Rows.Count < 1)
            {
                MessageBox.Show("No hay guías para transborar", "Atención - complete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //
            var aa = MessageBox.Show("Confirma que desea TRANSBORDAR?", "Atención - Confirme", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (aa == DialogResult.Yes)
            {
                if (graba() == false)
                {
                    MessageBox.Show("Se produjeron errores en el proceso de grabar" + Environment.NewLine + 
                        "Revise sus datos y pida confirmación al" + Environment.NewLine +
                         "personal de sistemas","Atención",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Se generó el transbordo con EXITO","Todo conforme",MessageBoxButtons.OK,MessageBoxIcon.Information);
                    this.Close();
                }
            }
        }
        private bool graba()
        {
            bool retorna = false;
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                if (lib.procConn(conn) == true)
                {
                    string actua = "update cabplacar set " +
                            "obsplacar=@obspl,cantfilas=cantfilas+@cantf,cantotpla=cantotpla+@canto,pestotpla=pestotpla+@pesto," +
                            "subtotpla=@subto,igvplacar=@igvpl,totplacar=totplacar+@totpl,totpagado=totpagado+@totpa,salxpagar=salxpagar+@salxp," +
                            "verApp=@verApp,userm=@asd,fechm=now(),diriplan4=@iplan,diripwan4=@ipwan,netbname=@nbnam " +
                            "where serplacar=@serpl and numplacar=@numpl";
                    using (MySqlCommand miact = new MySqlCommand(actua, conn))
                    {
                        miact.Parameters.AddWithValue("@serpl", tx_ser_pla_des.Text);
                        miact.Parameters.AddWithValue("@numpl", tx_num_pla_des.Text);
                        miact.Parameters.AddWithValue("@obspl", "Transbordado" + tx_serie.Text + "-" + tx_numero.Text);
                        miact.Parameters.AddWithValue("@cantf", tx_dat_o_tfil.Text);       // cantidad filas detalle
                        miact.Parameters.AddWithValue("@canto", tx_dat_o_tcan.Text);       // cant total de bultos
                        miact.Parameters.AddWithValue("@pesto", tx_dat_o_tpes.Text);       // peso total
                                                                                           //miact.Parameters.AddWithValue("@tipmo", tx_dat_mone.Text);    //TODAS LAS PLANILLAS SON EN MONEDA LOCAL 08/01/2021
                        miact.Parameters.AddWithValue("@subto", "0.00");
                        miact.Parameters.AddWithValue("@igvpl", "0.00");
                        miact.Parameters.AddWithValue("@totpl", tx_dat_o_totf.Text);        // total flete
                        miact.Parameters.AddWithValue("@totpa", tx_dat_o_tpag.Text);        // total pagado
                        miact.Parameters.AddWithValue("@salxp", tx_dat_o_tsal.Text);        // saldo por cobrar al momento de grabar la planilla
                        miact.Parameters.AddWithValue("@verApp", verapp);
                        miact.Parameters.AddWithValue("@asd", asd);
                        miact.Parameters.AddWithValue("@iplan", lib.iplan());
                        miact.Parameters.AddWithValue("@ipwan", TransCarga.Program.vg_ipwan);
                        miact.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                        miact.ExecuteNonQuery();
                    }
                    // detalle
                    if (dataGridView1.Rows.Count > 0)
                    {
                        int fila = 1;
                        for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                        {
                            if (dataGridView1.Rows[i].Cells[0].Value != null)
                            {
                                if (dataGridView1.Rows[i].Cells[0].Value.ToString().Trim() != "")
                                {
                                    string inserd2 = "insert into detplacar (idc,serplacar,numplacar,fila,numpreg,serguia,numguia,totcant,totpeso,totflet,codmone,estadoser,origreg," +
                                        "verApp,userm,fechm,diriplan4,diripwan4,netbname,platracto,placarret,autorizac,confvehic,brevchofe,brevayuda,rucpropiet,fechope,pagado,salxcob) " +
                                        "values (@idr,@serpl,@numpl,@fila,@numpr,@sergu,@numgu,@totca,@totpe,@totfl,@codmo,@estad,@orireg," +
                                        "@verApp,@asd,now(),@iplan,@ipwan,@nbnam,@platr,@placa,@autor,@confv,@brevc,@breva,@rucpr,@fecho,@paga,@xcob)";
                                    using (MySqlCommand micon = new MySqlCommand(inserd2, conn))
                                    {
                                        // Fila,Serie,Número,Bultos,Kgs,Mon,Flete,Pagado,Saldo,CodMon
                                        micon.Parameters.AddWithValue("@idr", tx_idr_des.Text);
                                        micon.Parameters.AddWithValue("@serpl", tx_ser_pla_des.Text);
                                        micon.Parameters.AddWithValue("@numpl", tx_num_pla_des.Text);
                                        micon.Parameters.AddWithValue("@fila", dataGridView1.Rows[i].Cells[0].Value.ToString());
                                        micon.Parameters.AddWithValue("@numpr", "");
                                        micon.Parameters.AddWithValue("@sergu", dataGridView1.Rows[i].Cells[1].Value.ToString());
                                        micon.Parameters.AddWithValue("@numgu", dataGridView1.Rows[i].Cells[2].Value.ToString());
                                        micon.Parameters.AddWithValue("@totca", dataGridView1.Rows[i].Cells[3].Value.ToString());
                                        micon.Parameters.AddWithValue("@totpe", dataGridView1.Rows[i].Cells[4].Value.ToString());
                                        micon.Parameters.AddWithValue("@totfl", dataGridView1.Rows[i].Cells[6].Value.ToString());
                                        micon.Parameters.AddWithValue("@codmo", dataGridView1.Rows[i].Cells[9].Value.ToString());
                                        micon.Parameters.AddWithValue("@estad", tx_estad_des.Text);
                                        micon.Parameters.AddWithValue("@orireg", "M");              // origen del registro manual, cuando viene desde el form de guias es A
                                        micon.Parameters.AddWithValue("@verApp", verapp);
                                        micon.Parameters.AddWithValue("@asd", asd);
                                        micon.Parameters.AddWithValue("@iplan", lib.iplan());
                                        micon.Parameters.AddWithValue("@ipwan", TransCarga.Program.vg_ipwan);
                                        micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                                        micon.Parameters.AddWithValue("@platr", tx_plac_des.Text);
                                        micon.Parameters.AddWithValue("@placa", tx_carret_des.Text);
                                        micon.Parameters.AddWithValue("@autor", tx_aut_des.Text);
                                        micon.Parameters.AddWithValue("@confv", tx_confv_des.Text);
                                        micon.Parameters.AddWithValue("@brevc", tx_chof_des.Text);
                                        micon.Parameters.AddWithValue("@breva", tx_ayu_des.Text);
                                        micon.Parameters.AddWithValue("@rucpr", tx_prop_des.Text);
                                        micon.Parameters.AddWithValue("@fecho", tx_fech_des.Text.Substring(6, 4) + "-" + tx_fech_des.Text.Substring(3, 2) + "-" + tx_fech_des.Text.Substring(0, 2));
                                        micon.Parameters.AddWithValue("@paga", dataGridView1.Rows[i].Cells[7].Value.ToString());    // 
                                        micon.Parameters.AddWithValue("@xcob", dataGridView1.Rows[i].Cells[8].Value.ToString());    // 
                                        micon.ExecuteNonQuery();
                                        fila += 1;
                                    }
                                }
                            }
                        }
                        // llamada al procedimiento para numerar las filas 
                        // numdetpla
                        using (MySqlCommand micon = new MySqlCommand("numdetpla", conn))
                        {
                            micon.CommandType = CommandType.StoredProcedure;
                            micon.Parameters.AddWithValue("@vseri", tx_ser_pla_des.Text);
                            micon.Parameters.AddWithValue("@vnume", tx_num_pla_des.Text);
                            micon.ExecuteNonQuery();
                        }
                    }
                    // actualizamos planilla origen, campos transbordo y estado anulado o transbordado
                    string actcab = "update cabplacar set transSer=@serD,transNum=@numD,transFech=@fechD,transUser=@userD,estadoser=@esta " +
                        "where serplacar=@serO and numplacar=@numO";
                    using (MySqlCommand micon = new MySqlCommand(actcab, conn))
                    {
                        micon.Parameters.AddWithValue("@serO", tx_serie.Text);
                        micon.Parameters.AddWithValue("@numO", tx_numero.Text);
                        micon.Parameters.AddWithValue("@serD", tx_ser_pla_des.Text);
                        micon.Parameters.AddWithValue("@numD", tx_num_pla_des.Text);
                        micon.Parameters.AddWithValue("@fechD", tx_fech_des.Text.Substring(6,4) + "-" + tx_fech_des.Text.Substring(3, 2) + "-" + tx_fech_des.Text.Substring(0, 2));
                        micon.Parameters.AddWithValue("@userD", asd);
                        micon.Parameters.AddWithValue("@esta", v_esta);
                        micon.ExecuteNonQuery();
                    }
                    retorna = true;
                }
            }
            return retorna;
        }
    }
}
