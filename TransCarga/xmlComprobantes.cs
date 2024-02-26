using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Diagnostics;

namespace TransCarga
{
    class xmlComprobantes
    {
        // string de conexion
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + login.data + ";";
        public static string CadenaConexion = "Data Source=TransCarga.db";
        //
        string[] vs = {"", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "",     // 20
                       "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "",    // 20
                       "", "", "", "", "", "", "", ""};     // 8
        string[] va = { "", "", "", "", "", "", "", "", "", "", "", "", "", "" };       // 14
        string[,] dt = new string[10, 10] {
            { "", "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "", "" }
        }; // 6 columnas, 10 filas
        string[] cu = { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };    // 29
        string glosdetra = "";

        libreria lib = new libreria();
        NumLetra numLetra = new NumLetra();

        private void jalainfo()                 // obtiene datos de imagenes y variables
        {
            for (int t = 0; t < Program.dt_enlaces.Rows.Count; t++)
            {
                DataRow row = Program.dt_enlaces.Rows[t];
                if (row["formulario"].ToString() == "facelect")
                {
                    if (row["campo"].ToString() == "detraccion" && row["param"].ToString() == "glosa") glosdetra = row["valor"].ToString().Trim();    // glosa detraccion
                }
            }
        }

        public bool CreaTablaLiteDV()                  // llamado en el load del form, crea las tablas al iniciar
        {
            bool retorna = false;
            using (SqliteConnection cnx = new SqliteConnection(CadenaConexion))
            {
                cnx.Open();
                string sqlborra = "DROP TABLE IF EXISTS dt_cabdv; DROP TABLE IF EXISTS dt_detdv; DROP TABLE IF EXISTS dt_docrel";
                using (SqliteCommand cmdB = new SqliteCommand(sqlborra, cnx))
                {
                    cmdB.ExecuteNonQuery();
                }
                string sqlTabla = "create table dt_cabdv (" +
                    // cabecera
                    "id integer primary key autoincrement, " +
                    "EmisRuc varchar(11), " +           // ruc del emisor               - 16
                    "EmisNom varchar(150), " +          // Razón social del emisor      - 15
                    "EmisCom varchar(150), " +          // Nombre Comercial del emisor  - 14
                    "CodLocA varchar(4), " +            // Código local anexo emisor    - 17
                    "EmisUbi varchar(6), " +            // ubigeo del emisor
                    "EmisDir varchar(200), " +
                    "EmisDep varchar(50), " +
                    "EmisPro varchar(50), " +
                    "EmisDis varchar(50), " +
                    "EmisUrb varchar(50), " +           // urbanización, pueblo, localidad
                    "EmisPai varchar(2), " +            // código sunat del país emisor
                    "EmisCor varchar(100), " +          // correo del emisor de la guía
                    "EmisTel varchar(11), " +           // teléfono del emisor
                    "NumDVta varchar(12), " +           // serie+numero
                    "FecEmis varchar(10), " +
                    "HorEmis varchar(8), " +
                    "CodComp varchar(2), " +            // código sunat del comprobante
                    "FecVcto varchar(10), " +           // Fecha de vencimiento del comprobante
                    "TipDocu varchar(2), " +            // SUNAT:Identificador de Tipo de Documento
                    "CodLey1 varchar(4), " +             // codigo sunat de leyenda MONTO EN LETRAS
                    "MonLetr varchar(150), " +           // monto en letras
                    "CodMonS varchar(3)," +              // código internacional de moneda 
                                                         // datos del destinatario
                    "DstTipdoc varchar(2), " +          // código sunat del tipo de documento del destinatario  - 18
                    "DstNumdoc varchar(11), " +         // número del documento del destinatario                - 18
                    "DstNomTdo varchar(50), " +         // glosa, texto o nombre sunat del doc del destinatario
                    "DstNombre varchar(150), " +        // nombre o razón social del destinatario
                    "DstDirecc varchar(200), " +        // dirección del destinatario                           - 20
                    "DstDepart varchar(50), " +
                    "DstProvin varchar(50), " +
                    "DstDistri varchar(50), " +
                    "DstUrbani varchar(50), " +         // urbanización, pueblo, localidad
                    "DstUbigeo varchar(6), " +          // ubigeo de la direc del cliente                       - 20
                    "DstCorre varchar(100), " +         // correo del cliente
                    "DstTelef varchar(11), " +          // teléfono del cliente
                                                        // Información de descuentos Globales               // no usamos dsctos globales 17/06/2023 - 21

                    // Información de importes 
                    "ImpTotImp decimal(12,2), " +       // Monto total de impuestos                             - 22 TaxAmount
                    "ImpOpeGra decimal(12,2), " +       // Monto las operaciones gravadas                       - 23 TaxableAmount
                                                        //"ImpOpeExo decimal(12,2), " +     // Monto las operaciones Exoneradas                     - 24
                                                        //"ImpOpeIna decimal(12,2), " +     // Monto las operaciones inafectas del impuesto         - 25
                                                        //"ImpOpeGra decimal(12,2), " +     // Monto las operaciones gratuitas                      - 26
                    "ImpIgvTot decimal(12,2), " +       // Sumatoria de IGV                                     - 27
                                                        //"ImpISCTot decimal(12,2), " +      // Sumatoria de ISC                                     - 28
                    "ImpOtrosT decimal(12,2), " +       // Sumatoria de Otros Tributos                          - 29
                    "IgvCodSun varchar(1), " +          // schemeAgencyID="6"
                    "IgvConInt varchar(4), " +          // 1000
                    "IgvNomSun varchar(4), " +          // IGV
                    "IgvCodInt varchar(4), " +          // VAT
                    "TotValVta decimal(12,2), " +       // Total valor de venta                                 - 30
                    "TotPreVta decimal(12,2), " +       // Total precio de venta (incluye impuestos)            - 31
                    "TotDestos decimal(12,2), " +       // Monto total de descuentos del comprobante            - 32
                    "TotOtrCar decimal(12,2), " +       // Monto total de otros cargos del comprobante          - 33
                    "TotaVenta decimal(12,2), " +        // Importe total de la venta, cesión en uso o del servicio prestado - 34
                    "CanFilDet integer, " +              // Cantidad filas de detalle
                    "CtaDetra varchar(20), " +           // Cta detracción banco de la nación
                    "PorDetra decimal(5,1), " +          // % de la detracción
                    "ImpDetra decimal(12,2), " +         // Importe de la detracción EN SOLES, la cuenta del BN es el soles
                    "GloDetra varchar(200), " +          // Glosa general de la detracción
                    "CodTipDet varchar(3), " +           // Código sunat tipo de detraccion (027 transporte de carga)
                    "CondPago varchar(10), " +           // Condicion de pago
                    "CodTipDoc varchar(2), " +           // Código sunat para el tipo de documento, FT=01, BV=03, etc
                    "CodTipOpe varchar(4), " +           // Código sunat para el tipo de operación, 0101=Vta, interna facturas y boletas
                    "TipoCamb decimal(8,2), " +          // tipo de cambio
                                                         // ENCABEZADO-TRASLADOBIENES
                    "cu_cpapp varchar(2), " +            // Código país del punto de origen
                    "cu_ubipp varchar(6), " +            // Ubigeo del punto de partida 
                    "cu_deppp varchar(50), " +           // Departamento del punto de partida
                    "cu_propp varchar(50), " +           // Provincia del punto de partida
                    "cu_dispp varchar(50), " +           // Distrito del punto de partida
                    "cu_urbpp varchar(50), " +           // Urbanización del punto de partida
                    "cu_dirpp varchar(200), " +          // Dirección detallada del punto de partida
                    "cu_cppll varchar(2), " +            // Código país del punto de llegada
                    "cu_ubpll varchar(6), " +            // Ubigeo del punto de llegada
                    "cu_depll varchar(50), " +           // Departamento del punto de llegada
                    "cu_prpll varchar(50), " +           // Provincia del punto de llegada
                    "cu_dipll varchar(50), " +           // Distrito del punto de llegada
                    "cu_ddpll varchar(200), " +          // Dirección detallada del punto de llegada
                    "cu_placa varchar(7), " +            // Placa del Vehículo
                    "cu_confv varchar(7), " +            // Configuracion vehicular
                    "cu_coins varchar(15), " +           // Constancia de inscripción del vehículo o certificado de habilitación vehicular
                    "cu_marca varchar(50), " +           // Marca del Vehículo
                    "cu_breve varchar(15), " +           // Nro.de licencia de conducir
                    "cu_ructr varchar(11), " +           // RUC del transportista
                    "cu_nomtr varchar(200), " +          // Razón social del Transportista
                    "cu_modtr varchar(2), " +            // Modalidad de Transporte
                    "cu_pesbr decimal(10,2), " +         // Total Peso Bruto
                    "cu_motra varchar(2), " +            // Código de Motivo de Traslado
                    "cu_fechi varchar(10), " +           // Fecha de Inicio de Traslado
                    "cu_remtc varchar(15), " +           // Registro MTC
                    "cu_nudch varchar(15), " +           // Nro.Documento del conductor
                    "cu_tidch varchar(2), " +            // Tipo de Documento del conductor
                    "cu_plac2 varchar(7), " +            // Placa del Vehículo secundario
                    "cu_insub varchar(2), " +             // Indicador de subcontratación
                    "cu_marCU varchar(1) " +             // "1"=carga unica, "0"=carga normal
                ")";
                using (SqliteCommand cmd = new SqliteCommand(sqlTabla, cnx))
                {
                    cmd.ExecuteNonQuery();
                }
                // ********************* DETALLE ************************ //
                sqlTabla = "create table dt_detdv (" +
                    "id integer primary key autoincrement, " +
                    "NumDVta varchar(12), " +
                    "Numline integer, " +            // Número de orden del Ítem                             - 35
                    "Cantprd integer, " +            // Cantidad y Unidad de medida por ítem                 - 36
                    "CodMone varchar(3), " +         // Codigo internacional de moneda                       - 37
                    "ValVtaI decimal(12,2), " +      // Valor de venta del ítem                              - 37
                    "PreVtaU decimal(12,2), " +     // Precio de venta unitario por item y código           - 38
                                                    // Valor referencial unitario por ítem en operaciones no onerosas   - 39
                                                    // Descuentos por Ítem                                  - 40
                                                    // Cargos por item                                      - 41
                    "ValIgvI decimal(12,2), " +      // Afectación al IGV por ítem                           - 42
                                                     // Afectación al ISC por ítem                           - 43
                    "DesDet1 varchar(100), " +      // Descripción detallada                                - 44
                    "DesDet2 varchar(100), " +
                    "CodIntr varchar(50), " +       // Código de producto                                   - 45
                                                    // Código de producto SUNAT                             - 46
                                                    // Propiedades Adicionales del Ítem                     - 47
                    "ValUnit decimal(12,2), " +     // Valor unitario del ítem                              - 48
                    "ValPeso real, " +              // peso de la carga, va unido a la unidad de medida 
                    "UniMedS varchar(3), " +        // codigo unidad de medida de sunat
                    "GuiaTra varchar(13), " +       // numero guía relacionada
                    "CodTipG varchar(2), " +        // codigo sunat tipo de guía relacionada
                    "PorcIgv varchar(2), " +        // % del igv en números (18)
                    "CodSunI varchar(2), " +        // codigo sunat del igv, (10)
                    "CodSunT varchar(4), " +        // codigo sunat del tributo, (1000)
                    "NomSunI varchar(10), " +       // nombre sunat del impuesto, (IGV)
                    "NomIntI varchar(10), " +       // nombre internacional del impuesto, (VAT)
                    "GuiaRem varchar(50) " +        // guias de remision de la guía transportista
                    ")";
                using (SqliteCommand cmd = new SqliteCommand(sqlTabla, cnx))
                {
                    cmd.ExecuteNonQuery();
                    retorna = true;
                }
            }

            return retorna;
        }

        public bool llenaTablaLiteDV(string tipdo, string serdo, string numdo,
            string[] cabecera, string[,] detalle, string[] varios, string[] cunica)          // llena tabla con los datos del comprobante y llama al app que crea el xml
        {
            bool retorna = false;
            #region tipos de datos en matrices
            /*
            cabecera[0]    // serie (F001)
            cabecera[1]    // numero|
            cabecera[2]    // tx_dat_tdv.Text, siglas del tipo de documento
            cabecera[3]    // direccion emisor
            cabecera[4]    // nombre del tipo de documento
            cabecera[5]    // fecha de emision formato dd/mm/aaaa
            cabecera[6]    // tx_nomRem.Text -> nombre del cliente del comprobante
            cabecera[7]    // tx_numDocRem.Text -> numero documento del cliente
            cabecera[8]      // tx_dirRem.Text -> dirección cliente
            cabecera[9]      // distrito de la direccion
            cabecera[10]    // provincia de la direccion
            cabecera[11]    // departamento de la dirección
            cabecera[12]    // cantidad de filas de detalle
            cabecera[13]    // tx_subt.Text -> Sub total del comprobante
            cabecera[14]    // igv del comprobante
            cabecera[15]    // importe total del comprobante
            cabecera[16]    // cmb_mon.Text -> Simbolo de la moneda
            cabecera[17]    // tx_fletLetras.Text
            cabecera[18]    // CONTADO o CREDITO
            cabecera[19]    // tx_dat_dpla.Text -> dias de plazo credito
            cabecera[20]    // glosdetra -> Glosa para la detracción
            cabecera[21]    // codigo sunat tipo comprobante
            cabecera[22]    // tipoDocEmi -> CODIGO SUNAT tipo de documento RUC/DNI del cliente
            cabecera[23]    // provee => "factDirecta"
            cabecera[24]    // restexto -> texto del resolucion sunat del ose/pse
            cabecera[25]    // autoriz_OSE_PSE -> autoriz del ose/pse
            cabecera[26]    // webose -> web del ose/pse
            cabecera[27]      // usuario creador
            cabecera[28]      // local de emisión
            cabecera[29]      // glosa despedida
            cabecera[30]     // nombre del emisor del comprobante
            cabecera[31]     // ruc del emisor
            cabecera[32]     // fecha vencimiento del comprob.
            cabecera[33]     // forma de pago incluyendo # de cuotas (siempre es 1 cuota en Transcarga)
            cabecera[34]     // modalidad de transporte
            cabecera[35]     // motivo de traslado
            cabecera[36]     // nombre de la moneda
            cabecera[37]     // tot operaciones inafectas
            cabecera[38]     // tot operaciones exoneradas
            cabecera[39] 
            cabecera[40]     // codigo sunat de la moneda
            cabecera[41]     // codigo ubigeo de la dire del cliente
            cabecera[42]     // monto valor del flete en moneda nacional 
            cabecera[43]     // codigo tipo operaion sunat 0101 o 1004 
            cabecera[44]     // tipo de cambio de la operacion
            cabecera[45]     // codigo de la moneda del comprobante
            cabecera[46]     // correo electronico del cliente
            cabecera[47]     // telefono del cliente

            cunica[0]          // "placa");
            cunica[1]          // "confv");
            cunica[2]          // "autoriz");
            cunica[3]           // "cargaEf");
            cunica[4]           // "cargaUt");
            cunica[5]           // "rucTrans");
            cunica[6]           // "nomTrans");
            cunica[7]           // "fecIniTras");
            cunica[8]          // "dirPartida");
            cunica[9]           // "ubiPartida");
            cunica[10]         // "dirDestin");
            cunica[11]         // "ubiDestin");
            cunica[12]         // "dniChof");
            cunica[13]         // "brevete");
            cunica[14]         // "valRefViaje");
            cunica[15]         // "valRefVehic");
            cunica[16]         // "valRefTon");
            cunica[17]          // depart punto de partida
            cunica[18]          // provin punto de partida
            cunica[19]          // distrit punto de partida
            cunica[20]          // depart punto de llegada
            cunica[21]          // provin punto de llegada
            cunica[22]          // distrit punto de llegada
            cunica[23]         // Modalidad de Transporte
            cunica[24]         // Total Peso Bruto    
            cunica[25]          // Código de Motivo de Traslado 
            cunica[26]          // registro MTC del transportista
            cunica[27]         // Tipo de Documento del conductor
            cunica[28]         // marca de carga unica true=si || false=no

            for (int o=0; o <= int.Parse(cabecera[12]); o++)
            {
                detalle[o, 0];   // detalle fila o - dataGridView1.Rows[l].Cells["OriDest"]
                detalle[o, 1];   // dataGridView1.Rows[l].Cells["Cant"]
                detalle[o, 2];   // dataGridView1.Rows[l].Cells["umed"]
                detalle[o, 3];   // guia transportista
                detalle[o, 4];   // descripcion de la carga
                detalle[o, 5];   // documento relacionado remitente de la guia transportista
                detalle[o, 6];   // valor unitario
                detalle[o, 7];   // precio unitario
                detalle[o, 8];   // total
                detalle[o, 9];   // codigo moneda de la fila
            }

            varios[0];         // Ruta y nombre del logo del emisor electrónico
            varios[1];         // glosa del servicio en facturacion
            varios[2];         // Código Transcarga del tipo de documento Factura 
            varios[3];         // porcentaje detracción
            varios[4];         // monto detracción
            varios[5];         // cta. detracción
            varios[6];         // concatenado de Guias Transportista para Formato de cargas unicas
            varios[7];         // ruta y nombre del png codigo QR
            varios[8];         // 
            varios[9]          // moneda por defecto MN del sistema
            varios[10]         // valor igv en procentaje 
            varios[11]         // rutaxml 
            varios[12]         // rutaCertifc
            varios[13]         // claveCertif
            */
            #endregion

            if (true)   // tipdo == null || tipdo == ""
            {
                using (SqliteConnection cnx = new SqliteConnection(CadenaConexion))
                {
                    string fecemi = cabecera[5].Substring(6, 4) + "-" + cabecera[5].Substring(3, 2) + "-" + cabecera[5].Substring(0, 2);
                    string fansi = DateTime.Parse(fecemi).AddDays(double.Parse((cabecera[19] == "") ? "0" : cabecera[19])).Date.ToString("yyyy-MM-dd");        // fecha de emision + dias plazo credito 
                    string cdvta = cabecera[2].Substring(0, 1) + lib.Right(cabecera[0], 3) + "-" + cabecera[1];

                    cnx.Open();
                    using (SqliteCommand cmd = new SqliteCommand("delete from dt_cabdv where id>0", cnx))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    using (SqliteCommand cmd = new SqliteCommand("delete from dt_detdv where id>0", cnx))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    // CABECERA
                    string metela = "insert into dt_cabdv (" +
                        "EmisRuc,EmisNom,EmisCom,CodLocA,EmisUbi,EmisDir,EmisDep,EmisPro,EmisDis,EmisUrb,EmisPai,EmisCor,NumDVta,FecEmis,HorEmis,CodComp,FecVcto," +
                        "TipDocu,CodLey1,MonLetr,CodMonS,DstTipdoc,DstNumdoc,DstNomTdo,DstNombre,DstDirecc,DstDepart,DstProvin,DstDistri,DstUrbani,DstUbigeo,ImpTotImp," +
                        "ImpOpeGra,ImpIgvTot,ImpOtrosT,IgvCodSun,IgvConInt,IgvNomSun,IgvCodInt,TotValVta,TotPreVta,TotDestos,TotOtrCar,TotaVenta," +
                        "CanFilDet,CtaDetra,PorDetra,ImpDetra,GloDetra,CodTipDet,CondPago,CodTipOpe,TipoCamb," +
                        "cu_cpapp,cu_ubipp,cu_deppp,cu_propp,cu_dispp,cu_urbpp,cu_dirpp,cu_cppll,cu_ubpll,cu_depll,cu_prpll,cu_dipll,cu_ddpll,cu_confv," +
                        "cu_placa,cu_coins,cu_marca,cu_breve,cu_ructr,cu_nomtr,cu_modtr,cu_pesbr,cu_motra,cu_fechi,cu_remtc,cu_nudch,cu_tidch,cu_plac2,cu_insub,cu_marCU) " +
                        "values (" +
                        "@EmisRuc,@EmisNom,@EmisCom,@CodLocA,@EmisUbi,@EmisDir,@EmisDep,@EmisPro,@EmisDis,@EmisUrb,@EmisPai,@EmisCor,@NumDVta,@FecEmis,@HorEmis,@CodComp,@FecVcto," +
                        "@TipDocu,@CodLey1,@MonLetr,@CodMonS,@DstTipd,@DstNumd,@DstNomT,@DstNomb,@DstDire,@DstDepa,@DstProv,@DstDist,@DstUrba,@DstUbig,@ImpTotI," +
                        "@ImpOpeG,@ImpIgvT,@ImpOtro,@IgvCodS,@IgvConI,@IgvNomS,@IgvCodI,@TotValV,@TotPreV,@TotDest,@TotOtrC,@TotaVen," +
                        "@CanFilD,@CtaDetr,@PorDetr,@ImpDetr,@GloDetr,@CodTipD,@CondPag,@CodTipO,@TipoCam," +
                        "@cu_cpapp,@cu_ubipp,@cu_deppp,@cu_propp,@cu_dispp,@cu_urbpp,@cu_dirpp,@cu_cppll,@cu_ubpll,@cu_depll,@cu_prpll,@cu_dipll,@cu_ddpll,@cu_confv," +
                        "@cu_placa,@cu_coins,@cu_marca,@cu_breve,@cu_ructr,@cu_nomtr,@cu_modtr,@cu_pesbr,@cu_motra,@cu_fechi,@cu_remtc,@cu_nudch,@cu_tidch,@cu_plac2,@cu_insub,@cu_marCU)";
                    using (SqliteCommand cmd = new SqliteCommand(metela, cnx))
                    {
                        // cabecera
                        cmd.Parameters.AddWithValue("@EmisRuc", Program.ruc);                 // "20430100344"
                        cmd.Parameters.AddWithValue("@EmisNom", Program.cliente);             // "J&L Technology SAC"
                        cmd.Parameters.AddWithValue("@EmisCom", "");                          // nombre comercial
                        cmd.Parameters.AddWithValue("@CodLocA", Program.codlocsunat);         // codigo sunat local anexo emisor
                        cmd.Parameters.AddWithValue("@EmisUbi", Program.ubidirfis);           // "070101"
                        cmd.Parameters.AddWithValue("@EmisDir", Program.dirfisc);             // "Calle Sigma Mz.A19 Lt.16 Sector I"
                        cmd.Parameters.AddWithValue("@EmisDep", Program.depfisc);             // "Callao"
                        cmd.Parameters.AddWithValue("@EmisPro", Program.provfis);             // "Callao"
                        cmd.Parameters.AddWithValue("@EmisDis", Program.distfis);             // "Callao"
                        cmd.Parameters.AddWithValue("@EmisUrb", "-");                         // "Bocanegra"
                        cmd.Parameters.AddWithValue("@EmisPai", "PE");                        // país del emisor
                        cmd.Parameters.AddWithValue("@EmisCor", Program.mailclte);            // "neto.solorzano@solorsoft.com"
                        cmd.Parameters.AddWithValue("@NumDVta", cdvta);         // "V001-98000006"
                        cmd.Parameters.AddWithValue("@FecEmis", fecemi);              // "2023-05-19"
                        cmd.Parameters.AddWithValue("@HorEmis", DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second);  // "12:21:13"
                        cmd.Parameters.AddWithValue("@CodComp", "");                      // codigo del comprobante
                        cmd.Parameters.AddWithValue("@FecVcto", fansi);

                        cmd.Parameters.AddWithValue("@TipDocu", tipdo);             // SUNAT:Identificador de Tipo de Documento
                        cmd.Parameters.AddWithValue("@CodLey1", "1000");
                        cmd.Parameters.AddWithValue("@MonLetr", "SON: " + cabecera[17]);  
                        cmd.Parameters.AddWithValue("@CodMonS", cabecera[40]);
                        cmd.Parameters.AddWithValue("@DstTipd", cabecera[22]);
                        cmd.Parameters.AddWithValue("@DstNumd", cabecera[7]);
                        cmd.Parameters.AddWithValue("@DstNomT", "");                // glosa, texto o nombre sunat del doc del destinatario
                        cmd.Parameters.AddWithValue("@DstNomb", cabecera[6]);       // "<![CDATA[" + tx_nomRem.Text + "]]>"  ... no funca
                        cmd.Parameters.AddWithValue("@DstDire", cabecera[8]);       // "<![CDATA[" + tx_dirRem.Text + "]]>"
                        cmd.Parameters.AddWithValue("@DstDepa", cabecera[11]);
                        cmd.Parameters.AddWithValue("@DstProv", cabecera[10]);
                        cmd.Parameters.AddWithValue("@DstDist", cabecera[9]);
                        cmd.Parameters.AddWithValue("@DstUrba", "");
                        cmd.Parameters.AddWithValue("@DstUbig", cabecera[41]);     // codigo ubigeo de la dire del cliente
                        cmd.Parameters.AddWithValue("@ImpTotI", cabecera[14]);       // Monto total de impuestos

                        cmd.Parameters.AddWithValue("@ImpOpeG", cabecera[13]);      // Monto las operaciones gravadas
                        cmd.Parameters.AddWithValue("@ImpIgvT", cabecera[14]);       // Sumatoria de IGV
                        cmd.Parameters.AddWithValue("@ImpOtro", "0");               // Sumatoria de Otros Tributos
                        cmd.Parameters.AddWithValue("@IgvCodS", "6");               // schemeAgencyID="6"
                        cmd.Parameters.AddWithValue("@IgvConI", "1000");            // 1000
                        cmd.Parameters.AddWithValue("@IgvNomS", "IGV");             // IGV
                        cmd.Parameters.AddWithValue("@IgvCodI", "VAT");             // VAT
                        cmd.Parameters.AddWithValue("@TotValV", cabecera[13]);      // Total valor de venta
                        cmd.Parameters.AddWithValue("@TotPreV", cabecera[15]);     // Total precio de venta (incluye impuestos)
                        cmd.Parameters.AddWithValue("@TotDest", "0");
                        cmd.Parameters.AddWithValue("@TotOtrC", "0");
                        cmd.Parameters.AddWithValue("@TotaVen", cabecera[15]);
                        string detrac = "no";
                        double vtotdet = 0;
                        if (decimal.Parse(cabecera[42]) > decimal.Parse(Program.valdetra))
                        {
                            detrac = "si";
                            vtotdet = Math.Round(double.Parse(cabecera[42]) * double.Parse(Program.pordetra) / 100, 2);    // totalDetraccion 
                        }
                        cmd.Parameters.AddWithValue("@CanFilD", cabecera[12]);
                        cmd.Parameters.AddWithValue("@CtaDetr", (detrac == "si") ? Program.ctadetra : "");
                        cmd.Parameters.AddWithValue("@PorDetr", (detrac == "si") ? Program.pordetra : "");
                        cmd.Parameters.AddWithValue("@ImpDetr", (detrac == "si") ? vtotdet : 0);
                        cmd.Parameters.AddWithValue("@GloDetr", (detrac == "si") ? cabecera[20] + " " + Program.ctadetra : ""); // glosdetra + " " + Program.ctadetra : ""
                        cmd.Parameters.AddWithValue("@CodTipD", (detrac == "si") ? Program.coddetra : "");
                        cmd.Parameters.AddWithValue("@CondPag", (cabecera[18] == "CONTADO" || cabecera[18] == "Contado") ? "Contado" : "Credito");
                        cmd.Parameters.AddWithValue("@CodTipO", cabecera[43]);          // 0101=venta interna, 1001=vta interna sujeta a detracción, 1004=Op. Sujeta a Detracción - Servicios de Transporte Carga
                        cmd.Parameters.AddWithValue("@TipoCam", cabecera[44]);          // Tipo de cambio 
                        cmd.Parameters.AddWithValue("@cu_cpapp", "PE");                 // Código país del punto de origen
                        cmd.Parameters.AddWithValue("@cu_ubipp", cunica[9]);           // Ubigeo del punto de partida 
                        cmd.Parameters.AddWithValue("@cu_deppp", cunica[17]);         // Departamento del punto de partida
                        cmd.Parameters.AddWithValue("@cu_propp", cunica[18]);         // Provincia del punto de partida 
                        cmd.Parameters.AddWithValue("@cu_dispp", cunica[19]);         // Distrito del punto de partida
                        cmd.Parameters.AddWithValue("@cu_urbpp", "");                // Urbanización del punto de partida
                        cmd.Parameters.AddWithValue("@cu_dirpp", cunica[8]);         // Dirección detallada del punto de partida
                        cmd.Parameters.AddWithValue("@cu_cppll", "PE");              // Código país del punto de llegada
                        cmd.Parameters.AddWithValue("@cu_ubpll", cunica[11]);         // Ubigeo del punto de llegada
                        cmd.Parameters.AddWithValue("@cu_depll", cunica[20]);         // Departamento del punto de llegada
                        cmd.Parameters.AddWithValue("@cu_prpll", cunica[21]);         // Provincia del punto de llegada
                        cmd.Parameters.AddWithValue("@cu_dipll", cunica[22]);         // Distrito del punto de llegada
                        cmd.Parameters.AddWithValue("@cu_ddpll", cunica[10]);         // Dirección detallada del punto de llegada
                        cmd.Parameters.AddWithValue("@cu_placa", cunica[0]);         // Placa del Vehículo
                        cmd.Parameters.AddWithValue("@cu_confv", cunica[1]);         // configuración vehicular
                        cmd.Parameters.AddWithValue("@cu_coins", cunica[2]);         // Constancia de inscripción del vehículo o certificado de habilitación vehicular
                        cmd.Parameters.AddWithValue("@cu_marca", "");                // Marca del Vehículo  
                        cmd.Parameters.AddWithValue("@cu_breve", "");                // Nro.de licencia de conducir
                        cmd.Parameters.AddWithValue("@cu_ructr", cunica[5]);         // RUC del transportista
                        cmd.Parameters.AddWithValue("@cu_nomtr", cunica[6]);         // Razón social del Transportista
                        cmd.Parameters.AddWithValue("@cu_modtr", cunica[23]);         // Modalidad de Transporte
                        cmd.Parameters.AddWithValue("@cu_pesbr", cunica[24]);         // Total Peso Bruto    02
                        cmd.Parameters.AddWithValue("@cu_motra", cunica[25]);          // Código de Motivo de Traslado
                        cmd.Parameters.AddWithValue("@cu_fechi", cunica[7]);         // Fecha de Inicio de Traslado 
                        cmd.Parameters.AddWithValue("@cu_remtc", cunica[26]);         // Registro MTC
                        cmd.Parameters.AddWithValue("@cu_nudch", cunica[12]);         // Nro.Documento del conductor 
                        cmd.Parameters.AddWithValue("@cu_tidch", cunica[27]);         // Tipo de Documento del conductor
                        cmd.Parameters.AddWithValue("@cu_plac2", "");         // Placa del Vehículo secundario
                        cmd.Parameters.AddWithValue("@cu_insub", (cunica[5] != Program.ruc) ? "true" : "false");         // Indicador de subcontratación (true/false)
                        if (cunica[28] == "true")
                        {
                            cmd.Parameters.AddWithValue("@cu_marCU", "1");          // 1=carga unica, 0=carga normal
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@cu_marCU", "0");          // 1=carga unica, 0=carga normal
                        }
                        cmd.ExecuteNonQuery();
                    }
                    // DETALLE
                    for (int i = 0; i < int.Parse(cabecera[12]); i++)
                    {
                        string glosser2 = "";       // detalle de la linea
                        string descrip = "";        // descripcion del la linea
                        double preunit = 0;         // precio unitario de la linea
                        double valunit = 0;         // valor sin igv de la linea
                        double sumimpl = 0;         // igv de la fila

                        glosser2 = detalle[i, 0] + " - " +
                            detalle[i, 1] + " " +
                            detalle[i, 2] + " " + detalle[i, 5];
                        descrip = detalle[i, 4];

                        if (cabecera[45] == varios[9])
                        {
                            preunit = double.Parse(detalle[i, 7]);
                            valunit = double.Parse(detalle[i, 6]);
                            sumimpl = preunit - valunit;
                        }
                        else
                        {
                            if (detalle[i, 9] != varios[9]) // si la moneda de la fila es <> soles y la moneda del comprobante tambien es <> soles
                            {
                                preunit = double.Parse(detalle[i, 7]);
                                valunit = double.Parse(detalle[i, 6]);
                                sumimpl = preunit - valunit;
                            }
                            else
                            {   // la moneda de la fila = soles y la moneda del comprobante es <> soles ==> hay que convertirlo a dolares
                                preunit = Math.Round(double.Parse(detalle[i, 7]) / double.Parse(cabecera[45]), 2);
                                valunit = Math.Round(preunit / (1 + (double.Parse(varios[10]) / 100)), 2); // double.Parse(dataGridView1.Rows[i].Cells[4].Value.ToString()) / (1 + (double.Parse(v_igv) / 100));
                                sumimpl = Math.Round(preunit - valunit, 2); // double.Parse(dataGridView1.Rows[i].Cells[4].Value.ToString()) - valunit;
                            }
                        }
                        metela = "insert into dt_detdv (" +
                            "NumDVta,Numline,Cantprd,CodMone,ValVtaI,PreVtaU,ValIgvI,DesDet1,DesDet2,CodIntr,ValUnit,ValPeso,UniMedS," +
                            "GuiaTra,CodTipG,PorcIgv,CodSunI,CodSunT,NomSunI,NomIntI,GuiaRem) values (" +
                            "@NumGu,@Numli,@Cantp,@CodMo,@ValVt,@PreVt,@ValIg,@DesD1,@DesD2,@CodIn,@ValUn,@ValPe,@UniMe," +
                            "@GuiaT,@CodTG,@PIgvn,@CodSI,@CodST,@NomSI,@NomII,@GuiaR)";
                        using (SqliteCommand cmd = new SqliteCommand(metela, cnx))
                        {
                            cmd.Parameters.AddWithValue("@NumGu", cdvta);      // "V001-98000006"
                            cmd.Parameters.AddWithValue("@Numli", i + 1.ToString());
                            cmd.Parameters.AddWithValue("@Cantp", "1");    // dataGridView1.Rows[i].Cells[2].Value.ToString()
                            cmd.Parameters.AddWithValue("@CodMo", cabecera[40]);
                            cmd.Parameters.AddWithValue("@ValVt", valunit.ToString());  // valor venta  s/igv
                            cmd.Parameters.AddWithValue("@PreVt", preunit.ToString());  // precio venta c/igv
                            cmd.Parameters.AddWithValue("@ValIg", sumimpl.ToString());  // Afectación al IGV por ítem
                            cmd.Parameters.AddWithValue("@DesD1", varios[1] + " " + glosser2 + " " + descrip);             // "Servicio de Transporte de carga terrestre "
                            cmd.Parameters.AddWithValue("@DesD2", "");                  //"Dice contener Enseres domésticos"
                            cmd.Parameters.AddWithValue("@CodIn", "");                  // código del item
                            cmd.Parameters.AddWithValue("@ValUn", valunit.ToString());  // Valor unitario del ítem
                            cmd.Parameters.AddWithValue("@ValPe", "");                  // peso
                            cmd.Parameters.AddWithValue("@UniMe", "ZZ");    // dataGridView1.Rows[i].Cells[13].Value.ToString()
                            cmd.Parameters.AddWithValue("@GuiaT", detalle[i, 3]);     // serie(4)-numero(8)
                            cmd.Parameters.AddWithValue("@CodTG", "31");
                            cmd.Parameters.AddWithValue("@PIgvn", varios[10]);
                            cmd.Parameters.AddWithValue("@CodSI", "10");                // Código de tipo de afectación del IGV
                            cmd.Parameters.AddWithValue("@CodST", "1000");              // codigo sunat del tributo, (1000)
                            cmd.Parameters.AddWithValue("@NomSI", "IGV");               // nombre sunat del impuesto
                            cmd.Parameters.AddWithValue("@NomII", "VAT");               // nombre internacional del impuesto
                            cmd.Parameters.AddWithValue("@GuiaR", detalle[i, 5]);       // guias remitente de cada guía transportista
                            cmd.ExecuteNonQuery();
                        }
                    }
                    // llamada al programa de generación del xml del comprobante - F002-00010014
                    if (llamaXmlDocVta(varios, cdvta, tipdo) == true) retorna = true;
                    else 
                    {
                        MessageBox.Show("Error en generar el XML","Error Interno",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        retorna = false; 
                    }
                }
            }

            return retorna;
        }

        public bool llenaTablaLite(string tipdo, string serdo, string numdo, string[] var)  // codigo_tipo_comprobante, serie, numero, valorigv
        {
            bool retorna = false;
            string tipdoS = "";
            jalainfo();

            string consulta = "select a.id,DATE_FORMAT(a.fechope,'%d/%m/%Y') as fechope,a.martdve,a.tipdvta,a.serdvta,a.numdvta,a.ticltgr,a.tidoclt,a.nudoclt,a.nombclt,a.direclt,a.dptoclt,a.provclt,a.distclt,a.ubigclt,a.corrclt,a.teleclt," +
                        "a.locorig,a.dirorig,a.ubiorig,a.obsdvta,a.canfidt,a.canbudt,a.mondvta,a.tcadvta,a.subtota,a.igvtota,a.porcigv,a.totdvta,a.totpags,a.saldvta,a.estdvta,a.frase01,a.impreso," +
                        "a.tipoclt,a.m1clien,a.tippago,a.ferecep,a.userc,a.fechc,a.userm,a.fechm,b.descrizionerid as nomest,'' as cobra,a.idcaja,a.plazocred,a.totdvMN," +
                        "a.cargaunica,a.porcendscto,a.valordscto,a.conPago,a.pagauto,ifnull(ad.placa,'') as placa,ifnull(ad.confv,'') as confv,ifnull(ad.autoriz,'') as autoriz,ifnull(dd.codsunat,'') as tidocltS," +
                        "ifnull(ad.cargaEf,0) as cargaEf,ifnull(ad.cargaUt,0) as cargaUt,ifnull(ad.rucTrans,'') as rucTrans,ifnull(ad.nomTrans,'') as nomTrans,ifnull(date_format(ad.fecIniTras,'%Y-%m-%d'),'') as fecIniTras," +
                        "ifnull(ad.dirPartida,'') as dirPartida,ifnull(ad.ubiPartida,'') as ubiPartida,ifnull(ad.dirDestin,'') as dirDestin,ifnull(ad.ubiDestin,'') as ubiDestin,ifnull(ad.dniChof,'') as dniChof," +
                        "ifnull(ad.brevete,'') as brevete,ifnull(ad.valRefViaje,0) as valRefViaje,ifnull(ad.valRefVehic,0) as valRefVehic,ifnull(ad.valRefTon,0) as valRefTon,dv.codsunat,m.deta1,m.codsunat," +
                        "a.detPeso,ifnull(if(ifnull(v.numreg1,bf.referen2)='',bf.referen2,v.numreg1),bf.referen2) as regmtc,ifnull(tp.marca1,'0') as marca1 " +
                        "from cabfactu a " +
                        "left join adifactu ad on ad.idc=a.id and ad.tipoAd=1 " +
                        "left join desc_est b on b.idcodice=a.estdvta " +
                        "left join desc_doc dd on dd.idcodice=a.tidoclt " +
                        "left join desc_tdv dv on dv.idcodice=a.tipdvta " +
                        "left join desc_mon m on m.idcodice=a.mondvta " +
                        "left join vehiculos v on v.placa=ad.placa " +
                        "left join desc_tpa  tp on tp.idcodice=a.plazocred " +
                        "inner join baseconf bf " +
                        "where a.tipdvta=@tdv and a.serdvta=@ser and a.numdvta=@num";
                        //"left join cabcobran c on c.tipdoco=a.tipdvta and c.serdoco=a.serdvta and c.numdoco=a.numdvta and c.estdcob<>@coda " +

            string jalad = "select a.filadet,a.codgror,a.cantbul,d.unimedpro,a.descpro,a.pesogro,a.codmogr,a.totalgr," +
                "g.totgrMN,g.codMN,g.fechopegr,g.docsremit,g.tipmongri,concat(lo.descrizionerid,' - ',ld.descrizionerid) as orides," +
                "b.porcendscto,b.valordscto,d.unimedpro,b.tipdvta,b.serdvta,b.numdvta " +
                "from detfactu a left join cabguiai g on concat(g.sergui,'-',g.numgui)=a.codgror " +
                "left join detguiai d on d.idc=g.id " +
                "left join desc_loc lo on lo.idcodice=g.locorigen " +
                "left join desc_loc ld on ld.idcodice=g.locdestin " +
                "left join cabfactu b on b.id=a.idc " +
                "where b.tipdvta=@tdv and b.serdvta=@ser and b.numdvta=@num";

            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                if (conn.State ==  System.Data.ConnectionState.Open)
                {
                    using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                    {
                        micon.Parameters.AddWithValue("@tdv", tipdo);
                        micon.Parameters.AddWithValue("@ser", serdo);
                        micon.Parameters.AddWithValue("@num", numdo);
                        using (MySqlDataReader dr = micon.ExecuteReader())
                        {
                            if (dr != null)
                            {
                                if (dr.Read())
                                {
                                    vs[0] = dr.GetString("serdvta");
                                    vs[1] = dr.GetString("numdvta");
                                    vs[2] = dr.GetString("martdve");    // tipdvta

                                    vs[5] = dr.GetString("fechope").Substring(0, 10);
                                    //rb_remGR.Checked = (dr.GetString("ticltgr") == "1") ? true : false;
                                    //rb_desGR.Checked = (dr.GetString("ticltgr") == "2") ? true : false;
                                    //rb_otro.Checked = (dr.GetString("ticltgr") == "3") ? true : false;
                                    vs[6] = dr.GetString("nombclt");
                                    vs[7] = dr.GetString("nudoclt");
                                    vs[8] = dr.GetString("direclt");
                                    vs[9] = dr.GetString("distclt");
                                    vs[10] = dr.GetString("provclt");
                                    vs[11] = dr.GetString("dptoclt");
                                    vs[12] = dr.GetString("canfidt");
                                    vs[13] = Math.Round(dr.GetDecimal("subtota"), 2).ToString();
                                    vs[14] = Math.Round(dr.GetDecimal("igvtota"), 2).ToString();
                                    //,,,porcigv
                                    vs[15] = Math.Round(dr.GetDecimal("totdvta"), 2).ToString();           // total inc. igv
                                    vs[17] = numLetra.Convertir(vs[15], true) + dr.GetString("deta1");
                                    if (dr.GetString("conPago") != "")
                                    {
                                        if (dr.GetString("conPago") == "0") vs[18] = "CONTADO";
                                        if (dr.GetString("conPago") == "1") vs[18] = "CREDITO";
                                    }
                                    vs[19] = dr.GetString("marca1");
                                    vs[20] = glosdetra;
                                    vs[22] = dr.GetString("tidocltS");

                                    if (dr.GetString("userm") == "") vs[27] = lib.nomuser(dr.GetString("userc"));
                                    else vs[27] = lib.nomuser(dr.GetString("userm"));
                                    vs[40] = dr.GetString("codsunat");
                                    vs[41] = dr.GetString("ubigclt");
                                    vs[42] = Math.Round(dr.GetDecimal("totdvMN"), 2).ToString();
                                    string vtotdet = "0";
                                    vs[43] = "0101";
                                    if (decimal.Parse(vs[42]) > decimal.Parse(Program.valdetra))
                                    {
                                        vs[43] = "1004";
                                        vtotdet = Math.Round(double.Parse(vs[42]) * double.Parse(Program.pordetra) / 100, 2).ToString();    // totalDetraccion 
                                    }
                                    vs[44] = dr.GetString("tcadvta");
                                    vs[45] = dr.GetString("mondvta");
                                    vs[46] = dr.GetString("corrclt");
                                    vs[47] = dr.GetString("teleclt");
                                    //locorig,dirorig,ubiorig
                                    //tx_obser1.Text = dr.GetString("obsdvta");
                                    //tx_totcant.Text = dr.GetString("canbudt");  // total bultos
                                    //tx_pagado.Text = dr.GetString("totpags");
                                    //tx_salxcob.Text = dr.GetString("saldvta");
                                    //tx_dat_estad.Text = dr.GetString("estdvta");        // estado
                                    //tx_dat_tcr.Text = dr.GetString("tipoclt");          // tipo de cliente credito o contado
                                    //tx_dat_m1clte.Text = dr.GetString("m1clien");
                                    //tx_impreso.Text = dr.GetString("impreso");
                                    //tx_idcob.Text = dr.GetString("cobra");              // id de cobranza
                                    //tx_estado.Text = dr.GetString("nomest");   // lib.nomstat(tx_dat_estad.Text);
                                    //tx_valdscto.Text = dr.GetString("valordscto");
                                    //tx_dat_porcDscto.Text = dr.GetString("porcendscto");
                                    //tx_dat_plazo.Text = dr.GetString("plazocred");
                                    // campos de carga unica
                                    // a.placa,a.confveh,a.autoriz,a.detPeso,a.detputil,a.detMon1,a.detMon2,a.detMon3,a.dirporig,a.ubiporig,a.dirpdest,a.ubipdest,
                                    // ad.placa,ad.confv,ad.autoriz,ad.cargaEf,ad.cargaUt,ad.rucTrans,ad.nomTrans,ad.fecIniTras,ad.dirPartida,ad.ubiPartida,ad.dirDestin,ad.ubiDestin,ad.dniChof,ad.brevete,ad.valRefViaje,ad.valRefVehic,ad.valRefTon "
                                    if (true)       // dr.GetInt16("cargaunica") == 1  ... 16/02/2024
                                    {
                                        cu[0] = dr.GetString("placa");
                                        cu[1] = dr.GetString("confv");
                                        cu[2] = dr.GetString("autoriz");
                                        cu[3] = dr.GetString("cargaEf");
                                        cu[4] = dr.GetString("cargaUt");
                                        cu[5] = dr.GetString("rucTrans");
                                        cu[6] = dr.GetString("nomTrans");
                                        cu[7] = dr.GetString("fecIniTras");
                                        cu[8] = dr.GetString("dirPartida");
                                        cu[9] = dr.GetString("ubiPartida");
                                        cu[10] = dr.GetString("dirDestin");
                                        cu[11] = dr.GetString("ubiDestin");
                                        cu[12] = dr.GetString("dniChof");
                                        // brevete
                                        cu[14] = dr.GetString("valRefViaje");
                                        cu[15] = dr.GetString("valRefVehic");
                                        cu[16] = dr.GetString("valRefTon");
                                        if (dr.GetInt16("cargaunica") == 1) cu[28] = "true";
                                        string[] retub = lib.retDPDubigeo(cu[9]);
                                        cu[17] = retub[0];
                                        cu[18] = retub[1];
                                        cu[19] = retub[2];
                                        string[] retud = lib.retDPDubigeo(cu[11]);
                                        cu[20] = retud[0];
                                        cu[21] = retud[1];
                                        cu[22] = retud[2];
                                        cu[23] = "02";      // codigo modalidad de transporte .. esto deberia grabarse en la tabla adifactu
                                        cu[24] = dr.GetString("detPeso");
                                        cu[25] = "01";      // codigo motivo de traslado .. .. esto deberia grabarse en la tabla adifactu
                                        cu[26] = dr.GetString("regmtc");
                                        cu[27] = "1";
                                        cu[28] = dr.GetString("cargaunica");
                                    }
                                    tipdoS = dr.GetString("codsunat");
                                }
                                else
                                {
                                    //MessageBox.Show("No existe el número del documento de venta!", "Atención - dato incorrecto",
                                    //MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                }
                            }
                        }
                    }
                    // ***************************   detalle   ********************************
                    using (MySqlCommand micon = new MySqlCommand(jalad, conn))
                    {
                        micon.Parameters.AddWithValue("@tdv", tipdo);
                        micon.Parameters.AddWithValue("@ser", serdo);
                        micon.Parameters.AddWithValue("@num", numdo);
                        using (MySqlDataAdapter da = new MySqlDataAdapter(micon))
                        {
                            DataTable dtab = new DataTable();
                            da.Fill(dtab);
                            int x = 0;
                            foreach (DataRow row in dtab.Rows)
                            {
                                //var a = row[10].ToString().Substring(0, 10);
                                //string valorel = "";
                                //if (row[14].ToString().Trim() != "" && row[14].ToString().Trim().Substring(0, 4) != "0.00")
                                decimal vdf = 0;
                                {
                                    vdf = decimal.Parse(row[7].ToString()) / (1 + (decimal.Parse(var[10]) / 100));
                                }
                                dt[x, 0] = row[13].ToString();     // OriDest
                                dt[x, 1] = row[2].ToString();      // Cant (cant bultos)
                                dt[x, 2] = row[16].ToString();     // unidad de medida
                                dt[x, 3] = row[1].ToString();      // guias
                                dt[x, 4] = row[4].ToString();      // descrip
                                dt[x, 5] = row[11].ToString();     // guiasclte
                                dt[x, 6] = Math.Round(vdf, 2).ToString();   // valor unit
                                dt[x, 7] = row[7].ToString();      // precio unit
                                dt[x, 8] = row[7].ToString();      // total 
                                dt[x, 9] = row[12].ToString();     // codmondoc
                                    //row[6].ToString(),      // moneda (nombre)
                                    //row[8].ToString(),     // valorMN
                                    //row[9].ToString(),     // codmonloc
                                    //a.Substring(6, 4) + "-" + a.Substring(3, 2) + "-" + a.Substring(0, 2),     // fechaGR
                                    //valorel,               // valorel
                                //tx_dat_nombd.Text = row[3].ToString();
                                //glosser2 = dataGridView1.Rows[0].Cells["OriDest"].Value.ToString() + " - " + tx_totcant.Text.Trim() + " " + tx_dat_nombd.Text;
                                //glosser2 = row[13].ToString() + " - " + tx_totcant.Text.Trim() + " " + tx_dat_nombd.Text;

                                x += 1;
                            }
                            dtab.Dispose();
                        }
                    }
                }
            }
            // llamada a llenaTablaLiteDV
            if (llenaTablaLiteDV(tipdoS, serdo, numdo, vs, dt, var, cu) == true) retorna = true;
            else
            {
                MessageBox.Show("Error en completar datos del XML", "Error Interno", MessageBoxButtons.OK, MessageBoxIcon.Error);
                retorna = false;
            }

            return retorna;
        }
        
        public bool llamaXmlDocVta(string[] varios, string cdvta, string tipdo)
        {
            /*  p.Arguments = 
                ruta = args[0];                         // ruta donde se grabará el xml
                ruce = args[1];                         // ruc del emisor del comprobante
                docv = args[2];                         // comprobante en formato <ruc>-<codDV>-<serie>-<numero>
                ifir = args[3].ToLower();               // indicador si se debe firmar | true = si firmar, false = no firmar
                cert = args[4];                         // ruta y nombre del certificado .pfx
                clav = args[5];                         // clave del certificado
                tipg = args[6];                         // tipo de comprobante de venta codigo Sunat
            */
            bool retorna = false;
            string rutalocal = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            ProcessStartInfo p = new ProcessStartInfo();                                                // true = firma comprobante
            p.Arguments = varios[11] + " " + Program.ruc + " " +
                 cdvta + " " +
                true + " " + varios[12] + " " + varios[13] + " " + tipdo;
            p.FileName = @rutalocal + "/xmlDocVta/xmlDocVta.exe";
            var proc = Process.Start(p);
            proc.WaitForExit();
            if (proc.ExitCode == 1) retorna = true;
            else retorna = false;
            //
            return retorna;
        }
    }
}
