using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using WindowPV;
namespace SiasoftAppExt
{
    public partial class WindowPV : Window
    {        
        //Sia.PublicarPnt(9460, "WindowPV");  
        //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9460, "WindowPV");
        //ww.idemp=010;
        //ww.ShowInTaskbar=false;
        //ww.Owner = Application.Current.MainWindow;
        //ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
        //ww.ShowDialog();        
        public int idemp = 0;
        public string codbod = "";

        public int idregcabReturn = -1;
        public string codtrn = string.Empty;
        public string numtrn = string.Empty;

        //remisiones
        public DataTable TablaRemision = new DataTable();
        public string tercero = "";
        public string bodegaRemision = "";


        //Consigancion
        public DataTable TablaConsignacion = new DataTable();
        public string terceroConsignacion = "";
        public string bodConsignacion = "";

        public int pantallaTipo = 0;

        public string tipo_trans = "";


        //remachados
        public DataTable dt_remachados = new DataTable();                
        public string terceroRemachado = "";
        public string ordenRema = "";

        public WindowPV()
        {            
            InitializeComponent();
            //SiaWin = Application.Current.MainWindow;
            //idemp = SiaWin._BusinessId;            
            pantalla();

            LoadConfig();

            BTNcontizaciion.Focus();
        }

        private void LoadConfig()
        {
            try                
            {                

                //this.Title = "Op:" + bodega;
            }
            catch (Exception e)
            {
                MessageBox.Show("aqui-" + e.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (tipo_trans == "004")
            {
                TXT_pedCot.Text = "Cotizaciones";
            }
        }

        public void pantalla()
        {
            this.MaxHeight = 400;
            this.MinHeight = 400;
            this.MinWidth = 400;
            this.MaxWidth = 400;
        }

        private void BTNcontizaciion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cotizaciones ventana = new Cotizaciones(idemp);
                ventana.tipoTransaccion = tipo_trans;

                if (tipo_trans == "004")
                {
                    ventana.ShowInTaskbar = false;
                    ventana.Owner = Application.Current.MainWindow;
                    var newItem = new ComboBoxItem();                    
                    var newItem2 = new ComboBoxItem();
                    newItem2.Tag = "011";
                    newItem2.Content = "Cotizado";
                    ventana.TextBxCB_consulta.Items.Add(newItem2);
                    ventana.bodega = codbod;
                }
                else
                {
                    ventana.ShowInTaskbar = false;
                    ventana.Owner = Application.Current.MainWindow;
                    var newItem = new ComboBoxItem();
                    newItem.Tag = "505";
                    newItem.Content = "Pedidos";
                    ventana.TextBxCB_consulta.Items.Add(newItem);
                    var newItem2 = new ComboBoxItem();
                    newItem2.Tag = "011";
                    newItem2.Content = "Cotizado";
                    ventana.TextBxCB_consulta.Items.Add(newItem2);
                    ventana.bodega = codbod;
                }
                

                ventana.ShowDialog();

                pantallaTipo = ventana.PntTip;

                if (pantallaTipo != 0)
                {
                    idregcabReturn = Convert.ToInt32(ventana.idregcabReturn.ToString());
                    codtrn = ventana.codtrn.ToString();
                    numtrn = ventana.numtrn.ToString();
                    this.Close();
                }
                else
                {
                    this.Close();
                }


            }
            catch (Exception w)
            {
                MessageBox.Show("error 55" + w);
            }
        }

        private void BtnRemisiones_Click(object sender, RoutedEventArgs e)
        {
            Remisiones ventana = new Remisiones();
            ventana.tipoTransaccion = tipo_trans;

            ventana.ShowInTaskbar = false;
            ventana.Owner = Application.Current.MainWindow;
            ventana.ShowDialog();
            TablaRemision = ventana.tablaTemporal;

            if (TablaRemision.Rows.Count == 0)
            {
                pantallaTipo = 0;
            }
            else
            {
                pantallaTipo = ventana.PntTip;
                tercero = ventana.nit_bodega;
                bodegaRemision = ventana.bodegaRemision;
            }

            this.Close();
        }

        private void BTNconsignaciones_Click(object sender, RoutedEventArgs e)
        {            
            Cosignaciones ventana = new Cosignaciones();
            ventana.cod_bodPV = codbod;
            ventana.ShowInTaskbar = false;
            ventana.Owner = Application.Current.MainWindow;
            ventana.ShowDialog();

            pantallaTipo = ventana.PntTip;
            TablaConsignacion = ventana.temporal;
            terceroConsignacion = ventana.tercero;
            bodConsignacion = ventana.bodegaConsignacion;
          
            this.Close();
        }
        

        private void BtnRemachados_Click(object sender, RoutedEventArgs e)
        {
            Remachados w = new Remachados();
            w.ShowInTaskbar = false;
            w.Owner = Application.Current.MainWindow;
            w.ShowDialog();

            dt_remachados = w.dt_rem;
            terceroRemachado = w.tercero;
            ordenRema = w.num_ord;

            pantallaTipo = 4;

            this.Close();
        }






    }
}
