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
using System.Windows.Shapes;

namespace CtaMae_Empleado
{    
    public partial class RFID : Window
    {

        public bool saveCheck = false;
        public string rfidvalu = "";

        public RFID()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (saveCheck) TxRFID.Text = rfidvalu;                                    
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            saveCheck = true;
            rfidvalu = TxRFID.Text;
            this.Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            saveCheck = false;
            rfidvalu = "";
            this.Close();
        }

        
    }
}
