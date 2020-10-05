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

namespace MyLibrariSiasoft
{
    public partial class ButtonSiasoft : Button
    {

        public static readonly DependencyProperty MyPropertyProperty = DependencyProperty.Register(
            nameof(AccesoId),
            typeof(int),
            typeof(ButtonSiasoft),
            (PropertyMetadata)new UIPropertyMetadata((object)0)
        );

        public ButtonSiasoft()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Hola siasoft");
        }
        public int AccesoId
        {
            get
            {
                return (int)this.GetValue(ButtonSiasoft.MyPropertyProperty);
            }
            set
            {
                this.SetValue(ButtonSiasoft.MyPropertyProperty, (object)value);
            }
        }




    }
}
