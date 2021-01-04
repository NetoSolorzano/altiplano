using System;
using CrystalDecisions.CrystalReports.Engine;
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
                ReportDocument rpt = new ReportDocument();
                rpt.Load("formatos/cuadreCaja1.rpt");
                rpt.SetDataSource(_datosReporte);
                crystalReportViewer1.ReportSource = rpt;
            }
            if (_datosReporte.pendCob.Rows.Count > 0)
            {
                ReportDocument rpt = new ReportDocument();
                rpt.Load("formatos/pendCob1.rpt");
                rpt.SetDataSource(_datosReporte);
                crystalReportViewer1.ReportSource = rpt;
            }
        }
    }
}
