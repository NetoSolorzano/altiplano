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
    class impGRE_T
    {
        libreria lib = new libreria();
        string[] cab = { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "",        // 20
                         "", "", "", "", "", "", "", "", "", "", "" };      // 11
        string[,] det = new string[3,5] { { "", "", "", "", "" }, { "", "", "", "", "" }, { "", "", "", "", "" } };
        string[] var = { "", "", "", "", "", "", "", "", ""};       // 9
        string[] vch = { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };      // 17
        short copias = 0;
        string otro = "";               // ruta y nombre del png código QR
        public impGRE_T(int nCopias, string nomImp, string[] cabecera, string[,] detalle, string[] varios, string[] vehChof, string formato, string nomforCR)
        {
            copias = (short)nCopias;
            cab[0] = cabecera[0];   // serie de la GRE
            cab[1] = cabecera[1];   // corre de la GRE
            cab[2] = cabecera[2];   // fecha
            cab[3] = cabecera[3];   // dirección sede de la guía
            cab[4] = cabecera[4];   // Datos relacionados 1: tipo doc origen -> cmb_docorig.Text
            cab[5] = cabecera[5];   // Datos relacionados 1: numero doc origen -> tx_docsOr.Text
            cab[6] = cabecera[6];   // Datos relacionados 1: ruc doc origen -> tx_rucEorig.Text
            cab[7] = cabecera[7];   // Datos relacionados 2: tipo doc origen -> tx_dat_docOr2.Text
            cab[8] = cabecera[8];   // Datos relacionados 2: numero doc origen -> tx_docsOr2.Text
            cab[9] = cabecera[9];   // Datos relacionados 2: ruc doc origen -> tx_rucEorig2.Text
            cab[10] = cabecera[10];   // Datos remitente -> cmb_docRem.Text
            cab[11] = cabecera[11];   // Datos remitente -> tx_numDocRem.Text
            cab[12] = cabecera[12];   // Datos remitente -> tx_nomRem.Text.Trim()
            cab[13] = cabecera[13];   // Datos destinatario -> cmb_docDes.Text
            cab[14] = cabecera[14];   // Datos destinatario -> tx_numDocDes.Text
            cab[15] = cabecera[15];   // Datos destinatario -> tx_nomDrio.Text.Trim()
            cab[16] = cabecera[16];     // Fecha de traslado -> tx_pla_fech.Text
            cab[17] = cabecera[17];     // Peso de la carga -> tx_totpes.Text.Trim()
            cab[18] = cabecera[18];     // Unid medida del peso, KG o TM
            cab[19] = cabecera[19];   // Direccion de partida - dirección -> tx_dirRem.Text
            cab[20] = cabecera[20];   // Direccion de partida - departamento 
            cab[21] = cabecera[21];   // Direccion de partida - provincia
            cab[22] = cabecera[22];   // Direccion de partida - distrito
            cab[23] = cabecera[23];   // Direccion de llegada - dirección -> tx_dirDrio.Text
            cab[24] = cabecera[24];   // Direccion de llegada - departamento
            cab[25] = cabecera[25];   // Direccion de llegada - provincia
            cab[26] = cabecera[26];   // Direccion de llegada - distrito
            cab[27] = cabecera[27];     // usuario creador
            cab[28] = cabecera[28];     // local de emisión
            cab[29] = cabecera[29];     // numero de pre guía (orden de servicio)
            cab[30] = cabecera[30];     // flete de la guía

            det[0, 0] = detalle[0, 0];  // detalle fila 1
            det[0, 1] = detalle[0, 1];
            det[0, 2] = detalle[0, 2];
            det[0, 3] = detalle[0, 3];
            det[0, 4] = detalle[0, 4];
            if (det[1, 0] != "")
            {
                det[1, 0] = detalle[1, 0];  // detalle fila 2
                det[1, 1] = detalle[1, 1];
                det[1, 2] = detalle[1, 2];
                det[1, 3] = detalle[1, 3];
                det[1, 4] = detalle[1, 4];
            }
            if (det[2, 0] != "")
            {
                det[2, 0] = detalle[2, 0];  // detalle fila 3
                det[2, 1] = detalle[2, 1];
                det[2, 2] = detalle[2, 2];
                det[2, 3] = detalle[2, 3];
                det[2, 4] = detalle[2, 4];
            }

            var[0] = varios[0];         // Varios: texto del código QR ->tx_dat_textoqr.Text
            var[1] = varios[1];         // Ruta y nombre de la imagen QR
            var[2] = varios[2];         // despedida 1
            var[3] = varios[3];         // despedid2
            var[4] = varios[4];         // Glosa final comprobante 1 -> "Representación impresa sin valor legal de la"
            var[5] = varios[5];         // Glosa final comprobante 2 -> "Guía de Remisión Electrónica de Transportista"
            var[6] = varios[6];         // consignatario
            var[7] = varios[7];         // telefono remitente
            var[8] = varios[8];         // telefono destinatario

            vch[0] = vehChof[0];        // Vehiculos - Placa veh principal -> tx_pla_placa.Text
            vch[1] = vehChof[1];        // Vehiculos - Autoriz. vehicular -> tx_pla_autor.Text
            vch[2] = vehChof[2];        // Vehiculos - Num Registro MTC -> 
            vch[3] = vehChof[3];        // Vehiculos - Conf. vehicular ->
            vch[4] = vehChof[4];        // Vehiculos - Placa carreta -> 
            vch[5] = vehChof[5];        // Vehiculos - Autoriz. vehicular -> 
            vch[6] = vehChof[6];        // Vehiculos - Num Registro MTC -> 
            vch[7] = vehChof[7];        // Vehiculos - Conf. vehicular ->
            vch[8] = vehChof[8];          // Choferes - Dni chofer principal ->
            vch[9] = vehChof[9];          // Choferes - Brevete chofer principal ->  tx_pla_brevet.Text
            vch[10] = vehChof[10];        // Choferes - Nombres -> tx_pla_nomcho.Text
            vch[11] = vehChof[11];        // Choferes - Apellidos -> tx_pla_nomcho.Text
            vch[12] = vehChof[12];        // Choferes - Dni chofer secundario ->
            vch[13] = vehChof[13];        // Choferes - Brevete chofer secundario ->
            vch[14] = vehChof[14];        // Choferes - Nombres ->
            vch[15] = vehChof[15];        // Choferes - Apellidos ->

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

                    if (var[0] != "")
                    {
                        string codigo = var[0];                             // tx_dat_textoqr.Text
                        if (File.Exists(@var[1])) File.Delete(@var[1]);
                        //
                        var qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
                        var qrCode = qrEncoder.Encode(codigo);
                        var renderer = new GraphicsRenderer(new FixedModuleSize(5, QuietZoneModules.Two), Brushes.Black, Brushes.White);
                        using (var stream = new FileStream(@var[1], FileMode.Create))
                        renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, stream);
                    }
                    else
                    {
                        if (File.Exists(@var[1])) File.Delete(@var[1]);
                        var[1] = "";
                    }
                    if (nomImp != "" && nomforCR != "")
                    {
                        conClie data = generaReporte("nomforCR");
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
                case "A4":
                    if (var[0] != "")
                    {
                        string codigo = var[0];                             // tx_dat_textoqr.Text
                        if (File.Exists(@var[1])) File.Delete(@var[1]);
                        var qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
                        var qrCode = qrEncoder.Encode(codigo);
                        var renderer = new GraphicsRenderer(new FixedModuleSize(5, QuietZoneModules.Two), Brushes.Black, Brushes.White);
                        using (var stream = new FileStream(@var[1], FileMode.Create))
                            renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, stream);
                    }
                    else
                    {
                        if (File.Exists(@var[1])) File.Delete(@var[1]);
                        var[1] = "";
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

        public void imprime_TK(object sender, PrintPageEventArgs e)     // TK
        {
            {
                // DATOS PARA EL TICKET
                string nomclie = Program.cliente;
                string rasclie = Program.cliente;
                string rucclie = Program.ruc;
                string dirclie = Program.dirfisc;
                // TIPOS DE LETRA PARA EL DOCUMENTO FORMATO TICKET
                Font lt_gra = new Font("Arial", 11);                // grande
                Font lt_tit = new Font("Arial", 10);       // mediano
                Font lt_med = new Font("Arial", 9);                 // normal textos
                Font lt_medN = new Font("Arial", 9, FontStyle.Bold);                 // normal textos EN NEGRITA
                Font lt_peq = new Font("Arial", 8);                 // pequeño
                Font lt_peqN = new Font("Arial", 8, FontStyle.Bold);                 // pequeño EN NEGRITA
                                                                     //
                float anchTik = 7.8F;                               // ancho del TK en centimetros
                int coli = 5;                                       // columna inicial
                float posi = 20;                                    // posicion x,y inicial
                int alfi = 15;                                      // alto de cada fila
                float ancho = 360.0F;                               // ancho de la impresion
                //int copias = 1;                                     // cantidad de copias del ticket
                //
                for (int i = 1; i <= copias; i++)
                {
                    // ************************ código QR *************************** //
                    float lt = 0;
                    PointF puntoF = new PointF(lt, posi);
                    puntoF = new PointF(coli, posi);
                    // imprimimos el NOMBRE Y RUC DEL EMISOR
                    posi = posi + 1;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString(rasclie, lt_gra, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    //lt = (ancho - e.Graphics.MeasureString("RUC: " + rucclie, lt_gra).Width) / 2;
                    posi = posi + alfi;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("RUC: " + rucclie, lt_gra, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    // imprimimos el titulo del comprobante y el numero
                    string serie = cab[0];                                           // tx_serie.Text;
                    string corre = cab[1];                                           // tx_numero.Text;
                    string titdoc = "Guía de Remisión Electrónica Transportista";
                    posi = posi + alfi + 8;
                    //float lt = (lib.CentimeterToPixel(anchTik) - e.Graphics.MeasureString(titdoc, lt_gra).Width) / 2;
                    //lt = (ancho - e.Graphics.MeasureString(titdoc, lt_tit).Width) / 2;
                    puntoF = new PointF(1, posi);
                    e.Graphics.DrawString(titdoc, lt_tit, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi + 8;
                    string titnum = "Nro. " + serie + " - " + corre;
                    //lt = (lib.CentimeterToPixel(anchTik) - e.Graphics.MeasureString(titnum, lt_gra).Width) / 2;
                    lt = (ancho - e.Graphics.MeasureString(titnum, lt_gra).Width) / 2;
                    puntoF = new PointF(lt - 10, posi);
                    e.Graphics.DrawString(titnum, lt_gra, Brushes.Black, puntoF, StringFormat.GenericTypographic);

                    if (var[0] != "")
                    {
                        string codigo = var[0];                             // tx_dat_textoqr.Text
                        var rnd = Path.GetRandomFileName();
                        otro = Path.GetFileNameWithoutExtension(rnd);       // 
                        otro = otro + ".png";
                        //
                        var qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
                        var qrCode = qrEncoder.Encode(codigo);
                        var renderer = new GraphicsRenderer(new FixedModuleSize(5, QuietZoneModules.Two), Brushes.Black, Brushes.White);
                        using (var stream = new FileStream(otro, FileMode.Create))
                            renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, stream);
                        Bitmap png = new Bitmap(otro);
                        posi = posi + alfi + 7;
                        lt = (lib.CentimeterToPixel(anchTik) - lib.CentimeterToPixel(3)) / 2 + 20;
                        puntoF = new PointF(lt, posi);
                        SizeF cuadro = new SizeF(lib.CentimeterToPixel(3), lib.CentimeterToPixel(3));    // 5x5 cm
                        RectangleF rec = new RectangleF(puntoF, cuadro);
                        e.Graphics.DrawImage(png, rec);
                        png.Dispose();
                    }
                    else
                    {
                        StringFormat sf = new StringFormat();
                        sf.Alignment = StringAlignment.Center;
                        sf.LineAlignment = StringAlignment.Center;
                        Pen pen = new Pen(Color.Black, 1);
                        pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;

                        posi = posi + alfi + 7;
                        lt = (lib.CentimeterToPixel(anchTik) - lib.CentimeterToPixel(3)) / 2 + 20;
                        puntoF = new PointF(lt, posi);
                        Point point = new Point((int)lt, (int)posi);
                        SizeF cuadro = new SizeF(lib.CentimeterToPixel(3), lib.CentimeterToPixel(3));    // 5x5 cm
                        RectangleF rec = new RectangleF(puntoF, cuadro);
                        Rectangle recM = new Rectangle(point, new Size(lib.CentimeterToPixel(3), lib.CentimeterToPixel(3)));
                        e.Graphics.DrawRectangle(pen, recM);
                        e.Graphics.DrawString("X", lt_gra, Brushes.Black, rec, sf);
                    }
                    posi = posi + alfi * 7;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("Dom.Fiscal", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    SizeF cuad = new SizeF(lib.CentimeterToPixel(anchTik) - (30), alfi * 2);
                    RectangleF recdom = new RectangleF(puntoF, cuad);
                    e.Graphics.DrawString(dirclie, lt_peq, Brushes.Black, recdom, StringFormat.GenericTypographic);
                    posi = posi + alfi * 2;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("Sucursal", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    cuad = new SizeF(lib.CentimeterToPixel(anchTik) - (20), alfi * 2);
                    recdom = new RectangleF(puntoF, cuad);
                    e.Graphics.DrawString(cab[3], lt_peq, Brushes.Black, recdom, StringFormat.GenericTypographic);

                    // imprimimos los datos de emisión
                    posi = posi + alfi * 2;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("Datos de Emisión", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("F. Emisión", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 135, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 140, posi);
                    e.Graphics.DrawString(cab[2], lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    /*
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Hora Emisión", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 135, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 140, posi);
                    e.Graphics.DrawString("", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    */
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Pre guía (O/S)", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 135, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 140, posi);
                    e.Graphics.DrawString(cab[29], lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    // imprimimos los documentos relacionados
                    posi = posi + alfi * 2;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("Documentos relacionados", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Tipo de documento", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 135, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 140, posi);
                    e.Graphics.DrawString(cab[4], lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Nro. de documento", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 135, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 140, posi);
                    e.Graphics.DrawString(cab[5], lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Ruc del emisor", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 135, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 140, posi);
                    e.Graphics.DrawString(cab[6], lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    if (cab[7] != "")
                    {
                        posi = posi + alfi;
                        puntoF = new PointF(coli + 20, posi);
                        e.Graphics.DrawString("Tipo de documento", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        puntoF = new PointF(coli + 135, posi);
                        e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        puntoF = new PointF(coli + 140, posi);
                        e.Graphics.DrawString(cab[7], lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        posi = posi + alfi;
                        puntoF = new PointF(coli + 20, posi);
                        e.Graphics.DrawString("Nro. de documento", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        puntoF = new PointF(coli + 135, posi);
                        e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        puntoF = new PointF(coli + 140, posi);
                        e.Graphics.DrawString(cab[8], lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        posi = posi + alfi;
                        puntoF = new PointF(coli + 20, posi);
                        e.Graphics.DrawString("Ruc del emisor", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        puntoF = new PointF(coli + 135, posi);
                        e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        puntoF = new PointF(coli + 140, posi);
                        e.Graphics.DrawString(cab[9], lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    }
                    // imprimimos los datos de envio
                    posi = posi + alfi * 2;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("Datos del Envío", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Remitente", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 135, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 140, posi);
                    e.Graphics.DrawString(cab[10] + " " + cab[11], lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString(cab[12], lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Destinatario", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 135, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 140, posi);
                    e.Graphics.DrawString(cab[13] + " " + cab[14], lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString(cab[15], lt_peqN, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Fecha de Traslado", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 135, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 140, posi);
                    if (cab[16] != "") e.Graphics.DrawString(cab[16].Substring(6, 4) + "-" + cab[16].Substring(3, 2) + "-" + cab[16].Substring(0, 2),
                        lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Peso Bruto", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 135, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 140, posi);
                    if (cab[17].Trim() != "" && cab[17].Trim().Trim() != "0") e.Graphics.DrawString(cab[17] + " " + ((cab[18] == "K") ? "KGM" : "TNM"), 
                        lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Dirección de Partida", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    cuad = new SizeF(lib.CentimeterToPixel(anchTik) - (coli + 20), alfi * 2);
                    recdom = new RectangleF(puntoF, cuad);
                    e.Graphics.DrawString(cab[19].Trim() + " " + cab[20].Trim() + " " + cab[21].Trim() + " " + cab[22].Trim(),
                        lt_peq, Brushes.Black, recdom, StringFormat.GenericTypographic);
                    posi = posi + alfi * 2;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Dirección de Llegada", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    cuad = new SizeF(lib.CentimeterToPixel(anchTik) - (coli + 20), alfi * 2);
                    recdom = new RectangleF(puntoF, cuad);
                    e.Graphics.DrawString(cab[23].Trim() + " " + cab[24].Trim() + " " + cab[25].Trim() + " " + cab[26].Trim(),
                        lt_peq, Brushes.Black, recdom, StringFormat.GenericTypographic);
                    
                    // imprimimos datos del vehiculo
                    posi = posi + alfi * 3;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("Datos del Vehículo", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Placa", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 135, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 140, posi);
                    if (vch[0] != "") e.Graphics.DrawString(vch[0], lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Autorización", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 135, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 140, posi);
                    if (vch[1] != "") e.Graphics.DrawString(vch[1], lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    
                    // imprimimos los datos del chofer
                    posi = posi + alfi * 2;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("Datos del Chofer", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Licencia", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 135, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 140, posi);
                    if (vch[9] != "") e.Graphics.DrawString(vch[9], lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Nombre", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    if (vch[10].Trim() + vch[11].Trim() != "") e.Graphics.DrawString(vch[10].Trim() + vch[11].Trim(), lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    // row["numdcho"] = tx_pla_dniChof.Text;                                       // Numero de documento de identidad 

                    // imprimimos los bienes a transportar
                    posi = posi + alfi * 2;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("Bienes a transportar", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    for (int z=0; z < 3; z++)       // #fila [0], cantprodi [1], unimedpro [2], descprodi [3], pesoprodi [4]
                    {
                        if (det[z, 4] != "")
                        {
                            puntoF = new PointF(coli + 20, posi);
                            e.Graphics.DrawString(det[z, 1] + " " + det[z, 2] + " - " + det[z, 4] + " " + ((cab[18] == "K") ? "KGM" : "TNM"),
                                lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                            string gDetalle = det[z, 3];
                            double xxx = (e.Graphics.MeasureString(gDetalle, lt_peq).Width / lib.CentimeterToPixel(anchTik)) + 1;
                            cuad = new SizeF(lib.CentimeterToPixel(anchTik) - (coli + 10), alfi * (int)xxx);
                            posi = posi + alfi;
                            puntoF = new PointF(coli, posi);
                            recdom = new RectangleF(puntoF, cuad);
                            e.Graphics.DrawString(gDetalle, lt_med, Brushes.Black, recdom, StringFormat.GenericTypographic);
                            posi = posi + alfi;
                        }
                    }
                    // final del comprobante
                    string repre = var[4];      //  "Representación impresa sin valor legal de la";
                    lt = (ancho - e.Graphics.MeasureString(repre, lt_med).Width) / 2;
                    posi = posi + alfi + 10.0F;
                    puntoF = new PointF(lt, posi);
                    e.Graphics.DrawString("", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli, posi);
                    string previo = " M.N. " + decimal.Parse(cab[30]).ToString("#0");     // var[5];
                    //lt = (ancho - e.Graphics.MeasureString(previo, lt_med).Width) / 2;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString(previo, lt_medN, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi * 4;
                    puntoF = new PointF(coli + 60, posi);
                    e.Graphics.DrawString("----------------------------------------", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + 10;
                    puntoF = new PointF(coli + 60, posi);
                    e.Graphics.DrawString("           Recibi Conforme", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi * 2;
                    string locyus = cab[28] + " - " + cab[27];
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString(locyus, lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("Imp. " + DateTime.Now, lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi + alfi;
                    //puntoF = new PointF((lib.CentimeterToPixel(anchTik) - e.Graphics.MeasureString(despedida, lt_med).Width) / 2, posi);
                    puntoF = new PointF((ancho - e.Graphics.MeasureString(var[2], lt_med).Width) / 2, posi);
                    e.Graphics.DrawString(var[2], lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi + alfi;
                    //puntoF = new PointF(coli, posi);
                    //e.Graphics.DrawString(".", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                }
                
            }
        }

        private conClie generaReporte(string cristalito)                                 // formato A5
        {
            conClie guiaT = new conClie();
            conClie.gr_ind_cabRow rowcabeza = guiaT.gr_ind_cab.Newgr_ind_cabRow();
            // CABECERA
            rowcabeza.formatoRPT = cristalito;
            rowcabeza.id = "0";         // no tenemos este dato en la clase
            rowcabeza.estadoser = "";   // no tenemos este dato en la clase
            rowcabeza.sergui = cab[0];
            rowcabeza.numgui = cab[1];
            rowcabeza.numpregui = cab[29];   // numero de la pre-guia, obligatorio en GRE
            rowcabeza.fechope = cab[2];
            rowcabeza.fechTraslado = cab[16];
            // cab[3] = cabecera[3];   // dirección sede de la guía
            rowcabeza.frase1 = "";  // campo para etiqueta "ANULADO"        // no tenemos este dato en la clase
            rowcabeza.frase2 = "";  // campo para etiqueta "TIENE CLAVE"    // no tenemos este dato en la clase
            // origen - destino
            rowcabeza.nomDestino = "";      // no tenemos este dato en la clase
            rowcabeza.direDestino = cab[23];
            rowcabeza.dptoDestino = cab[24];
            rowcabeza.provDestino = cab[25];
            rowcabeza.distDestino = cab[26];
            rowcabeza.nomOrigen = cab[28];      // nombre del local de emisión
            rowcabeza.direOrigen = cab[19];
            rowcabeza.dptoOrigen = cab[20];
            rowcabeza.provOrigen = cab[21];
            rowcabeza.distOrigen = cab[22];
            // remitente
            rowcabeza.docRemit = cab[10];
            rowcabeza.numRemit = cab[11];
            rowcabeza.nomRemit = cab[12];
            rowcabeza.direRemit = cab[19];       // no tenemos este dato en la clase
            rowcabeza.dptoRemit = cab[20];       // no tenemos este dato en la clase
            rowcabeza.provRemit = cab[21];       // no tenemos este dato en la clase
            rowcabeza.distRemit = cab[22];       // no tenemos este dato en la clase
            // destinatario
            rowcabeza.docDestinat = cab[13];
            rowcabeza.numDestinat = cab[14];
            rowcabeza.nomDestinat = cab[15];
            rowcabeza.direDestinat = cab[23];       // no tenemos este dato en la clase
            rowcabeza.distDestinat = cab[26];       // no tenemos este dato en la clase
            rowcabeza.provDestinat = cab[25];       // no tenemos este dato en la clase
            rowcabeza.dptoDestinat = cab[24];       // no tenemos este dato en la clase
            // importes
            rowcabeza.pesTotCar = cab[17];
            rowcabeza.uniMedPes = cab[18];
            rowcabeza.nomMoneda = "";           // no tenemos este dato en la clase - EN GRE no imprimimos valores 
            rowcabeza.igv = "";                 // no tenemos este dato en la clase - EN GRE no imprimimos valores 
            rowcabeza.subtotal = "";            // no tenemos este dato en la clase - EN GRE no imprimimos valores 
            rowcabeza.total = cab[30];          // Flete del servicio (solo para impresión, no va a sunat)
            // documentos origen
            rowcabeza.tipDocRel1 = cab[4];         // Datos relacionados 1: tipo doc origen -> cmb_docorig.Text
            rowcabeza.docscarga = cab[5];
            rowcabeza.rucDocRel1 = cab[6];         // Datos relacionados 1: ruc doc origen -> tx_rucEorig.Text
            rowcabeza.tipDocRel2 = cab[7];         // Datos relacionados 2: tipo doc origen -> tx_dat_docOr2.Text
            rowcabeza.docscarga2 = cab[8];         // Datos relacionados 2: numero doc origen -> tx_docsOr2.Text
            rowcabeza.rucDocRel2 = cab[9];         // Datos relacionados 2: ruc doc origen -> tx_rucEorig2.Text
            // pie
            rowcabeza.marcamodelo = "";         // no tenemos este dato en la clase
            rowcabeza.autoriz = vch[1];
            rowcabeza.dniChoSec = vch[12];        // Choferes - Dni chofer secundario ->
            rowcabeza.brevAyuda = vch[13];
            rowcabeza.nomAyuda = vch[14] + " " + vch[15];
            rowcabeza.dniChoPrin = vch[8];          // Choferes - Dni chofer principal ->
            rowcabeza.brevChofer = vch[9];
            rowcabeza.nomChofer = vch[10] + " " + vch[11];
            rowcabeza.placa = vch[0];
            rowcabeza.regMTCve1 = vch[2];        // Vehiculos - Num Registro MTC -> 
            rowcabeza.camion = vch[4];
            rowcabeza.confvehi = vch[3] + vch[7];
            rowcabeza.autoriz2 = vch[5];        // Vehiculos - Autoriz. vehicular -> 
            rowcabeza.regMTCve2 = vch[6];       // Vehiculos - Num Registro MTC -> 

            rowcabeza.rucPropiet = "";          // no tenemos este dato en la clase
            rowcabeza.nomPropiet = "";          // no tenemos este dato en la clase

            rowcabeza.fechora_imp = DateTime.Now.ToString();
            rowcabeza.userc = cab[27];
            rowcabeza.horEmiCre = "";           // hora de emisión ... para efectos de la impresion en TK y A5 no importa ... 10/10/2023
            // rowcabeza.fecEmiCre = cab[]      // falta en la clase
            // varios
            rowcabeza.varTexoQR = var[0];
            rowcabeza.varTexLibr = @var[1];      // texto del QR en formato byte[]
            rowcabeza.varTexDes1 = var[2];
            rowcabeza.varTexDes2 = var[3];
            rowcabeza.varGloFin1 = var[4];
            rowcabeza.varGloFin2 = var[5];
            rowcabeza.consignat = var[6];
            rowcabeza.telremit = var[7];
            rowcabeza.teldesti = var[8];
            //
            guiaT.gr_ind_cab.Addgr_ind_cabRow(rowcabeza);
            //
            // DETALLE  
            for (int y=0; y<3; y++)
            {
                if (det[y, 0] != "")
                {
                    conClie.gr_ind_detRow rowdetalle = guiaT.gr_ind_det.Newgr_ind_detRow();
                    rowdetalle.id = "0";
                    rowdetalle.fila = det[y, 0];    // dt[y, 0] Num de fila
                    rowdetalle.cant = det[y, 1];    // dt[y, 1] Cant.
                    rowdetalle.codigo = "";         // no estamos usando
                    rowdetalle.umed = det[y, 2];      // dt[y, 2] Unidad de medida
                    rowdetalle.descrip = det[y, 3];   // dt[y, 3] Descripción
                    rowdetalle.precio = "";         // no estamos usando
                    rowdetalle.total = "";          // no estamos usando
                    rowdetalle.peso = det[y, 4];  // dt[y, 4] peso
                    guiaT.gr_ind_det.Addgr_ind_detRow(rowdetalle);
                }
            }
            //
            return guiaT;
        }
    }
    
}
