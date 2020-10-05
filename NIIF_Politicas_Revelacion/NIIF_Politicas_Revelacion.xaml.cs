using Politicas_Revelacion;
using Syncfusion.UI.Xaml.Grid;
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

namespace SiasoftAppExt
{

    //Sia.PublicarPnt(9524, "Politicas_Revelacion");   
    public partial class Politicas_Revelacion : Window
    {
        dynamic SiaWin;
        int idemp = 0;
        string cnEmp = "";

        public Politicas_Revelacion()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            LoadConfig();
            cargarYear();
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


        public void cargarYear()
        {
            string cadena = "select BusinessAno from Seg_PeriodoProjectBusiness group by BusinessAno order by BusinessAno";
            DataTable dt = SiaWin.Func.SqlDT(cadena, "años", 0);
            CB_year.ItemsSource = dt.DefaultView;
        }

        public DataTable returnAño(string year)
        {
            string cadena = "select BusinessAno from Seg_PeriodoProjectBusiness where  BusinessAno!='"+ year + "' group by BusinessAno order by BusinessAno";
            DataTable dt = SiaWin.Func.SqlDT(cadena, "años", 0);
            return dt;
        }

        private void BtnMemo_Click(object sender, RoutedEventArgs e)
        {
            string tag = (sender as Button).Tag.ToString().Trim();
            string tit = "", txt = "", map = "";

            DataRowView row = (DataRowView)dataGridCxC.SelectedItems[0];

            switch (tag)
            {
                case "politicaTag":
                    tit = "Politica";
                    txt = row["politica"].ToString();
                    map = "politica";
                    break;
                case "revelacionTag":
                    tit = "Revelacion";
                    txt = row["revelacion"].ToString();
                    map = "revelacion";
                    break;
                case "proceTag":
                    tit = "Proceso";
                    txt = row["proce"].ToString();
                    map = "proce";
                    break;
            }

            Memo ventana = new Memo();
            ventana.title = tit;
            ventana.text = txt;
            ventana.column = map;
            ventana.id = row["idrow"].ToString();
            ventana.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ventana.ShowInTaskbar = false;
            ventana.Owner = Application.Current.MainWindow;
            ventana.ShowDialog();

            if (ventana.bandera == true) cargar(CB_year.SelectedValue.ToString());            
        }

        private void CB_year_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            cargar(CB_year.SelectedValue.ToString());
        }


        public void cargar(string year)
        {
            string query = "select idrow,nota,revelacion,politica,proce,descrip,fecha,tipn,estado, ";
            query += "iif(tipn=0 , 'Informativa' , 'Caracter Especifico') as tipo, ";
            query += "iif(estado=0 , 'Inactivo' , 'Activo') as est ";
            query += "from  NI_Notas where ano='"+year+"' ";

            DataTable dt = SiaWin.Func.SqlDT(query, "notas", idemp);
            dataGridCxC.ItemsSource = dt.DefaultView;
            TX_total.Text = dataGridCxC.View.Records.Count().ToString();
        }


        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            if (CB_year.SelectedIndex>=0)
            {
                Modify ventana = new Modify();
                ventana.NameBtn = "NEW";
                ventana.año = CB_year.SelectedValue.ToString();
                ventana.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                ventana.ShowInTaskbar = false;
                ventana.Owner = Application.Current.MainWindow;
                ventana.ShowDialog();

                if (ventana.banderaUpdate == true) cargar(CB_year.SelectedValue.ToString());
            }
            else MessageBox.Show("Seleccione año para agregar la nota");
        }

        private void BtnEdi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridCxC.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)dataGridCxC.SelectedItems[0];
                    Modify ventana = new Modify();
                    ventana.NameBtn = "UPDATE";
                    ventana.idrow = row["idrow"].ToString().Trim();
                    ventana.Var_Nota = row["nota"].ToString().Trim();
                    ventana.TxNota.IsEnabled = false;
                    ventana.TxNota.IsReadOnly = false;
                    ventana.Var_Desc = row["descrip"].ToString().Trim();
                    ventana.Var_Esta = Convert.ToInt32(row["estado"] is DBNull ? 0 : row["estado"]);
                    ventana.Var_Tip = Convert.ToInt32(row["tipn"] is DBNull ? 0 : row["tipn"]);  
                    ventana.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    ventana.ShowInTaskbar = false;
                    ventana.Owner = Application.Current.MainWindow;
                    ventana.ShowDialog();
                    if (ventana.banderaUpdate == true) cargar(CB_year.SelectedValue.ToString());
                }
                else MessageBox.Show("Seleccione el valor a editar");
            }
            catch (Exception w)
            {
                MessageBox.Show("aaa"+w);   
            }
            
        }

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridCxC.SelectedIndex >= 0)
            {
                DataRowView row = (DataRowView)dataGridCxC.SelectedItems[0];
                MessageBoxResult result = MessageBox.Show("USTED DESEA ELIMINAR LA NOTA " + row["Nota"].ToString().Trim() + " ", "Siasoft?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes) {
                    string id = row["idrow"].ToString();
                    string query = "delete NI_Notas where idrow='" + id + "' ";
                    if (SiaWin.Func.SqlCRUD(query, idemp) == true) cargar(CB_year.SelectedValue.ToString());
                }
            }
            else MessageBox.Show("Seleccione el valor que desea eliminar");            
        }

        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            if (CB_year.SelectedIndex >= 0)
            {
                Año ventana = new Año();
                ventana.dtNew = returnAño(CB_year.SelectedValue.ToString());
                ventana.AñoOrig= CB_year.SelectedValue.ToString();
                ventana.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                ventana.ShowInTaskbar = false;
                ventana.Owner = Application.Current.MainWindow;
                ventana.ShowDialog();
            }
            else MessageBox.Show("seleccione el año de origen de copia");
            
        }



    }
}
