﻿using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;

namespace PvFacturaAnular
{
    public partial class ConsultaDocumentos : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        public string fechaini = "";
        public string fechafin = "";
        public string codbod = "";
        public string Documento = "";
        public string tipoTrn = "";
        public int idregcab = 0;
        public ConsultaDocumentos()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            pantalla();
        }
        public void pantalla()
        {
            this.MinHeight = 500;
            this.MaxHeight = 500;
            this.MinWidth = 850;
            this.MaxWidth = 850;
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
                this.Title = "Documentos:" + cod_empresa + "-" + nomempresa;                
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }
     
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
            try
            {                
                string cadena = "select cabeza.cod_trn,cabeza.num_trn,cabeza.fec_trn,cabeza.idreg,ter.nom_ter,sum(cantidad) as cantidad,sum(tot_tot) as tot_tot from InCab_doc as cabeza ";
                cadena = cadena + "inner join InCue_doc as cuerpo on cabeza.idreg = cuerpo.idregcab	";
                cadena = cadena + "inner join comae_ter as ter on cabeza.cod_cli = ter.cod_ter	";
                cadena = cadena + "where cuerpo.cod_bod='"+ codbod + "' and fec_trn between '"+fechaini+"' and '"+fechafin+" 23:59:59' and cabeza.cod_trn='" + tipoTrn+"' ";
                cadena = cadena + " group by cabeza.cod_trn,cabeza.num_trn,cabeza.fec_trn,cabeza.idreg,ter.nom_ter   order by cabeza.fec_trn desc";

                DataTable dt = SiaWin.Func.SqlDT(cadena, "Factura", idemp);                                
                DataGridDoc.ItemsSource = dt.DefaultView;
                Total.Text = dt.Rows.Count.ToString();
            }
            catch (Exception w)
            {
                MessageBox.Show("error en el Loaded:" + w);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridDoc.SelectedIndex>=0)
            {
                DataRowView row = (DataRowView)DataGridDoc.SelectedItems[0];
                Documento = row["num_trn"].ToString();
                tipoTrn = row["cod_trn"].ToString();
                idregcab = Convert.ToInt32(row["idreg"].ToString());
                this.Close();
            }            
        }

        
        private void DataGridDoc_PreviewKeyDown(object sender, KeyEventArgs e)
        {        
            if (e.Key == Key.F5)
            {
                if (DataGridDoc.SelectedIndex>=0) BTNcons.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));                
            }
        }





    }
}