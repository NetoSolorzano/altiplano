using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;
using Microsoft.Data.Sqlite;
using System.Xml;
using System.Windows.Forms;

namespace TransCarga
{
    class acGRE_sunat
    {
        int plazoT = 0;                 // Sunat Webservice - Cantidad en segundos
        int tiempoT = 0;                // 
        string sunat_TokenAct = "";     // 
        // string de conexion
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + login.data + ";";
        public static string CadenaConexion = "Data Source=TransCarga.db";

        public bool sunat_api(string cg, string nomTabla, string[] c_t, string tx_idr, string tx_serie, string tx_numero, string rutaxml)         // uso de api sunat
        {
            bool retorna = false;
            string tokenSql = conex_token_(c_t);           // este metodo funciona bien .. 26/05/2023
            if (tokenSql != null && tokenSql != "" && tokenSql.Substring(0,5) != "ERROR")
            {
                string aXml = Program.ruc + "-" + cg + "-" + tx_serie + "-" + tx_numero + ".xml";
                string aZip = Program.ruc + "-" + cg + "-" + tx_serie + "-" + tx_numero + ".zip";
                if (aXml != "")
                {
                    // - zipear el xml, 
                    if (File.Exists(rutaxml + aZip) == true)
                    {
                        File.Delete(rutaxml + aZip);
                    }
                    using (ZipArchive zip = ZipFile.Open(rutaxml + aZip, ZipArchiveMode.Create))
                    {
                        string source = rutaxml + aXml;
                        zip.CreateEntryFromFile(source, aXml);
                    }
                    // - byte[]ar el zip, 
                    var bytexml = File.ReadAllBytes(rutaxml + aZip);
                    var base64 = Convert.ToBase64String(bytexml);
                    // - hashear 
                    string hash = "";
                    using (SHA256 sha256 = SHA256.Create())
                    {
                        hash = string.Concat(sha256.ComputeHash(bytexml).Select(x => x.ToString("x2")));
                    }
                    // Postear 
                    string url = "https://api-cpe.sunat.gob.pe/v1/contribuyente/gem/comprobantes/" + aXml.Replace(".xml", "");
                    var oData = new
                    {
                        archivo = new
                        {
                            nomArchivo = aZip,
                            arcGreZip = base64,
                            hashZip = hash
                        }
                    };
                    var json = JsonConvert.SerializeObject(oData);
                    //var Body = new StringContent(json, Encoding.UTF8, "application/json");
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    var poste = new RestClient(url);
                    poste.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Authorization", "Bearer " + tokenSql);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddParameter("application/json", json, ParameterType.RequestBody);
                    //
                    IRestResponse response = poste.Execute(request);
                    var result = JsonConvert.DeserializeObject<Ticket_RptaR>(response.Content);
                    if (response.ResponseStatus.ToString() != "Completed") retorna = false;
                    else retorna = true;
                    // actualizamos los campos de la tabla 
                    string actua = "update " + nomTabla + " set nticket=@nti,fticket=@fti,estadoS=@est,cdr=@cdr where idg=@idg";
                    using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
                    {
                        conn.Open();
                        using (MySqlCommand micon = new MySqlCommand(actua, conn))
                        {
                            micon.Parameters.AddWithValue("@idg", tx_idr);
                            micon.Parameters.AddWithValue("@nti", result.numTicket);
                            micon.Parameters.AddWithValue("@fti", result.fecRecepcion);
                            micon.Parameters.AddWithValue("@est", "Enviado");
                            micon.Parameters.AddWithValue("@cdr", "0");
                            micon.ExecuteNonQuery();
                        }
                    }
                }
            }
            return retorna;
        }
        public string conex_token_(string[] c_t)                                                                                                  // token de conexión
        {
            string retorna = "";
            using (SqliteConnection cnx = new SqliteConnection(CadenaConexion))
            {
                cnx.Open();
                using (SqliteCommand micon = new SqliteCommand("select id, sunat_plazoT, sunat_horaT, sunat_TokenAct from sunat_webservices", cnx))
                {
                    using (SqliteDataReader lite = micon.ExecuteReader())
                    {
                        if (lite.Read())
                        {
                            if (lite.GetString(2) == "") tiempoT = 0;
                            else 
                            { 
                                //tiempoT = (int)(DateTime.Now.TimeOfDay.Subtract(TimeSpan.Parse(lite.GetString(2))).TotalSeconds);
                                tiempoT = (int)(DateTime.Now - DateTime.Parse(lite.GetString(2))).TotalSeconds;
                            }
                            plazoT = lite.GetInt16(1);
                            sunat_TokenAct = lite.GetString(3);
                        }
                    }
                }
            }
            //MessageBox.Show(tiempoT.ToString(),"now - sunat_horaT");
            if (tiempoT >= (plazoT - 60))             // un minuto antes que venza la vigencia del token
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var client = new RestClient("https://api-seguridad.sunat.gob.pe/v1/clientessol/" + c_t[0] + "/oauth2/token/"); // client_id_sunat + "/oauth2/token/"
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddHeader("Cookie", "TS019e7fc2=014dc399cb268cb3d074c3b37bb5b735ab83b63307dfe5263ff5802065fe226640c58236dcd71073fbe01e3206d01bfa3c513e69c4");
                request.AddParameter("grant_type", "password");
                request.AddParameter("scope", c_t[1]);                          // scope_sunat         "https://api-cpe.sunat.gob.pe"
                request.AddParameter("client_id", c_t[2]);                      // client_id_sunat     "9613540b-a94d-45c6-b201-7521413ed391"
                request.AddParameter("client_secret", c_t[3]);                  // client_pass_sunat   "gmlqIVugA1+Fgd1wUN6Kyg=="
                request.AddParameter("username", c_t[4]);                       // u_sol_sunat         "20430100344PTIONVAL"
                request.AddParameter("password", c_t[5]);                       // c_sol_sunat         "patocralr"
                IRestResponse response = client.Execute(request);
                if (response.StatusCode.ToString() != "OK")
                {
                    MessageBox.Show("NO se pudo obtener el token" + Environment.NewLine + response.StatusDescription, "Error obteniendo token", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    retorna = "ERROR - NO se pudo obtener el token";
                }
                else
                {
                    var result = JsonConvert.DeserializeObject<TokenR>(response.Content);
                    retorna = result.access_token;
                    using (SqliteConnection cnx = new SqliteConnection(CadenaConexion))
                    {
                        cnx.Open();
                        using (SqliteCommand micon = new SqliteCommand("update sunat_webservices set sunat_plazoT=@sp,sunat_horaT=@sh,sunat_TokenAct=@st", cnx))
                        {
                            micon.Parameters.AddWithValue("@sp", result.expires_in);
                            micon.Parameters.AddWithValue("@sh", DateTime.Now);   // DateTime.Now.TimeOfDay
                            micon.Parameters.AddWithValue("@st", result.access_token);
                            micon.ExecuteNonQuery();
                            //MessageBox.Show("Acabo de actualizar en sqlite el token: " + result.access_token);
                        }
                    }
                }
            }
            else
            {
                //MessageBox.Show("Estamos dentro del plazo " + tiempoT.ToString());
                retorna = sunat_TokenAct;     // retorna el token actual
            }
            return retorna;
        }
        public Tuple<string, string> consultaC(string nomTabla, string tx_idr, string ticket, string token, string tx_serie, string tx_numero, string rutaxml)     // consulta comprobante
        {
            Tuple<string, string> retorna = null;  // = new Tuple <string, string> ("","");
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var client = new RestClient("https://api-cpe.sunat.gob.pe/v1/contribuyente/gem/comprobantes/envios/" + ticket);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer " + token);
            IRestResponse response = client.Execute(request);
            
            if (response.ResponseStatus.ToString() == "Error") // Rpta == null
            {
                retorna = new Tuple<string, string>("Error", response.ErrorMessage.ToString()); //tx_estaSunat.Text = "Error";
                //tx_estaSunat.Tag = response.Content.ToString();
                //retorna = tx_estaSunat.Text;
                /*
                using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
                {
                    conn.Open();
                    string actua = "update " + nomTabla + " set estadoS=@est,cdr=@cdr where idg=@idg";
                    using (MySqlCommand micon = new MySqlCommand(actua, conn))
                    {
                        micon.Parameters.AddWithValue("@est", "Error");
                        micon.Parameters.AddWithValue("@cdr", response.ErrorMessage.ToString());
                        micon.Parameters.AddWithValue("@idg", tx_idr);
                        micon.ExecuteNonQuery();
                    }
                } */
            }
            else
            {
                try
                {
                    var Rpta = JsonConvert.DeserializeObject<Rspta_ConsultaR>(response.Content);
                    if (Rpta.arcCdr != null)
                    {
                        string CodRrpta = Rpta.codRespuesta.ToString();
                        if (CodRrpta == "98")
                        {
                            retorna = new Tuple<string, string>("En proceso", "En proceso");
                        }
                        if (CodRrpta == "99" && Rpta.indCdrGenerado == "1") // enviado con error y CDR generado
                        {
                            retorna = new Tuple<string, string>("Aceptado", "Error");
                        }
                        if (CodRrpta == "99" && Rpta.indCdrGenerado == "0") // enviado con error sin CDR generado
                        {
                            retorna = new Tuple<string, string>("Rechazado", "Error");
                        }
                        if (CodRrpta == "0")                                // enviado OK con CDR generado
                        {
                            retorna = new Tuple<string, string>("Aceptado", "Aceptado");
                        }
                        if (CodRrpta != "98" && Rpta.indCdrGenerado == "1")             // CDR generado con y sin error
                        {
                            // descompone el arcCDR para obtener los datos del QR
                            string cuidado = convierteCDR((nomTabla == "adiguiar") ? "09" : "31", Rpta.arcCdr, tx_serie, tx_numero, rutaxml);
                            if (cuidado != null && cuidado != "")
                            {
                                using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
                                {
                                    conn.Open();
                                    string actua = "update " + nomTabla + " set estadoS=@est,cdr=@cdr,cdrgener=@gen,textoQR=@tqr where idg=@idg";  // ,fticket=@ftk
                                    using (MySqlCommand micon = new MySqlCommand(actua, conn))
                                    {
                                        micon.Parameters.AddWithValue("@est", "Aceptado");
                                        micon.Parameters.AddWithValue("@cdr", Rpta.arcCdr.ToString());
                                        micon.Parameters.AddWithValue("@gen", Rpta.indCdrGenerado.ToString());
                                        micon.Parameters.AddWithValue("@tqr", cuidado);
                                        //micon.Parameters.AddWithValue("", );
                                        micon.Parameters.AddWithValue("@idg", tx_idr);
                                        micon.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                        else
                        {
                            //tx_estaSunat.Text = (CodRrpta == "98") ? "En Proceso" : "Rechazado";
                            //retorna = tx_estaSunat.Text;
                            //retorna = new Tuple<string, string>((CodRrpta == "98") ? "En Proceso" : "Rechazado", (CodRrpta == "98") ? "En Proceso" : "Rechazado");
                            /*
                            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
                            {
                                conn.Open();
                                string actua = "update " + nomTabla + " set estadoS=@est,cdr=@cdr,cdrgener=@gen where idg=@idg";  // (serie, numero, , @seg, @nug, @nti, @fti)";
                                using (MySqlCommand micon = new MySqlCommand(actua, conn))
                                {
                                    micon.Parameters.AddWithValue("@est", (CodRrpta == "0") ? "Aceptado" : (CodRrpta == "98") ? "En Proceso" : "Rechazado");
                                    micon.Parameters.AddWithValue("@cdr", Rpta.arcCdr.ToString());
                                    micon.Parameters.AddWithValue("@gen", Rpta.indCdrGenerado.ToString());
                                    micon.Parameters.AddWithValue("@idg", tx_idr);
                                    micon.ExecuteNonQuery();
                                }
                            } */
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,"Error en Deserialización");  // Rpta.codRespuesta.ToString(),Rpta.indCdrGenerado
                    retorna = new Tuple<string, string>("Error", ex.Message);
                }
            }
            return retorna;
        }
        public string convierteCDR(string cg, string arCdr, string serie, string corre, string ruta)                                              // genera el cdr
        {
            string retorna = "";

            if (File.Exists(ruta + "temporal.zip"))   // @"c:/temp/temporal.zip"
            {
                File.Delete(ruta + "temporal.zip");   // @"c:/temp/temporal.zip"
            }
            string archi = "R-" + Program.ruc + "-" + cg + "-" + serie + "-" + corre + ".xml";
            if (File.Exists(ruta + archi))           // @"c:/temp/" + archi
            {
                File.Delete(ruta + archi);           // @"c:/temp/" + archi
            }
            // grabamos en memoria el xml y obtenemos el dato del tag <cbc:DocumentDescription> ahí esta el texto a convertir en código QR
            //byte[] xmlbytes = Base64DecodeString(arCdr);
            byte[] xmlbytes = Convert.FromBase64CharArray(arCdr.ToCharArray(), 0, arCdr.Length);
            FileStream fstrm = new FileStream(ruta + "temporal.zip", FileMode.CreateNew, FileAccess.Write);   // @"c:/temp/temporal.zip"
            BinaryWriter writer = new BinaryWriter(fstrm);
            writer.Write(xmlbytes);
            writer.Close();
            fstrm.Close();

            System.IO.Compression.ZipFile.ExtractToDirectory(ruta + "temporal.zip", ruta);        // @"c:/temp/temporal.zip", @"c:/temp/"
            FileStream archiS = new FileStream(ruta + archi, FileMode.Open, FileAccess.Read);        // @"c:/temp/" + archi, FileMode.Open, FileAccess.Read
            XmlDocument archiXml = new XmlDocument();
            archiXml.Load(archiS);
            XmlNode fqr = archiXml.GetElementsByTagName("DocumentDescription", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2").Item(0);
            retorna = fqr.InnerText;
            archiS.Close();

            return retorna;
        }
    }
}
