using System;
using System.Collections.Generic;
using System.Data;
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


using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System.Data.OleDb;
using System.Drawing;
using System.Data.SqlClient;
using Microsoft.Win32;
using System.Reflection;
using System.Diagnostics;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9537, "PDFview");  
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9537, "PDFview");
    //ww.ShowInTaskbar=false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
    //ww.ShowDialog();   
    public partial class PDFview : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public PDFview()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;         
        }

      





    }
}
