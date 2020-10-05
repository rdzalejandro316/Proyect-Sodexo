using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;


namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9480,"ImporatDocXML");    
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9480, "ImporatDocXML");
    //ww.ShowInTaskbar=false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
    //ww.ShowDialog(); 


    public class Productos : IDataErrorInfo
    {

        public Boolean existencia;
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
                ImporatDocXML principal = new ImporatDocXML();

                principal.minibandera = principal.BuscarRef(Referencia);
                existencia = principal.minibandera;
                if (principal.minibandera == false)
                {
                    return "La referencia no existe: " + this.Referencia;
                }

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

    public partial class ImporatDocXML : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public Boolean minibandera = false;

        private ObservableCollection<Productos> _productos;
        public ObservableCollection<Productos> Produ
        {
            get { return _productos; }
            set { _productos = value; }
        }
        public object iva = new object();
        public string ruta;
        public string second;
        public System.Data.DataTable TablaXML = new DataTable();
        public Boolean bandera = false;
        public DateTime fechaPeri;
        public DateTime FechaDocumento;
        public DateTime FechaXML;
        public List<object> lalo = new List<object>();


        List<object> totaList = new List<object>();
        List<object> produList = new List<object>();
        List<object> legaList = new List<object>();
        List<object> subList = new List<object>();
        List<object> uniqueList = new List<object>();
        XNamespace fe = "http://www.dian.gov.co/contratos/facturaelectronica/v1";
        XNamespace cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
        XNamespace cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";

        public ImporatDocXML()
        {
            InitializeComponent();

            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId; ;
            LoadConfig();


            TablaXML.Columns.Add("cod_ref");
            TablaXML.Columns.Add("Descripcion");
            TablaXML.Columns.Add("Cantidad");
            TablaXML.Columns.Add("Valor_unitario");
            TablaXML.Columns.Add("Valor_Iva");
            TablaXML.Columns.Add("Valor_SubTotal");
            TablaXML.Columns.Add("Valor_TotalIva");
            TablaXML.Columns.Add("Valor_Total");
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
                this.Title = "Importar XML - Empresa:" + cod_empresa + "-" + nomempresa;
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void Llenar()
        {
            TablaXML.Clear();
            int cont = 0, con1 = 0, con2 = 0, con3 = 0, con4 = 0, con9 = 0, conFina = 0;
            XElement xelement = XElement.Load(ruta);

            var unique = from el in xelement.Elements(fe + "InvoiceLine") select el;
            var sub = from el in xelement.Elements(fe + "TaxTotal").Elements().Elements() select el;
            var codigo = from el in xelement.Elements(fe + "InvoiceLine").Descendants(cbc + "ID") select el;
            var cantidad = from el in xelement.Elements(fe + "InvoiceLine").Elements(cbc + "InvoicedQuantity") select el;
            var description = from el in xelement.Elements(fe + "InvoiceLine").Elements(fe + "Item").Elements(cbc + "Description") select el;
            var valUnit = from el in xelement.Elements(fe + "InvoiceLine").Elements(fe + "Price").Elements(cbc + "PriceAmount") select el;
            var IVA = from el in xelement.Elements(fe + "TaxTotal").Elements(fe + "TaxSubtotal").Elements(cbc + "Percent") select el;
            var valTot = from el in xelement.Elements(fe + "InvoiceLine").Elements(cbc + "LineExtensionAmount") select el;
            var totalPago = xelement.Descendants(cbc + "PayableAmount");

            var numFac = xelement.Descendants(cbc + "ID").FirstOrDefault();
            var fechFac = xelement.Descendants(cbc + "IssueDate").FirstOrDefault();

            foreach (var el in unique)
            {
                cont += 1;
            }
            object[] sharpArray = new object[cont];
            object[] codigoArray = new object[cont];
            object[] cantidadArrray = new object[cont];
            object[] descripcionArray = new object[cont];
            object[] valunitArray = new object[cont];
            object[] totArray = new object[cont];

            FacTXT.Text = numFac.Value;

            //DateTime dateValue = Convert.ToDateTime(fechFac.Value);
            //FechaXML = dateValue;
            TX_FecXML.Text = fechFac.Value;
            _productos = new ObservableCollection<Productos>();
            foreach (var item in codigo)
            {
                if (Regex.Matches(item.Value, "_").Count == 1)
                {
                    Console.WriteLine("Sirve");
                }
                string[] textos = item.Value.Split('_');
                codigoArray[con1] = textos[1];
                lalo.Add(textos[1]);
                sharpArray[con1] = textos[0];
                con1 += 1;
            }
            foreach (var item in cantidad)
            {
                string def = QuitarZero(item.Value);
                cantidadArrray[con2] = def;
                con2 += 1;
            }
            foreach (var item in description)
            {
                descripcionArray[con3] = item.Value;
                con3 += 1;
            }
            foreach (var item in valUnit)
            {
                string def = QuitarZero(item.Value);
                valunitArray[con4] = def;
                con4 += 1;
            }
            foreach (var item in IVA)
            {
                string def = QuitarZero(item.Value);
                iva = def;
            }
            foreach (var item in valTot)
            {
                string def = QuitarZero(item.Value);
                totArray[con9] = def;
                con9 += 1;
            }
            int tutIVA = 0, tutal;
            foreach (var item in unique)
            {
                tutIVA = ((Convert.ToInt32(totArray[conFina]) * Convert.ToInt32(iva)) / 100);
                tutal = (tutIVA + Convert.ToInt32(totArray[conFina]));
                //TablaXML.Rows.Add(codigoArray[conFina], cantidadArrray[conFina], descripcionArray[conFina], valunitArray[conFina], "IVA", iva, "Descuento", totArray[conFina]);
                _productos.Add(new Productos(Convert.ToString(codigoArray[conFina]), Convert.ToString(descripcionArray[conFina]), Convert.ToString(cantidadArrray[conFina]), Convert.ToString(valunitArray[conFina]), Convert.ToString(iva), Convert.ToString(totArray[conFina]), Convert.ToString(tutIVA), Convert.ToString(tutal)));
                conFina += 1;//En el xml no se encuentran datos del descuento por producto, solo el total de el descuento al final de la factura en pdf                
            }
            foreach (var item in sub)
            {
                subList.Add(item.Value);
            }
            foreach (var item in totalPago)
            {
                TxtTotal.Text = (item.Value);
            }
            //foreach (var item in factura)
            //{
            //    FacTXT.Text += (" " + item.Value);
            //}
            //DataProducto.ItemsSource = TablaXML.DefaultView;
            //DataProducto.Columns.Add(new GridTextColumn() { MappingName = "Referencia" });
            DataProducto.ItemsSource = Produ;
            TotalReg.Text = Convert.ToString(DataProducto.View.Records.Count());
            string sTotal = Convert.ToString(subList[0]);
            double STotal = Convert.ToDouble(sTotal);
            Sotal.Text = (sTotal);
            TIVA.Text = Convert.ToString(subList[1]);
            txtIva.Text = "Iva " + iva + "% : "; ;

        }
        public string QuitarZero(string zero)
        {
            string[] esplit = zero.Split('.');
            //MessageBox.Show(esplit[0]);
            return esplit[0];
        }

        private void LeerXML()
        {
            try
            {


                if (ruta != null)
                {
                    XElement xelement = XElement.Load(ruta);
                    var proID = xelement.Elements(fe + "AccountingSupplierParty").Elements(fe + "Party").Elements().Elements(cbc + "Name");
                    var proNIT = xelement.Elements(fe + "AccountingSupplierParty").Elements(fe + "Party").Elements().Elements(cbc + "ID");
                    var proDir = xelement.Elements(fe + "AccountingSupplierParty").Elements(fe + "Party").Elements().Elements().Elements().Elements(cbc + "Line");
                    var cliID = xelement.Elements(fe + "AccountingCustomerParty").Elements(fe + "Party").Elements().Elements(cbc + "Name");
                    var cliNIT = xelement.Elements(fe + "AccountingCustomerParty").Elements(fe + "Party").Elements().Elements(cbc + "ID");
                    var cliDir = xelement.Elements(fe + "AccountingCustomerParty").Elements(fe + "Party").Elements().Elements().Elements().Elements(cbc + "Line");
                    var city = xelement.Descendants(cbc + "CityName");
                    var dire = xelement.Descendants(cbc + "Line");
                    var tele = xelement.Descendants(cbc + "Telephone");
                    var unique = from el in xelement.Elements(fe + "InvoiceLine") select el;
                    var priNod = from el in xelement.Elements(fe + "InvoiceLine").Elements() select el;
                    var producto = from el in xelement.Elements(fe + "InvoiceLine").Elements().Elements() select el;
                    var total = from el in xelement.Elements(fe + "InvoiceLine").Elements().Elements().Elements() select el;
                    List<object> idList = new List<object>();
                    List<object> cityList = new List<object>();
                    foreach (var item in proID)//Nombre proveedor
                    {
                        NombreTXT.Text = item.Value;
                    }
                    foreach (var item in proNIT)
                    {
                        NITTXT.Text = item.Value;
                    }
                    foreach (var item in proDir)
                    {
                        DirTXT.Text = item.Value;
                    }
                    foreach (var item in cliID)
                    {
                        NombreTXT2.Text = item.Value;
                    }
                    foreach (var item in cliNIT)
                    {
                        NITTXT2.Text = item.Value;
                    }
                    foreach (var item in cliDir)
                    {
                        DirTXT2.Text = item.Value;
                    }
                    foreach (XElement el in producto)
                    {
                        produList.Add(el.Value);
                    }
                    foreach (var item in total)
                    {
                        totaList.Add(item.Value);
                    }
                    foreach (var item in city)
                    {
                        cityList.Add(item.Value);
                    }
                    Llenar();
                }
                else
                {
                    //App.Current.MainWindow.Close();
                    MessageBox.Show("no pudo leer");
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("no se pudo leer bien:"+w);
            }
        }

        private void BuscarArchivo()
        {
            try
            {                

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
                {
                    DefaultExt = ".xml",
                    Filter = "XML Files (*.xml)|*.xml"
                };
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    string filename = dlg.FileName;
                    ruta = filename;
                }
                LeerXML();
            }
            catch (Exception w)
            {
                MessageBox.Show("error:" + w);
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BuscarArchivo();
        }

        public Boolean validarNitNumeroFact()
        {
            string cadena = "select num_trn,cod_prv,doc_ref from InCab_doc where cod_prv='" + NITTXT.Text + "' and doc_ref='" + FacTXT.Text + "' ";
            DataTable tabla = SiaWin.Func.SqlDT(cadena, "Clientes", idemp);


            if (tabla.Rows.Count > 0)
            {
                MessageBox.Show("se encontro el numero de la factura en el documento:" + tabla.Rows[0]["num_trn"].ToString());
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean validarFecha()
        {
            string fecMesDoc = FechaDocumento.ToString("MM").Trim();
            string fecAnoDoc = FechaDocumento.ToString("yyyy").Trim();

            string FecMesxml = FechaXML.ToString("MM").Trim();
            string FecAnoxml = FechaXML.ToString("yyyy").Trim();

            if (fecMesDoc == FecMesxml && fecAnoDoc == FecAnoxml)
            {
                return false;
            }
            else
            {
                MessageBox.Show("la fecha del Documento es diferente a la fecha del XML fecha_documento:" + FechaDocumento.ToString("dd/MM/yyyy").Trim() + " fecha_xml:" + FechaXML.ToString("dd/MM/yyyy").Trim() + " debe de pertenecer al mismo periodo");
                return true;
            }
        }

        private void BTNvalidar_Click(object sender, RoutedEventArgs e)
        {


            foreach (var data in _productos)
            {
                System.Data.DataRow row = TablaXML.NewRow();
                row["cod_ref"] = data.Referencia;
                row["Descripcion"] = data.Nombre;
                row["Cantidad"] = data.Cantidad;
                row["Valor_unitario"] = data.ValUnit;
                row["Valor_Iva"] = data.IVA;
                row["Valor_SubTotal"] = data.SubTotal;
                row["Valor_TotalIva"] = data.ValIVA;
                row["Valor_Total"] = data.Total;
                TablaXML.Rows.Add(row);
            }

            //if (TablaXML.Rows.Count <= 0) { MessageBox.Show("ingrese un XML para validar"); return; };

            //if (validarNitNumeroFact() == true) return;

            //if (validarFecha() == true) return;

            //recorrer();
            //foreach (var item in Produ)
            //{
            //    MessageBox.Show("refenecia:"+item.Referencia); 
            //}
            bandera = true;
            this.Close();
            #region Validacion
            if (GrillaRed(_productos) == true)
            {
                bandera = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Verifique que no existan errores en la grilla");
            }
            #endregion

            //if (minibandera == true)
            //{
            //     MessageBox.Show("ya puede");
            //}
            //else
            //{
            //    MessageBox.Show("no puede");
            //}

            //MessageBox.Show("GridError:" + GridError);

            //bandera = true;
            //this.Close();

        }

        public void recorrer()
        {
            var reflector = this.DataProducto.View.GetPropertyAccessProvider();
            int a = 1;
            foreach (var row in DataProducto.View.Records)
            {
                foreach (var column in DataProducto.Columns)
                {
                    if (column.MappingName == "cod_ref")
                    {
                        //                        row[a]["cod_ref"] = "";
                        // var rowData = DataProducto.GetRecordAtRowIndex(a);

                        //var referencias = reflector.GetValue(rowData, "cod_ref");
                        //break;
                    }
                }
                a = a + 1;
            }
        }

        public Boolean BuscarRef(string referencia)
        {
            string cadena = "select * from inmae_ref where cod_ref='" + referencia + "' ";
            DataTable dt = SiaWin.Func.SqlDT(cadena, "Clientes", idemp);
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                //BTNvalidar.IsEnabled = false;
                return false;
            }
        }

        public Boolean Buscar(string referencia)
        {
            string cadena = "select * from inmae_ref where cod_ref='" + referencia + "' ";
            DataTable dt = SiaWin.Func.SqlDT(cadena, "Clientes", idemp);
            if (dt.Rows.Count > 0)
            {
                MessageBox.Show("encontrado-----:" + referencia);
                return true;
            }
            else
            {
                MessageBox.Show("no esta---:" + referencia);
                return false;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TX_FecDoc.Text = fechaPeri.ToString();
            FechaDocumento = fechaPeri;
        }


        private void DataProducto_CurrentCellValueChanged(object sender, CurrentCellValueChangedEventArgs e)
        {

        }

        private void TextBox1_KeyPress(object sender, KeyEventArgs e)
        {
            //textBox2.AppendText($"KeyUp code: {e.KeyCode}, value: {e.KeyValue}, modifiers: {e.Modifiers}" + "\r\n");
        }


        private void DataProducto_CurrentCellValidating(object sender, CurrentCellValidatingEventArgs e)
        {

            try
            {
                if ((Keyboard.IsKeyDown(Key.F8)) || (Keyboard.IsKeyDown(Key.F6)))
                {
                    int idr = 0; string code = ""; string nom = "";
                    dynamic winb = SiaWin.WindowBuscar("inmae_ref", "cod_ref", "nom_ref", "cod_ref", "idrow", "Maestra dereferencia", SiaWin.Func.DatosEmp(idemp), false, "", idEmp: idemp);
                    winb.ShowInTaskbar = false;
                    winb.Owner = Application.Current.MainWindow;
                    winb.Height = 300;
                    winb.Width = 400;
                    winb.ShowDialog();
                    idr = winb.IdRowReturn;
                    code = winb.Codigo;
                    nom = winb.Nombre;
                    winb = null;



                    //Productos productos = new Productos(Convert.ToString(code), Convert.ToString(nom), "", "", "", "");


                    //_productos.Add(new Productos(Convert.ToString(code), Convert.ToString(nom),"","","",""));
                    //var uiElement = e.OriginalSource as UIElement;
                    //uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));


                    //Productos row = (DataRowView)DataProducto.SelectedItems[0];
                    //row["cod_ref"] = code;

                    //Productos pr = new Productos(code, code, code, code, code, code);
                    //pr.Referencia = "ejmeplo";
                    //DataProducto.UpdateDataRow(e.RowColumnIndex.RowIndex);

                    //if (idr > 0)
                    //{
                    //    TB_CodigoZonaSuc.Text = code;
                    //    TB_ZonaSuc.Text = nom;
                    //    var uiElement = e.OriginalSource as UIElement;
                    //    uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    //}
                }
                else
                {
                    MessageBox.Show("no llego");
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error!!:" + w);
            }
        }



        private void DataProducto_CurrentCellBeginEdit(object sender, CurrentCellBeginEditEventArgs e)
        {
            try
            {

                int idr = 0; string code = ""; string nom = "";
                dynamic winb = SiaWin.WindowBuscar("inmae_ref", "cod_ref", "nom_ref", "cod_ref", "idrow", "Maestra dereferencia", SiaWin.Func.DatosEmp(idemp), false, "", idEmp: idemp);
                winb.ShowInTaskbar = false;
                winb.Owner = Application.Current.MainWindow;
                winb.Height = 300;
                winb.Width = 400;
                winb.ShowDialog();
                idr = winb.IdRowReturn;
                code = winb.Codigo;
                nom = winb.Nombre;
                winb = null;


                //e.Cancel = async;

                int a = DataProducto.SelectedIndex;
                _productos[a - 1].Referencia = "Vikas-Modified";
                //Productos productos = new Productos(Convert.ToString(code), Convert.ToString(nom), "", "", "", "");
                //productos.Referencia = "PRUEBA";

            }
            catch (Exception w)
            {
                MessageBox.Show("error!!:" + w);
            }
        }



        public Boolean GrillaRed(ObservableCollection<Productos> p)
        {

            Boolean bandera = true;
            foreach (var item in p)
            {
                if (BuscarRef(item.Referencia.ToString()) == false)
                {
                    bandera = false;
                }
                //MessageBox.Show("ref:" + item.Referencia + " EMPYTY:" +string.IsNullOrEmpty(item.Error));

                //if (item.Error.Length>0)
                //{
                //    MessageBox.Show("enytp");
                //    return false;
                //}
                ////if (!string.IsNullOrEmpty(item.Error))
                ////{
                ////    MessageBox.Show("enytp");
                ////    return false;
                ////}                


            }

            //MessageBox.Show("llego a este");
            return bandera;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            bandera = false;
            this.Close();
        }

        private void DataProducto_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_productos[DataProducto.SelectedIndex].Referencia))
                {

                    DataProducto.View.Refresh();
                    if (e.Key == Key.F8 || e.Key == Key.Enter || e.Key == Key.Delete || e.Key == Key.Back)
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
                        _productos[DataProducto.SelectedIndex].Referencia = code.Trim();
                        _productos[DataProducto.SelectedIndex].Nombre = nom.Trim();
                        winb = null;

                        DataProducto.View.Refresh();

                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error!!:" + w);
            }
        }
    }
}

