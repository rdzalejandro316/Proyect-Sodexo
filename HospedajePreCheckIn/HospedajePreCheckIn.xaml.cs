using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using HospedajePreCheckIn;
using HospedajePreCheckIn.Converter;
using Microsoft.Win32;
using Syncfusion.XlsIO;
using System.IO;
using Syncfusion.UI.Xaml.Grid.Converter;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9672,"HospedajePreCheckIn");
    //Sia.TabU(9672);


    public partial class HospedajePreCheckIn : UserControl
    {
        dynamic SiaWin;
        dynamic tabitem;
        public int idemp = 0;
        string cnEmp = "";
        string codemp = string.Empty;        
        string MsgError = "";

        List<Slots> ColuHeadears = new List<Slots>();
        List<claseMes> columnas = new List<claseMes>();

        public HospedajePreCheckIn(dynamic tabitem1)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            tabitem = tabitem1;
            tabitem.Title = "Hospedaje-PreChek-In";
            //tabitem.Logo(9, ".png");
            tabitem.MultiTab = false;
            idemp = SiaWin._BusinessId;
            
            LoadConfig();
            loadConfigMap();           
        }

        
        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                codemp = foundRow["BusinessCode"].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                tabitem.Logo(idLogo, ".png");
                tabitem.MultiTab = false;
                FechaIni.Text = DateTime.Now.ToShortDateString();
                FechaIniDetalle.Text = DateTime.Now.ToShortDateString();

                DataTable dt = SiaWin.Func.SqlDT("select rtrim(puntoventacodigo) as puntoventacodigo,rtrim(puntoventacodigo)+'-'+rtrim(puntoventanombre) as puntoventanombre from CtMae_PuntosDeVenta where Estado=1;", "pv", idemp);
                if (dt.Rows.Count > 0)
                {
                    CBpunto.ItemsSource = dt.DefaultView;
                    CBpunto.DisplayMemberPath = "puntoventanombre";
                    CBpunto.SelectedValuePath = "puntoventacodigo";
                    CBpunto.SelectedIndex = 1;
                }

                loadCOncepto();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void loadCOncepto()
        {

            Config.loadColores();
            dataGridOcupacion.UpdateLayout();

            CHDisponible.Background = Config.ToSolidColorBrush(Config.ColorD);
            CHOcupacion.Background = Config.ToSolidColorBrush(Config.ColorO);
            CHSalida.Background = Config.ToSolidColorBrush(Config.ColorS);
            CHPreche.Background = Config.ToSolidColorBrush(Config.ColorP);
            CHReservado.Background = Config.ToSolidColorBrush(Config.ColorR);
            CHLimpiesa.Background = Config.ToSolidColorBrush(Config.ColorL);
            CHMante.Background = Config.ToSolidColorBrush(Config.ColorM);
            CHCancel.Background = Config.ToSolidColorBrush(Config.ColorC);
            CHIncativa.Background = Config.ToSolidColorBrush(Config.ColorI);
        }

        private void CBpunto_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                DataTable dtpv = SiaWin.Func.SqlDT("select * From CtMae_PuntosDeVenta where PuntoVentaCodigo='" + (sender as ComboBox).SelectedValue.ToString() + "'", "pv", idemp);
                if (dtpv.Rows.Count > 0)
                {
                    (sender as ComboBox).Tag = dtpv.Rows[0]["CampamentoCodigo"].ToString();
                    dataGridOcupacion.ItemsSource = null;                    
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("errro al buscar codigo:" + w);
            }
        }

        public void loadConfigMap()
        {
            try
            {
                string sqlconfig = "select  BackgroundSalHoy,ForegroundSalHoy,BackgroundLHoy,ForegroundLHoy,";
                sqlconfig += "BackgroundDisp,ForegroundDisp,BackgroundOcup,ForegroundOcup,BackgroundRes,";
                sqlconfig += "ForegroundRes,BackgroundMant,ForegroundMant,BackgroundLim,ForegroundLim,BackgroundToCam,ForegroundToCam,StackedHeaderRow,GridTotales ";
                sqlconfig += "from CtMae_ConfigMapaVivienda;";

                DataTable dt = SiaWin.Func.SqlDT(sqlconfig, "campamento", idemp);
                if (dt.Rows.Count > 0)
                {

                    GridSalenHoy.Background = Config.ToSolidColorBrush(dt.Rows[0]["BackgroundSalHoy"].ToString().Trim());
                    SalenHoy.Foreground = Config.ToSolidColorBrush(dt.Rows[0]["ForegroundSalHoy"].ToString().Trim());

                    GridLleganHoy.Background = Config.ToSolidColorBrush(dt.Rows[0]["BackgroundLHoy"].ToString().Trim());
                    LleganHoy.Foreground = Config.ToSolidColorBrush(dt.Rows[0]["ForegroundLHoy"].ToString().Trim());

                    GridDisponibles.Background = Config.ToSolidColorBrush(dt.Rows[0]["BackgroundDisp"].ToString().Trim());
                    Disponibles.Foreground = Config.ToSolidColorBrush(dt.Rows[0]["ForegroundDisp"].ToString().Trim());

                    GridOcupacion.Background = Config.ToSolidColorBrush(dt.Rows[0]["BackgroundOcup"].ToString().Trim());
                    Ocupacion.Foreground = Config.ToSolidColorBrush(dt.Rows[0]["ForegroundOcup"].ToString().Trim());

                    GridReservada.Background = Config.ToSolidColorBrush(dt.Rows[0]["BackgroundRes"].ToString().Trim());
                    Reservada.Foreground = Config.ToSolidColorBrush(dt.Rows[0]["ForegroundRes"].ToString().Trim());

                    GridMantenimiento.Background = Config.ToSolidColorBrush(dt.Rows[0]["BackgroundMant"].ToString().Trim());
                    Mantenimiento.Foreground = Config.ToSolidColorBrush(dt.Rows[0]["ForegroundMant"].ToString().Trim());
                                        
                    
                    GridTotalLimpieza.Background = Config.ToSolidColorBrush(dt.Rows[0]["BackgroundLim"].ToString().Trim());
                    TotalLimpieza.Foreground = Config.ToSolidColorBrush(dt.Rows[0]["ForegroundLim"].ToString().Trim());

                    GridTotalCamas.Background = Config.ToSolidColorBrush(dt.Rows[0]["BackgroundToCam"].ToString().Trim());
                    TotalCamas.Foreground = Config.ToSolidColorBrush(dt.Rows[0]["ForegroundToCam"].ToString().Trim());
                    


                    Config.StackHeadRow = Convert.ToBoolean(dt.Rows[0]["StackedHeaderRow"]);

                    if (!Config.StackHeadRow)
                    {
                        dataGridOcupacion.StackedHeaderRows.Clear();
                    }
                    else
                    {
                        HeaderGrid();
                    }

                    bool gridTot = Convert.ToBoolean(dt.Rows[0]["GridTotales"]);
                    GridTotales.Visibility = gridTot ? Visibility.Visible : Visibility.Hidden;
                    if (gridTot)
                    {
                        BtnVisiTo.Visibility = Visibility.Hidden;
                        Grid.SetRow(GridGrilla, 2);
                    }
                    else
                    {
                        BtnVisiTo.Visibility = Visibility.Visible;
                        Grid.SetRow(GridGrilla, 1);
                        Grid.SetRowSpan(GridGrilla, 2);
                    }
                }


            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar:" + w);
            }
        }

        private void BtnConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Configuracion win = new Configuracion();
                win.ShowInTaskbar = false;
                win.Owner = Application.Current.MainWindow;
                win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                win.ShowDialog();

                if (win.actualizo)
                {
                    loadConfigMap();
                    loadCOncepto();
                }
                //Asignacion win = new Asignacion();
                //win.pv = "02";
                //win.campamento = "01";
                //win.ShowInTaskbar = false;
                //win.Owner = Application.Current.MainWindow;
                //win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                //win.ShowDialog();



            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir la configuracion");
            }            
        }

        private void BtnVisiTo_Click(object sender, RoutedEventArgs e)
        {
            string tag = (sender as Button).Tag.ToString();
            if (tag=="A")
            {
                (sender as Button).Tag = "B";
                GridTotales.Visibility = Visibility.Visible;
                Grid.SetRow(GridGrilla, 2);
            }
            else
            {
                (sender as Button).Tag = "A";
                GridTotales.Visibility = Visibility.Hidden;
                Grid.SetRow(GridGrilla, 1);
                Grid.SetRowSpan(GridGrilla, 2);
            }            
        }


        #region tab precheck in
        private async void BtnConsulta_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                sfBusyIndicator.IsBusy = true;
                dataGridPreCheckIn.ClearFilters();
                var slowTask = Task<DataTable>.Factory.StartNew(() => LoadData(source.Token), source.Token);
                await slowTask;
                if (((DataTable)slowTask.Result).Rows.Count > 0)
                {
                    //dataGridCxC.ItemsSource = ((DataTable)slowTask.Result).DefaultView;
                    dataGridPreCheckIn.ItemsSource = ((DataTable)slowTask.Result).DefaultView;
                    Tx_Total.Text = ((DataTable)slowTask.Result).Rows.Count.ToString();
                }
                else
                {
                    Tx_Total.Text = "0";
                }
                sfBusyIndicator.IsBusy = false;
            }
            catch (Exception w)
            {
                MessageBox.Show("··:" + w);
                dataGridPreCheckIn.ClearFilters();
                sfBusyIndicator.IsBusy = false;
                Tx_Total.Text = "0";
            }

        }

        private DataTable LoadData(CancellationToken cancellationToken)
        {
            DataTable dt = new DataTable();
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT NitEmpresaExterna, DocumentoConsumidor, rtrim(PrimerNombre)+' '+rtrim(SegundoNombre)+' '+rtrim(PrimerApellido)+' ' +rtrim(SegundoApellido) as nombre, CorreoElectronico, FechaIngreso, FechaFinalEstadia, CodigoSucursal, Categoria, Subcategoria, clientes.id_cliente,precheckin.fecha_aded,idrow,precheckin.Estado,cod_vivienda,id_habitacion,precheckin.id_cama,cancelacion,message_cancelacion,");
                sb.Append(" Empleados.id_cliente,clientes.nom_cliente,clientes.ID_CCO,Cent_Costos.nom_cco,Cent_Costos.id_categoria,Categorias.nom_categoria ");
                sb.Append(" FROM PreCheckIn ");
                sb.Append(" inner join Empleados on PreCheckIn.DocumentoConsumidor = Empleados.Cedula ");
                sb.Append(" inner join Clientes on Clientes.ID_CLIENTE = Empleados.id_cliente ");
                sb.Append(" inner join Cent_Costos on Cent_Costos.id_cco = Clientes.ID_CCO ");
                sb.Append(" inner join Categorias on Categorias.id_categoria = Cent_Costos.id_categoria ");
                sb.Append(" where precheckin.estado=0 order by fechaingreso");
                
                String query = "select * From PreCheckIn  where estado=0  order by fechaingreso";
                dt = SiaWin.Func.SqlDT(sb.ToString(), "precheckin", 0);
            }
            catch (Exception e)
            {
                MessageBox.Show("eee:" + e);
                return null;
            }
            return dt;
        }

        private void BtnAsignar_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridPreCheckIn.SelectedIndex < 0) return;
            //Sia.PublicarPnt(9585,"HospedajePreCheckIn");
            try
            {

                DataRowView row = (DataRowView)dataGridPreCheckIn.SelectedItems[0];
                if (row == null)
                {
                    MessageBox.Show("Registro sin datos");
                    return;
                }

                string id = row["idrow"].ToString();
                string cedula = row["documentoconsumidor"].ToString();
                string nombre = row["nombre"].ToString();
                //string nombre2 = row[8].ToString();
                //string apellido1 = row[9].ToString();
                //string apellido2 = row[10].ToString();
                string fechaing = row["fechaingreso"].ToString();
                string fechasal = row["fechafinalestadia"].ToString();

                AsignarVivienda WinAsignar = new AsignarVivienda();
                WinAsignar.cedula_asi = cedula;
                WinAsignar.nombre_emp = nombre;
                WinAsignar.fec_ing = System.DateTime.Now.ToString();
                WinAsignar.fec_sal = DateTime.Now.ToString();
                WinAsignar.idrowPrecke = id;
                //MessageBox.Show("a1");
                //DateTime dateTime = DateTime.Parse(fechaing);
                //MessageBox.Show("MyDateTime:"+ dateTime);

                //WinAsignar.fec_ing = Convert.ToDateTime(fechaing);

                //WinAsignar.LblNombre.Content = nombre1.Trim() + " " + nombre2.Trim() + apellido1.Trim() + " " + apellido2.Trim();
                //WinAsignar.LblFechaIngreso.Content = fechaing;
                //WinAsignar.LblFechaSalida.Content = fechasal;

                WinAsignar.ShowInTaskbar = false;
                WinAsignar.Owner = Application.Current.MainWindow;
                WinAsignar.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                WinAsignar.ShowDialog();

                if (WinAsignar.actualizo == true)
                {
                    actualizarGrid(row, WinAsignar);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("arui:" + ex.Message);
            }

            //                var dr1 = dataGridCxC.SelectedItems;

        }

        public void actualizarGrid(DataRowView row, AsignarVivienda editar)
        {
            row["ViviendaCodigo"] = editar.Cbx_vivienda.SelectedValue;
            row["HabitacionCodigo"] = editar.Cbx_habitacion.SelectedValue;
            row["CamaCodigo"] = editar.Cbx_cama.SelectedValue;
            dataGridPreCheckIn.View.Refresh();
        }


        private void dataGridPreCheckIn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataRowView row = (DataRowView)dataGridPreCheckIn.SelectedItems[0];
            if (row == null)
            {
                MessageBox.Show("Registro sin datos");
                return;
            }
            string column = ((SfDataGrid)sender).CurrentColumn.MappingName;
            string idrow = row["idrow"].ToString();
            //MessageBox.Show(idrow);
            //string codvivienda = row["cod_vivienda"].ToString();
            //string codhabitacion = row["id_habitacion"].ToString();
            //string id_cama = row["id_cama"].ToString();
            //string id_esthab = row[column].ToString();
            string estado = row["estado"].ToString();
            string id = row["idrow"].ToString();
            string cedula = row["documentoconsumidor"].ToString();
            string nombre = row["nombre"].ToString();
            //string nombre2 = row[8].ToString();
            //string apellido1 = row[9].ToString();
            //string apellido2 = row[10].ToString();
            string fechaing = row["fechaingreso"].ToString();
            string fechasal = row["fechafinalestadia"].ToString();

            AsignarVivienda WinAsignar = new AsignarVivienda();
            WinAsignar.cedula_asi = cedula;
            WinAsignar.nombre_emp = nombre;
            WinAsignar.fec_ing = System.DateTime.Now.ToString();
            WinAsignar.fec_sal = DateTime.Now.ToString();
            WinAsignar.idrowPrecke = id;
            WinAsignar.ShowInTaskbar = false;
            WinAsignar.Owner = Application.Current.MainWindow;
            WinAsignar.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            WinAsignar.ShowDialog();



            if (estado == "1") return;
            StringBuilder sbsql = new StringBuilder();


        }
        #endregion


        #region Mapa de vivienda

        private async void BtrnConsularOcupacion_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(FechaIni.Text))
                {
                    MessageBox.Show("seleccione un fecha de corte", "alerta", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (CBpunto.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione un punto de venta", "alerta", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }



                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;

                dataGridOcupacion.ItemsSource = null;
                dataGridOcupacion.ClearFilters();

                if (dataGridOcupacion.Columns.Count > 5)
                {
                    int rr = dataGridOcupacion.Columns.Count;
                    foreach (var col in dataGridOcupacion.Columns.Reverse())
                    {
                        if (rr > 5)
                        {
                            dataGridOcupacion.Columns.Remove(col);
                        }
                        rr--;
                    }
                }


                sfBusyIndicatorVivieanda.IsBusy = true;

                string fecha = FechaIni.Text;
                string pv = CBpunto.SelectedValue.ToString();

                MsgError = "";
                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadDataOcupacion(fecha, pv, source.Token), source.Token);
                await slowTask;
                if (MsgError != "")
                {
                    sfBusyIndicatorVivieanda.IsBusy = false;
                    MessageBox.Show("Error:" + MsgError);
                    return;
                }
                if (slowTask == null) return;

                dataGridOcupacion.DataContext = ((DataSet)slowTask.Result).Tables[0];

                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {                    

                    foreach (System.Data.DataColumn dc in ((DataSet)slowTask.Result).Tables[0].Columns)
                    {
                        var field1 = dc.ColumnName.ToString();
                        char dig = Convert.ToChar(field1.Substring(0, 1));
                        CultureInfo ci = new CultureInfo("Es-Es");

                        //hoy
                        if (char.IsDigit(dig))
                        {
                            DateTime dtime = Convert.ToDateTime(field1.ToString());
                            //hoy
                            string DiaSemana = ci.DateTimeFormat.GetDayName(dtime.DayOfWeek).ToString().Trim();
                            string nomMes = dtime.ToString("MMMM").Substring(0, 1).ToLower().Trim() + dtime.ToString("MMMM").Substring(1, 2).Trim();
                            string daycolumn = DiaSemana.Substring(0, 3).Trim() + Environment.NewLine + dtime.Day.ToString().Trim() + Environment.NewLine + nomMes;

                            //string daycolumn = DiaSemana + Environment.NewLine + dtime.Day.ToString().Trim() + Environment.NewLine + nomMes;


                            //lista para solo agregar los meses y luegos agruparlos
                            ColuHeadears.Add(new Slots() { Date = dtime.Month, Year = dtime.Year });

                            if (dtime == FechaIni.SelectedDate)
                            {
                                var template = new DataTemplate();
                                template.VisualTree = new FrameworkElementFactory(typeof(TextBlock));
                                template.VisualTree.SetValue(TextBlock.ForegroundProperty, Config.ToSolidColorBrush("#03A9F4"));
                                //template.VisualTree.SetValue(TextBlock.BackgroundProperty, Brushes.Gray);
                                template.VisualTree.SetValue(TextBlock.TextProperty, daycolumn);
                                dataGridOcupacion.Columns.Add(new GridTextColumn() { HeaderText = daycolumn, HeaderTemplate = template, MappingName = field1.ToString(), Width = 35, TextAlignment = TextAlignment.Center, HorizontalHeaderContentAlignment = HorizontalAlignment.Center, AllowFiltering = false });
                            }
                            else
                            {
                                dataGridOcupacion.Columns.Add(new GridTextColumn() { HeaderText = daycolumn, MappingName = field1.ToString(), Width = 35, TextAlignment = TextAlignment.Center, AllowFiltering = false });
                            }
                        }
                    }


                    #region header para info de mes
                    dataGridOcupacion.StackedHeaderRows.Clear();

                    var query = from c in ColuHeadears
                                group c by new
                                {
                                    c.Date,
                                    c.Year,
                                } into gcs
                                select new Slots()
                                {
                                    Date = gcs.Key.Date,
                                    Year = gcs.Key.Year,
                                };

                    foreach (var item in query)
                    {

                        int mes = Convert.ToInt32(item.Date);
                        int año = Convert.ToInt32(item.Year);
                        string valor = "";


                        DateTime dtmes = new DateTime();

                        for (int i = 1; i <= DateTime.DaysInMonth(año, mes); i++)
                        {
                            DateTime dt = new DateTime(año, mes, i);
                            dtmes = new DateTime(año, mes, i);
                            string fec = dt.ToString("yyyy-MM-dd");
                            valor += i < DateTime.DaysInMonth(DateTime.Now.Year, mes) ?
                                        fec + "," : fec;
                        }


                        columnas.Add(new claseMes()
                        {
                            ColmFec = valor,
                            NameMonth = dtmes.ToString("MMMM"),
                        });
                    }


                    if (Config.StackHeadRow)
                        HeaderGrid();
                    
                    #endregion



                    dataGridOcupacion.ItemsSource = ((DataSet)slowTask.Result).Tables[0].DefaultView;
                    TotalesMapaDeVivienda();
                }
                else
                {
                    MessageBox.Show("SIN REGISTROS");
                }

                sfBusyIndicatorVivieanda.IsBusy = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //VentasPorProducto.ItemsSource = ds.Tables[0];
        }

        public void HeaderGrid() 
        {

            StackedHeaderRow stackedHeaderRow = new StackedHeaderRow();
            stackedHeaderRow.StackedColumns.Add(new StackedColumn()
            {
                ChildColumns = "PuntoVentaNombre,ViviendaCodigo,HabitacionCodigo,cnt_camas,CamaCodigo",
                HeaderText = "informacion",
            });

            foreach (var item in columnas)
            {
                string campos = item.ColmFec;
                string mes = item.NameMonth;
                stackedHeaderRow.StackedColumns.Add(new StackedColumn()
                {
                    ChildColumns = campos,
                    HeaderText = mes.Trim(),
                });
            }

            dataGridOcupacion.StackedHeaderRows.Add(stackedHeaderRow);
        }


        private DataSet LoadDataOcupacion(string Fi, string pv, CancellationToken cancellationToken)
        {
            //DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(SiaWin.Func.DatosEmp(idemp));
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                cmd = new SqlCommand("_EmpCt_MapaDeViviendaPorDias", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaCorte", Fi);
                cmd.Parameters.AddWithValue("@Pventa", pv);
                cmd.Parameters.AddWithValue("@Tipoconsulta", 0);
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds);
                con.Close();
            }
            catch (SqlException ex)
            {
                MsgError = ex.Message;

                return null;
            }
            catch (Exception e)
            {
                MsgError = e.Message;

                return null;
            }
            return ds;
        }

        private void TotalesMapaDeVivienda()
        {
            try
            {
                var records = dataGridOcupacion.View.Records;
                //MessageBox.Show(records.Count.ToString());
                int _colu = 1;
                int _di = 0, _oc = 0, _re = 0, _pr = 0, _ma = 0;
                DateTime dtime = Convert.ToDateTime(FechaIni.SelectedDate);
                foreach (var record in records)
                {
                    var dataRowView = record.Data as DataRowView;
                    if (dataRowView != null)
                    {
                        var columns = dataRowView.DataView.Table.Columns;
                        _colu = 1;
                        foreach (System.Data.DataColumn column in dataRowView.DataView.Table.Columns)
                        {
                            if (_colu > 5)
                            {
                                //hoy


                                if (dtime.ToString("yyyy-MM-dd") == column.ColumnName)
                                {
                                    //MessageBox.Show(dtime.ToString("yyyy-MM-dd") + "-" + column.ColumnName);
                                    //MessageBox.Show(column.ColumnName==);
                                    string _value = (string)dataRowView[column.ColumnName];
                                    //MessageBox.Show(column.ColumnName+"-"+_value);
                                    if (_value == "D") _di++;
                                    if (_value == "O") _oc++;
                                    if (_value == "R") _re++;
                                    if (_value == "P") _pr++;
                                    if (_value == "M") _ma++;
                                    break;
                                }
                            }
                            _colu++;
                            //if (_colu > 10) return;

                            //var dictionary = ((IDictionary<string, object>)(expandoObject));
                            //dictionary[column.ColumnName] = row[column];
                        }

                    }
                }
                Ocupacion.Text = _oc.ToString();
                Disponibles.Text = _di.ToString();
                Reservada.Text = _re.ToString();
                Mantenimiento.Text = _ma.ToString();
                LleganHoy.Text = _pr.ToString();
                TotalCamas.Text = dataGridOcupacion.View.Records.Count.ToString();
                //SalenHoy.Text

            }
            catch (System.Exception _error)
            {
                string ee = _error.Message;
                MessageBox.Show(_error.Message);
            }
        }

        private void dataGridOcupacion_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)dataGridOcupacion.SelectedItems[0];
                if (row == null)
                {
                    MessageBox.Show("Registro sin datos");
                    return;
                }
                string column = ((SfDataGrid)sender).CurrentColumn.MappingName;
                string codvivienda = row["ViviendaCodigo"].ToString().Trim();
                string codhabitacion = row["HabitacionCodigo"].ToString().Trim();
                string id_cama = row["CamaCodigo"].ToString().Trim();
                string id_esthab = row[column].ToString();
                

                if (id_esthab == "D")
                {
                    Asignacion win = new Asignacion();
                    win.fecha = column;
                    win.pv = CBpunto.SelectedValue.ToString();
                    win.campamento = CBpunto.Tag.ToString().Trim();

                    win.vivienda = codvivienda;
                    win.habitacion = codhabitacion;
                    win.cama = id_cama;

                    win.ShowInTaskbar = false;
                    win.Owner = Application.Current.MainWindow;
                    win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    win.ShowDialog();

                    bool flag = win.inserto;
                    List<string> lista = win.fechasList;

                    if (flag == true)
                    {
                        foreach (var item in lista)
                        {
                            row[item] = win.concept;                            
                        }
                        dataGridOcupacion.View.Refresh();
                    }
                }
                else
                {
                    EstadoActual win = new EstadoActual();                    
                    win.vivienda = codvivienda;
                    win.habitacion  = codhabitacion;
                    win.cama = id_cama;
                    win.codEstado = id_esthab;
                    win.fecha = column;
                    win.ShowInTaskbar = false;
                    win.Owner = Application.Current.MainWindow;
                    win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    win.ShowDialog();
                }

                //StringBuilder sbsql = new StringBuilder();
                //sbsql.Append("select top 1  reservas.IdRow,reservas.EmpleadoCodigo,empleados.Empleadonombres,empleados.EmpleadoApellidos,clientes.EmpresaNombre, reservas.fechaingreso,reservas.FechaSalida,Reservas.CamaCodigo ");
                //sbsql.Append(" from ctdoc_viviendas as reservas ");
                //sbsql.Append(" inner join CtMae_ViviendaHabitacionCamaEstado as HabitacionesEstado on reservas.EstadoCamaCodigo = HabitacionesEstado.EstadoCamaCodigo ");
                //sbsql.Append(" inner join ctmae_Empleados as Empleados on Empleados.EmpleadoCodigo = reservas.EmpleadoCodigo ");
                //sbsql.Append(" inner join ctmae_empresas as  Clientes on Clientes.EmpresaCodigo = Reservas.EmpresaCodigo ");
                //sbsql.Append(" where reservas.ViviendaCodigo = '" + codvivienda.Trim() + "' AND reservas.habitacioncodigo = '" + codhabitacion.Trim() + "' and reservas.CamaCodigo = '" + id_cama.Trim() + "' and HabitacionesEstado.alias = '" + id_esthab.Trim() + "' ");
                //sbsql.Append(" and '" + column + "' between reservas.FechaIngreso and reservas.FechaSalida ");

               
                //DataTable dt = SiaWin.Func.SqlDT(sbsql.ToString(), "EstadoVivienda", idemp);
                //if (dt == null) return;
                //if (dt.Rows.Count > 0)
                //{
                //}

            }
            catch (Exception ex)
            {
                MessageBox.Show("error double click:"+ex);
            }


        }

        public void actualizarReserva(DataRowView row, AsignarVivienda editar)
        {
            row["ViviendaCodigo"] = editar.Cbx_vivienda.SelectedValue;
            row["HabitacionCodigo"] = editar.Cbx_habitacion.SelectedValue;
            row["CamaCodigo"] = editar.Cbx_cama.SelectedValue;
            dataGridPreCheckIn.View.Refresh();
        }

        private void BtnExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;
                var excelEngine = dataGridOcupacion.ExportToExcel(dataGridOcupacion.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];

                SaveFileDialog sfd = new SaveFileDialog
                {
                    FilterIndex = 2,
                    Filter = "Excel 97 to 2003 Files(*.xls)|*.xls|Excel 2007 to 2010 Files(*.xlsx)|*.xlsx|Excel 2013 File(*.xlsx)|*.xlsx"
                };

                if (sfd.ShowDialog() == true)
                {
                    using (Stream stream = sfd.OpenFile())
                    {
                        if (sfd.FilterIndex == 1)
                            workBook.Version = ExcelVersion.Excel97to2003;
                        else if (sfd.FilterIndex == 2)
                            workBook.Version = ExcelVersion.Excel2010;
                        else
                            workBook.Version = ExcelVersion.Excel2013;
                        workBook.SaveAs(stream);
                    }

                    //Message box confirmation to view the created workbook.
                    if (MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archvo",
                                        MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        //Launching the Excel file using the default Application.[MS Excel Or Free ExcelViewer]
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }
            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error al exportar");
            }
        }
        #endregion


        #region mapa de vivienda detalle
        private async void BtrnConsularOcupacionDetalle_Click(object sender, RoutedEventArgs e)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            sfBusyIndicatorVivieandaDetalle.IsBusy = true;
            //dataGridCxC.ClearFilters();
            string fecha = FechaIniDetalle.Text;
            //string fec_fin = TX_fecfin.Text.Trim();
            var slowTask = Task<DataSet>.Factory.StartNew(() => LoadDataOcupacionDetalle(fecha, source.Token), source.Token);

            await slowTask;
            if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
            {
                Style cellStyle = new Style(typeof(GridCell));
                Setter setterTextContentHorizonalAlignment = new Setter();
                setterTextContentHorizonalAlignment.Property = DataGridCell.HorizontalContentAlignmentProperty;
                setterTextContentHorizonalAlignment.Value = HorizontalAlignment.Center;

                Setter setterTextContentVerticalAlignment = new Setter();
                setterTextContentVerticalAlignment.Property = DataGridCell.VerticalContentAlignmentProperty;
                setterTextContentVerticalAlignment.Value = VerticalAlignment.Center;

                Setter setterTextHorizontalAlignment = new Setter();
                setterTextHorizontalAlignment.Property = DataGridCell.HorizontalAlignmentProperty;
                setterTextHorizontalAlignment.Value = HorizontalAlignment.Center;

                Setter setterTextVerticalAlignment = new Setter();
                setterTextVerticalAlignment.Property = DataGridCell.VerticalAlignmentProperty;
                setterTextVerticalAlignment.Value = VerticalAlignment.Center;
                Setter setterTextBackground = new Setter();
                setterTextVerticalAlignment.Property = DataGridCell.BackgroundProperty;
                setterTextVerticalAlignment.Value = Brushes.Red;

                cellStyle.Setters.Add(setterTextContentHorizonalAlignment);
                cellStyle.Setters.Add(setterTextContentVerticalAlignment);
                cellStyle.Setters.Add(setterTextHorizontalAlignment);
                cellStyle.Setters.Add(setterTextVerticalAlignment);


                //Trigger running = new Trigger() { Property = TextBlock.TextProperty, Value = "1" };                


                //Style style = new Style(typeof(TextBlock));
                //Trigger running = new Trigger() { Property = TextBlock.TextProperty, Value = "1" };
                //Trigger stopped = new Trigger() { Property = TextBlock.TextProperty, Value = "2" };

                //stopped.Setters.Add(new Setter() { Property = TextBlock.BackgroundProperty, Value = Brushes.Blue });
                //running.Setters.Add(new Setter() { Property = TextBlock.BackgroundProperty, Value = Brushes.Green });

                //style.Triggers.Add(running);
                //style.Triggers.Add(stopped);

                DataTable dt = ((DataSet)slowTask.Result).Tables[0];


                foreach (System.Data.DataColumn dc in ((DataSet)slowTask.Result).Tables[0].Columns)
                {
                    var field1 = dc.ColumnName.ToString();
                    char dig = Convert.ToChar(field1.Substring(0, 1));
                    CultureInfo ci = new CultureInfo("Es-Es");


                    if (char.IsDigit(dig))
                    {

                        DateTime dtime = Convert.ToDateTime(field1.ToString());
                        string DiaSemana = ci.DateTimeFormat.GetDayName(dtime.DayOfWeek).ToString();
                        string nomMes = dtime.ToString("MMMM").Substring(0, 1).ToUpper() + dtime.ToString("MMMM").Substring(1, 2);
                        string daycolumn = DiaSemana.Substring(0, 3) + "-" + dtime.Day.ToString() + "-" + nomMes;
                        //MessageBox.Show(dtime.DayOfWeek.ToString()+" "+ dtime.Day.ToString()+" "+nomMes);
                        //MessageBox.Show(field1.ToString());

                        dataGridOcupacionDetalle.Columns.Add(new GridTextColumn() { HeaderText = daycolumn, MappingName = field1.ToString(), Width = 75, TextAlignment = TextAlignment.Center, CellStyle = cellStyle });


                        GridTextColumn colNameStatus2 = new GridTextColumn();
                        colNameStatus2.HeaderText = daycolumn;
                        colNameStatus2.Width = 75;
                        colNameStatus2.MappingName = field1.ToString();

                        //dataGridOcupacion.Columns.Add(colNameStatus2);

                        Style style1 = new Style(typeof(TextBlock));
                        Trigger running1 = new Trigger() { Property = TextBlock.TextProperty, Value = "Running" };
                        Trigger stopped1 = new Trigger() { Property = TextBlock.TextProperty, Value = "Stopped" };

                        //stopped.Setters.Add(new Setter() { Property = TextBlock.BackgroundProperty, Value = Brushes.Blue });
                        //running.Setters.Add(new Setter() { Property = TextBlock.BackgroundProperty, Value = Brushes.Green });

                        //style1.Triggers.Add(running);
                        //style1.Triggers.Add(stopped);






                    }

                }
                dataGridOcupacionDetalle.ItemsSource = ((DataSet)slowTask.Result).Tables[0].DefaultView;

            }
            else
            {
                MessageBox.Show("SIN REGISTROS");
            }
            sfBusyIndicatorVivieandaDetalle.IsBusy = false;
        }

        private DataSet LoadDataOcupacionDetalle(string Fi, CancellationToken cancellationToken)
        {
            //DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(SiaWin.Func.DB_Acceso);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                cmd = new SqlCommand("MapaDeViviendas", con);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@FechaCorte", FechaIni.SelectedDate);//if you have parameters.
                cmd.Parameters.AddWithValue("@FechaCorte", Fi);
                cmd.Parameters.AddWithValue("@Pventa", 0);//if you have parameters.
                cmd.Parameters.AddWithValue("@Cliente", 0);//if you have parameters.
                cmd.Parameters.AddWithValue("@Tipoconsulta", 1);//if you have parameters.                
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds);
                con.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("eee:" + e);
                return null;
            }
            return ds;
        }

        #endregion


        #region importacion

        
        private void BtnImportar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnUpload_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

    }

    public class Slots
    {
        public int Year { get; set; }
        public int Date { get; set; }
    }

    public class claseMes
    {
        public string ColmFec { get; set; }
        public string NameMonth { get; set; }
    }


}
