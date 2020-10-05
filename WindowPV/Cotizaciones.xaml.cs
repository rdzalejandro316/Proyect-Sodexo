using Syncfusion.UI.Xaml.Grid;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WindowPV
{

    //Sia.PublicarPnt(9460, "WindowPV");  
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9460, "WindowPV");
    //ww.idemp=010;
    //ww.ShowInTaskbar=false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
    //ww.ShowDialog();
    public partial class Cotizaciones : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        public int idregcabReturn = -1;
        public string codtrn = string.Empty;
        public string numtrn = string.Empty;
        public string bodega = "";

        public DataTable tabla;

        DataTable tablaCuerpo = new DataTable();

        public Boolean bandera = false;

        public Boolean actualizaDoc = false;

        public int PntTip = 0;

        public Boolean addRow = false;

        public string tipoTransaccion = "";

        public string campoDescTip = "";
        public string campoDescLin = "";

        public Cotizaciones(int idEmpresa)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = idEmpresa;
            idemp = idEmpresa;
            LoadConfig();
            pantalla();

            TextBxCB_consulta.Focus();
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
                this.Title = "Pedidos y Cotizaciones - Empresa:" + cod_empresa + "-" + nomempresa;
            }
            catch (Exception e)
            {
                MessageBox.Show("aqui-" + e.Message);
            }
        }

        public void pantalla()
        {
            this.MinHeight = 650;
            this.MaxHeight = 650;
            this.MinWidth = 1200;
            this.MaxWidth = 1200;            
        }

        public void vaciarCamposPie()
        {
            pie_Codigo.Text = "";
            pie_PrecLi.Text = "";
            pie_iva.Text = "";
            pie_valconiva.Text = "";
            pie_decu.Text = "";
            pie_Subt.Text = "";
            pie_total.Text = "";
            Nota.Text = "";
        }


        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (e.Key == System.Windows.Input.Key.F5)
                {
                    if (dataGridCabeza.SelectedIndex >= 0 )
                    {
                        BTNfacturar.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    }

                }
            }
            catch (Exception w)
            {
                MessageBox.Show("@888"+w);
            }            
        }


        public void consultaCabeza()
        {
            try
            {

                //TextBxCB_consulta.SelectedIndex.ToString();

                string where = "and cabeza.bod_tra='" + bodega + "' ";

                var tag = ((ComboBoxItem)TextBxCB_consulta.SelectedItem).Tag.ToString();
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter();
                cmd = new SqlCommand("_EmpPvConsultaFactura", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@cod_trn", tag.ToString());
                cmd.Parameters.AddWithValue("@_codemp", cod_empresa);
                cmd.Parameters.AddWithValue("@where", where);
                cmd.Parameters.AddWithValue("@fechaIni", DateTime.Today.AddMonths(-1).ToString("dd/MM/yyyy"));
                cmd.Parameters.AddWithValue("@fechaFin", DateTime.Now.ToString("dd/MM/yyyy"));
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                con.Close();
                tabla = ds.Tables[0];
                dataGridCabeza.ItemsSource = tabla.DefaultView;

                dataGridCabeza.SelectionChanged += dataGridCabeza_SelectionChanged;
                dataGridCuerpo.SelectionChanged += dataGridCuerpo_SelectionChanged;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar el procedimiento almacenado" + w);
            }
        }

        private void dataGridCabeza_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            try
            {
                cabeza();
                DataRowView row = (DataRowView)dataGridCabeza.SelectedItems[0];                
                Nota.Text = row["des_mov"].ToString();
                factura(row["num_trn"].ToString());
            }
            catch (Exception w)
            {                
                MessageBox.Show("change:" + w);
            }

        }

        public void factura(string pedido)
        {
            string query = "select idregcab,num_trn from InCue_doc where doc_cruc='" + pedido+"' and cod_trn='005' ";
            DataTable DTcompra = SiaWin.Func.SqlDT(query, "Compra", idemp);
            if (DTcompra.Rows.Count>0)
            {
                DocumentoCompra.Text = DTcompra.Rows[0]["num_trn"].ToString();
                idregCompra.Text = DTcompra.Rows[0]["idregcab"].ToString();
                BTNdetalle.Visibility = Visibility.Visible;
            }
            else
            {
                DocumentoCompra.Text = "Ninguno";
                BTNdetalle.Visibility = Visibility.Hidden;
            }

        }


        public void cabeza()
        {
            try
            {

                //dataGridCuerpo.SelectedItems.Clear();
                //dataGridCuerpo.ItemsSource = 0;
                var tag = ((ComboBoxItem)TextBxCB_consulta.SelectedItem).Tag.ToString();
                DataRowView row = (DataRowView)dataGridCabeza.SelectedItems[0];
                string idreg = row["idreg"].ToString();
                string num_trn = row["num_trn"].ToString();


                //string cadena = "select cuerpo.idreg,cuerpo.idregcab,cuerpo.cod_ref,referencia.nom_ref,cuerpo.cantidad,cuerpo.val_uni,cuerpo.subtotal,cuerpo.por_des,cuerpo.tot_tot,referencia.val_ref,cuerpo.val_iva,cuerpo.val_des from InCue_doc as cuerpo ";
                //cadena += "inner join InMae_ref as referencia on cuerpo.cod_ref = referencia.cod_ref ";
                //cadena += "inner join InCab_doc as cabeza on cuerpo.idregcab = cabeza.idreg ";
                ////cadena += "where cuerpo.cod_trn='" + tag.ToString() + "' and (" + where + ") ";
                //cadena += "where cuerpo.cod_trn='" + tag.ToString() + "' and  cuerpo.idregcab='" + idreg + "'  ";

                //dataGridCuerpo.ItemsSource = null;
                //tablaCuerpo = SiaWin.Func.SqlDT(cadena, "Clientes", idemp);               
                //dataGridCuerpo.ItemsSource = tablaCuerpo.DefaultView;


                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpPvCotizacion", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@cod_trn", tag);
                cmd.Parameters.AddWithValue("@idreg", idreg);
                cmd.Parameters.AddWithValue("@num_trn", num_trn);
                cmd.Parameters.AddWithValue("@_codemp", cod_empresa);
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                con.Close();
                dataGridCuerpo.ItemsSource = ds.Tables[0].DefaultView;

            }
            catch (Exception w)
            {
                MessageBox.Show("Seleciona una celda de la grilla:" + w);
            }
        }

        private void BTNfacturar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DocumentoCompra.Text != "Ninguno")
                {
                    if (MessageBox.Show("Este documento ya tiene algunos items facturados desea facturarlo..?", "Guardar Traslado", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) {

                        return;
                    }
                }

                DataRowView row = (DataRowView)dataGridCabeza.SelectedItems[0];
                if (row != null)
                {
                    idregcabReturn = Convert.ToInt32(row["idreg"].ToString());
                    codtrn = row["cod_trn"].ToString();
                    numtrn = row["num_trn"].ToString();
                }
                PntTip = 1;
                this.Close();
            }
            catch (Exception)
            {

                MessageBox.Show("Seleccione un Documento para Realizar la Facturacion");
            }
        }

        private void dataGridCuerpo_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)dataGridCuerpo.SelectedItems[0];
                pie_Codigo.Text = row["cod_ref"].ToString().Trim();

                decimal val_ref = Convert.ToDecimal(row["val_ref"]);
                pie_PrecLi.Text = val_ref.ToString("C");

                decimal val_iva = Convert.ToDecimal(row["val_iva"]);
                pie_iva.Text = val_iva.ToString("C");

                decimal total = Convert.ToDecimal(row["tot_tot"].ToString().Trim());
                decimal cantidad = Convert.ToDecimal(row["cantidad"].ToString().Trim());
                decimal valConIva = total / cantidad;

                pie_valconiva.Text = valConIva.ToString("C");

                decimal val_des = Convert.ToDecimal(row["val_des"].ToString().Trim());
                pie_decu.Text = val_des.ToString("C");

                decimal subtotal = Convert.ToDecimal(row["subtotal"].ToString().Trim());
                pie_Subt.Text = subtotal.ToString("C");
                pie_total.Text = total.ToString("C");
            }
            catch (Exception w)
            {
                vaciarCamposPie();
            }

        }

        public void pie()
        {
            DataRowView row = (DataRowView)dataGridCuerpo.SelectedItems[0];
            pie_Codigo.Text = row["cod_ref"].ToString().Trim();

            decimal val_ref = Convert.ToDecimal(row["val_ref"]);
            pie_PrecLi.Text = val_ref.ToString("C");

            decimal val_iva = Convert.ToDecimal(row["val_iva"]);
            pie_iva.Text = val_iva.ToString("C");

            decimal total = Convert.ToDecimal(row["tot_tot"].ToString().Trim());
            decimal cantidad = Convert.ToDecimal(row["cantidad"].ToString().Trim());
            decimal valConIva = total / cantidad;

            pie_valconiva.Text = valConIva.ToString("C");

            decimal val_des = Convert.ToDecimal(row["val_des"].ToString().Trim());
            pie_decu.Text = val_des.ToString("C");

            decimal subtotal = Convert.ToDecimal(row["subtotal"].ToString().Trim());
            pie_Subt.Text = subtotal.ToString("C");
            pie_total.Text = total.ToString("C");

        }

        private void dataGridCuerpo_CurrentCellValueChanged(object sender, CurrentCellValueChangedEventArgs e)
        {
            actualizaDoc = true;
        }

        public decimal cargarPorIva(string referencia)
        {
            decimal iva = 0;
            string cadena = "select REF.cod_tip,REF.val_ref,REF.cod_tiva,TIV.por_iva from inmae_ref REF ";
            cadena = cadena + "inner join InMae_tiva TIV on TIV.cod_tiva = ref.cod_tiva ";
            cadena = cadena + "where cod_ref='" + referencia + "'  ";

            DataTable dt = SiaWin.Func.SqlDT(cadena, "Clientes", idemp);

            if (dt.Rows.Count > 0)
            {
                iva = Convert.ToDecimal(dt.Rows[0]["por_iva"]);
            }

            return iva;
        }

        private void dataGridCuerpo_CurrentCellEndEdit(object sender, CurrentCellEndEditEventArgs e)
        {
            try
            {
                if (actualizaDoc == true)
                {
                    DataRowView row = (DataRowView)dataGridCuerpo.SelectedItems[0];
                    string idreg_cue = row["idreg"].ToString();
                    decimal val_uni = Convert.ToDecimal(row["val_uni"]);
                    string cod_ref = row["cod_ref"].ToString();

                    decimal cantidad = Convert.ToDecimal(row["cantidad"]);
                    decimal subtotal = val_uni * cantidad;//subtotal
                    decimal iva = cargarPorIva(cod_ref);
                    decimal valIva = (subtotal * iva) / 100;//val_iv
                    decimal total = subtotal + valIva;//tot_tot


                    string cadena = "update InCue_doc set cantidad=" + cantidad.ToString("F", CultureInfo.InvariantCulture) + ",subtotal=" + subtotal.ToString("F", CultureInfo.InvariantCulture) + ",val_iva=" + valIva.ToString("F", CultureInfo.InvariantCulture) + ",tot_tot=" + total.ToString("F", CultureInfo.InvariantCulture) + "  where idreg='" + idreg_cue + "' ";
                    //MessageBox.Show(cadena);
                    actualizaDoc = false;
                    SiaWin.Func.SqlDT(cadena, "PPP", idemp);

                    row["subtotal"] = subtotal;
                    row["val_iva"] = valIva;
                    row["tot_tot"] = total;

                    pie();
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("Error @" + w);
            }


        }
        
        private void BTNaddRef_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)dataGridCabeza.SelectedItems[0];
                var tag = ((ComboBoxItem)TextBxCB_consulta.SelectedItem).Tag.ToString();

                AddReferencia ventana = new AddReferencia();
                ventana.idregcab = Convert.ToInt32(row["idreg"]);
                ventana.numeroDoc = row["num_trn"].ToString();
                ventana.trn = tag;
                ventana.codigo_ter = row["cod_cli"].ToString();
                ventana.bodega = bodega;
                ventana.campoDesInTer_tip = campoDescTip;
                ventana.campoDeslinea = campoDescLin;
                ventana.ShowInTaskbar=false;
                ventana.Owner = Application.Current.MainWindow;
                ventana.WindowStartupLocation=WindowStartupLocation.CenterScreen;
                ventana.ShowDialog();

                if (ventana.bandera == true)
                {
                    cabeza();
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("seleciona un documento:" + w);
            }
        }

        private void TextBxCB_consulta_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            dataGridCabeza.SelectionChanged -= dataGridCabeza_SelectionChanged;
            dataGridCuerpo.SelectionChanged -= dataGridCuerpo_SelectionChanged;

            dataGridCabeza.SelectedItems.Clear();
            dataGridCuerpo.SelectedItems.Clear();
            dataGridCuerpo.ItemsSource = 0;
            vaciarCamposPie();

            try
            {
                consultaCabeza();
            }
            catch (Exception w)
            {
                MessageBox.Show("dropdown " + w);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (tipoTransaccion == "004")
            {
                campoDescTip = "des_mos";
                campoDescLin = "Por_des";
            }
            else
            {
                campoDescTip = "por_des";
                campoDescLin = "por_desc";
            }
        }



        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {

            DataRowView GridCab = (DataRowView)dataGridCabeza.SelectedItems[0];
            string documento = GridCab["num_trn"].ToString();

            DataRowView GridCue = (DataRowView)dataGridCuerpo.SelectedItems[0];
            string referencia = GridCue["cod_ref"].ToString();
            string idreg = GridCue["idreg"].ToString();

            MessageBoxResult result = MessageBox.Show("USTED DESEA ELIMINAR LA REFERENCIA " + referencia.Trim() + " DEL DOCUMENTO:" + documento.Trim() + " ", "Siasoft?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var trans = ((ComboBoxItem)TextBxCB_consulta.SelectedItem).Tag.ToString();
                string cadena = "delete InCue_doc where idreg='" + idreg + "'  ";
                SiaWin.Func.SqlCRUD(cadena, idemp);
                MessageBox.Show("referencia eliminada");
                cabeza();
            }

        }

        private void BTNsalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BTNdetalle_Click(object sender, RoutedEventArgs e)
        {
            DetalleCompra ventana = new DetalleCompra();
            ventana.idreg = idregCompra.Text;
            ventana.num_trn = DocumentoCompra.Text;
            ventana.ShowInTaskbar = false;
            ventana.Owner = Application.Current.MainWindow;
            ventana.ShowDialog();
        }

        
    }
}

