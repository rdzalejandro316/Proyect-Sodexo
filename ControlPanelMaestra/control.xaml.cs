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
    
    public partial class control : UserControl
    {
        public static readonly DependencyProperty MyPropertyProperty = DependencyProperty.Register(
            nameof(AccesoId),typeof(int),typeof(control),(PropertyMetadata)new UIPropertyMetadata((object)0)
        );

        public int AccesoId
        {
            get { return (int)this.GetValue(control.MyPropertyProperty); }
            set { this.SetValue(control.MyPropertyProperty, (object)value); }
        }

        public int Tipo
        {
            get { return (int)this.GetValue(control.MyPropertyProperty); }
            set { this.SetValue(control.MyPropertyProperty, (object)value); }
        }

        public control()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (Tipo == 1)
            {
                Previous.Visibility = Visibility.Collapsed;
                Next.Visibility = Visibility.Collapsed;
            }
            else
            {
                Previous.Visibility = Visibility.Visible;
                Next.Visibility = Visibility.Visible;
            }
        }





    }
}
