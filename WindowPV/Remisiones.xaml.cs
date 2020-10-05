using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WindowPV
{

    public partial class Remisiones : Window
    {
        //Sia.PublicarPnt(9460, "WindowPV"); 
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";       

        public DataTable tabla = new DataTable();
        public DataTable tablaTemporal = new DataTable();
        public string bodegaRemision = "";

        public string nit_bodega;

        double procentaje_desc;
        double valor_ref;
        double iva;
        string cod_tiva;
        public int PntTip = 0;

        public string tipoTransaccion = "";

        public string campoDescTip = "";
        public string campoDescuentoLinea = "";

        double total_prod = 0;

        public int posicion = 0;

        public Remisiones()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId; ;
            LoadConfig();
            pantalla();



            tablaTemporal.Columns.Add("val_ref", typeof(string));
            tablaTemporal.Columns.Add("cod_ref", typeof(string));
            tablaTemporal.Columns.Add("nom_ref", typeof(string));
            tablaTemporal.Columns.Add("val_uni", typeof(decimal));
            tablaTemporal.Columns.Add("cantidad", typeof(decimal));
            tablaTemporal.Columns.Add("val_iva", typeof(decimal));
            tablaTemporal.Columns.Add("por_des", typeof(decimal));
            tablaTemporal.Columns.Add("por_iva", typeof(decimal));                    
            tablaTemporal.Columns.Add("subtotal", typeof(decimal));            
            tablaTemporal.Columns.Add("total", typeof(decimal));

            tablaTemporal.Columns.Add("nom_tip", typeof(string));
            tablaTemporal.Columns.Add("nom_prv", typeof(string));
        }
    
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            BodCod.Focus();

            if (tipoTransaccion.Trim() == "004")
            {
                campoDescTip = "des_mos";
                campoDescuentoLinea = "por_des";
            }
            else
            {
                campoDescTip = "por_des";
                campoDescuentoLinea = "por_desc";
            }

        }

        private void LoadConfig()
        {
            try
            {
                DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Remisiones - Empresa:" + cod_empresa + "-" + nomempresa;
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        public void pantalla()
        {
            this.MinHeight = 600;
            this.MaxHeight = 600;
            this.MinWidth = 1200;
            this.MaxWidth = 1200;
        }

        private void Txt_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                string idTab = ((TextBox)sender).Tag.ToString();
                if (idTab.Length > 0)
                {
                    string tag = ((TextBox)sender).Tag.ToString();
                    string cmptabla = ""; string cmpcodigo = ""; string cmpnombre = ""; string cmporden = ""; string cmpidrow = ""; string cmptitulo = ""; string cmpconexion = ""; bool mostrartodo = true; string cmpwhere = "";   
                    if (string.IsNullOrEmpty(tag)) return;

                    if (tag == "inmae_bod")
                    {
                        cmptabla = tag; cmpcodigo = "cod_bod"; cmpnombre = "UPPER(nom_bod)"; cmporden = "cod_bod"; cmpidrow = "cod_bod"; cmptitulo = "Maestra de Bodegas"; cmpconexion = cnEmp; mostrartodo = true; cmpwhere = "tipo_bod='4' ";
                    }

                    int idr = 0; string code = ""; string nom = "";
                    dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, cnEmp, mostrartodo, cmpwhere, idEmp: idemp);
                    winb.ShowInTaskbar = false;
                    winb.Owner = Application.Current.MainWindow;
                    winb.Height = 300;
                    winb.Width = 400;
                    winb.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    winb.ShowDialog();
                    idr = winb.IdRowReturn;
                    code = winb.Codigo;
                    nom = winb.Nombre;
                    winb = null;
                    if (idr > 0)
                    {
                        if (tag == "inmae_bod")
                        {
                            BodCod.Text = code;
                            BodNom.Text = nom;

                            GetNitBodega(code);
                        }
                        var uiElement = e.OriginalSource as UIElement;
                        uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                        BTNconsultar.Focus();
                    }
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }

        public void GetNitBodega(string bodega)
        {
            string cadena = "select cod_ter from inmae_bod where cod_bod='" + bodega + "' ";
            DataTable dt = SiaWin.Func.SqlDT(cadena, "Clientes", idemp);
            nit_bodega = dt.Rows[0]["cod_ter"].ToString();
        }

      

        private async void BTNconsultar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(BodCod.Text))
            {
                MessageBox.Show("seleccione la bodega que quiere consultar");
                return;
            }


            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            dataGridTabla.IsEnabled = false;
            sfBusyIndicator.IsBusy = true;

            string bod = BodCod.Text;
            string fec = DateTime.Now.ToString("dd/MM/yyyy");
            string emp = cod_empresa;

            var slowTask = Task<DataSet>.Factory.StartNew(() => CargarConsulta(bod, fec, emp, source.Token), source.Token);
            await slowTask;

            if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
            {
                tabla = ((DataSet)slowTask.Result).Tables[0];

                dataGridTabla.ItemsSource = tabla;
                Total.Text = tabla.Rows.Count.ToString();

                BTNfacturar.IsEnabled = true;
                BTNSalir.IsEnabled = true;
                sfBusyIndicator.IsBusy = false;
                dataGridTabla.IsEnabled = true;
            }


        }

        public DataSet CargarConsulta(string bodega,string fecha,string empresa, CancellationToken cancellationToken)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();                
                cmd = new SqlCommand("_EmpWindowsPVRemision", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Bod", bodega);
                cmd.Parameters.AddWithValue("@Fecha", fecha);                
                cmd.Parameters.AddWithValue("@Tip", "");
                cmd.Parameters.AddWithValue("@codemp", empresa);
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                con.Close();                
            }
            catch (Exception w)
            {
                MessageBox.Show("erro en la consulta" + w);
            }
            return ds;
        }


        private void dataGridTabla_CurrentCellEndEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)dataGridTabla.SelectedItems[0];
                decimal Saldo = Convert.ToDecimal(row["saldo_fin"]);
                decimal Cant = Convert.ToDecimal(row["cantidad"]);
                decimal valor = Convert.ToDecimal(row["val_ref"]);

                //MessageBox.Show("Cantidad:"+Cantidad);

                if (Cant == 0)
                {
                    //MessageBox.Show("no se hiso ningun cambio");
                    row["cantidad"] = "0.00";
                    row["subtotal"] = "";
                    row["val_uni"] = "";
                    row["val_iva"] = "";
                    row["por_des"] = "";
                    row["por_iva"] = "";
                    row["total"] = "";
                    TotalFacturar.Text = Convert.ToString(getCantidadActualPro());

                    
                    return;
                    
                }

                if (Cant > Saldo)
                {
                    MessageBox.Show("No puede ingresar una cantidad mayor a la que se encuentra en saldo");
                    row["cantidad"] = "0.00";                    
                    row["subtotal"] = "";
                    row["val_uni"] = "";
                    row["val_iva"] = "";
                    row["por_des"] = "";
                    row["por_iva"] = ""; 
                    row["total"] = "";

                    TotalFacturar.Text = Convert.ToString(getCantidadActualPro());
                }
                else
                {

                    ActualizaCamposRef(row);

                    
                    //CalcularDesc(row["cod_ref"].ToString());                    
                    //decimal _desc = 1 - Convert.ToDecimal(procentaje_desc) / 100;
                    //decimal valorUni = valor_ref * _desc / (1 + (iva) / 100);//val_uni                

                    //decimal subtotal = valorUni * Cantidad;//subtotal 
                    //row["subtotal"] = (string.Format(("{0:C}"), subtotal));

                    //row["val_uni"] = (string.Format(("{0:C}"), valorUni));

                    //int valorIva = (Convert.ToInt32(subtotal) * Convert.ToInt32(iva)) / 100;
                    //row["val_iva"] = (string.Format(("{0:C}"), valorIva));

                    //row["por_des"] = procentaje_desc;

                    //row["por_iva"] = iva;

                    //decimal total = subtotal + valorIva;//tot_tot
                    //row["total"] = (string.Format(("{0:C}"), total));                    
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error-edit:" + w);
                DataRowView row = (DataRowView)dataGridTabla.SelectedItems[0];
                row["cantidad"] = "0.00";
            }
        }

        public double getCantidadActualPro()
        {
            try
            {
                double valueCant = 0;

                int a = 1;
                var reflector = this.dataGridTabla.View.GetPropertyAccessProvider();
                foreach (var row in dataGridTabla.View.Records)
                {
                    foreach (var column in dataGridTabla.Columns)
                    {
                        if (column.MappingName == "cantidad")
                        {
                            var rowData = dataGridTabla.GetRecordAtRowIndex(a);
                            
                            var cant = reflector.GetValue(rowData, "cantidad");
                            

                            if (cant.ToString() != "0.00" && cant.ToString() != "0")
                            {
                                valueCant += Convert.ToDouble(cant);
                                
                            }
                            break;
                        }
                    }
                    a = a + 1;
                }

                return valueCant;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al facturar:" + w);
                return 0;
            }
        }




        public void ActualizaCamposRef(DataRowView row)
        {
            try
            {
                string cmpval_uni = "inmae_ref.val_ref as val_ref";
                //if (ConfigCSource.IsBusinessGroup == true) cmpval_uni = "inmae_ref.vrunc as val_ref";

                string query = "select inmae_ref.idrow,inmae_ref.cod_ref,inmae_ref.cod_ant,rtrim(nom_ref) as nom_ref,inmae_ref.cod_tip,inmae_ref.cod_tiva, ";
                query = query + "inmae_tiva.por_iva,inmae_ref.val_ref as precioLista," + cmpval_uni + ",isnull(InList_cli.Val_uni,0) as val_refList, ";
                query = query + "nom_tip,nom_prv,inmae_tip." + campoDescuentoLinea + " as '" + campoDescuentoLinea + "', ";
                query = query + "isnull(inter_tip." + campoDescTip + ",0) as '" + campoDescTip + "', ";
                query = query + "isnull(InList_cli.Por_des,0) as decuentoLista ";
                query = query + "FROM inmae_ref ";
                query = query + "inner join inmae_tiva on inmae_tiva.cod_tiva=inmae_ref.cod_tiva  ";
                query = query + "inner join inmae_tip on inmae_tip.cod_tip=inmae_ref.cod_tip  ";
                query = query + "left join inmae_prv on inmae_prv.cod_prv=inmae_ref.cod_prv  ";
                query = query + "left join inter_tip on inter_tip.Cod_ter='" + nit_bodega.Trim() + "' and inter_tip.cod_tip=inmae_Ref.cod_tip  ";
                query = query + "left join InList_cli on InList_cli.Cod_ter='" + nit_bodega.Trim() + "' and InList_cli.Cod_ref='" + row["cod_ref"].ToString().Trim() + "'  ";
                query = query + "where  inmae_ref.cod_ref='" + row["cod_ref"].ToString().Trim() + "' ";

                SqlDataReader dr = SiaWin.DB.SqlDR(query, idemp);

                while (dr.Read())
                {
                    decimal DecLista = Convert.ToDecimal(dr["val_refList"]);
                    double val_uni = 0;
                    double cantidad = Convert.ToDouble(row["cantidad"]);

                    iva = Convert.ToDouble(dr["por_iva"]);
                    cod_tiva = dr["cod_tiva"].ToString();


                    if (Convert.ToDouble(dr["decuentoLista"]) > 0)
                    {
                        procentaje_desc = Convert.ToDouble(dr["decuentoLista"]);
                    }
                    else if (Convert.ToDouble(dr[campoDescTip]) > 0)
                    {
                        procentaje_desc = Convert.ToDouble(dr[campoDescTip]);
                    }
                    else if (Convert.ToDouble(dr[campoDescuentoLinea]) > 0)
                    {
                        procentaje_desc = Convert.ToDouble(dr[campoDescuentoLinea]);
                    }

                    string valorRef = DecLista > 0 ? "val_refList" : "val_ref";
                    //_desc = 1 - Convert.ToDecimal(procentaje_desc) / 100;

                    if (valorRef == "val_refList")
                    {
                        if (iva > 0)
                        {
                            double _valref = Convert.ToDouble(dr[valorRef]) / (1 + (Convert.ToDouble(dr["por_iva"]) / 100));
                            val_uni = Math.Round(_valref, 0);
                            //valIva = _valref * (1 + (Convert.ToDouble(dr["por_iva"]) / 100));
                        }
                        if (iva == 0)
                        {
                            double _valref = Convert.ToDouble(dr[valorRef]);
                            val_uni = Math.Round(_valref, 0);
                            //valIva = _valref * (1 + (Convert.ToDouble(dr["por_iva"]) / 100));
                        }

                    }
                    else
                    {
                        if (iva > 0)
                        {
                            double _desc = 1 - (Convert.ToDouble(procentaje_desc)) / 100;
                            double _valref = Convert.ToDouble(dr["val_ref"]) * _desc / (1 + (Convert.ToDouble(dr["por_iva"]) / 100));
                            val_uni = Math.Round(_valref, 0);
                            //valIva = _valref * (1 + (Convert.ToDouble(dr["por_iva"]) / 100));
                        }
                        if (iva == 0)
                        {
                            double _valref = Convert.ToDouble(dr["val_ref"]);
                            val_uni = Math.Round(_valref, 0);
                            //valIva = _valref * (1 + (Convert.ToDouble(dr["por_iva"]) / 100));
                        }
                        //ConfigCSource.ValUnitMasIva = _valref * (1 + (Convert.ToDouble(dr["por_iva"]) / 100));
                    }


                    

                    double _descFinal = 1 - Convert.ToDouble(procentaje_desc) / 100;
                    
                    double subtotal = val_uni * cantidad;//subtotal 
                    row["subtotal"] = (string.Format(("{0:C}"), subtotal));
                    row["val_uni"] = (string.Format(("{0:C}"), val_uni));

                    double valorIva = (subtotal * iva) / 100;
                    row["val_iva"] = (string.Format(("{0:C}"), valorIva));

                    row["por_des"] = procentaje_desc;

                    row["por_iva"] = iva;

                    double total = subtotal + valorIva;//tot_tot
                    row["total"] = (string.Format(("{0:C}"), total));

                    
                    TotalFacturar.Text = Convert.ToString(getCantidadActualPro());
                }


            }
            catch (Exception)
            {
                MessageBox.Show("error al actualizar la referencia");
            }
        }

     

        private void BTNfacturar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (tabla.Rows.Count > 0)
                {
                    facturar();
                }
                else
                {
                    MessageBox.Show("debe de realizar una consulta para facturar");
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al reccorer:" + w);
            }
        }

        public void facturar()
        {
            try
            {
                int a = 1;
                var reflector = this.dataGridTabla.View.GetPropertyAccessProvider();
                foreach (var row in dataGridTabla.View.Records)
                {
                    foreach (var column in dataGridTabla.Columns)
                    {
                        if (column.MappingName == "cantidad")
                        {
                            var rowData = dataGridTabla.GetRecordAtRowIndex(a);
                            var referencias = reflector.GetValue(rowData, "cod_ref");
                            var nombre = reflector.GetValue(rowData, "nom_ref");
                            var valorRef = reflector.GetValue(rowData, "val_ref");
                            var valorUni = reflector.GetValue(rowData, "val_uni");
                            var cant = reflector.GetValue(rowData, "cantidad");
                            var valIva = reflector.GetValue(rowData, "val_iva");
                            var porDes = reflector.GetValue(rowData, "por_des");
                            var porIva = reflector.GetValue(rowData, "por_iva");
                            var subTot = reflector.GetValue(rowData, "subtotal");
                            var tot = reflector.GetValue(rowData, "total");

                            var linea = reflector.GetValue(rowData, "nom_tip");
                            var provedor = reflector.GetValue(rowData, "nom_prv");

                            if (cant.ToString() != "0.00" && cant.ToString() != "0")
                            {

                                decimal val_ref = Decimal.Parse(valorRef.ToString(), NumberStyles.Currency);
                                decimal val_uni = Decimal.Parse(valorUni.ToString(), NumberStyles.Currency);
                                decimal cantidad = Decimal.Parse(cant.ToString(), NumberStyles.Currency);
                                decimal val_iva = Decimal.Parse(valIva.ToString(), NumberStyles.Currency);
                                decimal por_des = Decimal.Parse(porDes.ToString(), NumberStyles.Currency);
                                decimal por_iva = Decimal.Parse(porIva.ToString(), NumberStyles.Currency);
                                decimal subtotal = Decimal.Parse(subTot.ToString(), NumberStyles.Currency);
                                decimal total = Decimal.Parse(tot.ToString(), NumberStyles.Currency);



                                //tablaTemporal.Columns.Add("val_ref", typeof(string));
                                //tablaTemporal.Columns.Add("cod_ref", typeof(string));
                                //tablaTemporal.Columns.Add("nom_ref", typeof(string));
                                //tablaTemporal.Columns.Add("val_uni", typeof(decimal));
                                //tablaTemporal.Columns.Add("cantidad", typeof(decimal));
                                //tablaTemporal.Columns.Add("val_iva", typeof(decimal));
                                //tablaTemporal.Columns.Add("por_des", typeof(decimal));
                                //tablaTemporal.Columns.Add("por_iva", typeof(decimal));
                                //tablaTemporal.Columns.Add("subtotal", typeof(decimal));
                                //tablaTemporal.Columns.Add("total", typeof(decimal));;

                                tablaTemporal.Rows.Add(val_ref, referencias, nombre, val_uni, cantidad, val_iva, por_des,por_iva,subtotal,total,linea,provedor);
                            }
                            break;
                        }
                    }
                    a = a + 1;
                }

                //tablaTemporal.Columns.Add("val_ref", typeof(string));
                //tablaTemporal.Columns.Add("cod_ref", typeof(string));
                //tablaTemporal.Columns.Add("nom_ref", typeof(string));
                //tablaTemporal.Columns.Add("val_uni", typeof(decimal));
                //tablaTemporal.Columns.Add("cantidad", typeof(decimal));
                //tablaTemporal.Columns.Add("val_iva", typeof(decimal));
                //tablaTemporal.Columns.Add("por_des", typeof(decimal));
                //tablaTemporal.Columns.Add("por_iva", typeof(decimal));
                //tablaTemporal.Columns.Add("subtotal", typeof(decimal));
                //tablaTemporal.Columns.Add("total", typeof(decimal));
                PntTip = 2;
                bodegaRemision = BodCod.Text.Trim();
                this.Close();
                //dataGridTabla.ItemsSource = tablaTemporal.DefaultView;

            }
            catch (Exception w)
            {
                MessageBox.Show("error al facturar:"+w);
            }
        }

        
        private void BTNSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
      

        private void BTNbuscar_Click(object sender, RoutedEventArgs e)
        {
            this.dataGridTabla.SearchHelper.FindNext(Texto_Busc.Text);
            this.dataGridTabla.SelectionController.MoveCurrentCell(this.dataGridTabla.SearchHelper.CurrentRowColumnIndex);
        }

        private void Limpiar_Click(object sender, RoutedEventArgs e)
        {
            this.dataGridTabla.SearchHelper.ClearSearch();
        }

        private void dataGridTabla_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F6)
            {
                //Buscar buscar = new Buscar();
                //buscar.ShowDialog();
                int idr = 0; string code = ""; string nom = "";
                dynamic winb = SiaWin.WindowBuscar("InMae_ref", "cod_ref", "nom_ref", "idrow", "idrow", "Maestra de referencia", SiaWin.Func.DatosEmp(idemp), false, "", idEmp: idemp);
                winb.Height = 400;
                winb.Width = 500;
                //winb.ShowInTaskbar = false;
                //winb.Owner = Application.Current.MainWindow;
                winb.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                winb.ShowDialog();
                idr = winb.IdRowReturn;
                code = winb.Codigo;
                nom = winb.Nombre;
                winb = null;

                try
                {
                    //MessageBox.Show("code:"+ code);

                    if (string.IsNullOrEmpty(code)) return;
                    
                    if (recorrer(code.Trim()) == true)
                    {
                        dataGridTabla.SearchHelper.SearchHighlightBrush = Brushes.Transparent;
                        this.dataGridTabla.SearchHelper.ClearSearch();
                        //this.dataGridTabla.SearchHelper.FindNext(code);                        
                        this.dataGridTabla.SearchHelper.FindNext(code);
                        this.dataGridTabla.SelectionController.MoveCurrentCell(this.dataGridTabla.SearchHelper.CurrentRowColumnIndex);
                        dataGridTabla.SearchHelper.SearchHighlightBrush = Brushes.Transparent;
                    }
                    else
                    {
                        MessageBox.Show("no se encuentra la referencia");
                    }
                }
                catch (Exception w)
                {

                    MessageBox.Show("recorrer *: "+w);
                }
                
                

            }


        }

        public Boolean recorrer(string WinRef)
        {            
            Boolean bandera = false;
            var reflector = this.dataGridTabla.View.GetPropertyAccessProvider();
            int a = 1;
            foreach (var row in dataGridTabla.View.Records)
            {
                foreach (var column in dataGridTabla.Columns)
                {
                    if (column.MappingName == "cod_ref")
                    {
                        var rowData = dataGridTabla.GetRecordAtRowIndex(a);

                        var referencias = reflector.GetValue(rowData, "cod_ref");

                        if (referencias.ToString().Trim() == WinRef)
                        {
                            bandera = true;
                            posicion = a;                            
                        }

                        break;
                    }
                }
                a = a + 1;
            }
            return bandera;
        }

        


    }
}
