namespace TransCarga
{
    partial class login
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(login));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Tx_pwd = new System.Windows.Forms.TextBox();
            this.Tx_user = new System.Windows.Forms.TextBox();
            this.tx_newcon = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.lb_titulo = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.Button2 = new System.Windows.Forms.Button();
            this.Button1 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.Tx_pwd);
            this.groupBox1.Controls.Add(this.Tx_user);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox1.Location = new System.Drawing.Point(271, 166);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(284, 75);
            this.groupBox1.TabIndex = 27;
            this.groupBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(40, -23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 16);
            this.label2.TabIndex = 18;
            this.label2.Text = "Usuario";
            // 
            // Tx_pwd
            // 
            this.Tx_pwd.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.Tx_pwd.Location = new System.Drawing.Point(66, 46);
            this.Tx_pwd.Name = "Tx_pwd";
            this.Tx_pwd.Size = new System.Drawing.Size(166, 20);
            this.Tx_pwd.TabIndex = 1;
            this.Tx_pwd.Text = "CLAVE";
            this.Tx_pwd.TextChanged += new System.EventHandler(this.Tx_pwd_TextChanged);
            this.Tx_pwd.Enter += new System.EventHandler(this.Tx_pwd_Enter);
            this.Tx_pwd.Leave += new System.EventHandler(this.Tx_pwd_Leave);
            // 
            // Tx_user
            // 
            this.Tx_user.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.Tx_user.Location = new System.Drawing.Point(66, 17);
            this.Tx_user.Name = "Tx_user";
            this.Tx_user.Size = new System.Drawing.Size(166, 20);
            this.Tx_user.TabIndex = 0;
            this.Tx_user.Text = "USUARIO";
            this.Tx_user.Enter += new System.EventHandler(this.Tx_user_Enter);
            this.Tx_user.Leave += new System.EventHandler(this.Tx_user_Leave);
            // 
            // tx_newcon
            // 
            this.tx_newcon.Location = new System.Drawing.Point(147, 3);
            this.tx_newcon.Name = "tx_newcon";
            this.tx_newcon.Size = new System.Drawing.Size(134, 20);
            this.tx_newcon.TabIndex = 18;
            this.tx_newcon.UseSystemPasswordChar = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.checkBox1);
            this.panel1.Controls.Add(this.tx_newcon);
            this.panel1.Location = new System.Drawing.Point(271, 246);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(284, 26);
            this.panel1.TabIndex = 29;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.checkBox1.Location = new System.Drawing.Point(26, 5);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(117, 17);
            this.checkBox1.TabIndex = 29;
            this.checkBox1.Text = "Cambia contraseña";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckStateChanged += new System.EventHandler(this.checkBox1_CheckStateChanged);
            // 
            // lb_titulo
            // 
            this.lb_titulo.BackColor = System.Drawing.Color.White;
            this.lb_titulo.Font = new System.Drawing.Font("Palatino Linotype", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_titulo.ForeColor = System.Drawing.Color.DimGray;
            this.lb_titulo.Location = new System.Drawing.Point(255, 36);
            this.lb_titulo.Name = "lb_titulo";
            this.lb_titulo.Size = new System.Drawing.Size(320, 132);
            this.lb_titulo.TabIndex = 0;
            this.lb_titulo.Text = "titulo";
            this.lb_titulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Location = new System.Drawing.Point(35, 38);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(200, 190);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 30;
            this.pictureBox1.TabStop = false;
            // 
            // Button2
            // 
            this.Button2.BackColor = System.Drawing.Color.Transparent;
            this.Button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button2.ForeColor = System.Drawing.Color.Transparent;
            this.Button2.Location = new System.Drawing.Point(537, -1);
            this.Button2.Name = "Button2";
            this.Button2.Size = new System.Drawing.Size(37, 37);
            this.Button2.TabIndex = 26;
            this.Button2.UseVisualStyleBackColor = false;
            this.Button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // Button1
            // 
            this.Button1.BackColor = System.Drawing.Color.Gray;
            this.Button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button1.ForeColor = System.Drawing.Color.Transparent;
            this.Button1.Location = new System.Drawing.Point(319, 284);
            this.Button1.Name = "Button1";
            this.Button1.Size = new System.Drawing.Size(196, 34);
            this.Button1.TabIndex = 0;
            this.Button1.Text = "INGRESAR";
            this.Button1.UseVisualStyleBackColor = false;
            this.Button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(22)))), ((int)(((byte)(204)))));
            this.panel2.Controls.Add(this.pictureBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(254, 347);
            this.panel2.TabIndex = 31;
            // 
            // login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.ClientSize = new System.Drawing.Size(574, 347);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.lb_titulo);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Button2);
            this.Controls.Add(this.Button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "login";
            this.Opacity = 0.9D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SolorSoft TransCarga";
            this.Load += new System.EventHandler(this.login_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.login_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        internal System.Windows.Forms.TextBox Tx_pwd;
        internal System.Windows.Forms.TextBox Tx_user;
        internal System.Windows.Forms.Button Button2;
        internal System.Windows.Forms.Button Button1;
        internal System.Windows.Forms.TextBox tx_newcon;
        private System.Windows.Forms.Panel panel1;
        internal System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lb_titulo;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}

