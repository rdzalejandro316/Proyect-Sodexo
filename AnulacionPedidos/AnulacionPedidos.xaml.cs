using Syncfusion.UI.Xaml.Grid;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9505,"AnulacionPedidos");
    //dynamic w = ((Inicio)Application.Current.MainWindow).WindowExt(9505,"AnulacionPedidos");
    //w.ShowInTaskbar = false;
    //w.Owner = Application.Current.MainWindow;
    //w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
    //w.ShowDialog(); 

    public partial class AnulacionPedidos : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        public string codemp;
        string cnEmp = "";
        string cod_empresa = "";

        public AnulacionPedidos()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId; ;
            LoadConfig();
            cargarConceptos();
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
                this.Title = "Anulacion de Pedidos - Empresa:" + cod_empresa + "-" + nomempresa;
                pantalla();
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        public void pantalla()
        {
            this.MinHeight = 500;
            this.MaxHeight = 500;
            this.MinWidth = 1000;
            this.MaxWidth = 1000;
        }

        public void cargarConceptos()
        {
            try
            {
                string query = "select cod_anu,det_anu from InConaped";
                DataTable dt_cue = SiaWin.Func.SqlDT(query, "Buscar", idemp);
                comboBoxConcept.ItemsSource = dt_cue.DefaultView;
            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error al cargar los conceptos");
            }


        }

        private void TX_documento_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {

                if (e.Key == Key.Enter || e.Key == Key.F8)
                {
                    string tag = ((TextBox)sender).Tag.ToString();
                    string cmptabla = ""; string cmpcodigo = ""; string cmpnombre = ""; string cmporden = ""; string cmpidrow = ""; string cmptitulo = ""; string cmpconexion = ""; bool mostrartodo = true; string cmpwhere = "";
                    if (string.IsNullOrEmpty(tag)) return;

                    if (tag == "incab_doc")
                    {
                        cmptabla = "incab_doc"; cmpcodigo = "cod_trn"; cmpnombre = "num_trn"; cmporden = "idreg"; cmpidrow = "idreg"; cmptitulo = "Documentos"; cmpconexion = cnEmp; mostrartodo = false; cmpwhere = "cod_trn='505' ";
                    }
                    if (tag == "incab_docConsulta")
                    {
                        cmptabla = "incab_doc"; cmpcodigo = "cod_trn"; cmpnombre = "num_trn"; cmporden = "idreg"; cmpidrow = "idreg"; cmptitulo = "Documentos"; cmpconexion = cnEmp; mostrartodo = false; cmpwhere = "cod_trn='505' ";
                    }

                    int code = 0; string nom = "";
                    dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, cnEmp, mostrartodo, cmpwhere, idEmp: idemp);

                    winb.ShowInTaskbar = false;
                    winb.Owner = Application.Current.MainWindow;
                    winb.Width = 500;
                    winb.Height = 350;
                    winb.ShowDialog();
                    code = winb.IdRowReturn;
                    nom = winb.Nombre;
                    winb = null;
                    if (code > 0)
                    {
                        if (tag == "incab_doc")
                        {
                            TX_idreg.Text = code.ToString();
                            TX_documento.Text = nom.Trim();
                        }
                        if (tag == "incab_docConsulta")
                        {
                            TX_idregConsulta.Text = code.ToString();
                            TX_documentoConsulta.Text = nom.Trim();
                        }
                        var uiElement = e.OriginalSource as UIElement;
                        uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    }
                    e.Handled = true;

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
                MessageBox.Show("ERRO EN PREVI:" + ex);
            }

        }

        private void TX_documento_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {

                if (string.IsNullOrEmpty(((TextBox)sender).Text)) return;

                string idTag = ((TextBox)sender).Tag.ToString();
                string val = ((TextBox)sender).Text.ToString();

                string cadena = "select idreg,num_trn from InCab_doc where num_trn='" + val + "' and cod_trn='505' ";
                //MessageBox.Show("cadena:"+ cadena);
                //MessageBox.Show("idemp:"+ idemp);

                DataTable tabla = SiaWin.Func.SqlDT(cadena, "Buscar", idemp);

                if (tabla.Rows.Count > 0)
                {
                    if (idTag == "incab_doc")
                    {
                        ((TextBox)sender).Text = tabla.Rows[0]["num_trn"].ToString();
                        TX_idreg.Text = tabla.Rows[0]["idreg"].ToString();
                    }
                    else
                    {
                        ((TextBox)sender).Text = tabla.Rows[0]["num_trn"].ToString();
                        TX_idregConsulta.Text = tabla.Rows[0]["idreg"].ToString();
                    }
                }
                else
                {
                    MessageBox.Show("el Documento ingresado no existe");
                    ((TextBox)sender).Text = "";
                    if (idTag == "incab_doc")
                    {
                        TX_idreg.Text = "";
                    }
                    else
                    {
                        TX_idregConsulta.Text = ""; ;
                    }
                }


            }
            catch (Exception W)
            {
                SiaWin.Func.SiaExeptionGobal(W);
                MessageBox.Show("ERROR EN LA LOST:" + W);
            }
        }

        private void BTNconsultar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                clearControls();
                dataGridCxC.ItemsSource = null;
                if (string.IsNullOrEmpty(TX_documento.Text) || string.IsNullOrEmpty(TX_idreg.Text)) { MessageBox.Show("Ingrese un documento para consultar"); return; }

                string sql_cab = "select cabeza.idreg,cabeza.num_trn,cabeza.fec_trn,cabeza.cod_cli,tercero.nom_ter from InCab_doc as cabeza ";
                sql_cab = sql_cab + "inner join comae_ter as tercero on cabeza.cod_cli = tercero.cod_ter ";
                sql_cab = sql_cab + "where idreg='" + TX_idreg.Text + "' ";

                DataTable dt_cab = SiaWin.Func.SqlDT(sql_cab, "Buscar", idemp);
                if (dt_cab.Rows.Count > 0)
                {
                    TXT_documento.Text = dt_cab.Rows[0]["num_trn"].ToString().Trim();
                    TXT_fecha.Text = dt_cab.Rows[0]["fec_trn"].ToString().Trim();
                    TXT_codigo.Text = dt_cab.Rows[0]["cod_cli"].ToString().Trim();
                    TXT_nombre.Text = dt_cab.Rows[0]["nom_ter"].ToString().Trim();
                }


                string sql_cue = "select cuerpo.est_anu,cuerpo.fec_anu,cuerpo.cod_anu,concepto.det_anu,cuerpo.idreg,cuerpo.idregcab,cuerpo.cod_ref,referencia.nom_ref,cuerpo.cantidad,cuerpo.val_uni,cuerpo.subtotal,cuerpo.por_des,cuerpo.tot_tot,referencia.val_ref,cuerpo.val_iva,cuerpo.val_des from InCue_doc as cuerpo ";
                sql_cue += "inner join InMae_ref as referencia on cuerpo.cod_ref = referencia.cod_ref ";
                sql_cue += "inner join InCab_doc as cabeza on cuerpo.idregcab = cabeza.idreg ";
                sql_cue += "left join InConaped as concepto on cuerpo.cod_anu = concepto.cod_anu ";
                sql_cue += "where cuerpo.cod_trn='505' and  cuerpo.idregcab='" + TX_idreg.Text + "'  ";
                DataTable dt_cue = SiaWin.Func.SqlDT(sql_cue, "Buscar", idemp);
                if (dt_cue.Rows.Count > 0)
                {
                    dataGridCxC.ItemsSource = dt_cue.DefaultView;
                    TX_total.Text = dt_cue.Rows.Count.ToString();
                }

            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("Error nen la consulta ppp:" + w);
            }
        }

        public void clearControls()
        {
            TXT_ref.Text = "";
            Chek_anular.IsChecked = false;
            Feha_Anu.Text = "";
            comboBoxConcept.SelectedItem = -1;
            


            TXT_documento.Text = "";
            TXT_fecha.Text = "";
            TXT_codigo.Text = "";
            TXT_nombre.Text = "";
        }

        private void dataGridCxC_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            try
            {
                if (dataGridCxC.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)dataGridCxC.SelectedItems[0];
                    TXT_ref.Text = row["cod_ref"].ToString().Trim();
                    Chek_anular.IsChecked = row["est_anu"].ToString() == "A" ? true : false;
                    Feha_Anu.Text = row["fec_anu"].ToString();
                    
                    comboBoxConcept.SelectedItem = -1;
                    comboBoxConcept.SelectedValue = row["cod_anu"].ToString();
                                        
                }
            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error en la seleccion");
            }
        }

        private void Chek_anular_Checked(object sender, RoutedEventArgs e)
        {
            Handle(sender as CheckBox);
        }

        private void Chek_anular_Unchecked(object sender, RoutedEventArgs e)
        {
            Handle(sender as CheckBox);
        }

        void Handle(CheckBox checkBox)
        {
            try
            {

                DataRowView row = (DataRowView)dataGridCxC.SelectedItems[0];
                bool flag = checkBox.IsChecked.Value;
                string query = "";
                if (flag == true)
                {
                    query = "update InCue_doc set est_anu='A' where idreg='" + row["idreg"].ToString() + "' and cod_trn='505'";
                }
                else
                {
                    query = "update InCue_doc set est_anu='' where idreg='" + row["idreg"].ToString() + "' and cod_trn='505'";
                }

                if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                {
                    string text = flag == true ? "ANULO REF: "+ row["cod_ref"].ToString().Trim()+" DEL DOCUMENTO:"+ row["idregcab"].ToString().Trim() + " ": "DESMARCO ANULACION REF: " + row["cod_ref"].ToString().Trim() + " DEL DOCUMENTO:" + row["idregcab"].ToString().Trim() + " ";

                    SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, -9, -1, -9, text, "");
                    row["est_anu"] = flag == true ? "A" : "";                    
                }
                else
                {
                    MessageBox.Show("fallo la operacion");
                }

            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("seleccione una referencia para anular1");
            }
        }

        private void Feha_Anu_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                if (dataGridCxC.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)dataGridCxC.SelectedItems[0];
                    if (Feha_Anu.Text.Length <= 0) return;
                    
                    //DateTime fecha = row["fec_anu"] != null ? Convert.ToDateTime(row["fec_anu"].ToString()) : Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy"));
                    //if (Feha_Anu.Text == fecha.ToString("dd/MM/yyyy")) return;

                    string query = "update InCue_doc set fec_anu='" + Feha_Anu.Text + "' where idreg='" + row["idreg"].ToString() + "' and cod_trn='505' ";
                    
                    if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                    {
                        //MessageBox.Show("query:"+ query);
                        row["fec_anu"] = Feha_Anu.Text;
                    }
                    else
                    {
                        MessageBox.Show("fallo la operacion");
                    }
                }


            }
            catch (Exception w) { MessageBox.Show("seleccione una referencia para anular2:" + w); }

        }

        private void comboBoxConcept_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {

                if (dataGridCxC.SelectedIndex >= 0)
                {
                    if (comboBoxConcept.SelectedIndex >= 0)
                    {
                        DataRowView rowCombo = (DataRowView)comboBoxConcept.Items[comboBoxConcept.SelectedIndex];
                        DataRowView rowGrilla = (DataRowView)dataGridCxC.SelectedItems[0];

                        if (rowCombo["det_anu"].ToString().Trim() != rowGrilla["det_anu"].ToString().Trim())
                        {
                            string query = "update InCue_doc set cod_anu='" + comboBoxConcept.SelectedValue + "' where idreg='" + rowGrilla["idreg"].ToString() + "' and cod_trn='505' ";
                            if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                            {
                                rowGrilla["cod_anu"] = rowCombo["cod_anu"].ToString(); 
                                rowGrilla["det_anu"] = rowCombo["det_anu"].ToString();
                            }
                        }

                    }
                }


            }
            catch (Exception w) {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("seleccione una referencia para anular3:" + w);
            }

        }




        //****************** segundo tab *****************************************************
        private void BTNconsultarConsulta_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dataGridConsulta.ItemsSource = null;
                if (string.IsNullOrEmpty(TX_documentoConsulta.Text) || string.IsNullOrEmpty(TX_idregConsulta.Text)) { MessageBox.Show("Ingrese un documento para consultar"); return; }

                string sql_cab = "select cabeza.idreg,cabeza.num_trn,cabeza.fec_trn,cabeza.cod_cli,tercero.nom_ter from InCab_doc as cabeza ";
                sql_cab = sql_cab + "inner join comae_ter as tercero on cabeza.cod_cli = tercero.cod_ter ";
                sql_cab = sql_cab + "where idreg='" + TX_idregConsulta.Text + "' ";

                DataTable dt_cab = SiaWin.Func.SqlDT(sql_cab, "Buscar", idemp);
                if (dt_cab.Rows.Count > 0)
                {
                    TXT_documento.Text = dt_cab.Rows[0]["num_trn"].ToString().Trim();
                    TXT_fecha.Text = dt_cab.Rows[0]["fec_trn"].ToString().Trim();
                    TXT_codigo.Text = dt_cab.Rows[0]["cod_cli"].ToString().Trim();
                    TXT_nombre.Text = dt_cab.Rows[0]["nom_ter"].ToString().Trim();
                }


                string sql_cue = "select cuerpo.est_anu,CONVERT(VARCHAR(10), cuerpo.fec_anu, 103) as fec_anu,cuerpo.cod_anu,concepto.det_anu,cuerpo.idreg,cuerpo.idregcab,cuerpo.cod_ref,referencia.nom_ref,cuerpo.cantidad,cuerpo.val_uni,cuerpo.subtotal,cuerpo.por_des,cuerpo.tot_tot,referencia.val_ref,cuerpo.val_iva,cuerpo.val_des from InCue_doc as cuerpo ";
                sql_cue += "inner join InMae_ref as referencia on cuerpo.cod_ref = referencia.cod_ref ";
                sql_cue += "inner join InCab_doc as cabeza on cuerpo.idregcab = cabeza.idreg ";
                sql_cue += "left join InConaped as concepto on cuerpo.cod_anu = concepto.cod_anu ";
                sql_cue += "where cuerpo.cod_trn='505' and  cuerpo.idregcab='" + TX_idregConsulta.Text + "'  ";
                DataTable dt_cue = SiaWin.Func.SqlDT(sql_cue, "Buscar", idemp);
                if (dt_cue.Rows.Count > 0)
                {
                    dataGridConsulta.ItemsSource = dt_cue.DefaultView;
                    TotConsulta.Text = dt_cue.Rows.Count.ToString();
                }

            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("Error nen la consulta DE ANULADOS:" + w);
            }
        }



    }
}
