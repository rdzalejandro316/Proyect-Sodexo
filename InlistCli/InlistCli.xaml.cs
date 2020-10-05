using System;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9519,"InlistCli");
    //dynamic WinDescto = ((Inicio)Application.Current.MainWindow).WindowExt(9519,"InlistCli");
    //WinDescto.ShowInTaskbar = false;
    //WinDescto.Owner = Application.Current.MainWindow;
    //WinDescto.WindowStartupLocation = WindowStartupLocation.CenterScreen;
    //WinDescto.ShowDialog(); 
    public partial class InlistCli : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";



        public InlistCli()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            idemp = SiaWin._BusinessId; ;
            LoadConfig();

            this.MinHeight = 500;
            this.MaxHeight = 500;
            this.MaxWidth = 1000;
            this.MinWidth = 1000;
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
                this.Title = "Bodegas en consignacion " + cod_empresa + "-" + nomempresa;

                TX_fec_ult.Text = DateTime.Now.ToString();
                TX_fec_venc.Text = DateTime.Now.ToString();
            }
            catch (Exception e)
            {
                SiaWin.Func.SiaExeptionGobal(e);
                MessageBox.Show("error en el load" + e.Message);
            }
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
                    winb.Height = 400;
                    winb.Width = 500;
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
                            TX_CodBod.Text = code;
                            TX_NomBod.Text = nom;

                            cargarList(code);
                        }
                        var uiElement = e.OriginalSource as UIElement;
                        uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    }
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                SiaWin.Func.SiaExeptionGobal(ex);
                MessageBox.Show(ex.Message.ToString());
            }
        }

        public void cargarList(string cod_bod)
        {
            try
            {
                string query = "select lista.Cod_bod,lista.Cod_ter,tercero.nom_ter,lista.Cod_ref,referencia.nom_ref,lista.val_ref,lista.Por_des,lista.Descto,lista.Valor,lista.Iva,lista.Val_uni, ";
                query += "lista.Fec_ult,lista.Fec_venc,lista.Cod_ant,tablaIva.por_iva ";
                query += "from InList_cli as lista ";
                query += "inner join Comae_ter as tercero on lista.Cod_ter=tercero.cod_ter ";
                query += "inner join InMae_ref as referencia on lista.Cod_ref=referencia.cod_ref ";
                query += "inner join InMae_tiva as tablaIva on referencia.cod_tiva=tablaIva.cod_tiva ";
                query += "where lista.Cod_bod='" + cod_bod + "' ";

                DataTable dt = SiaWin.Func.SqlDT(query, "Clientes", idemp);
                DataGridInlistCli.ItemsSource = dt.DefaultView;
            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error al cargar lista:" + w);
            }
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)DataGridInlistCli.SelectedItems[0];
                string tercero = row["Cod_ter"].ToString().Trim();
                string referencia = row["Cod_ref"].ToString().Trim();
                string bodega = row["Cod_bod"].ToString().Trim();

                string query = "delete InList_cli where Cod_ref='" + referencia + "' and Cod_ter='" + tercero + "' and Cod_bod='" + bodega + "' ";
                
                if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                {
                    MessageBox.Show("Eliminacion exitosa");
                }
                cargarList(TX_CodBod.Text.Trim());
            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error al eliminar:" + w);
            }
        }

        private void DataGridDesLine_CurrentCellEndEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)DataGridInlistCli.SelectedItems[0];
                decimal Val_ref = Convert.ToDecimal(row["Val_ref"]);
                decimal Por_des = Convert.ToDecimal(row["Por_des"]);
                decimal Descto = Convert.ToDecimal(row["Descto"]);
                string Valor = row["Valor"].ToString();
                decimal Val_uni = Convert.ToDecimal(row["Val_uni"]);

                string bodega = row["Cod_bod"].ToString().Trim();
                string tercero = row["Cod_ter"].ToString().Trim();
                string referencia = row["Cod_ref"].ToString().Trim();

                string query = "update InList_cli set Val_ref=" + Val_ref + ",Por_des=" + Por_des + ",Descto=" + Descto + ",Valor=" + Valor + ",Val_uni=" + Val_uni + " where Cod_bod='" + bodega + "' and Cod_ter='" + tercero + "' and Cod_ref='" + referencia + "' ";
                //string query = "update InTer_tip set Por_des=" + Por_des.ToString("F", CultureInfo.InvariantCulture) + ",des_mos=" + des_mos.ToString("F", CultureInfo.InvariantCulture) + ",des_ppag=" + des_ppag.ToString("F", CultureInfo.InvariantCulture) + " where Cod_ter='" + tercero.Trim() + "' and Cod_tip='" + linea + "' ";                

                if (SiaWin.Func.SqlCRUD(query, idemp) == false)
                {
                    MessageBox.Show("Fallo la actualizacion de la tabla");
                }
                else
                {
                    SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, -9, -1, -9, "ACTUALIZO AL CLIENTE:"+ tercero+" DE LA BODEGA:"+ bodega + " - INLIST_CLI ", "");
                }
                //SiaWin.Func.SqlCRUD(query, idemp);


            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error al editar" + w);
            }
        }

        public void bloquear(bool bloq)
        {
            if (bloq == true) GridMain.IsEnabled = bloq;
            if (bloq == false)
            {
                GridMain.IsEnabled = bloq;

                TX_tercero.Text = "";
                TX_referencia.Text = "";
                TX_referencia_ant.Text = "";
                TX_Nametercero.Text = "";
                TX_referenciaName.Text = "";
                TX_refCli.Text = "";
                TX_fec_ult.Text = DateTime.Now.ToString();
                TX_fec_venc.Text = DateTime.Now.ToString();
                TX_valRef.Value = 0;
                TX_Pordect.Value = 0;
                TX_Valor.Value = 0;
                TX_Iva.Value = 0;
                TX_ValUni.Value = 0;
            }
        }

        private void BTNadd_Click(object sender, RoutedEventArgs e)
        {
            if (TX_CodBod.Text == "")
            {
                MessageBox.Show("!selecione la bodega para poder adicionar¡");
                return;
            }
            bloquear(true);
        }

        private void BTNguardar_Click(object sender, RoutedEventArgs e)
        {
            if (TX_tercero.Text == "")
            {
                MessageBox.Show("llene el campo de tercero");
                return;
            }
                
            if (TX_referencia.Text=="" && TX_referencia_ant.Text=="")
            {
                MessageBox.Show("llene el campo de referencia");
                return;
            }

            if (TX_fec_ult.Text == "" || TX_fec_ult.Text.Length > 10 || TX_fec_venc.Text == "" || TX_fec_venc.Text.Length > 10 )
            {
                MessageBox.Show("llene los campo de fechas");
                return;
            }

            insert();
        }

        public void insert()
        {
            try
            {
                if (validarExistencia()==true)
                {
                    MessageBox.Show("La bodega,la referencia y el tercero ya estan registrados en la tabla");
                    return;
                }


                string query = "insert into InList_cli (Cod_bod,Cod_ter,Cod_ref,Ref_cli,Val_ref,Por_des,Descto,Valor,Iva,Val_uni,Fec_ult,Fec_venc,Cod_ant,fecha_aded) ";
                query += "values ('"+TX_CodBod.Text.Trim()+"','"+TX_tercero.Text.Trim() + "','"+TX_referencia.Text.Trim() + "','"+TX_refCli.Text.Trim() + "',"+ TX_valRef.Value + ","+ TX_Pordect.Value+","+TX_Descto.Value+","+TX_Valor.Value+","+ TX_Iva.Value+ ","+TX_ValUni.Value+",'"+ TX_fec_ult.Text + "','"+ TX_fec_venc.Text + "','"+ TX_referencia_ant.Text.Trim() + "','" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "') ";             
                if (SiaWin.Func.SqlCRUD(query, idemp) == false)
                {
                    MessageBox.Show("Fallo el insertar los datos");
                }
                else
                {
                    SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, -9, -1, -9, "INSERTO CLIENTE A LA LISTA DE LA BODEGA:"+ TX_CodBod.Text, "");
                    cargarList(TX_CodBod.Text.ToString().Trim());
                    bloquear(false);
                }
            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error al insert:"+w);
            }

        }

        public bool validarExistencia() {

            string query = "select * from InList_cli where Cod_ref='" + TX_referencia.Text + "' and Cod_ter='" + TX_tercero.Text + "' and Cod_bod='" + TX_CodBod.Text + "' ";
            DataTable dt = SiaWin.Func.SqlDT(query, "Clientes", idemp);
            if (dt.Rows.Count>0)            
                return true;            
            else            
                return false;            
        }


        private void BTNcancelar_Click(object sender, RoutedEventArgs e)
        {
            bloquear(false);
        }

        private void TX_LostFocus(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(((TextBox)sender).Text)) return;

            string Tag = ((TextBox)sender).Tag.ToString();
            string codigo = "";
            string nombre = "";            
            //TX_referencia_ant
            TextBox campoNombre = new TextBox();

            switch (Tag)
            {
                case "comae_ter":
                    codigo = "cod_ter"; nombre = "nom_ter"; campoNombre = (TextBox)this.FindName("TX_Nametercero");
                    break;
                case "inmae_ref":
                    codigo = "cod_ref"; nombre = "nom_ref"; campoNombre = (TextBox)this.FindName("TX_referenciaName"); 
                    break;
            }

            string cadena = "select * from " + Tag + "  where  " + codigo + "='" + ((TextBox)sender).Text.ToString() + "'  ";

            DataTable tabla = SiaWin.Func.SqlDT(cadena, "Buscar", idemp);
            if (tabla.Rows.Count > 0)
            {
                if (Tag == "inmae_ref") TX_referencia_ant.Text = tabla.Rows[0]["cod_ant"].ToString();   
                ((TextBox)sender).Text = tabla.Rows[0][codigo].ToString();
                campoNombre.Text = tabla.Rows[0][nombre].ToString();
            }
            else
            {
                MessageBox.Show("el codigo ingresado no existe");
                ((TextBox)sender).Text = "";
                campoNombre.Text = "";
                if (Tag == "inmae_ref") TX_referencia_ant.Text = "";
            }



        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            try
            {          
                string idTab = ((TextBox)sender).Tag.ToString();

                if (e.Key == Key.Enter || e.Key == Key.F8)
                {
                    if (idTab.Length > 0)
                    {
                        string tag = ((TextBox)sender).Tag.ToString();
                        string cmptabla = ""; string cmpcodigo = ""; string cmpnombre = ""; string cmporden = ""; string cmpidrow = ""; string cmptitulo = ""; string cmpconexion = ""; bool mostrartodo = true; string cmpwhere = "";
                        if (string.IsNullOrEmpty(tag)) return;
                        
                        if (tag == "comae_ter")
                        {
                            cmptabla = tag; cmpcodigo = "cod_ter"; cmpnombre = "nom_ter"; cmporden = "cod_ter"; cmpidrow = "cod_ter"; cmptitulo = "Maestra de Terceros"; cmpconexion = cnEmp; mostrartodo = false; cmpwhere = "";
                        }
                        if (tag == "inmae_ref")
                        {
                            cmptabla = tag; cmpcodigo = "cod_ref"; cmpnombre = "nom_ref"; cmporden = "cod_ref"; cmpidrow = "cod_ref"; cmptitulo = "Maestra de Referencias"; cmpconexion = cnEmp; mostrartodo = false; cmpwhere = "";
                        }

                        int idr = 0; string code = ""; string nom = "";
                        dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, cnEmp, mostrartodo, cmpwhere, idEmp: idemp);

                        winb.ShowInTaskbar = false;
                        winb.Owner = Application.Current.MainWindow;
                        winb.Height = 400;
                        winb.Width = 500;
                        winb.ShowDialog();
                        idr = winb.IdRowReturn;
                        code = winb.Codigo;
                        nom = winb.Nombre;
                        winb = null;
                        if (idr > 0)
                        {                            
                            if (tag == "comae_ter")
                            {
                                TX_tercero.Text = code.Trim();
                                TX_Nametercero.Text = nom;
                            }
                            if (tag == "inmae_ref")
                            {
                                TX_referencia.Text = code.Trim();
                                TX_referenciaName.Text = nom;
                                cod_anteriot(code.Trim());
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

        public void cod_anteriot(string codigo)
        {
            string query = "select * from inmae_ref where cod_ref='"+codigo.Trim()+"' ";
            DataTable tabla = SiaWin.Func.SqlDT(query, "Buscar", idemp);
            if (tabla.Rows.Count > 0) TX_referencia_ant.Text = tabla.Rows[0]["cod_ant"].ToString();
        }




    }
}
