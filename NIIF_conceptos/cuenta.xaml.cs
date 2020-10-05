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
using System.Windows.Shapes;

namespace NIIF_conceptos
{


    public partial class cuenta : Window
    {
        dynamic SiaWin;
        int idemp = 0;
        string cnEmp = "";

        public string codigo_info = "";
        public string nombre_info = "";
        public string codigo_conc = "";
        public string nombre_conc = "";


        string valueOld = "";

        public cuenta()
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
            try
            {
                TX_nombreInf.Text = nombre_info.Trim();
                LB_nombreInf.Text = codigo_info;

                TX_nombreCon.Text = nombre_conc.Trim();
                LB_nombreCon.Text = codigo_conc;

                cargarGrid();
            }
            catch (Exception w)
            {
                MessageBox.Show("ERROR QQQ" + w);
            }

        }

        public void cargarGrid()
        {
            try
            {
                string cadena = "select relacion.idrow,relacion.cod_cta,cuentas.nom_cta,relacion.saldoini,relacion.debito,relacion.credito,relacion.saldofin,relacion.compara,relacion.movi from NIRel_Niif as relacion ";
                cadena += "inner join Comae_cta as cuentas on relacion.cod_cta = cuentas.cod_cta ";
                cadena += "where cod_inf='" + LB_nombreInf.Text + "' and cod_con='" + LB_nombreCon.Text + "' ";
                DataTable dt = SiaWin.Func.SqlDT(cadena, "conceptos", idemp);
                dataGridCxC.ItemsSource = dt.DefaultView;
            }
            catch (Exception)
            {
                MessageBox.Show("error al cargar grid");
            }
        }

        private void DataGridCxC_CurrentCellEndEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs e)
        {
            try
            {
                SfDataGrid grid = sender as SfDataGrid;
                int columnindex = grid.ResolveToGridVisibleColumnIndex(e.RowColumnIndex.ColumnIndex);
                var column = grid.Columns[columnindex];

                if (column.MappingName == "cod_cta")
                {
                    DataRowView row = (DataRowView)dataGridCxC.SelectedItems[0];
                    //string valueold = row["cod_cta"].ToString(); ;

                    if (GetCuenta(row["cod_cta"].ToString().Trim()) == false)
                    {
                        MessageBox.Show("error no existe la cuenta");
                        row["cod_cta"] = valueOld;
                        return;
                    }

                    if (GetExistenciaTabla(LB_nombreInf.Text.Trim(), LB_nombreCon.Text.Trim(), row["cod_cta"].ToString().Trim()) == true)
                    {
                        MessageBox.Show("error la cuenta ya existe en el informe y el concepto");
                        row["cod_cta"] = valueOld;
                        return;
                    }

                    string id = row["idrow"].ToString();
                    string cuenta = row["cod_cta"].ToString();
                    string query = "update NIRel_Niif set cod_cta='" + cuenta + "' where idrow='" + id + "' ";
                    if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                        row["nom_cta"] = SiaWin.Func.cmp("comae_cta", "cod_cta", "nom_cta", Convert.ToInt32(cuenta), idemp);
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al editar!!@" + w);
            }


        }

        public bool GetCuenta(string cuenta)
        {
            bool flag = false;
            string query = "select * from Comae_cta where cod_cta='" + cuenta + "' ";
            DataTable dt = SiaWin.Func.SqlDT(query, "cuentas", idemp);
            if (dt.Rows.Count > 0) flag = true;
            return flag;
        }

        public bool GetExistenciaTabla(string cod_inf, string cod_con, string cod_cta)
        {
            bool flag = false;
            string query = "select * from NIRel_Niif where cod_inf='" + cod_inf + "' and cod_con='" + cod_con + "' and cod_cta='" + cod_cta + "' ";
            DataTable dt = SiaWin.Func.SqlDT(query, "tabla", idemp);
            if (dt.Rows.Count > 0) flag = true;
            return flag;
        }

        private void DataGridCxC_CurrentCellBeginEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellBeginEditEventArgs e)
        {
            DataRowView row = (DataRowView)dataGridCxC.SelectedItems[0];
            valueOld = row["cod_cta"].ToString();
        }

        private void DataGridCxC_CurrentCellValueChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellValueChangedEventArgs e)
        {
            try
            {
                SfDataGrid grid = sender as SfDataGrid;
                int columnindex = grid.ResolveToGridVisibleColumnIndex(e.RowColumnIndex.ColumnIndex);
                var column = grid.Columns[columnindex];
                if (column.GetType() == typeof(GridCheckBoxColumn))
                {
                    DataRowView row = (DataRowView)dataGridCxC.SelectedItems[0];
                    string id = row["idrow"].ToString();
                    int check = Convert.ToInt32(row[columnindex]);
                    string query = "update NIRel_Niif set " + column.MappingName + "=" + check + " where idrow='" + id + "'";
                    if (SiaWin.Func.SqlCRUD(query, idemp) == false) MessageBox.Show("Fallo la actualizacion de la tabla");
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al #30000:" + w);
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                string cmptabla = "comae_cta"; string cmpcodigo = "cod_cta"; string cmpnombre = "nom_cta"; string cmporden = "cod_cta"; string cmpidrow = "idrow"; string cmptitulo = "Maestra de Cuentas"; string cmpconexion = cnEmp; Boolean mostrartodo = false; string cmpwhere = "";
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

                if (idr > 0) insertCuenta(code.Trim());
            }
            catch (Exception w)
            {
                MessageBox.Show("Error en el add:" + w);
            }

        }

        public void insertCuenta(string code)
        {
            try
            {
                if (GetExistenciaTabla(LB_nombreInf.Text.Trim(), LB_nombreCon.Text.Trim(), code) == true)
                {
                    MessageBox.Show("error la cuenta ya existe en el informe y el concepto");
                    return;
                }

                string query = "insert into NIRel_Niif(cod_inf,cod_con,cod_cta)  values ('" + LB_nombreInf.Text.Trim() + "','" + LB_nombreCon.Text.Trim() + "','" + code + "')";
                if (SiaWin.Func.SqlCRUD(query, idemp) == true) cargarGrid();
            }
            catch (Exception w) { MessageBox.Show("error al insertar cuenta:" + w); }
        }

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridCxC.SelectedIndex >= 0)
            {
                DataRowView row = (DataRowView)dataGridCxC.SelectedItems[0];

                MessageBoxResult result = MessageBox.Show("USTED DESEA ELIMINAR LA CUENTA " + row["cod_cta"].ToString().Trim() + " ", "Siasoft?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {                    
                    string id = row["idrow"].ToString();
                    string query = "delete NIRel_Niif where idrow='" + id + "' ";
                    if (SiaWin.Func.SqlCRUD(query, idemp) == true) cargarGrid();
                }
            }
            else MessageBox.Show("seleciona una cuenta para eliminar");
        }



    }
}
