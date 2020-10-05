using Menuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9495,"Menuses");
    //Sia.TabU(9495);

    public partial class Menuses : UserControl
    {
        dynamic SiaWin;
        dynamic tabitem;
        int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public Menuses(dynamic tabitem1)
        {
            InitializeComponent();
            try
            {
                SiaWin = Application.Current.MainWindow;
                tabitem = tabitem1;
                idemp = SiaWin._BusinessId;
                LoadConfig();
            }
            catch (Exception w)
            {
                MessageBox.Show("Error en : " + w);
            }
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
                tabitem.Title = "Menus de prueba(" + aliasemp + ")";
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void Copiar_Click(object sender, RoutedEventArgs e)
        {
            //Contenido.Children.Clear();
            //Contenido.Children.Add(new Copiar());
        }
        private void Reclasificar_Click(object sender, RoutedEventArgs e)
        {
            //Contenido.Children.Clear();
            //Contenido.Children.Add(new Reclasificar());
        }

    }
}