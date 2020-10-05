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

    public partial class CtaHabitacion : Window
    {
        dynamic SiaWin;
        int idemp = 0;
        string cnEmp = "";
        DispatcherTimer disp = new DispatcherTimer();
        public string cod_vivienda;
        public string empresa;
        public string pv;

        //edicion
        string cod_hab = "";

        //cambios
        public bool ind_cambio = false;

        public CtaHabitacion()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            LoadConfig();
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
                Title = "Creacion de Habitaciones (" + aliasemp + ")";
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


        public Tuple<bool, int> cantidadCams(int camas)
        {
            bool flag = false; int cnt = 0;
            try
            {
                string query = "select sum(CtMae_Viviendas.Cnt_Camas) as cam_vivi,sum(CtMae_ViviendasHabitaciones.Cnt_Camas) as tot_camas, ";
                query += "sum(CtMae_Viviendas.Cnt_Camas)-sum(CtMae_ViviendasHabitaciones.Cnt_Camas) as restantes ";
                query += "from CtMae_Viviendas ";
                query += "inner join CtMae_ViviendasHabitaciones on CtMae_ViviendasHabitaciones.ViviendaCodigo = CtMae_Viviendas.ViviendaCodigo ";
                query += "where CtMae_Viviendas.ViviendaCodigo='"+cod_vivienda+"' and PuntoVentaCodigo='"+ pv + "' ";
                DataTable dt = SiaWin.Func.SqlDT(query, "campamento", idemp);
                if (dt.Rows.Count>0)
                {
                    cnt = Convert.ToInt32(dt.Rows[0]["restantes"]);                    
                    flag = cnt <= camas ? true:false;   
                }
            }

            catch (Exception w)
            {
                MessageBox.Show("error en la busqueda de camas:" + w);
            }
            return new Tuple<bool, int>(flag, cnt);
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                #region valiadaciones

                if (string.IsNullOrEmpty(TxCodigo.Text))
                {
                    NotificationOn("debe de ingresar el codigo de la vivienda");
                    return;
                }

                #endregion

                string con = BtnGuardar.Content.ToString().Trim();
                string query = "";
                if (con == "Agregar")
                {
                    query += "insert into CtMae_ViviendasHabitaciones (HabitacionCodigo,ViviendaCodigo,Cnt_Camas,Estado,Reservada,Nota,EmpresaCodigo) values ('" + TxCodigo.Text + "','" + cod_vivienda + "'," + UpCamas.Value + "," + Convert.ToInt32(CheckEstado.IsChecked) + "," + Convert.ToInt32(CheckReserva.IsChecked) + ",'" + TXnota.Text + "','" + empresa + "') ";
                }
                else
                {
                    query += "update CtMae_ViviendasHabitaciones set HabitacionCodigo='" + TxCodigo.Text + "',Cnt_Camas=" + UpCamas.Value + ",Estado=" + Convert.ToInt32(CheckEstado.IsChecked) + ",Reservada=" + Convert.ToInt32(CheckReserva.IsChecked) + ",Nota='" + TXnota.Text + "' where  HabitacionCodigo='" + cod_hab + "' and EmpresaCodigo='" + empresa + "' and  ViviendaCodigo='" + cod_vivienda + "'; ";
                }



                if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                {
                    string messa = con == "Agregar" ? "creacion de habitacion exitoso" : "Modificacion exitosa";
                    ind_cambio = true;
                    Dialogo(messa, 1);
                    clean();
                    load();
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al guardar:" + w);
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
            UpCamas.Value = 1;
            TXnota.Text = "";
            cod_hab = "";
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TxVivienda.Content = "(Vivienda " + cod_vivienda.Trim() + ")";
            load();
        }

        public async void load()
        {

            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

            string codpv = cod_vivienda;
            CtrlLoad.IsIndeterminate = true;

            var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(codpv, source.Token), source.Token);
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

        private DataSet LoadData(string vivi, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(cnEmp);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_ConsultaViviendaHabitacion", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Vivienda", vivi);
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

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Btnadd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGrid.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)dataGrid.SelectedItems[0];
                    CtaCamas ventana = new CtaCamas();
                    ventana.cod_hab = row["HabitacionCodigo"].ToString().Trim();
                    ventana.cod_viv = row["ViviendaCodigo"].ToString().Trim();
                    ventana.nombreHab = row["HabitacionCodigo"].ToString().Trim();
                    ventana.ShowInTaskbar = false;
                    ventana.Owner = Application.Current.MainWindow;
                    ventana.ShowDialog();
                    if (ventana.ind_cambio) ind_cambio = true;


                }
            }
            catch (Exception w)
            {
                MessageBox.Show("errro en la pantalla:" + w);
            }
        }

        private void BtEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BtnGuardar.Content = "Modificar";
                BtnCancelar.Content = "Cancelar Edicion";
                BtnCancelar.Tag = "C2";

                DataRowView row = (DataRowView)dataGrid.SelectedItems[0];
                TxCodigo.Text = row["HabitacionCodigo"].ToString().Trim();
                cod_hab = row["HabitacionCodigo"].ToString().Trim();
                UpCamas.Value = Convert.ToInt32(row["Cnt_Camas"]);
                CheckEstado.IsChecked = Convert.ToBoolean(row["Estado"]);
                CheckReserva.IsChecked = Convert.ToBoolean(row["Reservada"]);
                TxCodigo.Text = row["HabitacionCodigo"].ToString().Trim();
                TXnota.Text = row["Nota"].ToString().Trim();
            }
            catch (Exception w)
            {
                MessageBox.Show("errro al cargar edicion:" + w);
            }
        }

        private void DataGrid_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            try
            {
                if (dataGrid.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)dataGrid.SelectedItems[0];
                    TxCodigo.Text = row["HabitacionCodigo"].ToString().Trim();
                    UpCamas.Value = Convert.ToInt32(row["Cnt_Camas"]);
                    CheckEstado.IsChecked = Convert.ToBoolean(row["Estado"]);
                    CheckReserva.IsChecked = Convert.ToBoolean(row["Reservada"]);
                    TxCodigo.Text = row["HabitacionCodigo"].ToString().Trim();
                    TXnota.Text = row["Nota"].ToString().Trim();
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar:" + w, "alerta", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BTNDelEst_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedIndex >= 0)
            {
                DataRowView row = (DataRowView)dataGrid.SelectedItems[0];
                string vivienda = row["ViviendaCodigo"].ToString().Trim();
                string habitacion = row["HabitacionCodigo"].ToString().Trim();

                MessageBoxResult result = MessageBox.Show("USTED DESEA ELIMINAR LA HABITACION: " + habitacion, "Siasoft?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    string delete = "delete CtMae_ViviendasHabitaciones where ViviendaCodigo='" + vivienda + "' and HabitacionCodigo='"+ habitacion + "'; ";
                    if (SiaWin.Func.SqlCRUD(delete, idemp) == true)
                    {
                        load();
                        NotificationOn("eliminacion exitosa");
                        ind_cambio = true;
                    }
                }
            }
        }



    }
}

