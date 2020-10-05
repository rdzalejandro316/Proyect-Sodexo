using System;
using System.Windows;
using System.Windows.Input;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Controls;

namespace SiasoftApp
{
    /// <summary>
    /// Lógica de interacción para Window4.xaml
    /// </summary>
    public partial class WindowBuscar : Window
    {
        DataTable dt = new DataTable();
        DataTable dt1 = new DataTable();
        private string codigo;
        private string nombre;

        private int idrowreturn;
        private bool isLoadFiltro = false;
        Datos DB = new Datos();
        public bool MostrarBuscar = false;
        public int IdRowReturn
        {
            set { idrowreturn = value; }
            get { return idrowreturn; }
        }
        public string Codigo
        {
            set { codigo = value; }
            get { return codigo; }
        }
        public string Nombre
        {
            set { nombre = value; }
            get { return nombre; }
        }
        public DataTable DtRow
        {
            set { dt1 = value; }
            get { return dt1; }
        }
        public DataTable Dt
        {
            set { dt = value; }
            get { return dt; }
        }
        string cmptabla; string cmpcodigo; string cmpnombre; string cmporden; string cmpIdRow; string _conexion; bool mostrartodo; string where; bool returnDt; bool cacheData; int maxRecords; string paramInit = ""; int BusinnesId = 0;
        public WindowBuscar(string CmpTabla, string CmpCodigo, string CmpNombre, string CmpOrden, string CmpIdRow, string CmpTitulo, string _Conexion, bool MostrarTodo, string Where, DataTable dt = null, bool ReturnDt = false, bool CacheData = false, int MaxRecords = 0, string ParamInit = "", int idEmp = 0)
        {
            //parametro ini se usa para enviar variables en el where ejemplo declare @tipo as int;set @tipo = 1 y esta variable es usada en el where ejemplo tipo_con=@tipo
            InitializeComponent();
            cmptabla = CmpTabla.Trim();
            cmpcodigo = CmpCodigo.Trim();
            cmpnombre = CmpNombre.Trim();
            cmporden = CmpOrden.Trim();
            cmpIdRow = CmpIdRow;
            returnDt = ReturnDt;
            cacheData = CacheData;
            _conexion = _Conexion;
            mostrartodo = MostrarTodo;
            maxRecords = MaxRecords;
            where = Where.Trim();
            this.Title = CmpTitulo.Trim();
            BusinnesId = idEmp;
            //if (idEmp == 0) BusinnesId = ((Inicio)Application.Current.MainWindow)._BusinessId;
            //MessageBox.Show("windowsbuscar:"+BusinnesId.ToString());
            if (MostrarTodo == true)
            {
                if (!string.IsNullOrEmpty(where.Trim()))
                {
                    where = " where " + where;
                    //                    MessageBox.Show(where);
                }
                if (!string.IsNullOrEmpty(ParamInit))
                {
                    paramInit = ParamInit + ";";
                }

                dataGrid.ItemsSource = GetDataTable(where).DefaultView;
                //BtnBuscar.Visibility = Visibility.Collapsed;
                //TxtShear.Visibility = Visibility.Collapsed;
                dataGrid.SelectedIndex = 0;
                dataGrid.Focus();
                
                if (dataGrid.Items.Count < 20)
                {
                    TxtShear.Visibility = Visibility.Collapsed;
                    BtnBuscar.Visibility = Visibility.Collapsed;
                }
                if(MostrarBuscar==true)
                {
                    TxtShear.Visibility = Visibility.Visible;
                    BtnBuscar.Visibility = Visibility.Visible;
                    TxtShear.Focus();
                }
            }
            else
            {
                TxtShear.Focus();
            }
            //dataGrid.PreviewKeyDown += new KeyEventHandler(mainGrid_PreviewKeyDown);
        }
        void mainGrid_PreviewKeyDown(object sender, KeyEventArgs args)
        {
            if (args.Key == Key.Enter)
            {
                if (returnDt == true) SelectItem();
                args.Handled = true;
            }
            if (args.Key == Key.Left)
            {
                
                if(TxtShear.Visibility==Visibility.Visible)
                {
                
                    TxtShear.Focus();
                    args.Handled = true;
                    return;
                }
                if (mostrartodo == false)
                {
                    //TxtShear.Focus();
                    //args.Handled = true;
                }
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                dt.Clear();
                if (TxtShear.Text.Trim() == string.Empty) TxtShear.Focus();
                if (TxtShear.Text.Trim() == string.Empty) return;
                string bb = TxtShear.Text.Trim();
                dataGrid.ItemsSource = null;
                string www = TxtConvertText(bb);
                dataGrid.ItemsSource = GetDataTable(" where " + www).DefaultView;
                if (dataGrid.Items.Count == 0) return;
                //dataGrid.SelectedItem = dataGrid.Items[1];
                dataGrid.Focus();
                //dataGrid.SelectedIndex = 0;
                var uiElement = e.OriginalSource as UIElement;
                dataGrid.SelectedItem = dataGrid.Items[0];
                uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.Items[dataGrid.SelectedIndex], dataGrid.Columns[0]);
                dataGrid.CommitEdit();
                dataGrid.SelectedIndex = dataGrid.SelectedIndex;
                dataGrid.Focus();
                //dataGrid.ScrollIntoView(dataGrid.SelectedItem, dataGrid.Columns[0]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        public DataTable GetDataTable(string _where)
        {

            try
            {
                if (mostrartodo == false)
                {
                    if (!string.IsNullOrEmpty(_where))
                    {
                        if (!string.IsNullOrEmpty(where)) _where = _where + " and " + where;
                    }
                }
                string maxrec = "";
                //paramInit +
                if (maxRecords > 0) maxrec = " top " + maxRecords.ToString() + " ";
                string sql = paramInit + " select " + maxrec + cmpIdRow + " as idrow," + cmpcodigo + " as codigo," + cmpnombre + " as nombre  from " + cmptabla + _where + " order by " + cmporden;

                dt = DB.SqlDT(sql, "_Tmp", BusinnesId);
                
                //MessageBox.Show(sql);
                //SqlConnection conn1 = new SqlConnection(_conexion);
                //SqlCommand cmd1 = new SqlCommand(sql, conn1);
                //conn1.Open();
                //SqlDataReader dr = cmd1.ExecuteReader();
                //dt.Load(dr);
                TxtTotal.Content = "Total registros :" + dt.Rows.Count;
                //conn1.Close();
            }
            catch (SqlException SQLex)
            {
                MessageBox.Show("Error:" + SQLex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message);
            }
            return dt;
        }
        public DataTable GetRow(int _idrow)
        {
            try
            {
                string sql = "select *  from " + cmptabla + " where " + cmpIdRow + "=" + _idrow.ToString();
                dt1 = DB.SqlDT(sql, "_Tmp", BusinnesId);

                //SqlConnection conn1 = new SqlConnection(_conexion);
                //SqlCommand cmd1 = new SqlCommand(sql, conn1);
                //conn1.Open();
                //SqlDataReader dr = cmd1.ExecuteReader();
                //dt1.Load(dr);
                //conn1.Close();
            }
            catch (SqlException SQLex)
            {
                MessageBox.Show("Error:" + SQLex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message);
            }
            return dt1;
        }
        private string TxtConvertText(string txt)
        {
            string s = txt;
            // Split string on spaces.
            int inicount = 0;
            string cadena = "";
            string cadenaOR = "";
            string[] words = s.Split(' ');
            foreach (string word in words)
            {
                if (inicount == 0)
                {
                    cadena = "rtrim(" + cmpcodigo + ") like '%" + word + "%'";
                    cadenaOR = "rtrim(" + cmpnombre + ") like '%" + word + "%'";
                }
                else
                {
                    cadena = cadena + " and rtrim(" + cmpcodigo + ") like '%" + word + "%'";
                    cadenaOR = cadenaOR + " and rtrim(" + cmpnombre + ") like '%" + word + "%'";
                }
                inicount = inicount + 1;
            }
            return "("+cadena + " or " + cadenaOR+")";
        }
        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectItem();
            e.Handled = true;
        }
        private void SelectItem()
        {
            DataRowView row = (DataRowView)dataGrid.SelectedItems[0];
            if (row != null)
            {
                int nPnt = Int32.Parse(row[0].ToString());

                this.Codigo = row["codigo"].ToString();
                this.Nombre = row["nombre"].ToString();
                this.IdRowReturn = nPnt;
                if (returnDt == true && IdRowReturn > 0)
                {
                    this.DtRow = GetRow(IdRowReturn);
                    //if (mostrartodo == true) this.Dt = dt;
                }
            }
            else
            {
                this.IdRowReturn = -1;
            }
            this.Close();
        }
        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (dataGrid.Items.Count > 0)
                {
                    SelectItem();

                    e.Handled = true;

                }
            }
            if (e.Key == Key.Enter)
            {
                SelectItem();

                e.Handled = true;

            }
            if (e.Key == Key.Left)
            {
                //if (mostrartodo == false)
                //{
                    TxtShear.Focus();
                    e.Handled = true;
                //}
            }

        }
        private void TxtShear_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
                e.Handled = true;
            }
            if (e.Key == Key.Enter)
            {
                BtnBuscar.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                e.Handled = true;

            }
            if (e.Key == Key.Down)
            {
                if (dataGrid.Items.Count == 0) return;
                dataGrid.Focus();
                var uiElement = e.OriginalSource as UIElement;
                dataGrid.SelectedItem = dataGrid.Items[0];
                uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.Items[dataGrid.SelectedIndex], dataGrid.Columns[0]);
                dataGrid.CommitEdit();
                dataGrid.SelectedIndex = dataGrid.SelectedIndex;
                e.Handled = true;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
                e.Handled = true;
            }
            if(e.Key==Key.F8)   // exit emergencia
            {
                this.Close();
                e.Handled = true;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {


            if (where.Contains("%"))
            {
                isLoadFiltro = true;
                Buscar();
            }

        }
        private void Buscar()
        {
            try
            {
                dt.Clear();
                dataGrid.ItemsSource = null;
                
                dataGrid.ItemsSource = GetDataTable(" where " + where).DefaultView;
                if (dataGrid.Items.Count == 0) return;
                //dataGrid.SelectedItem = dataGrid.Items[1];
                dataGrid.Focus();
                //dataGrid.SelectedIndex = 0;

                dataGrid.SelectedItem = dataGrid.Items[0];

                dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.Items[dataGrid.SelectedIndex], dataGrid.Columns[0]);
                dataGrid.CommitEdit();
                dataGrid.SelectedIndex = dataGrid.SelectedIndex;
                dataGrid.Focus();
                //dataGrid.ScrollIntoView(dataGrid.SelectedItem, dataGrid.Columns[0]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }
    }
}
