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

namespace Ctamea_vivienda
{
    public partial class CtaConfig : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        public CtaConfig()
        {
            InitializeComponent();
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
                this.Title = "Configuracion de creacion automatica" + cod_empresa + "-" + nomempresa;

                string sql = "select inicialHab,aumentoHab,estadoHab,inicialCam,aumentoCam,estadoCam From CtMae_ViviendasConfig;";
                DataTable dt = SiaWin.Func.SqlDT(sql, "config", idemp);
                if (dt.Rows.Count > 0)
                {
                    string ini_hab = dt.Rows[0]["inicialHab"].ToString();
                    TxHab.Text = ini_hab;
                    CHinicialHab.IsChecked = string.IsNullOrWhiteSpace(ini_hab) ? false : true;
                    UDHab.Value = Convert.ToDouble(dt.Rows[0]["aumentoHab"]);
                    CHestadoHab.IsChecked = Convert.ToBoolean(dt.Rows[0]["estadoHab"]);


                    string ini_cam = dt.Rows[0]["inicialCam"].ToString();
                    TxCam.Text = ini_cam;
                    CHinicialCam.IsChecked = string.IsNullOrWhiteSpace(ini_cam) ? false : true;
                    UDCam.Value = Convert.ToDouble(dt.Rows[0]["aumentoCam"]);
                    CHestadoCam.IsChecked = Convert.ToBoolean(dt.Rows[0]["estadoCam"]);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "update CtMae_ViviendasConfig set inicialHab='"+ TxHab.Text.Trim() + "',aumentoHab="+Convert.ToInt32(UDHab.Value) + ",estadoHab="+Convert.ToInt32(CHestadoHab.IsChecked) +",inicialCam='"+ TxCam.Text.Trim() + "',aumentoCam="+ Convert.ToInt32(UDCam.Value) + ",estadoCam="+ Convert.ToInt32(CHestadoCam.IsChecked) + " ";
                if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                {
                    MessageBox.Show("actualizacion exitosa");
                    this.Close();
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al actualizar:" + w);
            }
        }

        private void CH_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).Tag.ToString().Trim() == "A")
            {
                TxHab.IsEnabled = true;                
            }
            if ((sender as CheckBox).Tag.ToString().Trim() == "B")
            {
                TxCam.IsEnabled = true;                
            }
        }

        private void CH_Unchecked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).Tag.ToString().Trim() == "A")
            {
                TxHab.IsEnabled = false;
                TxHab.Text = "";
            }
            if ((sender as CheckBox).Tag.ToString().Trim() == "B")
            {
                TxCam.IsEnabled = false;
                TxCam.Text = "";
            }
        }


    }
}
