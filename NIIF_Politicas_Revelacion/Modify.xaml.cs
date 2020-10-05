using System;
using System.Collections.Generic;
using System.Data;
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

namespace Politicas_Revelacion
{

    public partial class Modify : Window
    {
        public string NameBtn = "";
        public string año = "";

        dynamic SiaWin;
        int idemp = 0;
        string cnEmp = "";

        public string idrow = "";
        public string Var_Nota = "";
        public string Var_Desc = "";
        public int Var_Esta = 0;
        public int Var_Tip = 0;

        public bool banderaUpdate = false;
        public Modify()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BtnOpcion.Tag = NameBtn;
            Tx_año.Text = año;

            TxNota.Text = Var_Nota;
            TxDesc.Text = Var_Desc;
            CB_Esta.SelectedIndex = Var_Esta;
            CB_tipo.SelectedIndex = Var_Tip;
        }

        private void BtnOpcion_Click(object sender, RoutedEventArgs e)
        {
            if (BtnOpcion.Tag.ToString() == "NEW")
            {
                if (validar() == false) { MessageBox.Show("LLene todos los campos"); return; }
                if (existenciaNota(TxNota.Text.Trim()) == true) { MessageBox.Show("El codigo de la nota ya existe ingrese otro"); return; }

                string query = "insert into NI_Notas(nota,descrip,fecha,ano,tipn,estado) values ('" + TxNota.Text + "','" + TxDesc.Text + "','" + DateTime.Now.ToString() + "','" + Tx_año.Text + "'," + CB_tipo.SelectedIndex + "," + CB_Esta.SelectedIndex + ")";                
                if (SiaWin.Func.SqlCRUD(query, idemp) == true) { banderaUpdate = true; this.Close(); }
            }
            if (BtnOpcion.Tag.ToString() == "UPDATE")
            {                
                if (validar() == false) { MessageBox.Show("LLene todos los campos"); return; }                
                string query = "update NI_Notas set nota='" + TxNota.Text + "',descrip='" + TxDesc.Text + "',tipn=" + CB_tipo.SelectedIndex + ",estado=" + CB_Esta.SelectedIndex + " where idrow='" + idrow + "' ";
                if (SiaWin.Func.SqlCRUD(query, idemp) == true) { banderaUpdate = true; this.Close(); }
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public bool validar()
        {
            bool bandera = true;
            if (string.IsNullOrEmpty(TxNota.Text) || TxNota.Text == "") bandera = false;
            if (string.IsNullOrEmpty(TxDesc.Text) || TxDesc.Text == "") bandera = false;
            if (CB_tipo.SelectedIndex < 0) bandera = false;
            if (CB_Esta.SelectedIndex < 0) bandera = false;
            return bandera;
        }

        public bool existenciaNota(string id)
        {
            bool flag = false;
            string query = "select * from  NI_Notas where nota='" + id + "' ";            
            DataTable dt = SiaWin.Func.SqlDT(query, "notas", idemp);            
            if (dt.Rows.Count > 0) flag = true;
            return flag;
        }






    }
}
