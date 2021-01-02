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
            if (_datosReporte.cuadreCaja_cab.Rows.Count > 0)
            {

                    //repvtas_resumen _ventas = new repvtas_resumen();
                    //_ventas.SetDataSource(_datosReporte);
                    //crystalReportViewer1.ReportSource = _ventas;
            }
        }
    }
}
