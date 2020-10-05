using HospedajePreCheckIn.Converter;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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

namespace HospedajePreCheckIn
{
    
    public partial class EstadoActual : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        
        public string vivienda = "";
        public string habitacion = "";
        public string cama = "";
        public string codEstado = "";
        public string fecha = "";

        public EstadoActual()
        {
            InitializeComponent();            
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
                this.Title = "Detalle " + cod_empresa + "-" + nomempresa;
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                SiaWin = Application.Current.MainWindow;
                idemp = SiaWin._BusinessId;
                LoadConfig();                

                string query = "select CtMae_Campamentos.CampamentoNombre,ctdoc_viviendas.ViviendaCodigo,ctdoc_viviendas.HabitacionCodigo,ctdoc_viviendas.CamaCodigo,ctdoc_viviendas.EstadoCamaCodigo,CtMae_ViviendaHabitacionCamaEstado.EstadoCamaNombre, ";
                query += "ctdoc_viviendas.FechaRegistroIngreso,ctdoc_viviendas.FechaRegistroSalida, ";
                query += "ctdoc_viviendas.EmpleadoCodigo,CtMae_Empleados.EmpleadoNombres,CtMae_Empleados.EmpleadoApellidos,CtMae_Empleados.Foto, ";
                query += "CtMae_Empleados.CargoId,CtMae_EmpleadosCargos.CargoNombre,CtMae_Empleados.Celular,ctdoc_viviendas.Nota,CtMae_ViviendaHabitacionCamaEstado.EstadoCamaNombre,CtMae_ViviendaHabitacionCamaEstado.color_esthab ";
                query += "from ctdoc_viviendas  ";
                query += "inner join CtMae_ViviendaHabitacionCamaEstado on ctdoc_viviendas.EstadoCamaCodigo = CtMae_ViviendaHabitacionCamaEstado.EstadoCamaCodigo ";
                query += "inner join CtMae_Empleados on CtMae_Empleados.EmpleadoCodigo = ctdoc_viviendas.EmpleadoCodigo ";
                query += "inner join CtMae_EmpleadosCargos on CtMae_EmpleadosCargos.CargoId = CtMae_Empleados.CargoId ";
                query += "inner join CtMae_Viviendas on  ctdoc_viviendas.ViviendaCodigo = CtMae_Viviendas.ViviendaCodigo ";
                query += "inner join CtMae_PuntosDeVenta on CtMae_PuntosDeVenta.PuntoVentaCodigo = CtMae_Viviendas.PuntoVentaCodigo ";
                query += "inner join CtMae_Campamentos on CtMae_PuntosDeVenta.CampamentoCodigo = CtMae_Campamentos.CampamentoCodigo ";
                query += "where ctdoc_viviendas.ViviendaCodigo='"+vivienda+"' and  ";
                query += "ctdoc_viviendas.HabitacionCodigo='"+habitacion+"' and ctdoc_viviendas.CamaCodigo='"+cama+ "' and CtMae_ViviendaHabitacionCamaEstado.Alias='" + codEstado + "' ";
                query += "and '" + fecha+ "' between ctdoc_viviendas.FechaIngreso and ctdoc_viviendas.FechaSalida ";

                DataTable dtcampos = SiaWin.Func.SqlDT(query, "campos", idemp);

                if (dtcampos.Rows.Count>0)
                {
                    Tx_cedula.Text = dtcampos.Rows[0]["EmpleadoCodigo"].ToString();
                    if (dtcampos.Rows[0]["Foto"] != DBNull.Value) cargarImagen((byte[])dtcampos.Rows[0]["Foto"]);
                    TxCampamento.Text = dtcampos.Rows[0]["CampamentoNombre"].ToString();
                    TxVivienda.Text = dtcampos.Rows[0]["ViviendaCodigo"].ToString();
                    TxHabitacion.Text = dtcampos.Rows[0]["HabitacionCodigo"].ToString();
                    TxCama.Text = dtcampos.Rows[0]["CamaCodigo"].ToString();
                    TxFecIni.Text = dtcampos.Rows[0]["FechaRegistroIngreso"].ToString();
                    TxFecSal.Text = dtcampos.Rows[0]["FechaRegistroSalida"].ToString();
                    TxNombre.Text = dtcampos.Rows[0]["EmpleadoNombres"].ToString();
                    TxApellido.Text = dtcampos.Rows[0]["EmpleadoApellidos"].ToString();
                    TxCargo.Text = dtcampos.Rows[0]["CargoNombre"].ToString();
                    TxCelular.Text = dtcampos.Rows[0]["Celular"].ToString();
                    tx_nota.Text = dtcampos.Rows[0]["Nota"].ToString();

                    TxEstado.Text = dtcampos.Rows[0]["EstadoCamaNombre"].ToString();

                    TxEstado.Foreground = Config.ToSolidColorBrush(dtcampos.Rows[0]["color_esthab"].ToString());
                    GridEstado.Background = Config.ToSolidColorBrush(dtcampos.Rows[0]["color_esthab"].ToString());
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar:");
            }
        }

        public void cargarImagen(byte[] im)
        {
            try
            {
                byte[] blob = im;
                MemoryStream stream = new MemoryStream();
                stream.Write(blob, 0, blob.Length);
                stream.Position = 0;
                System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
                System.Windows.Media.Imaging.BitmapImage bi = new System.Windows.Media.Imaging.BitmapImage();
                bi.BeginInit();
                MemoryStream ms = new MemoryStream();
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);
                bi.StreamSource = ms;
                bi.EndInit();
                image1.Source = bi;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar al imagen:" + w);
            }
        }




    }
}
