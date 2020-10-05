using PuntoVentaTT.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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

    //Sia.PublicarPnt(10702, "PuntoVentaTT");  
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(10702, "PuntoVentaTT");
    //ww.ShowInTaskbar=false;    
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
    //ww.ShowDialog();   
    public partial class PuntoVentaTT : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        List<Referencias> items = new List<Referencias>();
        public ObservableCollection<Referencias> RefGrid { get; set; }        

        public PuntoVentaTT()
        {
            InitializeComponent();

            SiaWin = System.Windows.Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            RefGrid = new ObservableCollection<Referencias>();
            //lvUsers.DataContext = this;
            pruebas();
            lvUsers.ItemsSource = items;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvUsers.ItemsSource);
            view.Filter = UserFilter;
            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                Title = "Punto de venta (" + aliasemp + ")";

                string query = "select rtrim(cod_tip) as cod_tip,rtrim(nom_tip) as nom_tip from inmae_tip";
                DataTable dt = SiaWin.Func.SqlDT(query, "lineas", idemp);
                if (dt.Rows.Count > 0)
                {
                    CBlinea.ItemsSource = dt.DefaultView;
                    CBlinea.DisplayMemberPath = "nom_tip";
                    CBlinea.SelectedValuePath = "cod_tip";
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }
        public void pruebas()
        {
            items.Add(new Referencias() { cod_ref = "1", nom_ref = "coca cola" });
            items.Add(new Referencias() { cod_ref = "2", nom_ref = "pepsi" });
            items.Add(new Referencias() { cod_ref = "3", nom_ref = "seven up" });
            items.Add(new Referencias() { cod_ref = "4", nom_ref = "postobon" });
            items.Add(new Referencias() { cod_ref = "5", nom_ref = "hit" });
            items.Add(new Referencias() { cod_ref = "6", nom_ref = "agua" });
            items.Add(new Referencias() { cod_ref = "7", nom_ref = "cafe" });
            items.Add(new Referencias() { cod_ref = "8", nom_ref = "milo" });
            items.Add(new Referencias() { cod_ref = "9", nom_ref = "red bull" });
            items.Add(new Referencias() { cod_ref = "10", nom_ref = "monster" });
            items.Add(new Referencias() { cod_ref = "11", nom_ref = "colombiana" });
            items.Add(new Referencias() { cod_ref = "12", nom_ref = "tropical" });
            items.Add(new Referencias() { cod_ref = "13", nom_ref = "tinto" });
            items.Add(new Referencias() { cod_ref = "14", nom_ref = "perico" });
            items.Add(new Referencias() { cod_ref = "15", nom_ref = "vive 100" });
            items.Add(new Referencias() { cod_ref = "16", nom_ref = "energy" });
            items.Add(new Referencias() { cod_ref = "17", nom_ref = "speed max" });
        }

        #region filter

        private bool UserFilter(object item)
        {
            if (String.IsNullOrEmpty(txtFilter.Text))
                return true;
            else
                return ((item as Referencias).nom_ref.IndexOf(txtFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void txtFilter_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lvUsers.ItemsSource).Refresh();
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tag = (sender as Button).Tag.ToString();

                MessageBox.Show(tag);

                //Lista.Add(new Referencias() { cod_ref = "1", nom_ref = tag});

            }
            catch (Exception w)
            {
                MessageBox.Show("erro mamhuevo:" + w);
            }
        }

        private void CBlinea_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //if (CBlinea.SelectedIndex >= 0) Cargar();
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar referencias:" + w);
            }
        }

        public async void Cargar()
        {
            try
            {
                CancellationTokenSource source = new CancellationTokenSource();


                string linea = CBlinea.SelectedValue.ToString();

                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(linea, source.Token), source.Token);
                await slowTask;

                if (slowTask.IsCompleted)
                {
                    //lvUsers.UpdateLayout();

                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al consultar:" + w, "alerta", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }


        private DataSet LoadData(string linea, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(cnEmp);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("CTRG_referencias", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@cod_tip", linea);
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    //RefGrid.Clear();
                    foreach (DataRow item in ds.Tables[0].Rows)
                    {

                        RefGrid.Add(new Referencias()
                        {
                            cod_ref = item["cod_ref"].ToString(),
                            nom_ref = item["nom_ref"].ToString(),
                            val_ref = Convert.ToDecimal(item["val_ref"])
                        });
                    }
                }

                con.Close();
                return ds;
            }
            catch (Exception w)
            {
                MessageBox.Show(w.Message);
                return null;
            }

        }




        //public enum SexType { Male, Female };


    }





}
