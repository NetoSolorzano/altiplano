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
        string[] vs = {"","","","","","","","","","","","","", "", "", "", "", "", "", "",   // 20
                               "", "", "", "", "", "", "", "", "", ""};    // 10
        string[] va = { "", "", "", "", "", "", "", "", "" };       // 9
        string[,] dt = new string[10, 6] { 
            { "", "", "", "", "", "" }, { "", "", "", "", "", "" }, { "", "", "", "", "", "" }, { "", "", "", "", "", "" }, { "", "", "", "", "", "" },
            { "", "", "", "", "", "" }, { "", "", "", "", "", "" }, { "", "", "", "", "", "" }, { "", "", "", "", "", "" }, { "", "", "", "", "", "" }
        }; // 6 columnas, 10 filas

        short copias;
        string otro = "";               // ruta y nombre del png código QR

        public impDV(int nCopias, string nomImp, string[] cabecera, string[,] detalle, string[] varios, string formato, string nomforCR)
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
            vs[30] = cabecera[30];     // 

            for (int o=0; o <= int.Parse(vs[12]); o++)
            {
                dt[o, 0] = detalle[o, 0];   // detalle fila o - dataGridView1.Rows[l].Cells["OriDest"]
                dt[o, 1] = detalle[o, 1];   // dataGridView1.Rows[l].Cells["Cant"]
                dt[o, 2] = detalle[o, 2];   // dataGridView1.Rows[l].Cells["umed"]
                dt[o, 3] = detalle[o, 3];   // guia transportista
                dt[o, 4] = detalle[o, 4];   // descripcion de la carga
                dt[o, 5] = detalle[o, 5];   // documento relacionado remitente de la guia transportista
            }

            va[0] = varios[0];         // Ruta y nombre del logo del emisor electrónico
            va[1] = varios[1];         // glosa del servicio en facturacion
            va[2] = varios[2];         // siglas nombre de tipo de documento Factura 
            va[3] = varios[3];         // 
            va[4] = varios[4];         // 
            va[5] = varios[5];         // 
            va[6] = varios[6];         // 
            va[7] = varios[7];         // 
            va[8] = varios[8];         // 

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
                    if (va[0] != "")
                    {
                        string codigo = va[0];                             // tx_dat_textoqr.Text
                        if (File.Exists(@va[1])) File.Delete(@va[1]);
                        var qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
                        var qrCode = qrEncoder.Encode(codigo);
                        var renderer = new GraphicsRenderer(new FixedModuleSize(5, QuietZoneModules.Two), Brushes.Black, Brushes.White);
                        using (var stream = new FileStream(@va[1], FileMode.Create))
                            renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, stream);
                    }
                    else
                    {
                        if (File.Exists(@va[1])) File.Delete(@va[1]);
                        va[1] = "";
                    }
                    if (nomImp != "" && nomforCR != "")
                    {
                        conClie data = generaReporte(nomforCR);
                        ReportDocument repo = new ReportDocument();
                        repo.Load(nomforCR);
                        repo.SetDataSource(data);
                        repo.PrintOptions.PrinterName = nomImp;
                        repo.PrintToPrinter((short)nCopias, false, 1, 1);
                    }
                    if (nomImp != "" && nomforCR == "")
                    {

                    }
                    if (nomImp == "" && nomforCR != "")
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
                            string qqq = "GRT-" + dt[0, 3] + " " + dt[0, 4];
                            if (qqq.Length > 41) siz = new SizeF(lib.CentimeterToPixel(anchTik), 30);
                            else siz = new SizeF(lib.CentimeterToPixel(anchTik), 15);
                            recto = new RectangleF(puntoF, siz);
                            e.Graphics.DrawString(qqq, lt_peq, Brushes.Black, recto, StringFormat.GenericTypographic);
                            posi = posi + alfi;
                            if (qqq.Length > 41) posi = posi + alfi - 4;
                            puntoF = new PointF(coli, posi);
                            e.Graphics.DrawString("Según doc.cliente: " + dt[0, 5], lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
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
                    if (vs[2] == va[2])
                    {
                        // forma de pago
                        posi = posi + (alfi / 1.5F);
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
                        if (double.Parse(vs[15]) > double.Parse(Program.valdetra))
                        {
                            siz = new SizeF(lib.CentimeterToPixel(anchTik), 15 * 3);
                            puntoF = new PointF(coli, posi);
                            recto = new RectangleF(puntoF, siz);
                            e.Graphics.DrawString(vs[20].Trim() + " " + Program.ctadetra.Trim(), lt_peq, Brushes.Black, recto, StringFormat.GenericTypographic);
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
                    if (vs[23] != "factDirecta")
                    {
                        posi = posi + lib.CentimeterToPixel(3);
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
                    }
                    posi = posi + alfi * 3;
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
        private conClie generaReporte(string cristalito)
        {

        }
    }
}
