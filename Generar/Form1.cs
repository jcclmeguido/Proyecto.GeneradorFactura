using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Diagnostics;
using Axon.GFE;

namespace Generar
{

    public partial class Form1 : Form
    {

        #region VARIABLES GLOBALES
        Axon.DAL.Conexion oConexion = new Axon.DAL.Conexion();
        string baseDatos = ConfigurationManager.AppSettings["BaseDatos"].ToString();
        string dataSource = ConfigurationManager.AppSettings["DataSource"].ToString();
        string userId = ConfigurationManager.AppSettings["UserId"].ToString();
        string password = ConfigurationManager.AppSettings["Password"].ToString();
        string dbLocale = ConfigurationManager.AppSettings["dbLocale"].ToString();
        string clientLocale = ConfigurationManager.AppSettings["clientLocale"].ToString();
        Axon.DAL.TipoConexion tipoConexion = (Axon.DAL.TipoConexion)Convert.ToInt32(ConfigurationManager.AppSettings["TipoConn"].ToString());

        #endregion

        static string sSource = "WinSerAppCensador";
        static string sLog = "Application";
        static int idEvento;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Cronometro.Instancia.Iniciar();
            GeneraFacturaestandar();
            TimeSpan t = Cronometro.Instancia.Detener();
            MessageBox.Show(t.ToString("h'h 'm'm 's's'"));
            Debug.WriteLine(t.ToString("h'h 'm'm 's's'"));
            // ultimoide();

        }

        public void GeneraFacturaestandar()
        {
            int index = cbxEstadoFactura.SelectedIndex;
            string estadoFactura = string.Empty;

            if (index != -1)
            {
                //MessageBox.Show("Debe seleccionar un estado");
                switch (index)
                {
                    case 0:
                        estadoFactura = "100";
                        break;

                    case 1:
                        estadoFactura = "200";
                        break;

                    default:
                        break;
                }
            }

            int indexTipoFactura = cbxTipoFactura.SelectedIndex;
            string tipoFactura = string.Empty;

            if (indexTipoFactura != -1)
            {
                //MessageBox.Show("Debe seleccionar un estado");
                switch (indexTipoFactura)
                {
                    case 0:
                        tipoFactura = "1";
                        break;

                    case 1:
                        tipoFactura = "2";
                        break;

                    case 2:
                        tipoFactura = "3";
                        break;

                    default:
                        break;
                }
            }

            Axon.DAL.Conexion oConexion = new Axon.DAL.Conexion();
            oConexion.CargarDatosConfiguracion(tipoConexion, baseDatos, dataSource, userId, password, clientLocale, dbLocale);
            DBAxon db = new DBAxon();

            try
            {
                DataTable ult = new DataTable();
                ult = ObtenerUltimoIDE();
                int NroFacturaInicio = int.Parse(ult.Rows[0][0].ToString()) + 1;
                int esLote;

                if (!checkBox1.Checked)
                    esLote = 0; // 0 no es lote 1 es lote
                else
                    esLote = 1;

                int Identificadorlote = 0;
                int Cantidadlote = Convert.ToInt32(textBox2.Text); //cantidad de facturas que se van a generar 
                int contingencia = 0; // si las facturas son de contingencia  
                int CantidadDetalles = Convert.ToInt32(textBox1.Text);

                string query = "";
                string Cabecera;
                string Detalle;
                int iddf = 0;
                int nfac = 0;
                int item = 0;

                int limite = NroFacturaInicio + Cantidadlote;
                db.OpenFactoryConnection();
                db.SetLockModeToWait();
                db.BeginTransaction();

                for (int i = NroFacturaInicio; i < limite; i++)
                {
                    query = "";
                    iddf = i;
                    nfac = i;

                    Cabecera = "INSERT INTO fehfe" +
                    "(" +
                      "fehfenfac, fehfedire, fehfeciud, fehfezona, fehfenmed, fehfefemi, fehfegest, fehfemmes, fehfectdi, fehfeccuf, fehfendoc, " +
                      "fehfecomp, fehfecsuc,  fehfecpve, fehfenest, fehfersoc, fehfeteve, fehfeleve, fehfefeve, fehfeaeve, fehfensal, fehfedsal, " +
                      "fehfecmon, fehfemser, fehfedoco, fehfedico, fehfenpre,  fehfemtot, fehfemtaf, fehfemled, fehfeccli, fehfel317, fehfemtsi, " +
                      "fehfemtmo, fehfetcam, fehfemtoj, fehfemtsl, fehfecdse, fehfenemi, fehfecmpa, fehfemdes,  fehfeleye, fehfeusua, fehfentar, " +
                      "fehfepfac, fehfecpai, fehfepveh, fehfetenv, fehfemice, fehfenpro, fehfenrle, fehfecpag, fehfepent, fehfechue, fehfechab, " +
                      "fehfecmay, fehfecmen, fehfefiho, fehfenotu, fehfersot, fehfeckwh, fehfecmcu, fehfedley, fehfetase, fehfetalu, fehfeidca, " +
                      "fehfemtpu, fehfeomon, fehfeinco,  fehfepdes, fehfeldes, fehfepvbr, fehfegtfr, fehfesfro, fehfetffr, fehfemtfr, fehfemsin, " +
                      "fehferemi, fehfecons, fehfelapu, fehfeidbd, fehfemrcb, fehfestat,  fehfecres, fehfetres, fehfecufd, fehfecont, fehfelote, " +
                      "fehfeidlo, fehfeufac, fehfectip" +
                      ")" +
                    "VALUES" +
                    "(" +
                    "    " + nfac + "," +
                    "  'Gualberto villarroel 123', NULL, NULL, NULL, " +
                    "20190205162700" +
                    ", NULL, NULL, 1, NULL, '5642111', NULL, 0, 0, NULL, 'Juan Perez', NULL," +
                    "  NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, 50.5, NULL, NULL, 55421, NULL, NULL, 50.5, 6.97, NULL, NULL," +
                    tipoFactura + "," +   //fehfecdse
                    "  1028305029, " + //--fehfenemi
                    "  1," +
                    "  null, " + //--fehfemdes
                    "    'Ley N 453: Est', 'FTL', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL," +
                    "  NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL," +
                    "  0, " + estadoFactura + ", NULL, NULL, NULL, " +
                    "0, " + //fehfecont contingencia 
                    "0, " + //fehfelote es lote
                    "0, " + //fehfeidlo id del lote
                    "1, " +  //fehfeufac ultima factura
                    "1" +  //fehfectip codigo tipo factura
                    ");";

                    query += Cabecera;

                    for (int j = 1; j <= CantidadDetalles; j++)
                    {
                        Detalle = "              INSERT INTO fedfe" +
                                        "(                   fedfeiddf, fedfeitem, fedfeaeco, fedfecpsi, fedfecpro, fedfectha, fedfecnan, fedfedesc, fedfecdia, fedfecant, fedfemice, fedfepuni, fedfestot, fedfemdes, fedfeumed, fedfenser, fedfeespe, fedfeedet, fedfenqso, fedfeemed, fedfename, fedfenifm, fedfenofm, fedfefmed, fedfepdid, fedfenaci, fedfemrcb, fedfestat ) " +
                         "VALUES ( " + iddf + ", " + j + ", " +
                            " '123123', 123, 123, NULL, NULL, 'coca cola 1 litro', NULL, 5, NULL, 10.1, 10.1, " +
                            "NULL" +
                            ", 'botella', '1234567ADC', NULL, NULL, NULL,  NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL );";
                        query += Detalle;

                    }
                    db.DataAdapter(CommandType.Text, query);
                }

                db.CommitTransaction();
                db.CloseFactoryConnection();
                db = null;
            }
            catch (Exception ex)
            {
                db.RollbackTransaction();
                throw new Exception(ex.Message);
            }
        }

        public DataTable ObtenerUltimoIDE()
        {
            DataTable dtUltimaFactura = new DataTable();
            try
            {
                Axon.DAL.Conexion oConexion = new Axon.DAL.Conexion();
                DBAxon db = new DBAxon();
                db.OpenFactoryConnection();

                string query = "SELECT max(fehfeiddf) FROM fehfe";
                dtUltimaFactura = db.DataAdapter(CommandType.Text, query);
                db.CloseFactoryConnection();
                db = null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
           

            return dtUltimaFactura;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
