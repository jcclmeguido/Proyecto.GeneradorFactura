using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Configuration;


namespace Axon.DAL
{
    public enum TipoConexion
    {
           Informix =2,
           SQLServer=1
    }
    public class Conexion
    {
        //======================
        //Driver Nativo
        //======================
        //Informix
        //public static string strConn = " Database=tbsai;Host=192.168.100.205;Server=online;Service=1530; Protocol=onsoctcp;UID=saides;Password=kinoto08";
        //public static string provider = "IBM.Data.Informix";

        //======================
        //OLEDB 
        //======================
        //SQL Server
        //public static string strConn = @"Provider=SQLOLEDB;Data Source=FPEREYRA\sqlexpress;Initial Catalog=tbsai2;User Id=fpereyra;Password=123";
        //public static string provider = "System.Data.OleDb"; 

        //Informix
        //public static string strConn = @"Provider=Ifxoledbc.2;Persist Security Info=True;Data Source=tbase@base2520; User ID=fuentes; Password=fatimaftes; DB_Locale=en_US.819; Client_Locale=en_US.819;";
        //public static string strConn = @"Provider=Ifxoledbc.2;Persist Security Info=True;Data Source=tbase@sfi400; User ID=sfi400; Password=caparu2008; DB_Locale=en_US.819; Client_Locale=en_US.819;";
        public static string strConn = @"";
        //public static string strConn = @"Provider=Ifxoledbc.2;Persist Security Info=True;Data Source=tbase@online0; User ID=informix; Password=gansterpatadelana; DB_Locale=en_US.819; Client_Locale=en_US.819;";
        public static string provider = "System.Data.OleDb";
        public static int tipoConn = 2;


        //Socket y Reportes
        public static string userName = "saides";
        public static string password = "kinoto08";

        private const string CONFIG_CONN_FILE = "Config.xml";
        private const string STR_CONN = "StrConn";
        private const string PROVIDER = "Provider";
        private const string TIPO_CNX = "tipoConn";
        public static int variablex = 0;

        ////public static void InicializarWebConfig()
        ////{
        ////    strConn  = LeerParametrosWebConfig(STR_CONN);
        ////    provider = LeerParametrosWebConfig(PROVIDER);
        ////    tipoConn  = int.Parse (LeerParametrosWebConfig(TIPO_CNX));
        ////}


        //public static string LeerParametrosWebConfig(string clave)
        //{
        //     return System.Configuration.ConfigurationManager.AppSettings[clave].ToString();
        //}

        public void CargarDatosConfiguracion(TipoConexion tipoConexion, string baseDatos, string dataSource, string userId, string password, string dbLocale, string clientLocale)
        {
            Encriptacion.Encrypt oEncrypt = new Encriptacion.Encrypt();
            string claveDesencriptada = oEncrypt.DesencriptarCadena(password);
            string procInversion = oEncrypt.EncriptarCadena("P@ssw0rd");
            string procInversion2 = oEncrypt.EncriptarCadena("caparu2008");
            tipoConn = (int)tipoConexion;

            switch (tipoConexion)
            {
                case TipoConexion.Informix:
                    strConn = "Provider=Ifxoledbc.2;Persist Security Info=True;Data Source=" + dataSource + ";User ID=" + userId + ";Password=" + claveDesencriptada + ";DB_Locale=" + dbLocale + ";Client_Locale=" + clientLocale + ";";
                    break;
                    
                case TipoConexion.SQLServer:
                    strConn = "Provider=SQLOLEDB;Data Source=" + dataSource +";Initial Catalog=" + baseDatos + ";User Id="+ userId + ";Password=" + claveDesencriptada + ";";
                    break;

                default:
                    break;
            }
        }


        public static void ConfiguarConexionPorDefecto(DataTable dt)
        {
            //tipoConn     = Convert.ToInt32 (dr["tipoConn"]);
            //strConn     = dr["conexion"].ToString(); 
            tipoConn = Convert.ToInt32(dt.Rows[0]["tipoConn"]);
            strConn = "Provider=Ifxoledbc.2;Persist Security Info=True;Data Source=" + dt.Rows[0]["NOMBRE_BASE_DATO"].ToString().Trim() + ";User ID=" + dt.Rows[0]["UsuarioDB"].ToString().Trim() + ";Password=" + dt.Rows[0]["PasswordDB"].ToString().Trim() + ";DB_Locale=" + dt.Rows[0]["DB_Locale"].ToString().Trim() + ";Client_Locale=" + dt.Rows[0]["Client_Locale"].ToString().Trim() + ";";

            //tipoConn = int.Parse(ConfigurationManager.AppSettings["TipoConn"].ToString());
            //strConn = "Provider=Ifxoledbc.2;Persist Security Info=True;Data Source=" + ConfigurationManager.AppSettings["NOMBRE_BASE_DATO"].ToString() + ";User ID=" + ConfigurationManager.AppSettings["UsuarioDB"].ToString() + ";Password=" + ConfigurationManager.AppSettings["PasswordDB"].ToString() + ";DB_Locale=" + ConfigurationManager.AppSettings["DB_Locale"].ToString() + ";Client_Locale=" + ConfigurationManager.AppSettings["Client_Locale"].ToString() + ";";
        }

        public static DataTable LeerParametrosXML()
        {
            //Request.PhysicalApplicationPath
            DataSet ds = new DataSet();

            ds.ReadXml(AppDomain.CurrentDomain.BaseDirectory.ToString() + CONFIG_CONN_FILE, XmlReadMode.Auto);
            return ds.Tables[0];
        }

        public DataTable LeerDatosXML()
        {

            //Request.PhysicalApplicationPath
            DataSet ds = new DataSet();

            ds.ReadXml(AppDomain.CurrentDomain.BaseDirectory.ToString() + CONFIG_CONN_FILE, XmlReadMode.Auto);

            return ds.Tables[1];
        }

        public string DireccionPrueba()
        {
            string r = AppDomain.CurrentDomain.BaseDirectory.ToString() + CONFIG_CONN_FILE;
            return r;
        }

    }
}

