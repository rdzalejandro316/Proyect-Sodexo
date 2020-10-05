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
using System.Windows.Shapes;

namespace NIIF_conceptos
{

    public partial class Notas : Window
    {
        public string notaCode = "";
        public string AñoNota = "";

        dynamic SiaWin;
        int idemp = 0;
        string cnEmp = "";

        public Notas()
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
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string cadena = "select politica,revelacion,proce from NI_Notas where  nota='" + notaCode + "' and ano='" + AñoNota + "'";
            DataTable dt = SiaWin.Func.SqlDT(cadena, "ninotas", idemp);
            if (dt.Rows.Count>0)
            {
                Tx_Politica.Text = dt.Rows[0]["politica"].ToString();
                Tx_Revelacion.Text = dt.Rows[0]["revelacion"].ToString();
                Tx_Proceso.Text = dt.Rows[0]["proce"].ToString();
            }
        }



    }
}
