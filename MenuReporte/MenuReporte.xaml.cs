 using MenuReporte;
using Syncfusion.Windows.Reports;
using Syncfusion.Windows.Reports.Viewer;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9549,"MenuReporte");
    //Sia.TabU(9549);

    //pruebas
    //Sia.PublicarPnt(9552,"MenuReporte");
    //Sia.TabU(9552);
    public partial class MenuReporte : UserControl
    {
        dynamic SiaWin;
        dynamic tabitem;
        int idemp = 0;
        string cnEmp = "";


        public MenuReporte(dynamic tabitem1)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            tabitem = tabitem1;
            idemp = SiaWin._BusinessId;
            //tabitem.CerrarInactivo = true;
            LoadConfig();
            LoadItems();
            MenuBTN.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
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
                tabitem.Logo(idLogo, ".png");
                tabitem.Title = "Reportes (" + aliasemp + ")";
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


        private async void LoadItems()
        {
            try
            {
                Menu.Items.Clear();
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                sfBusyIndicator.IsBusy = true;

                var slowTask = Task<DataTable>.Factory.StartNew(() => SlowDude(source.Token), source.Token);
                await slowTask;

                if (((DataTable)slowTask.Result).Rows.Count > 0)
                {

                    foreach (DataRow row in ((DataTable)slowTask.Result).Rows)
                    {
                        TreeViewItem item = new TreeViewItem();
                        item.Cursor = Cursors.Hand;
                        item.FontSize = 13;

                        if (row["type_item"].ToString().Trim() == "1")
                        {

                            item.Tag = new TagMultiple()
                            {
                                Id_Row = row["idrow"].ToString().Trim(),
                                NamePnt = row["name_item"].ToString().Trim(),
                                TipoPnt = row["type_item"].ToString().Trim(),
                                IsRep = Convert.ToBoolean(row["id_parm"]),
                                urlRep = row["reporte"].ToString().Trim(),
                                Id_screen = Convert.ToInt32(row["id_Screen"]),
                                typePnt = row["typePnt"].ToString().Trim(),
                                idserver = Convert.ToInt32(row["idserver"]),
                                serverIp = row["ServerIP"].ToString().Trim(),
                                userServer = row["UserServer"].ToString().Trim(),
                                userServerPass = row["UserServerPassword"].ToString().Trim(),
                            };                            
                            item.Header = row["name_item"].ToString().Trim();
                            item.Selected += new RoutedEventHandler(TreeViewItem_Selected);
                            //item.MouseLeftButtonDown += TreeViewItem_Selected;
                            
                            Menu.Items.Add(item);
                        }
                        if (row["type_item"].ToString().Trim() == "2")
                        {

                            string parent = row["cod_itemP"].ToString().Trim();

                            TagMultiple tag = new TagMultiple()
                            {
                                Id_Row = row["idrow"].ToString().Trim(),
                                NamePnt = row["name_item"].ToString().Trim(),
                                TipoPnt = row["type_item"].ToString().Trim(),
                                IsRep = Convert.ToBoolean(row["id_parm"]),
                                urlRep = row["reporte"].ToString().Trim(),
                                Id_screen = Convert.ToInt32(row["id_Screen"]),
                                typePnt = row["typePnt"].ToString().Trim(),
                                idserver = Convert.ToInt32(row["idserver"]),
                                serverIp = row["ServerIP"].ToString().Trim(),
                                userServer = row["UserServer"].ToString().Trim(),
                                userServerPass = row["UserServerPassword"].ToString().Trim(),
                            };

                            string header = row["name_item"].ToString().Trim();
                            addNode(parent, header, tag);
                        }
                        if (row["type_item"].ToString().Trim() == "3")
                        {
                            string parent = row["cod_itemP"].ToString().Trim();

                            TagMultiple tag = new TagMultiple()
                            {
                                Id_Row = row["idrow"].ToString().Trim(),
                                NamePnt = row["name_item"].ToString().Trim(),
                                TipoPnt = row["type_item"].ToString().Trim(),
                                IsRep = Convert.ToBoolean(row["id_parm"]),
                                urlRep = row["reporte"].ToString().Trim(),
                                Id_screen = Convert.ToInt32(row["id_Screen"]),
                                typePnt = row["typePnt"].ToString().Trim(),
                                idserver = Convert.ToInt32(row["idserver"]),
                                serverIp = row["ServerIP"].ToString().Trim(),
                                userServer = row["UserServer"].ToString().Trim(),
                                userServerPass = row["UserServerPassword"].ToString().Trim(),
                            };

                            string header = row["name_item"].ToString().Trim();
                            addNode(parent, header, tag);
                        }

                    }


                    //item.MouseLeftButtonUp += click;

                }

                sfBusyIndicator.IsBusy = false;

            }
            catch (Exception w)
            {

                MessageBox.Show("Error-" + w);
            }
        }
      


        private DataTable SlowDude(CancellationToken cancellationToken)
        {
            string query = "select Menu_Reports.idrow,cod_itemP,name_item,type_item,id_Screen,id_parm,reporte,typePnt,idserver,id_acceso, ";
            query += "ReportServer.idrow as repId,ReportServer.ServerIP,ReportServer.UserServer,ReportServer.UserServerPassword ";
            query += "from Menu_Reports ";
            query += "left join ReportServer on Menu_Reports.idserver = ReportServer.idrow ";
            DataTable dt = SiaWin.Func.SqlDT(query, "Menu", 0);
            return dt;
        }

        public void addNode(string NodeParent, string headerSubItem, TagMultiple TagSubItem)
        {
            try
            {

                foreach (TreeViewItem item in Menu.Items)
                {
                    var MultiTag = (TagMultiple)item.Tag;
                    TreeViewItem Subitem = new TreeViewItem();
                    
                    //lo agregar como tipo tres

                    if (item.Items.Count > 0)
                    {
                        foreach (TreeViewItem itemSub in item.Items)
                        {
                            var MultiTagSub = (TagMultiple)itemSub.Tag;
                            if (MultiTagSub.Id_Row == NodeParent.Trim())
                            {
                                Subitem.Name = "I" + TagSubItem.Id_Row;
                                Subitem.Header = headerSubItem;
                                Subitem.Tag = TagSubItem;
                                Subitem.Selected += new RoutedEventHandler(TreeViewItem_Selected);
                                //Subitem.MouseLeftButtonDown += TreeViewItem_Selected;
                                itemSub.Items.Add(Subitem);

                            }

                        }
                    }

                    //lo agregar como tipo 2
                    if (MultiTag.Id_Row == NodeParent.Trim())
                    {
                        Subitem.Name = "I" + TagSubItem.Id_Row;
                        Subitem.Header = headerSubItem;
                        Subitem.Tag = TagSubItem;
                        Subitem.Selected += new RoutedEventHandler(TreeViewItem_Selected);
                        //Subitem.MouseLeftButtonDown += TreeViewItem_Selected;
                        item.Items.Add(Subitem);
                    }

                }

            }
            catch (Exception w)
            {
                MessageBox.Show("*_*" + w);
            }
        }


        //private void TreeViewItem_Selected(object sender, MouseButtonEventArgs e)
        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)       
        {
            try
            {                
                TreeViewItem item = sender as TreeViewItem;
             

                if (item == e.OriginalSource)
                {                    
                    var MultiTag = (TagMultiple)(sender as TreeViewItem).Tag;

                    if (MultiTag.IsRep == true)
                    {
                        
                        switch (MultiTag.typePnt)
                        {                            
                            case "1"://abre un tab interno de esta pantalla
                                Syncfusion.Windows.Reports.Viewer.ReportViewer viewer = new Syncfusion.Windows.Reports.Viewer.ReportViewer();
                                viewer.ReportPath = MultiTag.urlRep;
                                viewer.ReportServerUrl = MultiTag.serverIp;
                                viewer.ProcessingMode = ProcessingMode.Remote;
                                viewer.ReportServerCredential = new System.Net.NetworkCredential(MultiTag.userServer, MultiTag.userServerPass);
                                List<DataSourceCredentials> crdentials = new List<DataSourceCredentials>();

                                foreach (var dataSource in viewer.GetDataSources())
                                {
                                    DataSourceCredentials credn = new DataSourceCredentials();
                                    credn.Name = dataSource.Name;
                                    credn.UserId = "wilmer.barrios@siasoftsas.com";
                                    credn.Password = "Camilo654321*";
                                    crdentials.Add(credn);
                                }
                                viewer.SetDataSourceCredentials(crdentials);
                                viewer.RefreshReport();

                                TabItemExt tabItemExt1 = new TabItemExt();
                                tabItemExt1.Header = MultiTag.NamePnt;
                                tabItemExt1.Content = viewer;
                                TabControlPricipal.Items.Add(tabItemExt1);
                                break;                            
                            case "3":
                                dynamic ww = SiaWin.WindowExt(9531, "MenuReporteWindow");
                                ww.tipo = MultiTag.IsRep;                                
                                ww.Server = MultiTag.serverIp;
                                ww.UserServer = MultiTag.userServer;
                                ww.UserServerPass = MultiTag.userServerPass;
                                ww.carpeta = MultiTag.urlRep;
                                ww.ShowInTaskbar = false;
                                ww.Owner = Application.Current.MainWindow;
                                ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
                                ww.ShowDialog(); 
                                break;                            
                        }
                    }
                    else
                    {                        
                        switch (MultiTag.typePnt)
                        {                                                        
                            case "2"://userconotrol reporte                                
                                SiaWin.TabU(MultiTag.Id_screen);
                                break;                            
                            case "4":
                                WebBrowser webPowBi = new WebBrowser();
                                string urlPowBi = MultiTag.urlRep;
                                webPowBi.Navigate(urlPowBi);
                                TabItemExt tabItemExtPowBi = new TabItemExt();
                                tabItemExtPowBi.Header = MultiTag.NamePnt;
                                tabItemExtPowBi.Content = webPowBi;
                                TabControlPricipal.Items.Add(tabItemExtPowBi);                                
                                break;
                            case "5":                                
                                WebBrowser web = new WebBrowser();
                                string url = MultiTag.urlRep;                                
                                web.Navigate(url);
                                TabItemExt tabItemExt = new TabItemExt();
                                tabItemExt.Header = MultiTag.NamePnt;
                                tabItemExt.Content = web;
                                TabControlPricipal.Items.Add(tabItemExt);
                                break;
                        }
                    }

                    //item.IsSelected = false;
                    //e.Handled = true;
                }                

            }
            catch (Exception w)
            {
                MessageBox.Show("error en el select del item consulte con el administrador:" + w);
            }
        }


        private void Button_Vis(object sender, RoutedEventArgs e)
        {
            string tag = ((ToggleButton)sender).Tag.ToString();

            if (tag == "1")
            {
                Thickness marginMenu = PanelMenu.Margin;
                marginMenu.Left = 0;
                PanelMenu.Margin = marginMenu;

                Thickness marginCont = conte.Margin;
                marginCont.Left = 300;
                conte.Margin = marginCont;
                MenuBTN.Tag = "2";
            }
            else
            {
                Thickness marginMenu = PanelMenu.Margin;
                marginMenu.Left = -300;
                PanelMenu.Margin = marginMenu;

                Thickness marginCont = conte.Margin;
                marginCont.Left = 0;
                conte.Margin = marginCont;
                MenuBTN.Tag = "1";
            }

        }

        private void BTNsetting_Click(object sender, RoutedEventArgs e)
        {
            //   SiaWin.PublicarPnt(9521, "MenuReporteSetting");
            dynamic WinDescto = SiaWin.WindowExt(9521, "MenuReporteSetting");
            WinDescto.ShowInTaskbar = false;
            WinDescto.Owner = Application.Current.MainWindow;
            WinDescto.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            WinDescto.ShowDialog();
            LoadItems();
        }

   


    }
}
