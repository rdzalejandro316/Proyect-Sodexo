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
    //Sia.PublicarPnt(9557,"Sodexo");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9557, "Sodexo");
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();
    public partial class Sodexo : Window
    {
        public Sodexo()
        {
            InitializeComponent();
            Navegador.Navigate("https://app.powerbi.com/view?r=eyJrIjoiYjYwY2M4ZjYtYmNmNi00YmViLThmODItZTBkNThmYmQ2OTk1IiwidCI6ImI2Y2Q5YzAyLTI0OGEtNGUyMi05YWVkLTE1NGExMzgyNTdmNyJ9");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Cod.Text = (sender as Button).Content.ToString().Trim();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
