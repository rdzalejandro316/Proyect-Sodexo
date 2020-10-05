using NotasDocumentos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(10700,"NotasDocumentos");    
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(10700, "NotasDocumentos");
    //ww.ShowInTaskbar=false;    
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
    //ww.numero_trn = "CMI-00020043";
    //ww.codigo_trn = "001";
    //ww.ShowDialog(); 

    public partial class NotasDocumentos : Window
    {

        dynamic SiaWin;
        int idemp = 0;
        string cnEmp = "";

        public string cod_empleado = "";        

        public NotasDocumentos()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
        }

        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadConfig();

                if (string.IsNullOrEmpty(cod_empleado))
                {
                    Win.IsEnabled = false;
                    Txt_ocu.Visibility = Visibility.Visible;
                    return;
                }

                TX_empleado.Text = cod_empleado;
                getList(cod_empleado.ToString());
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar");
            }
        }



        public void getList(string empleado)
        {
            try
            {
                string select = "select ROW_NUMBER() OVER(ORDER BY idrowcab ASC) AS id,EmpleadoCodigo,Fecha,Title,Nota,idrow from CtMae_EmpleadosNota where EmpleadoCodigo='" + empleado + "' ";
                DataTable tabla = SiaWin.Func.SqlDT(select, "Clientes", idemp);
                list.ItemsSource = tabla.Rows.Count > 0 ? tabla.DefaultView : null;
            }
            catch (Exception w)
            {
                MessageBox.Show("error en la consulta:" + w);
            }
        }




        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NotaAdd ww = new NotaAdd();
                ww.ShowInTaskbar = false;
                ww.Owner = Application.Current.MainWindow;
                ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                ww.idrow = TX_empleado.Tag.ToString();                
                ww.ShowDialog();

                if (ww.actualizo == true) getList(TX_empleado.Tag.ToString());
            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir la pantalla de adicion:" + w, "alerta");
            }

        }

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool flag = false;
                string query = "";
                for (int i = 0; i < list.Items.Count; i++)
                {
                    ContentPresenter c = (ContentPresenter)list.ItemContainerGenerator.ContainerFromItem(list.Items[i]);
                    ToggleButton tb = c.ContentTemplate.FindName("btnYourButtonName", c) as ToggleButton;
                    { }

                    if (tb.IsChecked.Value)
                    {
                        query += "delete CtMae_EmpleadosNota where idrow='" + tb.Tag + "';";
                        flag = true;
                    }
                }
                if (flag == true)
                {
                    if (MessageBox.Show("Usted desea eliminar las notas seleccionadas?", "Eliminar Notas", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                        {
                            MessageBox.Show("Eliminacion exitosa", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            getList(TX_empleado.Tag.ToString());
                        }
                    }
                }
                else
                {
                    MessageBox.Show("seleccione las notas que desea eliminar", "Opcion", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("ERROR AL ELIMINAR");
            }
        }










    }
}
