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
    public partial class CreacionConceptos : Window
    {
        dynamic SiaWin;
        int idemp = 0;
        string cnEmp = "";

        public string informeName = "";
        public string informeCode = "";


        string[] concepto = new string[6];

        public CreacionConceptos()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            LoadConfig();
            cargarClase();
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
            TX_info.Text = informeName.Trim();
            cargarGrupos(informeCode);
            Tx_cod_inf.Text = informeCode;
        }

        public void cargarClase()
        {
            string cadena = "select rtrim(clase) as clase,rtrim(nom_clase) as nom_clase from NI_clases";
            DataTable dt = SiaWin.Func.SqlDT(cadena, "clases", idemp);
            CB_clase.ItemsSource = dt.DefaultView;

        }

        public void cargarGrupos(string codGrup)
        {
            try
            {
                string cadena = "SELECT grupo,nom_grupo from NIMae_grupo where cod_inf='"+codGrup.Trim()+"' ";
                DataTable dt = SiaWin.Func.SqlDT(cadena, "conceptos", idemp);
                dataGridGrupo.ItemsSource = dt.DefaultView;
            }
            catch (Exception w)
            {
                MessageBox.Show("Error al cargar los grupos:"+w);
            }
        }

        private void DataGridGrupo_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            if (dataGridGrupo.SelectedIndex>=0)
            {
                DataRowView row = (DataRowView)dataGridGrupo.SelectedItems[0];
                string codGrupo = row["grupo"].ToString();
                string codInfo = informeCode.ToString();
                cargarTipos(codGrupo, codInfo);

                clearCol(2);
                Tx_cod_grup.Text = row["grupo"].ToString();
                formarConcepto();
            }            
        }




        private void DataGridTipos_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            if (dataGridTipos.SelectedIndex>=0)
            {
                DataRowView row = (DataRowView)dataGridTipos.SelectedItems[0];
                string codTipo = row["tipo"].ToString();

                clearCol(3);
                Tx_cod_tip.Text = codTipo;
                
                sumaOrd(Tx_cod_grup.Text, Tx_cod_tip.Text, Tx_cod_inf.Text);
                formarConcepto();
            }
        }

        public void sumaOrd(string grupo,string tipo,string info)
        {
            string cadena = "select ISNULL(SUM(ord_imp),0) as suma  from NIcon_niif where grupo='" + grupo+"' and tipo='"+tipo+"' and cod_inf='"+info+"'";
            DataTable dt = SiaWin.Func.SqlDT(cadena, "Suma", idemp);

            int ord =  dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["suma"])+1 : 0;
            Tx_cod_ord.Text = ord.ToString("D3");
            
        }

        public void formarConcepto()
        {
            string cod_inf = string.IsNullOrEmpty(Tx_cod_inf.Text) ? "": Tx_cod_inf.Text;
            string cod_gru = string.IsNullOrEmpty(Tx_cod_grup.Text) ? "" : Tx_cod_grup.Text;
            string cod_tip = string.IsNullOrEmpty(Tx_cod_tip.Text) ? "" : Tx_cod_tip.Text;
            string cod_ord = string.IsNullOrEmpty(Tx_cod_ord.Text) ? "" : Tx_cod_ord.Text;

            Tx_codigoCta.Text = cod_inf + cod_gru + cod_tip + cod_ord;
        }


        public void clearCol(int tipo)
        {
            if (tipo==1)
            {
                dataGridTipos.ItemsSource = null;
                Tx_cod_grup.Text = "";
                Tx_cod_tip.Text = "";
                Tx_cod_ord.Text = "";
                Tx_codigoCta.Text = "";
                TXT_nameCon.Text = "";
                CB_clase.SelectedIndex = -1;
            }
            if (tipo == 2)
            {
                Tx_cod_ord.Text = "";
                Tx_cod_tip.Text = "";             
            }
            if (tipo == 3)
            {
                Tx_cod_ord.Text = "";                
            }
        }

        public void cargarTipos(string codGrup,string codInf)
        {
            try
            {
                string cadena = "SELECT tipo,nom_tipo from NI_tipos where grupo='"+codGrup+"' and cod_inf='"+codInf+"'";
                DataTable dt = SiaWin.Func.SqlDT(cadena, "Tipos", idemp);
                dataGridTipos.ItemsSource = dt.DefaultView;
            }
            catch (Exception w)
            {
                MessageBox.Show("Error al cargar los tipos:" + w);
            }
        }

        private void BTNcancelar_Click(object sender, RoutedEventArgs e)
        {

            clearCol(1);
        }

        private void BTNcrear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (validarCampo()== false)
                {
                    MessageBox.Show("llene todos los campos");
                    return;
                }

                string insert = "insert into NIcon_niif (cod_niif,nom_con,cod_inf,grupo,tipo,ord_imp,clase) values " +
                    "('"+ Tx_codigoCta.Text + "','"+ TXT_nameCon.Text + "','" + Tx_cod_inf.Text + "','" + Tx_cod_grup.Text + "','" + Tx_cod_tip.Text + "',"+1+","+CB_clase.SelectedValue+")";

                if (SiaWin.Func.SqlCRUD(insert, idemp) == true)
                {
                    MessageBox.Show("creacion de concpeto exitosa");
                    clearCol(1);
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al guardar el concepto:"+w);
            }
        }

        public bool validarCampo()
        {
            bool bandera = true;
            if (string.IsNullOrEmpty(Tx_cod_inf.Text)) bandera = false;
            if (string.IsNullOrEmpty(Tx_cod_grup.Text)) bandera = false;
            if (string.IsNullOrEmpty(Tx_cod_tip.Text)) bandera = false;
            if (string.IsNullOrEmpty(Tx_cod_ord.Text)) bandera = false;
            if (string.IsNullOrEmpty(Tx_codigoCta.Text)) bandera = false;
            if (string.IsNullOrEmpty(TXT_nameCon.Text)) bandera = false;
            if (CB_clase.SelectedIndex<0) bandera = false;
            return bandera;
        }


    }
}
