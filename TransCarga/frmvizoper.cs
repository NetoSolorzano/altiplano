using System;
using System.Data;
using System.Windows.Forms;

namespace TransCarga
{
    public partial class frmvizoper : Form
    {
        conClie _datosReporte;

        private frmvizoper()
        {
            InitializeComponent();
        }

        public frmvizoper(conClie datos): this()
        {
            _datosReporte = datos;
        }

        private void frmvizoper_Load(object sender, EventArgs e)
        {
            if (_datosReporte.repvtas_cab.Rows.Count > 0)
            {
                DataRow row = _datosReporte.Tables["repvtas_cab"].Rows[0];
                if (row["modo"].ToString() == "resumen")
                {
                    //repvtas_resumen _ventas = new repvtas_resumen();
                    //_ventas.SetDataSource(_datosReporte);
                    //crystalReportViewer1.ReportSource = _ventas;
                }
                if (row["modo"].ToString() == "listado" && row["nudoclte"].ToString() == "")
                {
                    //repvtas_listado _ventas = new repvtas_listado();
                    //_ventas.SetDataSource(_datosReporte);
                    //crystalReportViewer1.ReportSource = _ventas;
                }
                if (row["modo"].ToString() == "listado" && row["nudoclte"].ToString() != "")
                {
                    //repvtas_xclte _ventas = new repvtas_xclte();
                    //_ventas.SetDataSource(_datosReporte);
                    //crystalReportViewer1.ReportSource = _ventas;
                }
            }
        }
    }
}
