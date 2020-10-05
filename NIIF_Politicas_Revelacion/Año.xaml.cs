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

    public partial class Año : Window
    {
        public DataTable dtNew;
        public string AñoOrig = "";

        dynamic SiaWin;
        int idemp = 0;
        string cnEmp = "";


        public Año()
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
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
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
            CB_year.ItemsSource = dtNew.DefaultView;
        }

        private void BTNCopy_Click(object sender, RoutedEventArgs e)
        {
            if (CB_year.SelectedIndex >= 0)
            {
                MessageBoxResult result = MessageBox.Show("USTED DESEA PASAR LAS NOTAS DEL AÑO " + AñoOrig + "  al año " + CB_year.SelectedValue.ToString() + " ", "Siasoft?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    string delete = "delete NI_Notas where ano='" + CB_year.SelectedValue.ToString() + "'; ";
                    if (SiaWin.Func.SqlCRUD(delete, idemp) == true)
                    {
                        DataTable dt = SiaWin.Func.SqlDT("select * from NI_Notas where ano='" + AñoOrig + "';", "notas", idemp);
                        string query = "";
                        foreach (DataRow item in dt.Rows)
                        {
                            query += "insert into NI_Notas (nota,descrip,revelacion,politica,fecha,ano,tipn,proce,estado) values " +
                                "('"+item["nota"].ToString().Trim() + "','" + item["descrip"].ToString().Trim() + "','" + item["revelacion"].ToString().Trim() + "','" + item["politica"].ToString().Trim() + "','" + item["fecha"].ToString().Trim() + "','"+ CB_year.SelectedValue.ToString() + "'," + item["tipn"] + ",'" + item["proce"].ToString().Trim() + "',"+item["estado"]+")";
                        }                        
                        if (SiaWin.Func.SqlCRUD(query, idemp) == true) { MessageBox.Show("las notas se pasaron exitosamente"); }
                    };

                }
            }
            else MessageBox.Show("seleccione el año");
        }

        private void BTNCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }
}
