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
    
    public partial class editar : Window
    {

        dynamic SiaWin;
        int idemp = 0;
        string cnEmp = "";

        public string id_con = "";
        public string nom_con = "";

        public bool actualizo = false;

        public string añoNota = "";

        public editar()
        {
            InitializeComponent();

            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            LoadConfig();
            pnt();
        }

        public void pnt()
        {
            this.MinHeight = 250;
            this.MaxHeight = 250;
            this.MinWidth = 500;
            this.MaxWidth = 500;
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
            TXTcodigo.Text = id_con.Trim();   
            TXnombre.Text = nom_con.Trim();


            cargarClase();
            selecClase(id_con.Trim());
        }


        public void cargarClase()
        {
            string cadena = "select rtrim(clase) as clase,rtrim(nom_clase) as nom_clase from NI_clases";
            DataTable dt = SiaWin.Func.SqlDT(cadena, "clases", idemp);
            CB_clase.ItemsSource = dt.DefaultView;
        }

        public void selecClase(string codniif)
        {
            string cadena = "select clase from NIcon_niif where cod_niif='"+codniif+"' ";
            DataTable dt = SiaWin.Func.SqlDT(cadena, "clases", idemp);
            CB_clase.SelectedValue = dt.Rows[0]["clase"];
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "update NIcon_niif set nom_con='" + TXnombre.Text + "',clase=" + CB_clase.SelectedValue + ",nota='" + TX_nota.Text.Trim() + "' where cod_niif='"+TXTcodigo.Text.Trim()+"' ";

                if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                {
                    MessageBox.Show("actualizacion exitosa");
                    actualizo = true;
                    this.Close();
                }                                
            }
            catch (Exception)
            {
                MessageBox.Show("error al actualar");
            }
        }

        private void SearchAño_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                string cmptabla = "NI_Notas"; string cmpcodigo = "nota"; string cmpnombre = "descrip"; string cmporden = "nota"; string cmpidrow = "idrow"; string cmptitulo = "Maestra de Notas"; string cmpconexion = cnEmp; Boolean mostrartodo = true; string cmpwhere = "ano='"+ añoNota + "'";
                int idr = 0; string code = ""; string nom = "";

                dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, SiaWin.Func.DatosEmp(idemp), mostrartodo, cmpwhere, idEmp: idemp);
                winb.ShowInTaskbar = false;
                winb.Owner = Application.Current.MainWindow;
                winb.Height = 400;
                winb.ShowDialog();
                idr = winb.IdRowReturn;
                code = winb.Codigo;
                nom = winb.Nombre;
                winb = null;

                if (idr > 0)
                {
                    TX_nota.Text = code.Trim();
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("Error en el add:" + w);
            }
        }




    }
}
