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

namespace Politicas_Revelacion
{    

    public partial class Memo : Window
    {
        dynamic SiaWin;
        int idemp = 0;
        string cnEmp = "";

        public string title = "";
        public string text = "";
        public string id = "";
        public string column = "";


        public bool bandera = false;
        public Memo()
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
            TxObser.Text = text;
            GB.Header = title;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string query = "update NI_Notas set  "+column+"='"+TxObser.Text.Trim()+"' where idrow='" +id+"' ";
            if (SiaWin.Func.SqlCRUD(query, idemp) == true) { bandera = true; this.Close();}

        }

        private void BtnCan_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }
}
