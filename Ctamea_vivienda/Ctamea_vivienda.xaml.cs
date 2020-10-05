using Ctamea_vivienda;
using MaterialDesignThemes.Wpf;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SiasoftAppExt
{

    //Sia.PublicarPnt(9675,"Ctamea_vivienda");
    //Sia.TabU(9675);

    public partial class Ctamea_vivienda : UserControl
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        dynamic tabitem;
        DispatcherTimer disp = new DispatcherTimer();


        public Ctamea_vivienda(dynamic tabitem1)
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            tabitem = tabitem1;
            LoadConfig();
            loadCombo();
        }

        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                tabitem.Logo(idLogo, ".png");
                tabitem.Title = "Maestra de vivienda (" + aliasemp + ")";
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        public void loadCombo()
        {
            try
            {
                DataTable dt = SiaWin.Func.SqlDT("select campamentocodigo,campamentonombre from CtMae_Campamentos where Estado=1;", "campamento", idemp);
                CBCampa.ItemsSource = dt.DefaultView;
                CBCampa.DisplayMemberPath = "campamentonombre";
                CBCampa.SelectedValuePath = "campamentocodigo";

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar los combos" + w);
            }
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

        private void SnackbarMessage_ActionClick(object sender, RoutedEventArgs e)
        {
            Notificaction.IsActive = false;
        }

        private void CBpunto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CBpunto.SelectedIndex >= 0) Cargar();
        }

        public Tuple<string, int> calificacion(int num, string califi)
        {
            string cal = ""; int c = 0;

            if (string.IsNullOrEmpty(califi))
            {
                switch (num)
                {
                    case 1: cal = "A"; break;
                    case 2: cal = "B"; break;
                    case 3: cal = "C"; break;
                    case 4: cal = "D"; break;
                    case 5: cal = "E"; break;
                }
            }
            else
            {
                switch (califi)
                {
                    case "A": c = 1; break;
                    case "B": c = 2; break;
                    case "C": c = 3; break;
                    case "D": c = 4; break;
                    case "E": c = 5; break;
                }
            }

            return new Tuple<string, int>(cal, c);
        }


        public async void Cargar()
        {
            try
            {
                #region validaciones

                if (CBCampa.SelectedIndex < 0)
                {
                    NotificationOn("seleccione un campamento");
                    return;
                }
                if (CBpunto.SelectedIndex < 0)
                {
                    NotificationOn("seleccione un punto de venta");
                    return;
                }

                #endregion

                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;

                string codpv = CBpunto.SelectedValue.ToString();
                CBpunto.IsEnabled = false;
                CBCampa.IsEnabled = false;
                CtrlLoad.IsIndeterminate = true;

                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(codpv, source.Token), source.Token);
                await slowTask;


                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ((DataSet)slowTask.Result).Tables[0];
                    foreach (DataRow item in dt.Rows)
                    {
                        var t = calificacion(0, item["Calificacion"].ToString().Trim());
                        item["Calificacion"] = t.Item2;
                    }

                    dataGrid.ItemsSource = dt.DefaultView;

                    tot_viv.Text = ((DataSet)slowTask.Result).Tables[1].Rows[0]["tot_vivienda"].ToString();
                    tot_hab.Text = ((DataSet)slowTask.Result).Tables[2].Rows[0]["tot_habitacion"].ToString();
                    tot_camas.Text = ((DataSet)slowTask.Result).Tables[3].Rows[0]["tot_camas"].ToString();

                }
                else
                {
                    DataRowView dv = (DataRowView)CBpunto.SelectedItem;
                    string s = dv.Row["puntoventanombre"].ToString().Trim();

                    NotificationOn("el punto de venta " + s + " no contiene viviendas registradas");
                }

                CtrlLoad.IsIndeterminate = false;
                CBpunto.IsEnabled = true;
                CBCampa.IsEnabled = true;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al consultar:" + w, "alerta", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private DataSet LoadData(string pv, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(cnEmp);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_ConsultaViviendaPuntoV", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@puntoVenta", pv);
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

        private void CBCampa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                dataGrid.ItemsSource = null;
                tot_viv.Text = "0"; tot_hab.Text = "0"; tot_camas.Text = "0";

                if (CBCampa.SelectedIndex >= 0)
                {

                    if (CBpunto.IsEnabled == false) CBpunto.IsEnabled = true;
                    DataTable dt = SiaWin.Func.SqlDT("select puntoventacodigo,puntoventanombre from CtMae_PuntosDeVenta where CampamentoCodigo = '" + CBCampa.SelectedValue.ToString() + "' and  Estado=1;", "pv", idemp);
                    if (dt.Rows.Count > 0)
                    {
                        CBpunto.ItemsSource = dt.DefaultView;
                        CBpunto.DisplayMemberPath = "puntoventanombre";
                        CBpunto.SelectedValuePath = "puntoventacodigo";
                    }
                    else
                    {
                        CBpunto.ItemsSource = null;
                        CBpunto.SelectedIndex = -1;
                    }

                }
                else
                {
                    CBpunto.IsEnabled = false;

                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al seleccionar el campamento:" + w);
            }
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                #region validaciones

                if (CBCampa.SelectedIndex < 0)
                {
                    NotificationOn("seleccione un campamento");
                    return;
                }
                if (CBpunto.SelectedIndex < 0)
                {
                    NotificationOn("seleccione un punto de venta");
                    return;
                }

                #endregion

                CtaMaeAdd ventana = new CtaMaeAdd();
                ventana.punto_v = CBpunto.SelectedValue.ToString();
                ventana.ShowInTaskbar = false;
                ventana.Owner = Application.Current.MainWindow;
                ventana.ShowDialog();
                if (ventana.act_int == true) Cargar();
            }
            catch (Exception w)
            {
                MessageBox.Show("errro al agregar:" + w);
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGrid.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)dataGrid.SelectedItems[0];

                    CtaMaeAdd ventana = new CtaMaeAdd();
                    ventana.punto_v = CBpunto.SelectedValue.ToString();
                    ventana.flag = true;
                    ventana.codeVivi = row["ViviendaCodigo"].ToString().Trim();
                    ventana.codeEmpre = row["EmpresaCodigo"].ToString().Trim();
                    ventana.ShowInTaskbar = false;
                    ventana.Owner = Application.Current.MainWindow;
                    ventana.ShowDialog();
                    if (ventana.act_int == true) Cargar();
                }
                else
                {
                    if (CBCampa.SelectedIndex < 0 || CBpunto.SelectedIndex < 0)
                    {
                        NotificationOn("debe de seleccionar el campamento o el punto de ventas");
                    }
                    else
                    {
                        NotificationOn("debe de seleccionar una habitacion");
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("errro al editar:" + w);
            }
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGrid.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)dataGrid.SelectedItems[0];
                    string vivienda = row["ViviendaCodigo"].ToString().Trim();

                    MessageBoxResult result = MessageBox.Show("USTED DESEA ELIMINAR LA VIVIENDA: " + vivienda, "Siasoft?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        string delete = "delete CtMae_Viviendas where ViviendaCodigo='"+vivienda+"' and PuntoVentaCodigo='"+ CBpunto.SelectedValue.ToString() + "'";
                        if (SiaWin.Func.SqlCRUD(delete, idemp) == true)
                        {
                            Cargar();
                            NotificationOn("eliminacion exitosa");
                        }
                    }
                    else
                    {
                        NotificationOn("seleccione una vivienda");
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("no se pudo borrar:" + w);
            }
        }

        private void BtnAddHabitacion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGrid.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)dataGrid.SelectedItems[0];
                    CtaHabitacion ventana = new CtaHabitacion();
                    ventana.cod_vivienda = row["ViviendaCodigo"].ToString().Trim();
                    ventana.empresa = row["EmpresaCodigo"].ToString().Trim();
                    ventana.ShowInTaskbar = false;
                    ventana.Owner = Application.Current.MainWindow;
                    ventana.ShowDialog();
                    if (ventana.ind_cambio == true) Cargar();
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("errro al abrir habitaciones:" + w);
            }
        }

        private void BtnConfig_Click(object sender, RoutedEventArgs e)
        {
            CtaConfig w = new CtaConfig();
            w.ShowInTaskbar = false;
            w.Owner = Application.Current.MainWindow;
            w.ShowDialog();
        }


    }
}
