using Syncfusion.UI.Xaml.Grid.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{


    // Sia.PublicarPnt(9507,"PvDocumentoCruce");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9507,"PvDocumentoCruce");
    //ww.ShowInTaskbar=false;
    //ww.cliente_cod="860054978";
    //ww.cliente_nom="ADISPETROL S.A.";
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
    //ww.ShowDialog();


    public partial class PvDocumentoCruce : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public string cliente_nom = "";
        public string cliente_cod = "";       
        public string referencia = "";

        public string Num_Trn = "";

        public PvDocumentoCruce()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            LoadConfig();
            pantalla();            
        }

        private void LoadConfig()
        {
            try
            {
                DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                window.Title = "Pedidos - Empresa:" + cod_empresa + "-" + nomempresa;
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        public void pantalla()
        {
            window.MinHeight = 500;
            window.MaxHeight = 500;
            window.MinWidth = 700;
            window.MaxWidth = 700;
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            TX_cliNom.Text = cliente_nom;
            TX_cliCod.Text = cliente_cod;
            TX_Ref.Text = referencia;
            cargarGrid();
        }

        public void cargarGrid() {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("EmpPV_DocumentoCruce", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@cod_ter", TX_cliCod.Text.ToString().Trim());
                cmd.Parameters.AddWithValue("@cod_ref", TX_Ref.Text.ToString().Trim());
                cmd.Parameters.AddWithValue("@cod_empresa", cod_empresa);
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                con.Close();

                dataGridCabeza.ItemsSource = ds.Tables[0];
                TotalRef.Text = ds.Tables[0].Rows.Count.ToString();
            }
            catch (Exception w)
            {
                MessageBox.Show("error en el windows loades" + w);
            }
        }

        private void validar_Click(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)dataGridCabeza.SelectedItems[0];
            string documento = row["num_trn"].ToString();
            Num_Trn = documento;
            this.Close();
        }



    }
}
