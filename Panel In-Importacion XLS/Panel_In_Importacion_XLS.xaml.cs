using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9500,"Panel_In_Importacion_XLS");    
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9500, "Panel_In_Importacion_XLS");
    //ww.ShowInTaskbar=false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
    //ww.ShowDialog(); 

    //https://www.microsoft.com/es-es/download/details.aspx?id=23734
    public class Productos : IDataErrorInfo
    {

        private string referencia;

        public string Referencia
        {
            get { return referencia; }
            set { referencia = value; }
        }
        private string nombre;

        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }
        private string cantidad;

        public string Cantidad
        {
            get { return cantidad; }
            set { cantidad = value; }
        }
        private string valUnit;

        public string ValUnit
        {
            get { return valUnit; }
            set { valUnit = value; }
        }
        private string iva;

        public string IVA
        {
            get { return iva; }
            set { iva = value; }
        }

        private string subTotal;

        public string SubTotal
        {
            get { return subTotal; }
            set { subTotal = value; }
        }
        private string valIVA;

        public string ValIVA
        {
            get { return valIVA; }
            set { valIVA = value; }
        }
        private string total;

        public string Total
        {
            get { return total; }
            set { total = value; }
        }
        private string codRef;

        public string codigo(string code)
        {
            CodReferencia = code;
            return null;
        }
        [Display(AutoGenerateField = false)]
        public string CodReferencia
        {
            get { return codRef; }
            set { codRef = value; }
        }

        [Display(AutoGenerateField = false)]

        public string Error
        {
            get
            {
                return string.Empty;
            }
        }

        public string this[string columnName]
        {
            get
            {
                if (!columnName.Equals("Referencia"))
                    return string.Empty;
                //if (this.Referencia.Contains("10-9C"))
                //    return "La referencia no existe: " + this.Referencia;
                Panel_In_Importacion_XLS principal = new Panel_In_Importacion_XLS();
                if (principal.BuscarRef(Referencia) == false)
                {
                    return "la referencia no existe: " + this.Referencia;
                }
                //if ()
                //{
                //    return "Referencia se repite en el documento";
                //}
                return string.Empty;
            }
        }

        public Productos(string refe, string nom, string cant, string unit, string Iva, string subto, string tIVA, string tot)
        {
            Referencia = refe;
            Nombre = nom;
            Cantidad = cant;
            ValUnit = unit;
            IVA = Iva;
            SubTotal = subto;
            ValIVA = tIVA;
            Total = tot;
        }
    }

    public partial class Panel_In_Importacion_XLS : System.Windows.Window
    {

        private ObservableCollection<Productos> _productos;
        public ObservableCollection<Productos> Produ
        {
            get { return _productos; }
            set { _productos = value; }
        }

        public System.Data.DataTable tablaXLS = new System.Data.DataTable();

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public Boolean bandera = false;

        public Panel_In_Importacion_XLS()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            idemp = SiaWin._BusinessId; ;
            LoadConfig();
        }

        
        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Importar XLS" + cod_empresa + "-" + nomempresa;
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        public string Buscar()
        {
            OpenFileDialog openfile = new OpenFileDialog();
            openfile.DefaultExt = ".xlsx";
            openfile.Filter = "(.xlsx)|*.xlsx";

            var browsefile = openfile.ShowDialog();

            return openfile.FileName;
        }

        //public string Guardar()
        //{

        //    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
        //    saveFileDialog1.Filter = "Excel File|*.xls";
        //    saveFileDialog1.Title = "Save an Excel File";
        //    saveFileDialog1.ShowDialog();


        //    if (saveFileDialog1.FileName != "")
        //    {
        //        // Saves the Image via a FileStream created by the OpenFile method.  
        //        System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
        //        // Saves the Image in the appropriate ImageFormat based upon the  
        //        // File type selected in the dialog box.  
        //        // NOTE that the FilterIndex property is one-based.  
        //        fs.Close();
        //        return saveFileDialog1.FileName;
        //    }
        //    return null;
        //}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            txtFilePath.Text = Buscar();
            var test = new ObservableCollection<Productos>();

            _productos = new ObservableCollection<Productos>();

            try
            {
                tablaXLS = ConvertExcelToDataTable(txtFilePath.Text);
                foreach (System.Data.DataRow row in tablaXLS.Rows)
                {
                    _productos.Add(new Productos(row[0].ToString(), row[1].ToString(), row[2].ToString(), row[3].ToString(), row[4].ToString(), row[5].ToString(), row[6].ToString(), row[7].ToString()));
                }

                //dtGrid.ItemsSource = tablaXLS.DefaultView;
                validarCamposEntrantes(_productos);
                //limpiarExcel(_productos);

                dtGrid.ItemsSource = Produ;
                TotalReg.Text = Convert.ToString(dtGrid.View.Records.Count());
            }
            catch (Exception w)
            {
                MessageBox.Show("El documento que desea importar debe ser el mismo de la pantilla:"+w);
            }
        }

        //public void limpiarExcel(ObservableCollection<Productos> _productos)
        //{
        //    try
        //    {
        //        foreach (var item in _productos)
        //        {
        //            if (item.Referencia == "") _productos.Remove(item);
        //        }
        //    }
        //    catch (Exception w)
        //    {
        //        MessageBox.Show("error al eliminmar:"+w);
        //    }

        //}


        public void validarCamposEntrantes(ObservableCollection<Productos> produ)
        {
            try
            {
                int i = 0;
                foreach (var data in _productos)
                {
                    if (BuscarRef(data.Referencia) == true)
                    {
                        string cadena = "select * from inmae_ref where cod_ref='" + data.Referencia + "' ";
                        System.Data.DataTable dt = SiaWin.Func.SqlDT(cadena, "Clientes", idemp);
                        _productos[i].Nombre = dt.Rows[0]["nom_ref"].ToString();
                    }
                    i++;
                }
            }
            catch (Exception)
            {

            }
        }






        public static System.Data.DataTable ConvertExcelToDataTable(string FileName)
        {
            System.Data.DataTable dtResult = null;
            int totalSheet = 0;
            using (OleDbConnection objConn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + FileName + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';"))
            {
                objConn.Open();
                OleDbCommand cmd = new OleDbCommand();
                OleDbDataAdapter oleda = new OleDbDataAdapter();
                DataSet ds = new DataSet();
                System.Data.DataTable dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                string sheetName = string.Empty;
                if (dt != null)
                {
                    var tempDataTable = (from dataRow in dt.AsEnumerable() where !dataRow["TABLE_NAME"].ToString().Contains("FilterDatabase") select dataRow).CopyToDataTable();
                    dt = tempDataTable;
                    totalSheet = dt.Rows.Count;
                    sheetName = dt.Rows[0]["TABLE_NAME"].ToString();
                }
                cmd.Connection = objConn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM [" + sheetName + "]";
                oleda = new OleDbDataAdapter(cmd);
                oleda.Fill(ds, "excelData");
                dtResult = ds.Tables["excelData"];
                objConn.Close();
                return dtResult; //Returning Dattable  
            }
        }

        public Boolean BuscarRef(string referencia)
        {
            string cadena = "select * from inmae_ref where cod_ref='" + referencia + "' ";
            System.Data.DataTable dt = SiaWin.Func.SqlDT(cadena, "Clientes", idemp);
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void BTNvalidar_Click(object sender, RoutedEventArgs e)
        {

            if (dtGrid.View.Records.Count <= 0) { MessageBox.Show("ingrese un XML para validar"); return; };

            //if (validarNitNumeroFact() == true) return;

            //if (validarFecha() == true) return;

            //recorrer();

            if (GrillaRed(_productos) == true)
            {
                llenarDataTable();
                this.Close();
                bandera = true;
            }
            else
            {
                MessageBox.Show("!existen errores en la grilla, por favor corrijalos¡");
            }

            //bandera = true;
            //this.Close();

        }



        public void llenarDataTable()
        {

            tablaXLS.Clear();

            foreach (var data in _productos)
            {
                System.Data.DataRow row = tablaXLS.NewRow();
                row[0] = data.Referencia;
                row[1] = data.Nombre;
                row[2] = data.Cantidad;
                row[3] = data.ValUnit;
                row[4] = data.IVA;
                row[5] = data.SubTotal;
                row[6] = data.ValIVA;
                row[7] = data.Total;
                tablaXLS.Rows.Add(row);
            }
        }

        public Boolean GrillaRed(ObservableCollection<Productos> p)
        {

            Boolean banderaP = true;
            foreach (var item in p)
            {
                //MessageBox.Show("refere:"+ item.Referencia.ToString());
                if (BuscarRef(item.Referencia.ToString()) == false)
                {
                    banderaP = false;
                }
            }
            return banderaP;
        }


        private void dtGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                int columnIndex = this.dtGrid.SelectionController.CurrentCellManager.CurrentRowColumnIndex.ColumnIndex;
                //var mappingName = this.dtGrid.Columns[columnIndex].MappingName;              
                if (columnIndex == 0)
                {
                    if (string.IsNullOrEmpty(_productos[dtGrid.SelectedIndex].Referencia) || BuscarRef(_productos[dtGrid.SelectedIndex].Referencia.Trim()) == false)
                    {
                        int idr = 0; string code = ""; string nom = "";
                        dynamic winb = SiaWin.WindowBuscar("inmae_ref", "cod_ref", "nom_ref", "cod_ref", "idrow", "Maestra de referencia", SiaWin.Func.DatosEmp(idemp), false, "", idEmp: idemp);
                        winb.ShowInTaskbar = false;
                        winb.Owner = System.Windows.Application.Current.MainWindow;
                        winb.Height = 300;
                        winb.Width = 400;
                        winb.ShowDialog();
                        idr = winb.IdRowReturn;
                        code = winb.Codigo;
                        nom = winb.Nombre;
                        _productos[dtGrid.SelectedIndex].Referencia = code.Trim();
                        _productos[dtGrid.SelectedIndex].Nombre = nom.Trim();
                        winb = null;
                        dtGrid.View.Refresh();
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error!!:" + w);
            }
        }

        //comentado
        private void dtGrid_CurrentCellEndEdit(object sender, CurrentCellEndEditEventArgs e)
        {
            //dtGrid.View.Refresh();

        }


        //comentado
        private void dtGrid_CurrentCellValidated(object sender, CurrentCellValidatedEventArgs e)
        {
            //int columnIndex = this.dtGrid.SelectionController.CurrentCellManager.CurrentRowColumnIndex.ColumnIndex;
            //var mappingName = this.dtGrid.Columns[columnIndex].MappingName;
            //if (e.Column.MappingName == mappingName)
            //{        
            //    var orderCollection = dtGrid.ItemsSource as ObservableCollection<Productos>;
            //    foreach (var record in orderCollection)
            //    {             
            //        if (record.Referencia == e.OldValue.ToString())                 
            //            record.Referencia = e.NewValue.ToString();
            //    }
            //    this.dtGrid.View.Refresh();
            //}
        }




        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            //this.Close();
            //bandera = false;
            foreach (var data in _productos)
            {
                MessageBox.Show("referencia:" + data.Referencia);
            }

        }

        private void Generar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
                app.Visible = true;
                app.WindowState = XlWindowState.xlMaximized;

                Workbook wb = app.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
                Worksheet ws = wb.Worksheets[1];


                ws.Range["A1"].Value = "Referencia";
                ws.Range["B1"].Value = "Nombre";
                ws.Range["C1"].Value = "Cantidad";
                ws.Range["D1"].Value = "Valor unidad";
                ws.Range["E1"].Value = "IVA";
                ws.Range["F1"].Value = "SubTotal";
                ws.Range["G1"].Value = "Valor IVA";
                ws.Range["H1"].Value = "Total";
                ws.Range["C2:C4"].Value = 0;
                ws.Range["D2:D4"].Value = 0;
                ws.Range["F2:F4"].FormulaLocal = "=(C2*D2)";
                ws.Range["G2"].FormulaLocal = "=(F2*E2)";
                ws.Range["H2"].FormulaLocal = "=SUMA(F2+G2)";


                //string ruta = Guardar();
                //wb.SaveAs(ruta);
            }
            catch (Exception w)
            {
                MessageBox.Show("erro al generar la plantilla:" + w);
            }
        }




    }
}