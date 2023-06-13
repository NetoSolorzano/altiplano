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
                         "", "", "", "", "", "", "", "", "" };      // 9
        string[,] det = new string[3,5] { { "", "", "", "", "" }, { "", "", "", "", "" }, { "", "", "", "", "" } };
        string[] var = { "", "", "", "", "", ""};       // 6
        string[] vch = { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };      // 16
        short copias = 0;
        string otro = "";               // ruta y nombre del png código QR
        public impGRE_T(int nCopias, string nomImp, string[] cabecera, string[,] detalle, string[] varios, string[] vehChof)
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
            var[1] = varios[1];         // 
            var[2] = varios[2];         // despedid1
            var[3] = varios[3];         // despedid2
            var[4] = varios[4];         // Glosa final comprobante 1 -> "Representación impresa sin valor legal de la"
            var[5] = varios[5];         // Glosa final comprobante 2 -> "Guía de Remisión Electrónica de Transportista"

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

            PrintDocument print = new PrintDocument();
            print.PrintPage += new PrintPageEventHandler(imprime_TK);
            print.PrinterSettings.PrinterName = nomImp;
            print.PrinterSettings.Copies = (short)nCopias;
            print.Print();
        }

        public void imprime_TK(object sender, PrintPageEventArgs e)    // object sender, System.Drawing.Printing.PrintPageEventArgs e
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
                Font lt_med = new Font("Arial", 9);                 // normal textos
                Font lt_peq = new Font("Arial", 8);                 // pequeño
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
                    lt = (ancho - e.Graphics.MeasureString(titdoc, lt_gra).Width) / 2;
                    puntoF = new PointF(lt, posi);
                    e.Graphics.DrawString(titdoc, lt_gra, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi + 8;
                    string titnum = "Nro. " + serie + " - " + corre;
                    //lt = (lib.CentimeterToPixel(anchTik) - e.Graphics.MeasureString(titnum, lt_gra).Width) / 2;
                    lt = (ancho - e.Graphics.MeasureString(titnum, lt_gra).Width) / 2;
                    puntoF = new PointF(lt, posi);
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

                    posi = posi + alfi * 7;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("Dom.Fiscal", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    SizeF cuad = new SizeF(lib.CentimeterToPixel(anchTik) - (20), alfi * 2);
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
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Hora Emisión", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 135, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 140, posi);
                    e.Graphics.DrawString(DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString(), lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);

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
                    e.Graphics.DrawString(cab[15], lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
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
                    for (int z=0; z < 3; z++)   // // #fila,a.cantprodi,a.unimedpro,a.descprodi,a.pesoprodi
                    {
                        if (det[z, 4] != "")
                        {
                            puntoF = new PointF(coli + 20, posi);
                            e.Graphics.DrawString(det[z, 4] + " " + ((cab[18] == "K") ? "KGM" : "TNM"),
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
                    posi = posi + alfi;
                    puntoF = new PointF(lt, posi);
                    e.Graphics.DrawString(repre, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli, posi);
                    string previo = var[5];     // "Guía de Remisión Electrónica de Transportista";
                    lt = (ancho - e.Graphics.MeasureString(previo, lt_med).Width) / 2;
                    puntoF = new PointF(lt, posi);
                    e.Graphics.DrawString(previo, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
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
    }
    
}
