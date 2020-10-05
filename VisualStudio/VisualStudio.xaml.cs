using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{

    //Sia.PublicarPnt(9559, "VisualStudio");  
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9559, "VisualStudio");
    //ww.ShowInTaskbar=false;    
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
    //ww.ShowDialog();

    public partial class VisualStudio : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";


        string iniXaml = "<Grid Background='White' Name='MainPanel'  HorizontalAlignment='Stretch' VerticalAlignment='Stretch' xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>" +
            "";
        string finXaml = "</Grid>";


        private DependencyObject rootElement;

        Button btn = new Button();

        public VisualStudio()
        {
            try
            {
                InitializeComponent();
                SiaWin = Application.Current.MainWindow;
                idemp = SiaWin._BusinessId;
                LoadConfig();


                //ViewCode.AllowDrop = true;
                //this.DragMove();
            }
            catch (Exception w)
            {
                MessageBox.Show("errorn en coins:" + w);
            }
        }

        private void ViewCode_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {                
                propertyGrid1.SelectedObject = ((object)e.OriginalSource);
            }
            catch (Exception w)
            {
                MessageBox.Show("crommm_" + w);
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {

                EditControl.Text = "" +
                    "<Button x:Name='BtNuevo1' Height='20px' Background='red'>Click</Button>" +
                    "<Button x:Name='BtNuevo2' Height='20px' Background='blue' Margin='0 50 0 0'>Click</Button>" +
                    "<TextBlock Text='hola a todos' Height='20px' Name='Element' Margin='0 300 0 0'/>";

                //propertyGrid1.SelectedObject = btn.FindName("BtNuevo"); ;
                //this.MouseDown += delegate { DragMove(); };
            }
            catch (Exception w)
            {
                MessageBox.Show("error en la edicion de w" + w);
            }

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
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }



        private void EditControl_TextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                this.rootElement = (DependencyObject)XamlReader.Parse(iniXaml + this.EditControl.Text.ToString() + finXaml);

                //this.ViewCode.Content = rootElement;
                UserControl control = new UserControl();
                control.Content = rootElement;

                this.ViewCode.Children.Add(control);
                LogErrores.Text = "Ok";
                LogErrores.Foreground = System.Windows.Media.Brushes.White;

            }
            catch (XamlParseException xpe)
            {
                LogErrores.Foreground = System.Windows.Media.Brushes.Red;
                LogErrores.Text = xpe.Message.ToString();
            }
            catch (Exception xpe)
            {

                LogErrores.Foreground = System.Windows.Media.Brushes.Red;
                LogErrores.Text = xpe.Message.ToString();
                //ActualizarPantalla = false;
            }
        }
        

        private void PropertyGrid1_SelectedPropertyItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MessageBox.Show("ecaaaaaaaaaaPropertyGrid1_IsStylusCapturedChanged");
        }



        


    }
}
