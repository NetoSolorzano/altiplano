namespace TransCarga
{
    partial class facelec
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(facelec));
            this.button1 = new System.Windows.Forms.Button();
            this.tx_modo = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(112, 122);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tx_modo
            // 
            this.tx_modo.Location = new System.Drawing.Point(87, 201);
            this.tx_modo.Name = "tx_modo";
            this.tx_modo.Size = new System.Drawing.Size(100, 20);
            this.tx_modo.TabIndex = 1;
            // 
            // facelec
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(287, 264);
            this.Controls.Add(this.tx_modo);
            this.Controls.Add(this.button1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "facelec";
            this.Text = "facelec";
            this.Activated += new System.EventHandler(this.facelec_Activated);
            this.Deactivate += new System.EventHandler(this.facelec_Deactivate);
            this.Load += new System.EventHandler(this.facelec_Load);
            this.Click += new System.EventHandler(this.facelec_Click);
            this.Enter += new System.EventHandler(this.facelec_Enter);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        public System.Windows.Forms.TextBox tx_modo;
    }
}