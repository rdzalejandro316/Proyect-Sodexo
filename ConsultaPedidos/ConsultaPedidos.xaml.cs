using Microsoft.Win32;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SiasoftAppExt
{


    //Sia.PublicarPnt(9476,"ConsultaPedidos");
    //Sia.TabU(9476);
    public partial class ConsultaPedidos : UserControl
    {


        dynamic SiaWin;
        dynamic tabitem;
        int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";


        public ConsultaPedidos(dynamic tabitem1)
        {
            InitializeComponent();

            SiaWin = Application.Current.MainWindow;
            tabitem = tabitem1;
            idemp = SiaWin._BusinessId;
            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                tabitem.Logo(idLogo, ".png");
                tabitem.Title = "Consulta Pedidos (" + aliasemp + ")";
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();


                DateTime fechatemp = DateTime.Today;
                fechatemp = new DateTime(fechatemp.Year, fechatemp.Month, 1);

                fecha_ini.Text = fechatemp.ToString();
                fecha_fin.Text = DateTime.Now.ToString();
                fecha_compra.Text = fechatemp.ToString();
            }
            catch (Exception e)
            {
                SiaWin.Func.SiaExeptionGobal(e);
                MessageBox.Show(e.Message);
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                //MessageBox.Show("cnEmp:" + cnEmp);
                string idTab = ((TextBox)sender).Tag.ToString();

                if (e.Key == Key.Enter || e.Key == Key.F8)
                {
                    if (idTab.Length > 0)
                    {
                        string tag = ((TextBox)sender).Tag.ToString();
                        string cmptabla = ""; string cmpcodigo = ""; string cmpnombre = ""; string cmporden = ""; string cmpidrow = ""; string cmptitulo = ""; string cmpconexion = ""; bool mostrartodo = true; string cmpwhere = "";
                        if (string.IsNullOrEmpty(tag)) return;

                        if (tag == "inmae_tip")
                        {
                            cmptabla = tag; cmpcodigo = "cod_tip"; cmpnombre = "nom_tip"; cmporden = "cod_tip"; cmpidrow = "cod_tip"; cmptitulo = "Maestra de Lineas"; cmpconexion = cnEmp; mostrartodo = true; cmpwhere = "";
                        }
                        if (tag == "inmae_bod")
                        {
                            cmptabla = tag; cmpcodigo = "cod_bod"; cmpnombre = "nom_bod"; cmporden = "cod_bod"; cmpidrow = "cod_bod"; cmptitulo = "Maestra de Bodegas"; cmpconexion = cnEmp; mostrartodo = true; cmpwhere = "";
                        }
                        if (tag == "comae_ter")
                        {
                            cmptabla = tag; cmpcodigo = "cod_ter"; cmpnombre = "nom_ter"; cmporden = "cod_ter"; cmpidrow = "cod_ter"; cmptitulo = "Maestra de Terceros"; cmpconexion = cnEmp; mostrartodo = false; cmpwhere = "";
                        }
                        if (tag == "inmae_prv")
                        {
                            cmptabla = tag; cmpcodigo = "cod_prv"; cmpnombre = "nom_prv"; cmporden = "cod_prv"; cmpidrow = "cod_prv"; cmptitulo = "Maestra de Provedor"; cmpconexion = cnEmp; mostrartodo = true; cmpwhere = "";
                        }


                        int idr = 0; string code = ""; string nom = "";
                        dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, cnEmp, mostrartodo, cmpwhere, idEmp: idemp);

                        winb.ShowInTaskbar = false;
                        winb.Owner = Application.Current.MainWindow;
                        winb.ShowDialog();
                        idr = winb.IdRowReturn;
                        code = winb.Codigo;
                        nom = winb.Nombre;
                        winb = null;
                        if (idr > 0)
                        {
                            if (tag == "inmae_tip")
                            {
                                TX_linea.Text = code.Trim();
                                TxBox_linea.Text = nom;
                            }
                            if (tag == "inmae_bod")
                            {
                                TX_bodega.Text = code.Trim();
                                TxBox_bodega.Text = nom;
                            }
                            if (tag == "comae_ter")
                            {
                                TX_cliente.Text = code.Trim();
                                TxBox_cliente.Text = nom;
                            }
                            if (tag == "inmae_prv")
                            {
                                TX_provedor.Text = code.Trim();
                                TxBox_provedor.Text = nom;
                            }

                            var uiElement = e.OriginalSource as UIElement;
                            uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                        }
                        e.Handled = true;
                    }
                    if (e.Key == Key.Enter)
                    {
                        var uiElement = e.OriginalSource as UIElement;
                        uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    }
                }

            }
            catch (Exception ex)
            {
                SiaWin.Func.SiaExeptionGobal(ex);
                MessageBox.Show(ex.Message.ToString());
            }
        }


        private void TX_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(((TextBox)sender).Text)) return;

            string idTag = ((TextBox)sender).Tag.ToString();
            string codigo = ""; string nombre = ""; TextBox campoNombre = new TextBox();
            switch (idTag)
            {
                case "inmae_tip":
                    codigo = "cod_tip"; nombre = "nom_tip"; campoNombre = (TextBox)this.FindName("TxBox_linea");
                    break;
                case "inmae_bod":
                    codigo = "cod_bod"; nombre = "nom_bod"; campoNombre = (TextBox)this.FindName("TxBox_bodega");
                    break;
                case "comae_ter":
                    codigo = "cod_ter"; nombre = "nom_ter"; campoNombre = (TextBox)this.FindName("TxBox_cliente");
                    break;
                case "inmae_prv":
                    codigo = "cod_prv"; nombre = "nom_prv"; campoNombre = (TextBox)this.FindName("TxBox_provedor");
                    break;
            }

            string cadena = "select * from " + idTag + "  where  " + codigo + "='" + ((TextBox)sender).Text.ToString() + "'  ";
            //campoNombre.Text = "oooooooooooo";


            DataTable tabla = SiaWin.Func.SqlDT(cadena, "Buscar", idemp);
            if (tabla.Rows.Count > 0)
            {
                ((TextBox)sender).Text = tabla.Rows[0][codigo].ToString();
                campoNombre.Text = tabla.Rows[0][nombre].ToString();
            }
            else
            {
                MessageBox.Show("el codigo ingresado no existe");
                ((TextBox)sender).Text = "";
                campoNombre.Text = "";
            }


        }


        private string ArmaWhere()
        {
            string cadenawhere = " ";
            string Linea = TX_linea.Text.Trim();
            string Bodega = TX_bodega.Text.Trim();
            string Cliente = TX_cliente.Text.Trim();
            string Provedor = TX_provedor.Text.Trim();

            if (!string.IsNullOrEmpty(Linea))
            {
                cadenawhere += " and referencia.cod_tip='" + Linea + "'  ";
            }
            if (!string.IsNullOrEmpty(Bodega))
            {
                cadenawhere += " and cuerpo.cod_bod='" + Bodega + "'  ";
            }
            if (!string.IsNullOrEmpty(Cliente))
            {
                cadenawhere += " and cabeza.cod_cli='" + Cliente + "'  ";
            }
            if (!string.IsNullOrEmpty(Provedor))
            {
                cadenawhere += " and cabeza.cod_prv='" + Provedor + "' ";
            }


            return cadenawhere;
        }


        private void dataGridCxC_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            try
            {

                if (dataGridConsulta.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)dataGridConsulta.SelectedItems[0];
                    string referencia = row["p_cod_ref"].ToString();

                    decimal saldoBod001 = SiaWin.Func.SaldoInv(referencia, "001", cod_empresa);
                    Bod1.Text = saldoBod001.ToString();

                    decimal saldoBod003 = SiaWin.Func.SaldoInv(referencia, "003", cod_empresa);
                    decimal saldoBod004 = SiaWin.Func.SaldoInv(referencia, "004", cod_empresa);
                    decimal saldoB3B4 = saldoBod003 + saldoBod004;
                    Bod3_4.Text = saldoB3B4.ToString();

                    decimal saldoBod010 = SiaWin.Func.SaldoInv(referencia, "010", cod_empresa);
                    Bod10.Text = saldoBod010.ToString();

                    decimal saldoBod012 = SiaWin.Func.SaldoInv(referencia, "012", cod_empresa);
                    decimal saldoBod013 = SiaWin.Func.SaldoInv(referencia, "013", cod_empresa);
                    decimal saldoB12B13 = saldoBod012 + saldoBod013;
                    Bod12_13.Text = saldoB12B13.ToString();

                    decimal saldoBod005 = SiaWin.Func.SaldoInv(referencia, "005", cod_empresa);
                    Bod5.Text = saldoBod005.ToString();

                    decimal saldoBod007 = SiaWin.Func.SaldoInv(referencia, "007", cod_empresa);
                    decimal saldoBod009 = SiaWin.Func.SaldoInv(referencia, "009", cod_empresa);
                    decimal saldoB7B9 = saldoBod007 + saldoBod009;
                    Bod7_9.Text = saldoB7B9.ToString();

                    decimal saldoBod017 = SiaWin.Func.SaldoInv(referencia, "017", cod_empresa);
                    decimal saldoBod019 = SiaWin.Func.SaldoInv(referencia, "019", cod_empresa);
                    decimal saldoB17B19 = saldoBod017 + saldoBod019;
                    Bod17_19.Text = saldoB17B19.ToString();

                    decimal saldoBod008 = SiaWin.Func.SaldoInv(referencia, "008", cod_empresa);
                    Bod8.Text = saldoBod008.ToString();

                    decimal saldoBod050 = SiaWin.Func.SaldoInv(referencia, "050", cod_empresa);
                    decimal saldoBod052 = SiaWin.Func.SaldoInv(referencia, "052", cod_empresa);
                    decimal saldoB50B52 = saldoBod050 + saldoBod052;
                    Bod50_52.Text = saldoB50B52.ToString();
                }
            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("erro en la seleccion:" + w);
            }

        }



        private async void BTNconsultar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                ConfigGrid.IsEnabled = false;
                sfBusyIndicator.IsBusy = true;

                dataGridConsulta.ItemsSource = null;

                string ffi = fecha_ini.Text.ToString();
                string fff = fecha_fin.Text.ToString();
                string where = ArmaWhere();
                string ffc = fecha_compra.Text.ToString();
                string whereAnular = TipoAnul.Text == "No" ? " and pedidos.est_anu<>'A' " : " ";

                if (string.IsNullOrEmpty(where)) where = " ";


                var slowTask = Task<DataSet>.Factory.StartNew(() => SlowDude(ffi, fff, ffc, where, whereAnular, cod_empresa, source.Token), source.Token);
                await slowTask;


                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {

                    dataGridConsulta.ItemsSource = ((DataSet)slowTask.Result).Tables[0];
                    Total.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();

                    TabControl1.SelectedIndex = 2;
                    TabControl1.SelectedIndex = 1;
                }

                ConfigGrid.IsEnabled = true;
                this.sfBusyIndicator.IsBusy = false;
                //GridConfiguracion.IsEnabled = true;
            }
            catch (Exception ex)
            {
                SiaWin.Func.SiaExeptionGobal(ex);
                MessageBox.Show("erro2:" + ex);
                this.Opacity = 1;
            }


        }


        private DataSet SlowDude(string FI, string FF, string FC, string where, string whereAnular, string cod_empresa, CancellationToken cancellationToken)
        {
            try
            {
                DataSet jj = LoadData(FI, FF, FC, where, whereAnular, cod_empresa, cancellationToken);
                return jj;
            }
            catch (Exception e)
            {
                SiaWin.Func.SiaExeptionGobal(e);
                MessageBox.Show(e.Message);
            }
            return null;
        }


        private DataSet LoadData(string FechaIN, string FechaFI, string FechaCompra, string Where, string Anulacion, string CodEmp, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpSpConsultaPedidos", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaIni", FechaIN);
                cmd.Parameters.AddWithValue("@FechaFin", FechaFI);
                cmd.Parameters.AddWithValue("@Where", Where);
                cmd.Parameters.AddWithValue("@fech_Compra", FechaCompra);
                cmd.Parameters.AddWithValue("@Anulacion", Anulacion);
                cmd.Parameters.AddWithValue("@codEmpresa", CodEmp);
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                con.Close();
                return ds;
            }
            catch (Exception e)
            {
                SiaWin.Func.SiaExeptionGobal(e);
                MessageBox.Show("en la consulta:" + e.Message);
                return null;
            }
        }

        private void BTNcancelar_Click(object sender, RoutedEventArgs e)
        {
            TX_linea.Text = "";
            TxBox_linea.Text = "";
            TX_bodega.Text = "";
            TxBox_bodega.Text = "";
            TX_cliente.Text = "";
            TxBox_cliente.Text = "";
            TX_provedor.Text = "";
            TxBox_provedor.Text = "";

        }

        private void ExportPDF_Click(object sender, RoutedEventArgs e)
        {
            var options = new PdfExportingOptions();
            options.FitAllColumnsInOnePage = true;
            options.ExcludeColumns.Add("cod_ant");
            options.ExcludeColumns.Add("nom_ter");
            options.ExcludeColumns.Add("nom_mer");
            options.ExcludeColumns.Add("nom_tip");
            var document = dataGridConsulta.ExportToPdf(options);
            document.PageSettings.Orientation = PdfPageOrientation.Landscape;
            //var document = new PdfDocument();
            //document.PageSettings.Orientation = PdfPageOrientation.Landscape;
            //var page = document.Pages.Add();
            //var PDFGrid = dataGridConsulta.ExportToPdfGrid(dataGridConsulta.View, options);            


            //PDFGrid.Draw(page, new PointF(), format);

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "PDF Files(*.pdf)|*.pdf"
            };

            if (sfd.ShowDialog() == true)
            {
                using (Stream stream = sfd.OpenFile())
                {
                    document.Save(stream);
                }

                //Message box confirmation to view the created Pdf file.

                if (MessageBox.Show("Do you want to view the Pdf file?", "Pdf file has been created",
                                    MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {

                    //Launching the Pdf file using the default Application.
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
            }
        }



        private void ExportEXCEL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;
                var excelEngine = dataGridConsulta.ExportToExcel(dataGridConsulta.View, options);
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


    }
}


