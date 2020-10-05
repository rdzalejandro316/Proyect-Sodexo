using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
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
    //Sia.PublicarPnt(9466,"Kardex");       
    //dynamic WinDescto = ((Inicio)Application.Current.MainWindow).WindowExt(9466,"Kardex");
    //WinDescto.ShowInTaskbar = false;
    //WinDescto.Owner = Application.Current.MainWindow;
    //WinDescto.WindowStartupLocation = WindowStartupLocation.CenterScreen;
    //WinDescto.ShowDialog(); 

    public partial class Kardex : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        public string codemp;
        public string idBod = string.Empty;
        public string codpvta = string.Empty;
        public string codref = string.Empty;
        public string codbod = string.Empty;
        int moduloid = 0;
        string cnEmp = "";

        public Kardex()
        {
            InitializeComponent();

            SiaWin = Application.Current.MainWindow;
            //idemp = SiaWin._BusinessId;
            TextBoxRef.Focus();

        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                string tag = ((TextBox)sender).Tag.ToString();

                if (tag == "inmae_ref")
                {
                    if (e.Key == System.Windows.Input.Key.Enter & string.IsNullOrEmpty(TextBoxRef.Text.Trim()))
                    {
                        dynamic ww = SiaWin.WindowExt(9326, "InBuscarReferencia");
                        ww.Height = 400;
                        ww.Conexion = SiaWin.Func.DatosEmp(idemp);
                        ww.idEmp = idemp;
                        ww.idBod = idBod;
                        ww.ShowInTaskbar = false;
                        ww.Owner = Application.Current.MainWindow;
                        ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        ww.ShowDialog();

                        if (!string.IsNullOrEmpty(ww.Codigo))
                        {
                            TextBoxRef.Text = ww.Codigo;
                            TXNomRef.Text = ww.Nombre;
                            TextBoxbod.Focus();
                        }
                        ww = null;
                        e.Handled = true;

                    }
                }
                if (tag == "inmae_bod")
                {

                    if (e.Key == System.Windows.Input.Key.Enter & string.IsNullOrEmpty(TextBoxbod.Text.Trim()))
                    {
                        string cmptabla = tag; string cmpcodigo = "cod_bod"; string cmpnombre = "nom_bod"; string cmporden = "cod_bod"; string cmpidrow = "idrow"; string cmptitulo = "Maestra de bodegas"; string cmpconexion = cnEmp; Boolean mostrartodo = true; string cmpwhere = "";
                        int idr = 0; string code = ""; string nom = "";

                        dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, SiaWin.Func.DatosEmp(idemp), mostrartodo, cmpwhere, idEmp: idemp);
                        winb.ShowInTaskbar = false;
                        winb.Owner = Application.Current.MainWindow;
                        winb.ShowDialog();
                        idr = winb.IdRowReturn;
                        code = winb.Codigo;
                        nom = winb.Nombre;
                        winb = null;

                        if (idr > 0)
                        {
                            TextBoxbod.Text = code;
                            TxNomBod.Text = nom;

                            //var uiElement = e.OriginalSource as UIElement;
                            //uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                            BtnConsultar.Focus();
                        }
                        e.Handled = true;
                    }
                }
                if (e.Key == Key.Enter & !string.IsNullOrEmpty(((TextBox)sender).Text.Trim()))
                {
                    var uiElement = e.OriginalSource as UIElement;
                    uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void TextBoxRef_LostFocus(object sender, RoutedEventArgs e)
        {
            BuscarCod(TextBoxRef.Text.Trim());            
        }

        private void TextBoxbod_LostFocus(object sender, RoutedEventArgs e)
        {
            BuscarBod(TextBoxbod.Text.Trim());  
        }

        private bool BuscarCod(string codigo)
        {
            if (string.IsNullOrEmpty(codigo)) return false;            
            bool ret = false;
            try
            {
                string cadena = "select cod_ref,nom_ref from inmae_ref where cod_ref='" + codigo + "' ";
                DataTable dt = SiaWin.Func.SqlDT(cadena, "referencia", SiaWin._BusinessId);

                //MessageBox.Show("SiaWin._BusinessId:" + SiaWin._BusinessId);
                if (dt.Rows.Count > 0)
                {
                    //TextBoxRef.Text = dt.Rows[0]["cod_ref"].ToString();
                    TXNomRef.Text = dt.Rows[0]["nom_ref"].ToString();
                    ret = true;
                }
                else
                {
                    MessageBox.Show("no existe esa referencia");
                    clean(1);
                    ret = false;
                }
            }
            catch (Exception ex)
            {
               MessageBox.Show("erro al buscar:"+ex.Message);
            }
            return ret;
        }

        private bool BuscarBod(string codigo)
        {
            if (string.IsNullOrEmpty(codigo)) return false;
            bool ret = false;
            try
            {
                string cadena = "select cod_bod,nom_bod from InMae_bod where cod_bod='" + codigo + "' ";
                DataTable dt = SiaWin.Func.SqlDT(cadena, "bodega", SiaWin._BusinessId);

                if (dt.Rows.Count > 0)
                {
                    TextBoxbod.Text = dt.Rows[0]["cod_bod"].ToString();
                    TxNomBod.Text = dt.Rows[0]["nom_bod"].ToString();
                    ret = true;
                }
                else
                {
                    MessageBox.Show("no existe la bodega que ingreso");
                    clean(2);
                    ret = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("erro al buscar:"+ex.Message);
            }
            return ret;
        }

        public void clean(int codigo)
        {
            if (codigo == 1)
            {
                TextBoxRef.Text = "";
                TXNomRef.Text = "";
            }
            if (codigo == 2)
            {
                TextBoxbod.Text = "";
                TxNomBod.Text = "";
            }
        }
        private void ResetValue()
        {
            TxtTotalUnEnt.Text = "00.0";
            TxtTotalUncosEnt.Text= "00.0";
            TxtTotalUncosSal.Text= "00.0";
            TxtTotalUncosSaldo.Text= "00.0";
            TxtTotalUncostEnt.Text = "00.0";
            TxtTotalUncostSal.Text = "00.0";
            TxtTotalUncostSaldo.Text = "00.0";
            TxtTotalUnEnt.Text = "00.0";
            TxtTotalUnSal.Text = "00.0";
            TxtTotalUnSaldo.Text = "00.0";
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ResetValue();
                if (FecIni.Text.Length <= 0)
                {
                    MessageBox.Show("debe de ingresar la fecha de corte"); 
                    FecIni.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(TextBoxRef.Text.Trim()))
                {
                    MessageBox.Show("debe de ingresar una referencia");
                    TextBoxRef.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(TextBoxbod.Text.Trim()))
                {
                    MessageBox.Show("debe de ingresar una bodega");
                    TextBoxbod.Focus();
                    return;
                }
                

                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpInventarioKardes", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Fecha", FecIni.Text);
                cmd.Parameters.AddWithValue("@Ref", TextBoxRef.Text);
                cmd.Parameters.AddWithValue("@Bods", TextBoxbod.Text);
                cmd.Parameters.AddWithValue("@codemp", codemp);
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                con.Close();
                GridKardex.ItemsSource = ds.Tables[0];
                if (ds.Tables[0].Rows.Count>0)
                {

                    GridKardex.Focus();
                    GridKardex.SelectedIndex = 0;

                    decimal CantEnt = Convert.ToDecimal(ds.Tables[0].Compute("Sum(ent_uni)", "").ToString());
                    decimal TotEnt = Convert.ToDecimal(ds.Tables[0].Compute("Sum(ent_ctotal)", "").ToString());                    
                    TxtTotalUnEnt.Text = CantEnt.ToString("N2");
                    TxtTotalUncostEnt.Text = TotEnt.ToString("N2");                    

                    int promedioEntrada = 0;
                    if (TotEnt>0 & CantEnt>0)
                    {
                        TxtTotalUncosEnt.Text = (TotEnt / CantEnt).ToString("N2");
                        promedioEntrada = Convert.ToInt32(TotEnt / CantEnt);
                    }
                    else
                    {
                        TxtTotalUncosEnt.Text = "0"; 
                    }
                    
                    ProEnt.Text = promedioEntrada.ToString();
                    decimal CantSal = Convert.ToDecimal(ds.Tables[0].Compute("Sum(sal_uni)", "").ToString());
                    decimal TotSal = Convert.ToDecimal(ds.Tables[0].Compute("Sum(sal_ctotal)", "").ToString());
                    TxtTotalUnSal.Text = CantSal.ToString("N2");
                    TxtTotalUncostSal.Text = TotSal.ToString("N2");

                    int promedioSalida = 0;
                    if (TotSal > 0 & CantSal > 0)
                    {
                        TxtTotalUncosSal.Text = (TotSal/ CantSal).ToString("N2");
                        promedioSalida = Convert.ToInt32(TotSal / CantSal);
                    }
                    else
                    {
                        TxtTotalUncosSal.Text = "0";
                    }

                    ProSal.Text = promedioSalida.ToString();

                    decimal CantSaldo =  Convert.ToDecimal(ds.Tables[0].Compute("Sum(saldo_uni)", "").ToString());
                    decimal TotSaldo = Convert.ToDecimal(ds.Tables[0].Compute("Sum(saldo_ctotal)", "").ToString());

                    TxtTotalUnSaldo.Text = CantSaldo.ToString("N2");
                    TxtTotalUncostSaldo.Text = TotSaldo.ToString("N2");

                    int promedioSaldo = 0;

                    if (TotSaldo > 0 & CantSaldo > 0)
                    {
                        promedioSaldo = Convert.ToInt32(TotSaldo / CantSaldo);
                        TxtTotalUncosSaldo.Text = (TotSaldo / CantSaldo).ToString("N2");
                    }
                    else
                    {
                        TxtTotalUncosSaldo.Text = "0";
                    }

                    
                    ProSaldo.Text = promedioSaldo.ToString();
                }


                Total.Text = ds.Tables[0].Rows.Count.ToString();
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar la consulta programada:" + w.ToString());
            }
        }

        private void ExportaXLS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;

                //options.ExcludeColumns.Add("idreg");
                //options.ExcludeColumns.Add("cod_trn");
                //options.ExcludeColumns.Add("nom_trn");
                //options.ExcludeColumns.Add("num_trn");
                //options.ExcludeColumns.Add("fec_trn");
                //options.ExcludeColumns.Add("bod_tra");
                //options.ExcludeColumns.Add("ent_uni");
                //options.ExcludeColumns.Add("ent_cost");
                //options.ExcludeColumns.Add("ent_ctotal");
                //options.ExcludeColumns.Add("sal_uni");
                //options.ExcludeColumns.Add("sal_cost");
                //options.ExcludeColumns.Add("sal_ctotal");
                //options.ExcludeColumns.Add("saldo_uni");
                //options.ExcludeColumns.Add("saldo_cost");
                //options.ExcludeColumns.Add("saldo_ctotal");



                var excelEngine = GridKardex.ExportToExcel(GridKardex.View, options);
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
                    if (MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archvo", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        //Launching the Excel file using the default Application.[MS Excel Or Free ExcelViewer]
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al exportar:"+w);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {                
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(SiaWin._BusinessId);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();                
                string nomempresa = foundRow["BusinessName"].ToString().Trim();

                if (string.IsNullOrEmpty(codemp))
                {
                    codemp = foundRow["BusinessCode"].ToString().Trim();
                }
                else
                {                    
                    DataTable dt = SiaWin.Func.SqlDT("select * from Business where BusinessCode='"+ codemp + "' ", "Empresas", 0);                    
                    int idEmpresa = 0;
                    if (dt.Rows.Count > 0) {                     
                        idEmpresa = Convert.ToInt32(dt.Rows[0]["BusinessId"]);
                    }                    
                    System.Data.DataRow foundRowEmpresa = SiaWin.Empresas.Rows.Find(idEmpresa);                    
                    nomempresa = foundRowEmpresa["BusinessName"].ToString().Trim();                    
                } 

                this.Title = "Kardex - Empresa:" + codemp + "-" + nomempresa;
                FecIni.Text = DateTime.Now.ToShortDateString();

                // idmodulo
                
                DataRow[] drmodulo = SiaWin.Modulos.Select("ModulesCode='IN'");
                if (drmodulo == null) this.IsEnabled = false;
                moduloid = Convert.ToInt32(drmodulo[0]["ModulesId"].ToString());
                //MessageBox.Show("Modulo id:"+moduloid.ToString());
                if (!string.IsNullOrEmpty(codref) )
                {
                    TextBoxbod.Text = codbod;
                    BuscarBod(codbod);
                    TextBoxRef.Text = codref;
                    BuscarCod(codref);
                    BtnConsultar.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar el Load:"+w);
            }
        }
        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();            
        }
        private void BtnDocumento_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)GridKardex.SelectedItems[0];
                if (row == null) return;
                int idreg = Convert.ToInt32(row["idreg"]);
                if (idreg <= 0) return;
                //public void TabTrn(int Pnt, int idemp, bool IntoWindows = false, int idregcab = 0, int idmodulo = 0, bool WinModal = true)
                SiaWin.TabTrn(0, idemp, true, idreg, moduloid, WinModal: true);
            }
            catch (Exception w)
            {
                MessageBox.Show("selecione una transaccion"+w);
            }
        }
    }
}
