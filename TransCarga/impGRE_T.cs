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
        string[] cab = { "", "", "", "", "", "", "", "", "", "", "", "", "" };  // aca aumentar
        string[] det = { "", ""};
        string[] var = { "", ""};
        short copias = 0;
        string otro = "";               // ruta y nombre del png código QR
        public impGRE_T(int nCopias, string nomImp, string[] cabecera, string[] detalle, string[] varios)
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

            det[0] = detalle[0];
            det[1] = detalle[1];

            var[0] = varios[0];     // Varios: texto del código QR ->tx_dat_textoqr.Text
            var[1] = varios[1];     // 

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
                Font lt_med = new Font("Arial", 9);                // normal textos
                Font lt_peq = new Font("Arial", 8);                 // pequeño
                                                                    //
                float anchTik = 7.8F;                               // ancho del TK en centimetros
                int coli = 5;                                      // columna inicial
                float posi = 20;                                    // posicion x,y inicial
                int alfi = 15;                                      // alto de cada fila
                float ancho = 360.0F;                                // ancho de la impresion
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
                    /*
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Destinatario", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 135, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 140, posi);
                    e.Graphics.DrawString(cmb_docDes.Text + " " + tx_numDocDes.Text, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString(tx_nomDrio.Text.Trim(), lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Fecha de Traslado", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 135, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 140, posi);
                    if (tx_pla_fech.Text != "") e.Graphics.DrawString(tx_pla_fech.Text.Substring(6, 4) + "-" + tx_pla_fech.Text.Substring(3, 2) + "-" + tx_pla_fech.Text.Substring(0, 2),
                        lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Peso Bruto", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 135, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 140, posi);
                    if (tx_totpes.Text.Trim() != "" && tx_totpes.Text.Trim() != "0") e.Graphics.DrawString(tx_totpes.Text + " " + ((rb_kg.Checked == true) ? rb_kg.Text : rb_tn.Text),
                        lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Dirección de Partida", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    cuad = new SizeF(lib.CentimeterToPixel(anchTik) - (coli + 20), alfi * 2);
                    recdom = new RectangleF(puntoF, cuad);
                    e.Graphics.DrawString(tx_dirRem.Text.Trim() + " " + tx_dptoRtt.Text.Trim() + " " + tx_provRtt.Text.Trim() + " " + tx_distRtt.Text.Trim(),
                        lt_peq, Brushes.Black, recdom, StringFormat.GenericTypographic);
                    posi = posi + alfi * 2;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Dirección de Llegada", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    cuad = new SizeF(lib.CentimeterToPixel(anchTik) - (coli + 20), alfi * 2);
                    recdom = new RectangleF(puntoF, cuad);
                    e.Graphics.DrawString(tx_dirDrio.Text.Trim() + " " + tx_dptoDrio.Text.Trim() + " " + tx_proDrio.Text.Trim() + " " + tx_disDrio.Text.Trim(),
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
                    if (tx_pla_placa.Text != "") e.Graphics.DrawString(tx_pla_placa.Text, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Autorización", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 135, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 140, posi);
                    if (tx_pla_autor.Text != "") e.Graphics.DrawString(tx_pla_autor.Text, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);

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
                    if (tx_pla_brevet.Text != "") e.Graphics.DrawString(tx_pla_brevet.Text, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Nombre", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    if (tx_pla_nomcho.Text != "") e.Graphics.DrawString(tx_pla_nomcho.Text, lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    // row["numdcho"] = tx_pla_dniChof.Text;                                       // Numero de documento de identidad 

                    // imprimimos los bienes a transportar
                    posi = posi + alfi * 2;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("Bienes a transportar", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString(tx_det_peso.Text + " " + ((rb_kg.Checked == true) ? rb_kg.Text : rb_tn.Text),
                        lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    string gDetalle = lb_glodeta.Text + " " + tx_det_desc.Text;
                    double xxx = (e.Graphics.MeasureString(gDetalle, lt_peq).Width / lib.CentimeterToPixel(anchTik)) + 1;
                    cuad = new SizeF(lib.CentimeterToPixel(anchTik) - (coli + 10), alfi * (int)xxx);
                    posi = posi + alfi;
                    puntoF = new PointF(coli, posi);
                    recdom = new RectangleF(puntoF, cuad);
                    e.Graphics.DrawString(gDetalle, lt_med, Brushes.Black, recdom, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    // final del comprobante
                    string repre = "Representación impresa sin valor legal de la";
                    lt = (ancho - e.Graphics.MeasureString(repre, lt_med).Width) / 2;
                    posi = posi + alfi * 2;
                    puntoF = new PointF(lt, posi);
                    e.Graphics.DrawString(repre, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli, posi);
                    string previo = "Guía de Remisión Electrónica de Transportista";
                    lt = (ancho - e.Graphics.MeasureString(previo, lt_med).Width) / 2;
                    puntoF = new PointF(lt, posi);
                    e.Graphics.DrawString(previo, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi * 2;
                    string locyus = tx_locuser.Text + " - " + tx_user.Text;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString(locyus, lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("Imp. " + DateTime.Now, lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi + alfi;
                    //puntoF = new PointF((lib.CentimeterToPixel(anchTik) - e.Graphics.MeasureString(despedida, lt_med).Width) / 2, posi);
                    puntoF = new PointF((ancho - e.Graphics.MeasureString(despedida, lt_med).Width) / 2, posi);
                    e.Graphics.DrawString(despedida, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi + alfi;
                    //puntoF = new PointF(coli, posi);
                    //e.Graphics.DrawString(".", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    */
                }
                
            }
        }
        //printDocument1.PrinterSettings.PrinterName = v_impTK;
        //printDocument1.PrinterSettings.Copies = 2;      // esto debería estar en una variable
        //printDocument1.Print();
    }
    
}
