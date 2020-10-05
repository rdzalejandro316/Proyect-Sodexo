using Syncfusion.Windows.Reports;
using Syncfusion.Windows.Reports.Viewer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{

    //Sia.PublicarPnt(9509,"ReportViewer");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9509,"ReportViewer");  
    //ww.ShowInTaskbar=false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
    //ww.ShowDialog();        

    public partial class ReportViewer : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public ReportViewer()
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
                DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Descuento por Linea - Empresa:" + cod_empresa + "-" + nomempresa;
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }
        

        public void reporte() {
            //Syncfusion.Windows.Reports.Viewer.ReportViewer viewer = new Syncfusion.Windows.Reports.Viewer.ReportViewer();
            viewer1.ReportPath = @"/Inventarios/ListaPrecios";
            viewer1.ReportServerUrl = @"http://192.168.0.12:7333/Reportservergs";
            viewer1.ProcessingMode = ProcessingMode.Remote;
            viewer1.ReportServerCredential = new System.Net.NetworkCredential(@"grupo\wilmer.barrios", "Siasoft2018*");
            viewer1.RefreshReport();
            List<DataSourceCredentials> crdentials = new List<DataSourceCredentials>();

            foreach (var dataSource in viewer1.GetDataSources())
            {
                DataSourceCredentials credn = new DataSourceCredentials();
                credn.Name = dataSource.Name;
                credn.UserId = "wilmer.barrios@siasoftsas.com";
                credn.Password = "Camilo654321*";
                crdentials.Add(credn);
            }
            viewer1.SetDataSourceCredentials(crdentials);
            viewer1.RefreshReport();
            
            //List<DataSourceCredentials> crdentials = new List<DataSourceCredentials>();

            //foreach (var dataSource in viewer.GetDataSources())
            //{
            //    DataSourceCredentials credn = new DataSourceCredentials();
            //    credn.Name = dataSource.Name;
            //    credn.UserId = "ssrs1";
            //    credn.Password = "RDLReport1";
            //    crdentials.Add(credn);
            //}
            //viewer.SetDataSourceCredentials(crdentials);
            //viewer.RefreshReport();

            //viewer.ServerReport.SetParameters(parameters);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            reporte();
        }


    }
}
