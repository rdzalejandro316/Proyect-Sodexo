using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Ctamea_vivienda
{    
    public partial class CtaCamas : Window
    {
        dynamic SiaWin;
        int idemp = 0;
        string cnEmp = "";
        DispatcherTimer disp = new DispatcherTimer();

        public string nombreHab = "";
        public string cod_hab = "";
        public string cod_viv = "";

        //valor anterior
        public string cod_cama = "";

        //indicardor de cambios
        public bool ind_cambio = false;

        public CtaCamas()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            LoadConfig();            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txhabitacion.Content = nombreHab;
            load();
        }

        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                Title = "Creacion de Camas (" + aliasemp + ")";
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }


        private void SnackbarMessage_ActionClick(object sender, RoutedEventArgs e)
        {
            Notificaction.IsActive = false;
        }

        public void NotificationOn(string messaage, int tipo = 0)
        {
            Notificaction.IsActive = true;
            NotiMessa.Content = messaage.Trim();
            disp.Interval = TimeSpan.FromMilliseconds(5000);
            disp.Tick += (sender, args) =>
            {
                Notificaction.IsActive = false;
                disp.Stop();
            };
            disp.Start();

            if (tipo == 0) disp.Start();
        }

        public async void load()
        {
            try
            {

                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;

                string habit = cod_hab;
                CtrlLoad.IsIndeterminate = true;

                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(habit, source.Token), source.Token);
                await slowTask;

                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    dataGrid.ItemsSource = ((DataSet)slowTask.Result).Tables[0];
                    tx_total.Content = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();
                }
                else
                {
                    dataGrid.ItemsSource = null;
                    tx_total.Content = "0";
                }

                CtrlLoad.IsIndeterminate = false;

            }
            catch (Exception w)
            {
                MessageBox.Show("errro al cargar:"+w);
            }
        }

        private DataSet LoadData(string habit, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(cnEmp);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_ConsultaViviendaHabitacionCamas", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Habitacion", habit);
                cmd.Parameters.AddWithValue("@viviendaCodigo", cod_viv);
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                con.Close();
                return ds;
            }
            catch (Exception w)
            {
                MessageBox.Show(w.Message);
                return null;
            }

        }

        public void Dialogo(string Message, int tipo)
        {
            titleDialog.Text = Message;
            switch (tipo)
            {
                case 1:
                    DialoPanel1.Visibility = Visibility.Visible;
                    DialoPanel2.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    DialoPanel2.Visibility = Visibility.Visible;
                    DialoPanel1.Visibility = Visibility.Collapsed;
                    break;
            }
            dialogo.IsOpen = true;
        }

        public void clean()
        {
            TxCodigo.Text = "";           
            CheckEstado.IsChecked = false;
            TXnota.Text = "";
            cod_cama = "";
        }


        private void BTNDelEst_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedIndex >= 0)
            {
                DataRowView row = (DataRowView)dataGrid.SelectedItems[0];
                string cama = row["CamaCodigo"].ToString().Trim();
                string habitacion = row["HabitacionCodigo"].ToString().Trim();

                MessageBoxResult result = MessageBox.Show("USTED DESEA ELIMINAR LA CAMA: " + cama, "Siasoft?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    string delete = "delete CtMae_ViviendaHabitacionCamas where ViviendaCodigo='" + cod_viv + "' and HabitacionCodigo='"+habitacion+"' and CamaCodigo='"+ cama + "'; ";
                    if (SiaWin.Func.SqlCRUD(delete, idemp) == true)
                    {
                        load();
                        NotificationOn("eliminacion exitosa");
                        ind_cambio = true;
                    }
                }
            }
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                #region valiadaciones

                if (string.IsNullOrEmpty(TxCodigo.Text))
                {
                    NotificationOn("debe de ingresar el codigo de la cama");
                    return;
                }

                #endregion


                string con = BtnGuardar.Content.ToString().Trim();
                string query = "";
                if (con == "Agregar")
                {
                    query += "insert into CtMae_ViviendaHabitacionCamas (ViviendaCodigo,HabitacionCodigo,CamaCodigo,Estado,Nota) values ('"+ cod_viv + "','" + cod_hab + "','" + TxCodigo.Text + "'," + Convert.ToInt32(CheckEstado.IsChecked) + ",'" + TXnota.Text + "') ";
                }
                else
                {
                    query += "update CtMae_ViviendaHabitacionCamas set CamaCodigo='" + TxCodigo.Text + "',Estado=" + Convert.ToInt32(CheckEstado.IsChecked) + ",Nota='" + TXnota.Text + "' where  HabitacionCodigo='" + cod_hab + "' and  ViviendaCodigo='" + cod_viv + "' and CamaCodigo='" + cod_cama + "'; ";
                }
                //MessageBox.Show(query);

                if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                {
                    string messa = con == "Agregar" ? "creacion de cama exitoso" : "Modificacion de cama exitosa";
                    ind_cambio = true;
                    Dialogo(messa, 1);
                    clean();
                    load();
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("ERROR al guardar:"+w,"alerta",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }



        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string con = (sender as Button).Tag.ToString().Trim();

                if (con == "C2")
                {
                    BtnCancelar.Content = "Cancelar";
                    BtnGuardar.Content = "Agregar";
                    BtnCancelar.Tag = "C1";
                    clean();
                }
                else
                {
                    Dialogo("¿Usted desea salir?", 2);
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cancelar:" + w);
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BtnGuardar.Content = "Modificar";
                BtnCancelar.Content = "Cancelar Edicion";
                BtnCancelar.Tag = "C2";

                DataRowView row = (DataRowView)dataGrid.SelectedItems[0];
                TxCodigo.Text = row["CamaCodigo"].ToString().Trim();
                cod_cama = row["CamaCodigo"].ToString().Trim();                
                CheckEstado.IsChecked = Convert.ToBoolean(row["Estado"]);                
                TXnota.Text = row["Nota"].ToString().Trim();
            }
            catch (Exception w)
            {
                MessageBox.Show("errro al cargar edicion:" + w);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DataGrid_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            try
            {
                if (dataGrid.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)dataGrid.SelectedItems[0];
                    TxCodigo.Text = row["CamaCodigo"].ToString().Trim();
                    CheckEstado.IsChecked = Convert.ToBoolean(row["Estado"]);
                    TXnota.Text = row["Nota"].ToString().Trim();
                }                
            }
            catch (Exception w)
            {
                MessageBox.Show("errro al cargar edicion:" + w);
            }
        }


    }
}
