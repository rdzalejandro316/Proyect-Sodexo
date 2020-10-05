using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    //Sia.PublicarPnt(9669, "Maestra");  
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9669, "Maestra");
    //ww.ShowInTaskbar=false;    
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
    //ww.ShowDialog();
    public partial class Maestra : Window
    {
        dynamic SiaWin;
        int idemp = 0;
        string cnEmp = "";

        string tabla = "modelMaestraAlejo";
        string codigo = "codigo";
        string nombre = "nombre";

        SqlDataAdapter da = new SqlDataAdapter();
        DataSet ds = new DataSet();
        private CollectionView View;

        public Maestra()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            LoadConfig();
            cargar();

            this.DataContext = ds;
            View = (CollectionView)(CollectionViewSource.GetDefaultView(ds));

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string value)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(value));
            }
        }

        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int idr = 0; string code = ""; string nom = "";
                dynamic winb = SiaWin.WindowBuscar(tabla, codigo, nombre, codigo, codigo, codigo, cnEmp, true, "", idEmp: idemp);
                winb.ShowInTaskbar = false;
                winb.Owner = Application.Current.MainWindow;
                winb.Width = 500;
                winb.Height = 300;
                winb.ShowDialog();
                idr = winb.IdRowReturn;
                code = winb.Codigo;
                nom = winb.Nombre;

                if (idr>0)
                {

                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        private void ActualizaCabCueValoresPredeterminados(string tabla)
        {
            foreach (System.Data.DataColumn dc in ds.Tables[tabla].Columns)
            {
                string value = dc.DataType.Name.ToString().ToLower();

                // ... Switch on the string.
                switch (value)
                {
                    case "string":
                        ds.Tables[tabla].Columns[dc.ToString()].DefaultValue = string.Empty;
                        break;
                    case "int32":
                        ds.Tables[tabla].Columns[dc.ToString()].DefaultValue = 0;
                        break;
                    case "int16":
                        ds.Tables[tabla].Columns[dc.ToString()].DefaultValue = 0;
                        break;
                    case "decimal":
                        ds.Tables[tabla].Columns[dc.ToString()].DefaultValue = 0;
                        break;
                    case "bool":
                        ds.Tables[tabla].Columns[dc.ToString()].DefaultValue = string.Empty;
                        break;
                    case "datetime":
                        ds.Tables[tabla].Columns[dc.ToString()].DefaultValue = DateTime.Now;
                        break;

                }
                //if (dc.DataType == typeof(string)) dsDoc.Tables[tabla].Columns[dc.ToString()].DefaultValue = string.Empty;
                //if (dc.DataType == typeof(int)) dsDoc.Tables[tabla].Columns[dc.ToString()].DefaultValue = 0;
                //if (dc.DataType == typeof(Int16)) dsDoc.Tables[tabla].Columns[dc.ToString()].DefaultValue = 0;
                //if (dc.DataType == typeof(decimal)) dsDoc.Tables[tabla].Columns[dc.ToString()].DefaultValue = 0;
                //if (dc.DataType == typeof(bool)) dsDoc.Tables[tabla].Columns[dc.ToString()].DefaultValue = false;
                //if (dc.DataType == typeof(double)) dsDoc.Tables[tabla].Columns[dc.ToString()].DefaultValue = 0;
            }
        }

        public void cargar()
        {
            try
            {
                SqlConnection cn = new SqlConnection(cnEmp);                
                cn.Open();
                da.SelectCommand =new SqlCommand("SELECT * FROM modelMaestraAlejo", cn);
                da.Fill(ds, "tabletemp");


                //DTcon.DataContext = ds;

            }
            catch (Exception w)
            {
                MessageBox.Show("were:"+w);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            cargar();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SiaWin.Browse(ds.Tables[0]);
        }


    }
}
