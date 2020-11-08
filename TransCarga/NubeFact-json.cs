/// C#
/// #########################################################
/// #### INTEGRACIÓN FÁCIL ####
/// +++++++++++++++++++++++++++++++++++++++++++++++++++++++
/// # ESTE CÓDIGO FUNCIONA PARA LA VERSIÓN ONLINE Y OFFLINE
/// # Visita www.nubefact.com/integracion para más información
/// +++++++++++++++++++++++++++++++++++++++++++++++++++++++

/// Debes usar la siguiente librería: http://www.newtonsoft.com/json
/// También puedes usar esta utilería para construir clases para el Json: http://jsonutils.com/

/// #########################################################
/// #### FORMA DE TRABAJO ####
/// +++++++++++++++++++++++++++++++++++++++++++++++++++++++
/// # PASO 1: Conseguir una RUTA y un TOKEN para trabajar con NUBEFACT (Regístrate o ingresa a tu cuenta en www.nubefact.com).
/// # PASO 2: Generar un archivo en formato .JSON o .TXT con una estructura que se detalla en este documento.
/// # PASO 3: Enviar el archivo generado a nuestra WEB SERVICE ONLINE u OFFLINE según corresponda usando la RUTA y el TOKEN.
/// # PASO 4: Generamos el archivo XML y PDF (Según especificaciones de la SUNAT) y te devolveremos INSTANTÁNEAMENTE los datos del documento generado.
/// # Para ver el documento generado ingresa a www.nubefact.com/login con tus datos de acceso, y luego a la opción "Ver Facturas, Boletas y Notas"
/// # IMPORTANTE: Enviaremos el XML generado a la SUNAT y lo almacenaremos junto con el PDF, XML y CDR en la NUBE para que tu cliente pueda consultarlo en cualquier momento, si así lo desea.
/// +++++++++++++++++++++++++++++++++++++++++++++++++++++++

/// USAR LO SIGUIENTE
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Text;


namespace TransCarga
{
    public class NubeFacT
    {
        /// #########################################################
        /// #### PASO 1: CONSEGUIR LA RUTA Y TOKEN ####
        /// +++++++++++++++++++++++++++++++++++++++++++++++++++++++
        /// # CUENTA DEMO PARA HACER PRUEBAS
        /// # Puedes usar la siguiente cuenta para hacer pruebas:
        /// #  - LINK: https://demo.nubefact.com/login
        /// #  - USUARIO: demo@nubefact.com
        /// #  - PASSWORD: demo@nubefact.com
        /// # Una vez que ingreses a esta cuenta ve a la opción API (Integración) para ver la RUTA y el TOKEN los cuales son:
        /// #  - RUTA: https://demo.nubefact.com/api/v1/03989d1a-6c8c-4b71-b1cd-7d37001deaa0
        /// #  - TOKEN: d0a80b88cde446d092025465bdb4673e103a0d881ca6479ebbab10664dbc5677
        /// # También puedes crear una cuenta propia para hacer pruebas más precisas.
        /// #
        /// # CREAR UNA CUENTA PROPIA
        /// #  - Regístrate gratis en www.nubefact.com/register
        /// #  - Ir la opción API (Integración).
        /// # IMPORTANTE: Para que la opción API (Integración) de tu cuenta propia esté activada necesitas escribirnos a soporte@nubefact.com o llámanos al teléfono: 01 468 3535 (opción 2) o celular (WhatsApp) 955 598762.
        /// +++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// # RUTA para enviar documentos
        public const string ruta = "https://api.nubefact.com/api/v1/a729db9a-258b-4d48-9be6-378a403a9ab5";      // "https://demo.nubefact.com/api/v1/03989d1a-6c8c-4b71-b1cd-7d37001deaa0";

        /// # TOKEN para enviar documentos
        public const string token = "873bd61c9be146a6941b210fd63bccd7bd6808d204d14b8b8c0c6e2e5ccd4dd3";      // "d0a80b88cde446d092025465bdb4673e103a0d881ca6479ebbab10664dbc5677";

        /// #########################################################
        /// #### PASO 2: GENERAR EL ARCHIVO PARA ENVIAR A NUBEFACT ####
        /// +++++++++++++++++++++++++++++++++++++++++++++++++++++++
        /// # - MANUAL para archivo JSON en el link: https://goo.gl/WHMmSb
        /// # - MANUAL para archivo TXT en el link: https://goo.gl/Lz7hAq
        /// +++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public static void Main()
        {
            ///CREAMOS EL JSON
            Invoice invoice = new Invoice();
            invoice.operacion = "generar_comprobante";
            invoice.tipo_de_comprobante = 1;
            invoice.serie = "FFF1";
            invoice.numero = 1;
            invoice.sunat_transaction = 1;
            invoice.cliente_tipo_de_documento = 6;
            invoice.cliente_numero_de_documento = "20600695771";
            invoice.cliente_denominacion = "NUBEFACT SA";
            invoice.cliente_direccion = "CALLE LIBERTAD 116 MIRAFLORES - LIMA - PERU";
            invoice.cliente_email = "";
            invoice.cliente_email_1 = "";
            invoice.cliente_email_2 = "";
            invoice.fecha_de_emision = DateTime.Now;
            invoice.fecha_de_vencimiento = DateTime.Now.AddDays(3);
            invoice.moneda = 1;
            invoice.tipo_de_cambio = "";
            invoice.porcentaje_de_igv = 18.00;
            invoice.descuento_global = "";
            invoice.total_descuento = "";
            invoice.total_anticipo = "";
            invoice.total_gravada = 600.0;
            invoice.total_inafecta = "";
            invoice.total_exonerada = "";
            invoice.total_igv = 108;
            invoice.total_gratuita = "";
            invoice.total_otros_cargos = "";
            invoice.total = 708;
            invoice.percepcion_tipo = "";
            invoice.percepcion_base_imponible = "";
            invoice.total_percepcion = "";
            invoice.detraccion = false;
            invoice.observaciones = "";
            invoice.documento_que_se_modifica_tipo = "";
            invoice.documento_que_se_modifica_serie = "";
            invoice.documento_que_se_modifica_numero = "";
            invoice.tipo_de_nota_de_credito = "";
            invoice.tipo_de_nota_de_debito = "";
            invoice.enviar_automaticamente_a_la_sunat = true;
            invoice.enviar_automaticamente_al_cliente = false;
            invoice.codigo_unico = "";
            invoice.condiciones_de_pago = "";
            invoice.medio_de_pago = "";
            invoice.placa_vehiculo = "";
            invoice.orden_compra_servicio = "";
            invoice.tabla_personalizada_codigo = "";
            invoice.formato_de_pdf = "";
            invoice.items = new List<Items>()
            {
                new Items()
                {
                    unidad_de_medida = "NIU",
                    codigo = "001",
                    descripcion = "DETALLE DEL PRODUCTO",
                    cantidad = 1,
                    valor_unitario = 500,
                    precio_unitario = 590,
                    descuento = "",
                    subtotal = 500,
                    tipo_de_igv = 1,
                    igv = 90,
                    total = 590,
                    anticipo_regularizacion = false,
                    anticipo_comprobante_serie = "",
                    anticipo_comprobante_numero = ""
                },
                new Items()
                {
                    unidad_de_medida = "ZZ",
                    codigo = "001",
                    descripcion = "DETALLE DEL SERVICIO",
                    cantidad = 5,
                    valor_unitario = 20,
                    precio_unitario = 23.60,
                    descuento = "",
                    subtotal = 100,
                    tipo_de_igv = 1,
                    igv = 18,
                    total = 118,
                    anticipo_regularizacion = false,
                    anticipo_comprobante_serie = "",
                    anticipo_comprobante_numero = ""
                },
            };
            string json = JsonConvert.SerializeObject(invoice, Formatting.Indented);
            Console.WriteLine(json);
            byte[] bytes = Encoding.Default.GetBytes(json);
            string json_en_utf_8 = Encoding.UTF8.GetString(bytes);

            /// #########################################################
            /// #### PASO 3: ENVIAR EL ARCHIVO A NUBEFACT ####
            /// +++++++++++++++++++++++++++++++++++++++++++++++++++++++
            /// # SI ESTÁS TRABAJANDO CON ARCHIVO JSON
            /// # - Debes enviar en el HEADER de tu solicitud la siguiente lo siguiente:
            /// # Authorization = Token token="8d19d8c7c1f6402687720eab85cd57a54f5a7a3fa163476bbcf381ee2b5e0c69"
            /// # Content-Type = application/json
            /// # - Adjuntar en el CUERPO o BODY el archivo JSON o TXT
            /// # SI ESTÁS TRABAJANDO CON ARCHIVO TXT
            /// # - Debes enviar en el HEADER de tu solicitud la siguiente lo siguiente:
            /// # Authorization = Token token="8d19d8c7c1f6402687720eab85cd57a54f5a7a3fa163476bbcf381ee2b5e0c69"
            /// # Content-Type = text/plain
            /// # - Adjuntar en el CUERPO o BODY el archivo JSON o TXT
            /// +++++++++++++++++++++++++++++++++++++++++++++++++++++++

            string json_de_respuesta = SendJson(ruta, json_en_utf_8, token);
            dynamic r = JsonConvert.DeserializeObject<Respuesta>(json_de_respuesta);
            string r2 = JsonConvert.SerializeObject(r, Formatting.Indented);
            dynamic json_r_in = JsonConvert.DeserializeObject<Respuesta>(r2);

            ///#########################################################
            ///#### PASO 4: LEER RESPUESTA DE NUBEFACT ####
            ///+++++++++++++++++++++++++++++++++++++++++++++++++++++++
            ///# Recibirás una respuesta de NUBEFACT inmediatamente lo cual se debe leer, verificando que no haya errores.
            ///# Debes guardar en la base de datos la respuesta que te devolveremos.
            ///# Escríbenos a soporte@nubefact.com o llámanos al teléfono: 01 468 3535 (opción 2) o celular (WhatsApp) 955 598762
            ///# Puedes imprimir el PDF que nosotros generamos como también generar tu propia representación impresa previa coordinación con nosotros.
            ///# La impresión del documento seguirá haciéndose desde tu sistema. Enviaremos el documento por email a tu cliente si así lo indicas en el archivo JSON o TXT.
            ///+++++++++++++++++++++++++++++++++++++++++++++++++++++++

            dynamic leer_respuesta = JsonConvert.DeserializeObject<Respuesta>(json_de_respuesta);
            if (leer_respuesta.errors == null)
            {
                Console.WriteLine(json_r_in);
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("TIPO: " + leer_respuesta.tipo);
                Console.WriteLine("SERIE: " + leer_respuesta.serie);
                Console.WriteLine("NUMERO: " + leer_respuesta.numero);
                Console.WriteLine("URL: " + leer_respuesta.url);
                Console.WriteLine("ACEPTADA_POR_SUNAT: " + leer_respuesta.aceptada_por_sunat);
                Console.WriteLine("DESCRIPCION SUNAT: " + leer_respuesta.sunat_description);
                Console.WriteLine("NOTA SUNAT: " + leer_respuesta.sunat_note);
                Console.WriteLine("CODIGO RESPUESTA SUNAT: " + leer_respuesta.sunat_responsecode);
                Console.WriteLine("SUNAT ERROR SOAP: " + leer_respuesta.sunat_soap_error);
                Console.WriteLine("PDF EN BASE64: " + leer_respuesta.pdf_zip_base64);
                Console.WriteLine("XML EN BASE64: " + leer_respuesta.xml_zip_base64);
                Console.WriteLine("CDR EN BASE64: " + leer_respuesta.cdr_zip_base64);
                Console.WriteLine("CODIGO QR: " + leer_respuesta.cadena_para_codigo_qr);
                Console.WriteLine("CODIGO HASH: " + leer_respuesta.codigo_hash);
                Console.WriteLine("CODIGO DE BARRAS: " + leer_respuesta.codigo_de_barras);
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("ERRORES: " + leer_respuesta.errors);
                Console.ReadKey();
            }
        }


        static string SendJson(string ruta, string json, string token)
        {
            try
            {
                using (var client = new WebClient())
                {
                    /// ESPECIFICAMOS EL TIPO DE DOCUMENTO EN EL ENCABEZADO
                    client.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
                    /// ASI COMO EL TOKEN UNICO
                    client.Headers[HttpRequestHeader.Authorization] = "Token token=" + token;
                    /// OBTENEMOS LA RESPUESTA
                    string respuesta = client.UploadString(ruta, "POST", json);
                    /// Y LA 'RETORNAMOS'
                    return respuesta;
                }
            }
            catch (WebException ex)
            {
                /// EN CASO EXISTA ALGUN ERROR, LO TOMAMOS
                var respuesta = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                /// Y LO 'RETORNAMOS'
                return respuesta;
            }
        }
    }


    /// CREAMOS LAS CLASES PARA CONSTRUIR EL JSON Y LEER LA RESPUESTA EN JSON
    public class Invoice
    {
        public string operacion { get; set; }
        public int tipo_de_comprobante { get; set; }
        public string serie { get; set; }
        public int numero { get; set; }
        public int sunat_transaction { get; set; }
        public int cliente_tipo_de_documento { get; set; }
        public string cliente_numero_de_documento { get; set; }
        public string cliente_denominacion { get; set; }
        public string cliente_direccion { get; set; }
        public string cliente_email { get; set; }
        public string cliente_email_1 { get; set; }
        public string cliente_email_2 { get; set; }
        public DateTime fecha_de_emision { get; set; }
        public DateTime fecha_de_vencimiento { get; set; }
        public int moneda { get; set; }
        public dynamic tipo_de_cambio { get; set; } //? MAKES NATURAL TYPES NULLABLE
        public double porcentaje_de_igv { get; set; }
        public dynamic descuento_global { get; set; }
        public dynamic total_descuento { get; set; }
        public dynamic total_anticipo { get; set; }
        public dynamic total_gravada { get; set; }
        public dynamic total_inafecta { get; set; }
        public dynamic total_exonerada { get; set; }
        public double total_igv { get; set; }
        public dynamic total_gratuita { get; set; }
        public dynamic total_otros_cargos { get; set; }
        public double total { get; set; }
        public dynamic percepcion_tipo { get; set; }
        public dynamic percepcion_base_imponible { get; set; }
        public dynamic total_percepcion { get; set; }
        public dynamic total_incluido_percepcion { get; set; }
        public bool detraccion { get; set; }
        public string observaciones { get; set; }
        public dynamic documento_que_se_modifica_tipo { get; set; }
        public string documento_que_se_modifica_serie { get; set; }
        public dynamic documento_que_se_modifica_numero { get; set; }
        public dynamic tipo_de_nota_de_credito { get; set; }
        public dynamic tipo_de_nota_de_debito { get; set; }
        public bool enviar_automaticamente_a_la_sunat { get; set; }
        public bool enviar_automaticamente_al_cliente { get; set; }
        public string codigo_unico { get; set; }
        public string condiciones_de_pago { get; set; }
        public string medio_de_pago { get; set; }
        public string placa_vehiculo { get; set; }
        public string orden_compra_servicio { get; set; }
        public string tabla_personalizada_codigo { get; set; }
        public string formato_de_pdf { get; set; }
        public List<Items> items { get; set; }
        public List<Guias> guias { get; set; }
    }

    public class Items
    {
        public string unidad_de_medida { get; set; }
        public string codigo { get; set; }
        public string descripcion { get; set; }
        public double cantidad { get; set; }
        public double valor_unitario { get; set; }
        public double precio_unitario { get; set; }
        public dynamic descuento { get; set; }
        public double subtotal { get; set; }
        public int tipo_de_igv { get; set; }
        public double igv { get; set; }
        public double total { get; set; }
        public bool anticipo_regularizacion { get; set; }
        public dynamic anticipo_comprobante_serie { get; set; }
        public dynamic anticipo_comprobante_numero { get; set; }
    }

    public class Guias
    {
        public int guia_tipo { get; set; }
        public string guia_serie_numero { get; set; }
    }

    public class Respuesta
    {
        public string errors { get; set; }
        public int tipo { get; set; }
        public string serie { get; set; }
        public int numero { get; set; }
        public string url { get; set; }
        public bool aceptada_por_sunat { get; set; }
        public string sunat_description { get; set; }
        public string sunat_note { get; set; }
        public string sunat_responsecode { get; set; }
        public string sunat_soap_error { get; set; }
        public string pdf_zip_base64 { get; set; }
        public string xml_zip_base64 { get; set; }
        public string cdr_zip_base64 { get; set; }
        public string cadena_para_codigo_qr { get; set; }
        public string codigo_hash { get; set; }
        public string codigo_de_barras { get; set; }
    }
}