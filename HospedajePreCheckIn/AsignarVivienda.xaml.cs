using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading;
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
    /// <summary>
    /// Lógica de interacción para AsignarVivienda.xaml
    /// </summary>
    public partial class AsignarVivienda : Window
    {

        public string idrowPrecke = "";
        public string cedula_asi = "";
        public string nombre_emp = "";
        public string fec_ing;
        public string fec_sal;

        public bool actualizo = false;

        dynamic SiaWin;
        public int idemp = 0;
        string dbAcceso = string.Empty;

        public AsignarVivienda()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            dbAcceso = SiaWin.Func.DB_Acceso;
        }


        private void ConsultaEmpleadoPreCheckIn(string cedula)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT NitEmpresaExterna, DocumentoConsumidor, PrimerNombre, SegundoNombre, PrimerApellido, SegundoApellido, CorreoElectronico, FechaIngreso, FechaFinalEstadia, CodigoSucursal, Categoria, Subcategoria, clientes.id_cliente,precheckin.fecha_aded,idrow,precheckin.Estado,cod_vivienda,id_habitacion,cancelacion,message_cancelacion,");
            sb.Append(" Empleados.id_cliente,clientes.nom_cliente,clientes.ID_CCO,Cent_Costos.nom_cco,Cent_Costos.id_categoria,Categorias.nom_categoria ");
            sb.Append(" clientes.nom_cliente,");
            sb.Append(" FROM PreCheckIn ");
            sb.Append(" inner join Empleados on PreCheckIn.DocumentoConsumidor = Empleados.Cedula ");
            sb.Append(" inner join Clientes on Clientes.ID_CLIENTE = Empleados.id_cliente ");
            sb.Append(" inner join Cent_Costos on Cent_Costos.id_cco = Clientes.ID_CCO ");
            sb.Append(" inner join Categorias on Categorias.id_categoria = Cent_Costos.id_categoria ");
            sb.Append("");
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {

                TX_fecini.Text = fec_ing.ToString();
                TX_fecsali.Text = fec_sal.ToString();
                Tx_empleado.Text = nombre_emp.ToString();
                Tx_empleado.Tag = idrowPrecke;


                SiaWin = Application.Current.MainWindow;
                idemp = SiaWin._BusinessId;
                dbAcceso = SiaWin.Func.DB_Acceso;

                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;


                sfBusyIndicator.IsBusy = true;
                dataGridCxC.ClearFilters();
                dataGridCxC.ItemsSource = null;
                string cedula = cedula_asi.Trim();

                source.CancelAfter(TimeSpan.FromSeconds(1));

                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(cedula, source.Token), source.Token);
                await slowTask;


                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    dataGridCxC.ItemsSource = ((DataSet)slowTask.Result).Tables[0].DefaultView;
                    Cbx_campa.ItemsSource = ((DataSet)slowTask.Result).Tables[1].DefaultView;

                    if (((DataSet)slowTask.Result).Tables[1].Rows.Count > 0 && ((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                    {
                        Cbx_campa.SelectedIndex = Convert.ToInt32(((DataSet)slowTask.Result).Tables[0].Rows[0]["id_pventa"]) - 1;

                        string query = "select Cod_Vivienda as cod_vivienda,Nom_Vivienda as nom_vivienda from Viviendas where id_pventa='" + ((DataSet)slowTask.Result).Tables[0].Rows[0]["id_pventa"].ToString() + "' ";
                        DataTable dtvivienda = SiaWin.Func.SqlDT(query, "vivienda", 0, cnOtros: dbAcceso);
                        Cbx_vivienda.ItemsSource = dtvivienda.DefaultView;
                    }

                    //Cbx_vivienda.ItemsSource = ((DataSet)slowTask.Result).Tables[2].DefaultView;
                    //Cbx_habitacion.ItemsSource = ((DataSet)slowTask.Result).Tables[3].DefaultView;                    
                }
                else
                    this.sfBusyIndicator.IsBusy = false;


                this.sfBusyIndicator.IsBusy = false;

            }
            catch (Exception ex)
            {
                MessageBox.Show("error:" + ex.Message);
                this.Opacity = 1;
            }
        }

        private DataSet LoadData(string cedula, CancellationToken cancellationToken)
        {
            try
            {
                DataSet ds = new DataSet();
                string query = "select top 5 reservas.IdRow,PuntoVentas.Id_pventa as id_pventa,PuntoVentas.Nom_pventa as nom_pventa,reservas.cod_Vivienda as cod_vivienda,reservas.id_habitacion as id_habitacion,isnull(reservas.id_cama,0) as cama,reservas.id_esthab, ";
                query += "reservas.Fecha_Ingreso as fecha_ingreso,reservas.Fecha_Salida as fecha_salida,HabitacionesEstado.alias,HabitacionesEstado.nom_esthab,empleados.id_cliente ";
                query += "from reservas  ";
                query += "inner join Viviendas on Viviendas.Cod_Vivienda=reservas.cod_Vivienda ";
                query += "inner join PuntoVentas on PuntoVentas.Id_pventa=Viviendas.id_pventa ";
                query += "inner join Empleados on Empleados.Cedula=Reservas.Cedula ";
                query += "inner join Clientes on clientes.ID_CLIENTE=reservas.Id_Cliente ";
                query += "inner join HabitacionesEstado on Reservas.id_esthab=HabitacionesEstado.id_esthab ";
                query += "where reservas.cedula=" + cedula + " ORDER BY Fecha_Ingreso DESC ";

                DataTable dt = SiaWin.Func.SqlDT(query, "precheckin", 0, cnOtros: dbAcceso);
                ds.Tables.Add(dt);

                DataTable dtcampos = SiaWin.Func.SqlDT("select Id_pventa as id_pventa,Nom_pventa as nom_pventa from PuntoVentas", "campos", 0, cnOtros: dbAcceso);
                ds.Tables.Add(dtcampos);

                //DataTable dtvivienda = SiaWin.Func.SqlDT("", "campos", 0, cnOtros: dbAcceso);
                //ds.Tables.Add(dtvivienda);

                return ds;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Cbx_campa.SelectedIndex >= 0 && Cbx_vivienda.SelectedIndex >= 0 && Cbx_habitacion.SelectedIndex >= 0)
                {
                    string update = "update PreCheckIn set cod_vivienda='" + Cbx_vivienda.SelectedValue + "',id_habitacion='" + Cbx_habitacion.SelectedValue + "',id_cama='" + Cbx_cama.SelectedValue + "' where idrow='" + Tx_empleado.Tag + "'; ";
                    DataTable dtupdate = SiaWin.Func.SqlDT(update, "campos", 0, cnOtros: dbAcceso);
                    MessageBox.Show("actualizacion exitosa");
                    actualizo = true;
                    correo();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("llene todos los campos correspondientes");
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al guardar");
            }
        }



        public void correo()
        {

            DateTime thisDay = DateTime.Today;
            string fecha = thisDay.ToString("d");

            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("rdzalejandro316@gmail.com");
                mail.To.Add("alejandro-rdz-@hotmail.com");
                mail.Subject = "pre check in exitoso";

                mail.IsBodyHtml = true;
                string htmlBody;

                htmlBody = "<!DOCTYPE html>" +
                "<html>" +
                "<head>" +
                "<title>recibo html</title>" +
                "<style type='text/css'>" +
                "html, body, div, span, applet, object, iframe,h1, h2, h3, h4, h5, h6, p, blockquote, pre,a, abbr, acronym, address, big, cite, code,del, dfn, em, img, ins, kbd, q, s, samp,small, strike, strong, sub, sup, tt, var,b, u, i, center,dl, dt, dd, ol, ul, li,fieldset, form, label, legend,table, caption, tbody, tfoot, thead, tr, th, td,article, aside, canvas, details, embed,figure, figcaption, footer, header, hgroup,menu, nav, output, ruby, section, summary,time, mark, audio, video {margin: 0;padding: 0;border: 0;font-size: 100%;vertical-align: baseline;}" +
                "article, aside, details, figcaption, figure,footer, header, hgroup, menu, nav, section {display: block;}" +
                "body {line-height: 1;}" +
                "ol, ul {list-style: none;}" +
                "blockquote, q {    quotes: none;}" +
                "blockquote:before, blockquote:after,q:before, q:after {content: '';content: none;}" +
                "table {border-collapse: collapse;border-spacing: 0;}" +
                "a{text-decoration:none;}" +
                ".recibo3{border: 1px solid;width: 400px;height: 400px;    margin-left: 10px;margin-top: 10px;background-color: #1F1F1F;}" +
                ".m1{width:80%;height:80%;margin-left:10%;margin-top:10%;border: 1px solid #1565C0;text-align: center;background: #9e9e9e29;}" +
                ".imag-m1{margin-top: 20px; width: 40px;height: 40px;margin-bottom: 10px;}" +
                ".m1-text{color: #bdc3c7;margin-top: 10px;}" +
                ".parrafo{padding: 5px;}" +
                ".parrafo .titulos1{color: #bdc3c7;}" +
                ".parrafo .titulos .res{color: #bdc3c7;}" +
                //".image {background-image:url("+PathImg+");}"+
                "</style>" +
                "</head>" +
                "<body>" +
                "<div class='recibo3'>" +
                "<div class='m1'>" +
                "<p class='parrafo'><span class='titulos1'>Checkin exitoso</span></p>" +
                "</div>" +
                "</div>" +
                "</body>" +
                "</html>";

                mail.Body = htmlBody;

                AlternateView avHtml = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);

                //LinkedResource pic1 = new LinkedResource(PathImg, MediaTypeNames.Image.Jpeg);
                //pic1.ContentId = "Pic1";
                //avHtml.LinkedResources.Add(pic1);

                mail.AlternateViews.Add(avHtml);

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("rdzalejandro316", "alejo3203339265");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                MessageBox.Show("mail Send");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                MessageBox.Show("no se envio: " + ex.ToString());
            }

        }


        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Cbx_campa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //MessageBox.Show("Cbx_campa.SelectedValue:"+ Cbx_campa.SelectedValue);
                if (Cbx_campa.SelectedIndex >= 0)
                {
                    Cbx_vivienda.ItemsSource = null;
                    Cbx_habitacion.ItemsSource = null;
                    string query = "select Cod_Vivienda as cod_vivienda,Nom_Vivienda as nom_vivienda from Viviendas where id_pventa='" + Cbx_campa.SelectedValue + "' ";
                    DataTable dtcampos = SiaWin.Func.SqlDT(query, "campos", 0, cnOtros: dbAcceso);
                    Cbx_vivienda.ItemsSource = dtcampos.DefaultView;
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("ERROR AL SELCCIONAR CAMPAMENTO:" + w);
            }
        }

        private void Cbx_vivienda_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (Cbx_vivienda.SelectedIndex >= 0)
                {
                    Cbx_habitacion.ItemsSource = null;
                    string query = "select Id_Habitacion as id_habitacion,can_camas from Habitaciones where COD_VIVIENDA='" + Cbx_vivienda.SelectedValue + "' ";
                    DataTable dtcampos = SiaWin.Func.SqlDT(query, "campos", 0, cnOtros: dbAcceso);
                    Cbx_habitacion.ItemsSource = dtcampos.DefaultView;
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("ERROR AL SELCCIONAR vivienda:" + w);
            }

        }

        private void Cbx_habitacion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (Cbx_habitacion.SelectedIndex >= 0 && Cbx_vivienda.SelectedIndex >= 0)
                {
                    Cbx_cama.ItemsSource = null;
                    string query = "select id_cama from HabitacionesCamas where id_habitacion='" + Cbx_habitacion.SelectedValue + "' and cod_vivienda='" + Cbx_vivienda.SelectedValue + "' ";
                    DataTable dtcampos = SiaWin.Func.SqlDT(query, "campos", 0, cnOtros: dbAcceso);
                    Cbx_cama.ItemsSource = dtcampos.DefaultView;
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("ERROR AL SELCCIONAR HABITACION:" + w);
            }
        }

        private void Cbx_cama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
