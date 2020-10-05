using System;
using System.Collections.Generic;
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
    //Sia.PublicarPnt(9458,"NIIF_Menu");
    //Sia.TabU(9458);

    public partial class NIIF_Menu : UserControl
    {
        dynamic SiaWin;
        dynamic tabitem;
        int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public NIIF_Menu(dynamic tabitem1)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            tabitem = tabitem1;
            tabitem.Title = "generacion de pedidos";
            tabitem.Logo(9, ".png");
            tabitem.MultiTab = false;
            idemp = SiaWin._BusinessId;
            LoadConfig();
        }


        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                tabitem.Logo(idLogo, ".png");
                tabitem.Title = "Menu NIIF (" + aliasemp + ")";
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }



        private void BTN_balance_Click(object sender, RoutedEventArgs e)
        {
            //.Sia.PublicarPnt(9458, "MenuNIIF");
            try
            {
                SiaWin.Sia.TabU(9458);
            }
            catch (Exception w)
            {

                MessageBox.Show("open" + w);
            }


        }

        private void BtnIr_Click(object sender, RoutedEventArgs e)
        {
            string namePnt = (sender as Button).Name.ToString().Trim();
            
            if (namePnt == "BTNconceptos")
            {
                dynamic WinDescto = SiaWin.WindowExt(9459, "NIIF_conceptos");
                WinDescto.ShowInTaskbar = false;
                WinDescto.Owner = Application.Current.MainWindow;
                WinDescto.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                WinDescto.ShowDialog();
            }
            if (namePnt == "BTNpoli_reve")
            {
                dynamic WinDescto = SiaWin.WindowExt(9524, "Politicas_Revelacion");
                WinDescto.ShowInTaskbar = false;
                WinDescto.Owner = Application.Current.MainWindow;
                WinDescto.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                WinDescto.ShowDialog();
            }
        }









    }
}





