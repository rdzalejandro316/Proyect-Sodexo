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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9558,"RecibosProvisionalesAnulados");
    //dynamic WinDescto = ((Inicio)Application.Current.MainWindow).WindowExt(9558,"RecibosProvisionalesAnulados");
    //WinDescto.ShowInTaskbar = false;
    //WinDescto.Owner = Application.Current.MainWindow;
    //WinDescto.WindowStartupLocation = WindowStartupLocation.CenterScreen;
    //WinDescto.ShowDialog(); 

    public partial class RecibosProvisionalesAnulados : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public RecibosProvisionalesAnulados()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            LoadConfig();
            getVendedor();

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
                this.Title = "Recibos Provisionales Anulados " + cod_empresa + "-" + nomempresa;
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        public void getVendedor()
        {
            string select = "select * from inmae_mer;";
            DataTable dt =  SiaWin.Func.SqlDT(select, "vendedor", idemp);
            CB_vend.ItemsSource = dt.DefaultView;
            CB_vend.SelectedValuePath = "cod_mer";
            CB_vend.DisplayMemberPath = "nom_mer";
        }


        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (validateCAm())
            {
                MessageBox.Show("llene los campos de vendedor y recibo provisional que son obligatorios");
                return;
            }            
            if (getAnulado(CB_vend.SelectedValuePath, Tx_rec.Text))
            {
                MessageBox.Show("el recibo a anular ya se encuentra anulado","Existencia",MessageBoxButton.OK,MessageBoxImage.Stop);
                return;
            }
            if (insert())
            {
                MessageBox.Show("recibo:"+ Tx_rec.Text + " anulado exitosamente");
                clean();
            }            
        }

        bool insert()
        {
            bool flag = false;
            try
            {
                if (MessageBox.Show("Usted desea anular el recibo:"+ Tx_rec.Text.Trim(), "Anular Recibo", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    string query = "insert  into co_rprovanu (cod_ven,rc_prov,num_trn,obs) values ('"+ CB_vend.SelectedValue+ "','"+ Tx_rec.Text.Trim()+ "','"+ Tx_Trn.Text.Trim()+ "','"+ Tx_obs.Text.Trim()+ "');";
                    if (SiaWin.Func.SqlCRUD(query, idemp) == true) flag = true;                    
                }        
            }
            catch (Exception w){MessageBox.Show("error al insertar:"+w);}
            return flag;
        }


        void clean()
        {
            CB_vend.SelectedIndex = -1;
            Tx_rec.Text = "";
            Tx_Trn.Text = "";
            Tx_obs.Text = "";
        }

        public bool getAnulado(string ven,string rec)
        {
            DataTable dt = SiaWin.Func.SqlDT("select * from co_rprovanu where cod_ven='' and  rc_prov=''","tabla",idemp);            
            return dt.Rows.Count > 0 ? true : false;
        }
        bool validateCAm()
        {
            bool flag = false;
            if (CB_vend.SelectedIndex < 0) flag = true;
            if (string.IsNullOrEmpty(Tx_rec.Text)) flag = true;
            return flag;
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



    }
}
