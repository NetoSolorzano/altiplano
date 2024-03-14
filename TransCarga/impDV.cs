using CrystalDecisions.CrystalReports.Engine;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransCarga
{
    class impDV
    {
        libreria lib = new libreria();
        string[] vs = {"", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "",     // 21
                       "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""};    // 21
        string[] va = { "", "", "", "", "", "", "", "", "", "" };       // 10
        string[,] dt = new string[10, 10] { 
            { "", "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "", "" }
        }; // 6 columnas, 10 filas
        string[] cu = { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };    // 18
        short copias;
        string otro = "";               // ruta y nombre del png código QR

        public impDV(int nCopias, string nomImp, string[] cabecera, string[,] detalle, string[] varios, string[] cunica, string formato, string nomforCR)
        {
            copias = (short)nCopias;
            vs[0] = cabecera[0];   // serie (F001)
            vs[1] = cabecera[1];   // numero
            vs[2] = cabecera[2];   // tx_dat_tdv.Text, siglas del tipo de documento
            vs[3] = cabecera[3];   // direccion emisor
            vs[4] = cabecera[4];   // nombre del tipo de documento
            vs[5] = cabecera[5];   // fecha de emision formato dd/mm/aaaa
            vs[6] = cabecera[6];   // tx_nomRem.Text -> nombre del cliente del comprobante
            vs[7] = cabecera[7];   // tx_numDocRem.Text -> numero documento del cliente
            vs[8] = cabecera[8];     // tx_dirRem.Text -> dirección cliente
            vs[9] = cabecera[9];     // distrito de la direccion
            vs[10] = cabecera[10];   // provincia de la direccion
            vs[11] = cabecera[11];   // departamento de la dirección
            vs[12] = cabecera[12];   // cantidad de filas de detalle
            vs[13] = cabecera[13];   // tx_subt.Text -> Sub total del comprobante
            vs[14] = cabecera[14];   // igv del comprobante
            vs[15] = cabecera[15];   // importe total del comprobante
            vs[16] = cabecera[16];   // cmb_mon.Text -> Simbolo de la moneda
            vs[17] = cabecera[17];   // tx_fletLetras.Text
            vs[18] = cabecera[18];   // CONTADO o CREDITO
            vs[19] = cabecera[19];   // tx_dat_dpla.Text -> dias de plazo credito
            vs[20] = cabecera[20];   // glosdetra -> Glosa para la detracción
            vs[21] = cabecera[21];   // codigo sunat tipo comprobante
            vs[22] = cabecera[22];   // tipoDocEmi -> CODIGO SUNAT tipo de documento RUC/DNI del cliente
            vs[23] = cabecera[23];   // provee => "factDirecta"
            vs[24] = cabecera[24];   // restexto -> texto del resolucion sunat del ose/pse
            vs[25] = cabecera[25];   // autoriz_OSE_PSE -> autoriz del ose/pse
            vs[26] = cabecera[26];   // webose -> web del ose/pse
            vs[27] = cabecera[27];     // usuario creador
            vs[28] = cabecera[28];     // local de emisión
            vs[29] = cabecera[29];     // glosa despedida
            vs[30] = cabecera[30];    // nombre del emisor del comprobante
            vs[31] = cabecera[31];    // ruc del emisor
            vs[32] = cabecera[32];    // fecha vencimiento del comprob.
            vs[33] = cabecera[33];    // forma de pago incluyendo # de cuotas (siempre es 1 cuota en Transcarga)
            vs[34] = cabecera[34];    // modalidad de transporte
            vs[35] = cabecera[35];    // motivo de traslado
            vs[36] = cabecera[36];    // nombre de la moneda
            vs[37] = cabecera[37];    // tot operaciones inafectas
            vs[38] = cabecera[38];    // tot operaciones exoneradas
            vs[39] = cabecera[39];      // 
            vs[40] = cabecera[40];      // dirección de la sucursal
            vs[41] = cabecera[41];      // observ. del comprobante

            cu[0] = cunica[0];          // "placa");
            cu[1] = cunica[1];          // "confv");
            cu[2] = cunica[2];          // "autoriz");
            cu[3] = cunica[3];          // "cargaEf");
            cu[4] = cunica[4];          // "cargaUt");
            cu[5] = cunica[5];          // "rucTrans");
            cu[6] = cunica[6];          // "nomTrans");
            cu[7] = cunica[7];          // "fecIniTras");
            cu[8] = cunica[8];          // "dirPartida");
            cu[9] = cunica[9];          // "ubiPartida");
            cu[10] = cunica[10];        // "dirDestin");
            cu[11] = cunica[11];        // "ubiDestin");
            cu[12] = cunica[12];        // "dniChof");
            cu[13] = cunica[13];        // "brevete");
            cu[14] = cunica[14];        // "valRefViaje");
            cu[15] = cunica[15];        // "valRefVehic");
            cu[16] = cunica[16];        // "valRefTon");
            cu[17] = cunica[17];        // registro mct

            for (int o=0; o <= int.Parse(vs[12]); o++)
            {
                dt[o, 0] = detalle[o, 0];   // detalle fila o - dataGridView1.Rows[l].Cells["OriDest"]
                dt[o, 1] = detalle[o, 1];   // dataGridView1.Rows[l].Cells["Cant"]
                dt[o, 2] = detalle[o, 2];   // dataGridView1.Rows[l].Cells["umed"]
                dt[o, 3] = detalle[o, 3];   // guia transportista
                dt[o, 4] = detalle[o, 4];   // descripcion de la carga
                dt[o, 5] = detalle[o, 5];   // documento relacionado remitente de la guia transportista
                dt[o, 6] = detalle[o, 6];   // valor unitario
                dt[o, 7] = detalle[o, 7];   // precio unitario
                dt[o, 8] = detalle[o, 8];   // total
            }

            va[0] = varios[0];         // Ruta y nombre del logo del emisor electrónico
            va[1] = varios[1];         // glosa del servicio en facturacion
            va[2] = varios[2];         // Código Transcarga del tipo de documento Factura 
            va[3] = varios[3];         // porcentaje detracción
            va[4] = varios[4];         // monto detracción
            va[5] = varios[5];         // cta. detracción
            va[6] = varios[6];         // concatenado de Guias Transportista para Formato de cargas unicas
            va[7] = varios[7];         // ruta y nombre del png codigo QR
            va[8] = varios[8];         // medio de pago sunat de la detracción
            va[9] = varios[9];         // tipo de cambio

            switch (formato)
            {
                case "TK":
                    PrintDocument print = new PrintDocument();
                    print.PrintPage += new PrintPageEventHandler(imprime_TK);
                    print.PrinterSettings.PrinterName = nomImp;
                    print.PrinterSettings.Copies = (short)nCopias;
                    print.Print();
                    break;
                case "A5":
                    // no hay comprobantes en A5 16/11/2023
                    break;
                case "A4":
                    if (true)
                    {
                        string separ = "|";
                        string codigo = vs[31] + separ + vs[21] + separ +
                            vs[0] + separ + vs[1] + separ +
                            vs[14] + separ + vs[15] + separ +
                            vs[5].Substring(6, 4) + "-" + vs[5].Substring(3, 2) + "-" + vs[5].Substring(0, 2) + separ + vs[22] + separ +
                            vs[7] + separ;

                        if (File.Exists(@va[7])) File.Delete(@va[7]);
                        var qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
                        var qrCode = qrEncoder.Encode(codigo);
                        var renderer = new GraphicsRenderer(new FixedModuleSize(5, QuietZoneModules.Two), Brushes.Black, Brushes.White);
                        using (var stream = new FileStream(@va[7], FileMode.Create))
                            renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, stream);
                    }
                    if (nomImp != "" && nomforCR != "")                     // impresion directa en impresora
                    {
                        conClie data = generaReporte(nomforCR);
                        ReportDocument repo = new ReportDocument();
                        repo.Load(nomforCR);
                        repo.SetDataSource(data);
                        repo.PrintOptions.PrinterName = nomImp;
                        repo.PrintToPrinter(copias, false, 1, 1);
                    }
                    if (nomImp != "" && nomforCR == "")
                    {

                    }
                    if (nomImp == "" && nomforCR != "")                     // visualización en pantalla
                    {
                        conClie datos = generaReporte(nomforCR);
                        frmvizoper visualizador = new frmvizoper(datos);
                        visualizador.Show();
                    }
                    break;
            }
        }

        public void imprime_TK(object sender, PrintPageEventArgs e)
        {
            {
                // DATOS PARA EL TICKET
                string nomclie = Program.cliente;
                string rasclie = Program.cliente;
                string rucclie = Program.ruc;
                string dirclie = Program.dirfisc;
                // TIPOS DE LETRA PARA EL DOCUMENTO FORMATO TICKET
                Font lt_gra = new Font("Arial", 11);                // grande
                Font lt_tit = new Font("Lucida Console", 10);       // mediano
                Font lt_med = new Font("Arial", 9);                // normal textos
                Font lt_peq = new Font("Arial", 8);                 // pequeño
                                                                    //
                float anchTik = 7.8F;                               // ancho del TK en centimetros
                int coli = 5;                                      // columna inicial
                float posi = 20;                                    // posicion x,y inicial
                int alfi = 15;                                      // alto de cada fila
                float ancho = 360.0F;                                // ancho de la impresion
                int copias = 1;                                     // cantidad de copias del ticket
                Image photo = Image.FromFile(va[0]); // logoclt
                for (int i = 1; i <= copias; i++)
                {
                    PointF puntoF = new PointF(coli, posi);
                    // imprimimos el logo o el nombre comercial del emisor
                    if (va[0] != "")
                    {
                        SizeF cuadLogo = new SizeF(lib.CentimeterToPixel(anchTik) - 20.0F, alfi * 6);
                        RectangleF reclogo = new RectangleF(puntoF, cuadLogo);
                        e.Graphics.DrawImage(photo, reclogo);
                    }
                    else
                    {
                        e.Graphics.DrawString(nomclie, lt_gra, Brushes.Black, puntoF, StringFormat.GenericTypographic);     // nombre comercial
                    }
                    float lt = (lib.CentimeterToPixel(anchTik) - e.Graphics.MeasureString(nomclie, lt_gra).Width) / 2;
                    posi = posi + alfi * 7;
                    lt = (ancho - e.Graphics.MeasureString(rasclie, lt_gra).Width) / 2;
                    puntoF = new PointF(lt, posi);
                    e.Graphics.DrawString(rasclie, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);     // razon social
                    posi = posi + alfi;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("Dom.Fiscal", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);     // direccion emisor
                    puntoF = new PointF(coli + 65, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 70, posi);
                    SizeF cuad = new SizeF(lib.CentimeterToPixel(anchTik) - (coli + 70), alfi * 2);
                    RectangleF recdom = new RectangleF(puntoF, cuad);
                    e.Graphics.DrawString(dirclie, lt_med, Brushes.Black, recdom, StringFormat.GenericTypographic);     // direccion emisor
                    posi = posi + alfi * 2;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("Sucursal", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);     // direccion emisor
                    puntoF = new PointF(coli + 65, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 70, posi);
                    cuad = new SizeF(lib.CentimeterToPixel(anchTik) - (coli + 70), alfi * 2);
                    recdom = new RectangleF(puntoF, cuad);
                    e.Graphics.DrawString(vs[3], lt_med, Brushes.Black, recdom, StringFormat.GenericTypographic);     // direccion emisor
                    posi = posi + alfi * 2;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("RUC ", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);     // ruc de emisor
                    puntoF = new PointF(coli + 65, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 70, posi);
                    e.Graphics.DrawString(rucclie, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);     // ruc de emisor
                    //string tipdo = cmb_tdv.Text;                                  // tipo de documento
                    string serie = vs[0];                                           // serie electrónica
                    string corre = vs[1];                                           // numero del documento electrónico
                    //string nota = tipdo + "-" + serie + "-" + corre;
                    string titdoc = vs[4];
                    posi = posi + alfi + 8;
                    lt = (lib.CentimeterToPixel(anchTik) - e.Graphics.MeasureString(titdoc, lt_gra).Width) / 2;
                    puntoF = new PointF(lt, posi);
                    e.Graphics.DrawString(titdoc, lt_gra, Brushes.Black, puntoF, StringFormat.GenericTypographic);                  // tipo de documento
                    posi = posi + alfi + 8;
                    string titnum = serie + " - " + corre;
                    lt = (lib.CentimeterToPixel(anchTik) - e.Graphics.MeasureString(titnum, lt_gra).Width) / 2;
                    puntoF = new PointF(lt, posi);
                    e.Graphics.DrawString(titnum, lt_gra, Brushes.Black, puntoF, StringFormat.GenericTypographic);                  // serie y numero
                    posi = posi + alfi + alfi;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("F. Emisión", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 65, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 70, posi);
                    e.Graphics.DrawString(vs[5], lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);                   // fecha y hora emision
                    posi = posi + alfi;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("Cliente", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);                  
                    puntoF = new PointF(coli + 65, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 70, posi);
                    if (vs[6].Trim().Length > 39) cuad = new SizeF(lib.CentimeterToPixel(anchTik) - (coli + 70), alfi * 2);
                    else cuad = new SizeF(lib.CentimeterToPixel(anchTik) - (coli + 70), alfi * 1);
                    recdom = new RectangleF(puntoF, cuad);
                    e.Graphics.DrawString(vs[6].Trim(), lt_peq, Brushes.Black, recdom, StringFormat.GenericTypographic);            // cliente
                    if (vs[6].Trim().Length > 39) posi = posi + alfi + alfi;
                    else posi = posi + alfi;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("RUC", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 65, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 70, posi);
                    e.Graphics.DrawString(vs[7], lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);                   // ruc/dni del cliente
                    posi = posi + alfi;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("Dirección", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);             // direccion
                    puntoF = new PointF(coli + 65, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 70, posi);
                    string dipa = vs[8].Trim() + Environment.NewLine + vs[9].Trim() + " - " + vs[10].Trim() + " - " + vs[11].Trim();
                    if (dipa.Length < 60) cuad = new SizeF(lib.CentimeterToPixel(anchTik) - (coli + 70), alfi * 2);
                    else cuad = new SizeF(lib.CentimeterToPixel(anchTik) - (coli + 70), alfi * 3);
                    RectangleF recdir = new RectangleF(puntoF, cuad);
                    e.Graphics.DrawString(vs[8].Trim() + Environment.NewLine +
                        vs[9].Trim() + " - " + vs[10].Trim() + " - " + vs[11].Trim(),
                        lt_peq, Brushes.Black, recdir, StringFormat.GenericTypographic);                                            // direccion
                    if (dipa.Length < 60) posi = posi + alfi + alfi;
                    else posi = posi + alfi + alfi + alfi;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString(" ", lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    // ***************** detalle del documento **************** //
                    StringFormat alder = new StringFormat(StringFormatFlags.DirectionRightToLeft);
                    SizeF siz = new SizeF(70, 15);
                    RectangleF recto = new RectangleF(puntoF, siz);
                    int tfg = int.Parse(vs[12]);
                    for (int l = 0; l < tfg; l++)
                    {
                        string textF2 = dt[l, 0] + " - " + dt[l, 1] + " " + dt[l, 2];
                        {
                            puntoF = new PointF(coli, posi);
                            e.Graphics.DrawString(va[1], lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                            posi = posi + alfi;
                            puntoF = new PointF(coli, posi);
                            e.Graphics.DrawString(textF2, lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                            posi = posi + alfi;
                            puntoF = new PointF(coli, posi);
                            string qqq = "GRT-" + dt[l, 3] + " " + dt[l, 4];
                            if (qqq.Length > 41) siz = new SizeF(lib.CentimeterToPixel(anchTik), 30);
                            else siz = new SizeF(lib.CentimeterToPixel(anchTik), 15);
                            recto = new RectangleF(puntoF, siz);
                            e.Graphics.DrawString(qqq, lt_peq, Brushes.Black, recto, StringFormat.GenericTypographic);
                            posi = posi + alfi;
                            if (qqq.Length > 41) posi = posi + alfi - 4;
                            puntoF = new PointF(coli, posi);
                            e.Graphics.DrawString("Según doc.cliente: " + dt[l, 5], lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                            posi = posi + alfi * 2;
                        }
                    }
                    // pie del documento ;
                    siz = new SizeF(70, 15);
                    if (vs[2] != va[2])         // Boleta
                    {
                        posi = posi + alfi;
                        puntoF = new PointF(coli, posi);
                        e.Graphics.DrawString("OP. GRAVADA", lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        puntoF = new PointF(coli + 190, posi);
                        RectangleF recst = new RectangleF(puntoF, siz);
                        e.Graphics.DrawString(vs[13], lt_peq, Brushes.Black, recst, alder);
                        posi = posi + alfi;
                        puntoF = new PointF(coli, posi);
                        e.Graphics.DrawString("OP. INAFECTA", lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        puntoF = new PointF(coli + 190, posi);
                        RectangleF recig = new RectangleF(puntoF, siz);
                        e.Graphics.DrawString("0.00", lt_peq, Brushes.Black, recig, alder);
                        posi = posi + alfi;
                        puntoF = new PointF(coli, posi);
                        e.Graphics.DrawString("OP. EXONERADA", lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        puntoF = new PointF(coli + 190, posi);
                        RectangleF recex = new RectangleF(puntoF, siz);
                        e.Graphics.DrawString("0.00", lt_peq, Brushes.Black, recex, alder);
                        posi = posi + alfi;
                        puntoF = new PointF(coli, posi);
                        e.Graphics.DrawString("IGV", lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        puntoF = new PointF(coli + 190, posi);
                        RectangleF recgv = new RectangleF(puntoF, siz);
                        e.Graphics.DrawString(vs[14], lt_peq, Brushes.Black, recgv, alder);
                        posi = posi + alfi;
                        puntoF = new PointF(coli, posi);
                        e.Graphics.DrawString("IMPORTE TOTAL " + vs[16], lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        puntoF = new PointF(coli + 190, posi);
                        recto = new RectangleF(puntoF, siz);
                        e.Graphics.DrawString(vs[15], lt_peq, Brushes.Black, recto, alder);
                    }
                    if (vs[2] == va[2])
                    {
                        posi = posi + alfi;
                        puntoF = new PointF(coli, posi);
                        e.Graphics.DrawString("OP. GRAVADA", lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        puntoF = new PointF(coli + 190, posi);
                        RectangleF recst = new RectangleF(puntoF, siz);
                        e.Graphics.DrawString(vs[13], lt_peq, Brushes.Black, recst, alder);
                        posi = posi + alfi;
                        puntoF = new PointF(coli, posi);
                        e.Graphics.DrawString("OP. INAFECTA", lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        puntoF = new PointF(coli + 190, posi);
                        RectangleF recig = new RectangleF(puntoF, siz);
                        e.Graphics.DrawString("0.00", lt_peq, Brushes.Black, recig, alder);
                        posi = posi + alfi;
                        puntoF = new PointF(coli, posi);
                        e.Graphics.DrawString("OP. EXONERADA", lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        puntoF = new PointF(coli + 190, posi);
                        RectangleF recex = new RectangleF(puntoF, siz);
                        e.Graphics.DrawString("0.00", lt_peq, Brushes.Black, recex, alder);
                        posi = posi + alfi;
                        puntoF = new PointF(coli, posi);
                        e.Graphics.DrawString("IGV", lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        puntoF = new PointF(coli + 190, posi);
                        RectangleF recgv = new RectangleF(puntoF, siz);
                        e.Graphics.DrawString(vs[14], lt_peq, Brushes.Black, recgv, alder);
                        posi = posi + alfi;
                        puntoF = new PointF(coli, posi);
                        e.Graphics.DrawString("IMPORTE TOTAL " + vs[16], lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        puntoF = new PointF(coli + 190, posi);
                        recto = new RectangleF(puntoF, siz);
                        e.Graphics.DrawString(vs[15], lt_peq, Brushes.Black, recto, alder);
                    }
                    posi = posi + alfi * 2;
                    puntoF = new PointF(coli, posi);
                    NumLetra nl = new NumLetra();
                    string monlet = "SON: " + vs[17];
                    if (monlet.Length <= 30) siz = new SizeF(lib.CentimeterToPixel(anchTik), alfi);
                    else siz = new SizeF(lib.CentimeterToPixel(anchTik), alfi * 2);
                    recto = new RectangleF(puntoF, siz);
                    e.Graphics.DrawString(monlet, lt_peq, Brushes.Black, recto, StringFormat.GenericTypographic);
                    if (monlet.Length <= 30) posi = posi + alfi;
                    else posi = posi + alfi + alfi;
                    // observaciones
                    if (vs[41].Trim() != "")
                    {
                        puntoF = new PointF(coli, posi);
                        decimal largo = vs[41].Trim().Length / 40;
                        decimal qw = Math.Ceiling(largo);
                        siz = new SizeF(lib.CentimeterToPixel(anchTik), alfi * (float)qw);
                        recto = new RectangleF(puntoF, siz);
                        e.Graphics.DrawString("Obs.:" + vs[41].Trim(), lt_peq, Brushes.Black, recto, StringFormat.GenericTypographic);
                        posi = posi + alfi * (float)qw;
                    }
                    if (vs[2] == va[2])
                    {
                        // forma de pago
                        posi = posi + alfi; // (alfi / 1.5F);
                        string ahiva = "";
                        if (vs[18] == "CREDITO")    // 
                        {
                            string _fechc = DateTime.Parse(vs[5]).AddDays(double.Parse(vs[19])).Date.ToString("dd-MM-yyyy");    // "yyyy-MM-dd"
                            ahiva = "- AL CREDITO -" + " 1 CUOTA - VCMTO: " + _fechc;
                        }
                        else
                        {
                            ahiva = "PAGO AL CONTADO " + vs[15];
                        }
                        puntoF = new PointF(coli, posi);
                        e.Graphics.DrawString(ahiva, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        posi = posi + alfi * 1.5F;
                        // leyenda de detracción
                        if (vs[20] != "")   // double.Parse(vs[15]) > double.Parse(Program.valdetra)
                        {
                            siz = new SizeF(lib.CentimeterToPixel(anchTik), 15 * 3);
                            puntoF = new PointF(coli, posi);
                            recto = new RectangleF(puntoF, siz);
                            e.Graphics.DrawString(vs[20].Trim() + " Cta.BN " + Program.ctadetra.Trim(), lt_peq, Brushes.Black, recto, StringFormat.GenericTypographic);
                            posi = posi + alfi * 3;
                        }
                    }
                    puntoF = new PointF(coli, posi);
                    string repre = "Representación impresa de la";
                    lt = (lib.CentimeterToPixel(anchTik) - e.Graphics.MeasureString(repre, lt_med).Width) / 2;
                    puntoF = new PointF(lt, posi);
                    e.Graphics.DrawString(repre, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli, posi);
                    string previo = vs[4];
                    lt = (lib.CentimeterToPixel(anchTik) - e.Graphics.MeasureString(previo, lt_med).Width) / 2;
                    puntoF = new PointF(lt, posi);
                    e.Graphics.DrawString(previo, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    
                    string separ = "|";
                    string codigo = rucclie + separ + vs[21] + separ +
                        serie + separ + vs[1] + separ +
                        vs[14] + separ + vs[15] + separ +
                        vs[5].Substring(6, 4) + "-" + vs[5].Substring(3, 2) + "-" + vs[5].Substring(0, 2) + separ + vs[22] + separ +
                        vs[7] + separ;
                    //
                    var rnd = Path.GetRandomFileName();
                    otro = Path.GetFileNameWithoutExtension(rnd);
                    otro = otro + ".png";
                    //
                    var qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
                    var qrCode = qrEncoder.Encode(codigo);
                    var renderer = new GraphicsRenderer(new FixedModuleSize(5, QuietZoneModules.Two), Brushes.Black, Brushes.White);
                    using (var stream = new FileStream(otro, FileMode.Create))
                        renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, stream);
                    Bitmap png = new Bitmap(otro);
                    posi = posi + alfi + 7;
                    lt = (lib.CentimeterToPixel(anchTik) - lib.CentimeterToPixel(3)) / 2;
                    puntoF = new PointF(lt, posi);
                    SizeF cuadro = new SizeF(lib.CentimeterToPixel(3), lib.CentimeterToPixel(3));    // 5x5 cm
                    RectangleF rec = new RectangleF(puntoF, cuadro);
                    e.Graphics.DrawImage(png, rec);
                    png.Dispose();
                    
                    // leyenda 2
                    posi = posi + lib.CentimeterToPixel(3);
                    if (vs[23] != "factDirecta")
                    {
                        lt = (lib.CentimeterToPixel(anchTik) - e.Graphics.MeasureString(vs[24], lt_med).Width) / 2;
                        puntoF = new PointF(lt, posi);
                        e.Graphics.DrawString(vs[24], lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        posi = posi + alfi;
                        lt = (lib.CentimeterToPixel(anchTik) - e.Graphics.MeasureString(vs[25], lt_med).Width) / 2;
                        puntoF = new PointF(lt, posi);
                        e.Graphics.DrawString(vs[25], lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        // centrado en rectangulo   *********************
                        StringFormat sf = new StringFormat();       //  *
                        sf.Alignment = StringAlignment.Center;      //  *
                        posi = posi + alfi + 5;
                        SizeF leyen = new SizeF(lib.CentimeterToPixel(anchTik) - 20, alfi * 3);
                        puntoF = new PointF(coli, posi);
                        leyen = new SizeF(lib.CentimeterToPixel(anchTik) - 20, alfi * 2);
                        RectangleF recley5 = new RectangleF(puntoF, leyen);
                        e.Graphics.DrawString(vs[26], lt_med, Brushes.Black, recley5, sf);
                        posi = posi + alfi * 3;
                    }
                    string locyus = vs[28] + " - " + vs[27];
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString(locyus, lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);                  // tienda y vendedor
                    posi = posi + alfi;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("Imp. " + DateTime.Now, lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi + alfi;
                    puntoF = new PointF((lib.CentimeterToPixel(anchTik) - e.Graphics.MeasureString(vs[29], lt_med).Width) / 2, posi);
                    e.Graphics.DrawString(vs[29], lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi + alfi;
                    //puntoF = new PointF(coli, posi);
                    //e.Graphics.DrawString(".", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                }
            }
        }
        private conClie generaReporte(string cristalito)            // cambiar a facturas/boletas
        {
            conClie DV = new conClie();
            conClie.cVta_cabRow cabRow = DV.cVta_cab.NewcVta_cabRow();
            // CABECERA
            cabRow.formatoRPT = cristalito;
            cabRow.id = "0";         // 
            cabRow.serie = vs[0];
            cabRow.numero = vs[1];
            cabRow.tipDoc = vs[2];
            cabRow.dirEmisor = vs[3];
            cabRow.nomTipdoc = vs[4];
            cabRow.fecEmi = vs[5];
            cabRow.nomClte = vs[6];
            cabRow.nDocClte = vs[7];
            cabRow.DirClte = vs[8];
            cabRow.distClte = vs[9];
            cabRow.provClte = vs[10];
            cabRow.depaClte = vs[11];
            cabRow.canfdet = vs[12];
            cabRow.subtotal = vs[13];
            cabRow.igv = vs[14];
            cabRow.total = vs[15];
            cabRow.moneda = vs[16];
            cabRow.fleteLetras = vs[17];
            cabRow.codicion = vs[18];
            cabRow.dplazo = vs[19];
            cabRow.glosaDet = vs[20];
            cabRow.tipcomSunat = vs[21];
            cabRow.tdClteSunat = vs[22];
            cabRow.provee = vs[23];
            cabRow.resolTex = vs[24];
            cabRow.autorizSunat = vs[25];
            cabRow.webose = vs[26];
            cabRow.userc = vs[27];
            cabRow.localEmi = vs[28];
            cabRow.glosDesped = vs[29];
            cabRow.nomEmisor = vs[30];    // nombre del emisor del comprobante
            cabRow.rucEmisor = vs[31];    // ruc del emisor
            cabRow.fecVence = vs[32];    // fecha vencimiento del comprob.
            cabRow.formaPago = vs[33];    // forma de pago incluyendo # de cuotas (siempre es 1 cuota en Transcarga)
            cabRow.modTransp = vs[34];    // modalidad de transporte
            cabRow.motTrasla = vs[35];    // motivo de traslado
            cabRow.nomMone = vs[36];      // nombre de la moneda
            cabRow.totOpInafec = vs[37];    // tot operaciones inafectas
            cabRow.totOpExone = vs[38];     // tot operaciones exoneradas
            cabRow.valCuota = vs[39];       // valor de la cuota
            cabRow.dirSucursal = vs[40];    // direccion de la sucursal
            cabRow.obsComp = vs[41];        // observaciones del comprobante
            DV.cVta_cab.AddcVta_cabRow(cabRow);
            
            // DETALLE
            for (int o = 0; o < int.Parse(vs[12]); o++)
            {
                conClie.cVta_detRow detRow = DV.cVta_det.NewcVta_detRow();
                detRow.id = "0";
                detRow.OriDest = dt[o, 0];      // ["OriDest"]
                detRow.cant = dt[o, 1];         // ["Cant"]
                detRow.umed = dt[o, 2];         // ["umed"]
                detRow.guiaT = dt[o, 3];        // guia transportista
                detRow.descrip = dt[o, 4];      // descripcion de la carga
                detRow.docRel1 = dt[o, 5];      // documento relacionado remitente de la guia transportista
                detRow.docRel2 = "";            // 
                detRow.valUnit = dt[o, 6];      // valor unitario
                detRow.preUnit = dt[o, 7];      // precio unitario
                detRow.Total = dt[o, 8];        // total fila
                detRow.peso = dt[o, 9];         // peso de la guía
                DV.cVta_det.AddcVta_detRow(detRow);
            }

            // CARGA UNICA
            conClie.cVta_cuRow cuRow = DV.cVta_cu.NewcVta_cuRow();
            cuRow.placa = cu[0];          // "placa");
            cuRow.confv = cu[1];          // "confv");
            cuRow.autoriz = cu[2];          // "autoriz");
            cuRow.cargaEf = cu[3];          // "cargaEf");
            cuRow.cargaUt = cu[4];          // "cargaUt");
            cuRow.rucTrans = cu[5];          // "rucTrans");
            cuRow.nomTrans = cu[6];          // "nomTrans");
            cuRow.fecIniTras = cu[7];          // "fecIniTras");
            cuRow.dirPartida = cu[8];          // "dirPartida");
            cuRow.ubiPartida = cu[9];          // "ubiPartida");
            cuRow.dirDestin = cu[10];        // "dirDestin");
            cuRow.ubiDestin = cu[11];        // "ubiDestin");
            cuRow.dniChof = cu[12];        // "dniChof");
            cuRow.brevete = cu[13];        // "brevete");
            cuRow.valRefViaje = cu[14];        // "valRefViaje");
            cuRow.valRefVehic = cu[15];        // "valRefVehic");
            cuRow.valRefTon = cu[16];        // "valRefTon");
            cuRow.regMTC = cu[17];          // registro mct
            DV.cVta_cu.AddcVta_cuRow(cuRow);

            // DATOS VARIOS
            conClie.cVta_vaRow vaRow = DV.cVta_va.NewcVta_vaRow();
            vaRow.id = "0";
            vaRow.cuenDet = va[5];
            vaRow.glosSerFact = va[1];
            vaRow.logoRutNom = va[0];
            vaRow.montDet = va[4];
            vaRow.porcDet = va[3];
            vaRow.guiasTrans = va[6];
            vaRow.ubicapng = va[7];
            vaRow.mpsdet = va[8];            // medio de pago sunat de la detracción
            vaRow.tipcambio = va[9];
            DV.cVta_va.AddcVta_vaRow(vaRow);
            return DV;
        }
    }
}
