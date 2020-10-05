using Newtonsoft.Json;
using PanelVentas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
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
    // Sia.PublicarPnt(9553,"PanelVentas");
    // Sia.TabU(9553);
    public partial class PanelVentas : UserControl
    {
        dynamic SiaWin;
        dynamic tabitem;
        int idemp = 0;
        int moduloid = 0;
        string cnEmp = "";        

        DispatcherTimer disp = new DispatcherTimer();

        HttpClient client = new HttpClient();
        public string urlPowerBy = "https://api.powerbi.com/beta/b6cd9c02-248a-4e22-9aed-154a138257f7/datasets/3e75ffe0-0fa2-4e21-a169-539071195191/rows?key=uNS2xXjUiBzUrOgYfSdOo9ACF%2BtavFi%2BxXZrMU%2FPgEm97JY0fo4N6gy7ivba0lX3m9mCi0BflWfnZfeVp2cj%2FA%3D%3D";


        public PanelVentas(dynamic tabitem1)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            tabitem = tabitem1;
            idemp = SiaWin._BusinessId;
            LoadConfig();
            disp.Interval = TimeSpan.FromMilliseconds(2000);
            disp.Tick += cargar;
            disp.Start();
        }

        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                //cnEmp = foundRow["BusinessCn"].ToString().Trim();
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                tabitem.Logo(idLogo, ".png");
                tabitem.Title = "Panel de ventas (" + aliasemp + ")";

                System.Data.DataRow[] drmodulo = SiaWin.Modulos.Select("ModulesCode='IN'");
                if (drmodulo == null) this.IsEnabled = false;
                moduloid = Convert.ToInt32(drmodulo[0]["ModulesId"].ToString());

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                MessageBox.Show("aqui88");


            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Navegador.Navigate("https://app.powerbi.com/");

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar:" + w);
            }

        }




        public async void cargar(object sender, EventArgs e)
        {
            try
            {
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;

                string f_i = "19/07/2019";
                string f_f = DateTime.Now.ToString();


                var slowTask = Task<DataSet>.Factory.StartNew(() => getTables(f_i, f_f, source.Token), source.Token);
                await slowTask;

                decimal Cn10 =
                    ((DataSet)slowTask.Result).Tables[0].Rows.Count > 0 ?
                    Convert.ToDecimal(((DataSet)slowTask.Result).Tables[0].Compute("Sum(neto)", "")) : 0;
                decimal Sub10 =
                    ((DataSet)slowTask.Result).Tables[0].Rows.Count > 0 ?
                    Convert.ToDecimal(((DataSet)slowTask.Result).Tables[0].Compute("Sum(subtotal)", "")) : 0;
                decimal Tot10 =
                    ((DataSet)slowTask.Result).Tables[0].Rows.Count > 0 ?
                    Convert.ToDecimal(((DataSet)slowTask.Result).Tables[0].Compute("Sum(total)", "")) : 0;

                decimal Cn20 =
                    ((DataSet)slowTask.Result).Tables[1].Rows.Count > 0 ?
                    Convert.ToDecimal(((DataSet)slowTask.Result).Tables[1].Compute("Sum(neto)", "")) : 0;
                decimal Sub20 =
                    ((DataSet)slowTask.Result).Tables[1].Rows.Count > 0 ?
                    Convert.ToDecimal(((DataSet)slowTask.Result).Tables[1].Compute("Sum(subtotal)", "")) : 0;
                decimal Tot20 =
                    ((DataSet)slowTask.Result).Tables[1].Rows.Count > 0 ?
                    Convert.ToDecimal(((DataSet)slowTask.Result).Tables[1].Compute("Sum(total)", "")) : 0;

                //MessageBox.Show("can:"+Cantidad10);

                var val = new Valores()
                {
                    Cantidad010 = Cn10,
                    Subtotal010 = Sub10,
                    Total010 = Tot10,
                    Cantidad020 = Cn20,
                    Subtotal020 = Sub20,
                    Total020 = Tot20,
                };

                var jsonString = JsonConvert.SerializeObject(val);
                var postPowerBi = HttpPostAsync(urlPowerBy, "[" + jsonString + "]");



            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar datos"+w);
            }
        }


        public async Task<HttpResponseMessage> HttpPostAsync(string url, string data)
        {
            HttpContent content = new StringContent(data);
            HttpResponseMessage response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            return response;
        }

        public DataSet getTables(string fec_ini, string fec_fin, CancellationToken cancellationToken)
        {
            SqlConnection con = new SqlConnection(SiaWin._cn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet ds = new DataSet();
            cmd = new SqlCommand("_EmpSpPanelDeVentas", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FechaIni", fec_ini);
            cmd.Parameters.AddWithValue("@FechaFin", fec_fin);
            da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            con.Close();
            return ds;
        }






    }
}
