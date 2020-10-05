using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

namespace WindowPV
{

    public partial class Cosignaciones : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public string cod_bodPV = "";

        public DataTable temporal = new DataTable();
        public string tercero = "";
        public string bodegaConsignacion = "";

        public int idregcabReturn = -1;
        public string codtrn = string.Empty;
        public string numtrn = string.Empty;

        public int PntTip = 0;

        public Cosignaciones()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId; ;
            LoadConfig();
            pantalla();        
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //BodCod.IsFocused = true;
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
                this.Title = "Consignaciones - Empresa:" + cod_empresa + "-" + nomempresa;

                BodCod.Focus();
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        public void pantalla()
        {
            this.MinHeight = 550;
            this.MaxHeight = 550;
            this.MinWidth = 1000;
            this.MaxWidth = 1000;
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

        private void Salir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
      
        private void BTNconsultar_Click(object sender, RoutedEventArgs e)
        {
            cargarConsulta();
        }

        public void cargarConsulta() {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpPvConsultaFactura", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@cod_trn", "145");
                cmd.Parameters.AddWithValue("@_codemp", cod_empresa);
                cmd.Parameters.AddWithValue("@where", " and bod_tra='"+BodCod.Text+ "' and cuerpo.cod_bod='"+cod_bodPV+"' ");
                
                cmd.Parameters.AddWithValue("@fechaIni", DateTime.Today.AddMonths(-1).ToString("dd/MM/yyyy"));
                cmd.Parameters.AddWithValue("@fechaFin", DateTime.Now.ToString("dd/MM/yyyy"));
                //cmd.Parameters.AddWithValue("@fechaIni", "01/06/2018");
                //cmd.Parameters.AddWithValue("@fechaFin", "01/07/2018");
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                con.Close();
                dataGridCabeza.ItemsSource = ds.Tables[0];
                Total.Text = ds.Tables[0].Rows.Count.ToString();
            }
            catch (Exception w)
            {
                MessageBox.Show("erro en la consulta:"+w);
            }
        }

        private void dataGridCabeza_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            try
            {                
                
                DataRowView row = (DataRowView)dataGridCabeza.SelectedItems[0];
                string idreg = row["idreg"].ToString();
                string num_trn = row["num_trn"].ToString();
                string cod_emp = cod_empresa;


                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpPvCuerpoConsignacion", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idreg", idreg);
                cmd.Parameters.AddWithValue("@num_trn", num_trn);
                cmd.Parameters.AddWithValue("@_codemp", cod_empresa);
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                con.Close();


                temporal = ds.Tables[0];
                dataGridCuerpo.ItemsSource = ds.Tables[0].DefaultView;
                TotalRef.Text = ds.Tables[0].Rows.Count.ToString();

                //string cadena = "select cuerpo.cod_ref,referencia.nom_ref,cuerpo.cantidad,cuerpo.val_uni,cuerpo.subtotal,cuerpo.por_des,cuerpo.tot_tot,referencia.val_ref,cuerpo.val_iva,cuerpo.val_des  from InCue_doc as cuerpo ";
                //cadena += "inner join InMae_ref as referencia on cuerpo.cod_ref = referencia.cod_ref ";
                //cadena += "inner join InCab_doc as cabeza on cuerpo.idregcab = cabeza.idreg ";                
                //cadena += "where cuerpo.cod_trn='145' and cuerpo.idregcab='"+idreg+"' ";

                //temporal = SiaWin.Func.SqlDT(cadena, "Clientes", idemp);
                //dataGridCuerpo.ItemsSource = temporal.DefaultView;
                //TotalRef.Text = temporal.Rows.Count.ToString();
            }
            catch (Exception w)
            {
                MessageBox.Show("error al carcar el detalle del cuerpo:" + w);
            }
        }

        private void BTNfacturar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //DataRowView row = (DataRowView)dataGridCabeza.SelectedItems[0];
                //    idregcabReturn = Convert.ToInt32(row["idreg"].ToString());
                //    codtrn = row["cod_trn"].ToString();
                //    numtrn = row["num_trn"].ToString();
                //}

                if (temporal.Rows.Count>0)
                {
                    DataRowView row = (DataRowView)dataGridCabeza.SelectedItems[0];
                    tercero = row["cod_cli"].ToString();
                    bodegaConsignacion = BodCod.Text.Trim();
                    PntTip = 3;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("La consignacion seleccionada no contiene nada");
                }                
            }
            catch (Exception)
            {

                MessageBox.Show("Seleccione un Documento");
            }

        }

        





    }
}

