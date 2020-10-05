using System;
using System.Windows;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Data;
using System.IO;

namespace SiasoftAppExt
{    
    public partial class CtAcceso : Window
    {
        //Sia.PublicarPnt(10699, "CtAcceso");  
        //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(10699, "CtAcceso");
        //ww.ShowInTaskbar=false;    
        //ww.Owner = Application.Current.MainWindow;
        //ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
        //ww.ShowDialog();
        
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        

        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Socket client = null;
        static private string guid = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
        static private bool writeLogToFile = false;
        static private SiaRestApi Api = new SiaRestApi("http://portal.siasoft.slm.cloud:4443/rest/api/sql");

        // variables 
        string _PuntoVentaCodigo = "01";
        string _PuntoVentaNombre = "";
        bool _PuntoVentaAlimentacion = false;
        bool _PuntoVentaTipoCapturaRfid = false;
        bool _PuntoVentaTipoCapturaFace = false;
        bool _PuntoVentaTipoCapturaManual = false;
        bool _PuntoVentaTipoCapturaPDF467 = false;  //lector de cedula colombiana
        int _MaxRegistrosMostrarEnServicios = 10;

        static Log log = new Log();
        static Log _log = new Log();

        DataTable dtconf;

        public CtAcceso()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            idemp = SiaWin._BusinessId; ;
            LoadConfig();
            ConsultaServicios();
            InitSocket();
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
                this.Title = "Accesos " + cod_empresa + "-" + nomempresa;
                dtconf = SiaWin.Func.SqlDT("select * from ctmae_config", "configuracion", idemp);
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        public void cargarImagen(byte[] im, Image fuente)
        {
            try
            {
                //cargarImagen((byte[])dt.Rows[0]["Foto"]);
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
                fuente.Source = bi;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar al imagen:" + w);
            }
        }

        private void InitSocket()
        {
            try
            {
                serverSocket = null;
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //IPAddress ia = IPAddress.Parse("192.168.2.40");
                //IPAddress ia = IPAddress.Parse("64.250.116.2100");

                IPEndPoint ie = new IPEndPoint(IPAddress.Any, 6666);
                serverSocket.Bind(ie);
                serverSocket.Listen(10);
                serverSocket.BeginAccept(null, 0, OnAccept, null);
            }
            catch (Exception ex)
            {
                StringBuilder SB = new StringBuilder();
                SB.Append("EXEC [dbo].[_EmpCt_DevicesFace] ");
                SB.Append("Error Device 0000" + ",");
                SB.Append("'" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "',");
                SB.Append("'" + "Error InitShocket Exception:" + ex.Message + " - " + ex.ToString() + "'");
                //InitSocket();
            }
        }

        private void OnAccept(IAsyncResult result)
        {
            byte[] buffer = new byte[65000];
            try
            {
                string headerResponse = "";
                if (serverSocket != null && serverSocket.IsBound)
                {
                    client = null;
                    //int xx = serverSocket.ReceiveBufferSize;

                    client = serverSocket.EndAccept(result);
                    var i = client.Receive(buffer);

                    headerResponse = (System.Text.Encoding.UTF8.GetString(buffer)).Substring(0, i);
                    // write received data to the console
                    if (headerResponse.Contains("xV4"))
                    {
                        //client.Shutdown(SocketShutdown.Both);
                        //client.Close();

                        return;
                    }

                    if (headerResponse.Contains("UE9TVCAv"))
                    {
                        client.Shutdown(SocketShutdown.Both);
                        client.Close();
                        return;
                    }
                    if (headerResponse.Substring(0, 4) == "POST")
                    {

                        if (headerResponse.Contains("{") && headerResponse.Contains("}"))
                        {
                            int pos = headerResponse.IndexOf("{");
                            string body = headerResponse.Substring(pos);
                            if (body.Trim() != "")
                            {
                                JObject jsonMsg = JObject.Parse(body);
                                var cmd = jsonMsg.Value<string>("operator");
                                if (cmd.Trim() == "HeartBeat")
                                {
                                    var device = (string)jsonMsg["info"]["DeviceID"];
                                   
                                    DateTime time = (DateTime)jsonMsg["info"]["Time"];
                                    log.DeviceID = "" + device;
                                    log.FechaUpdate = time.ToString("dd/MM/yyyy HH:mm:ss tt");
                                    System.Windows.Application.Current.Dispatcher.Invoke(delegate {
                                        GridMain.IsEnabled = true; lblMessage.Visibility = Visibility.Visible;
                                        //ImgOnline.Visibility = Visibility.Visible;
                                        //ImgOffline.Visibility = Visibility.Hidden;
                                        //ImgConect.Visibility = Visibility.Hidden;
                                    });


                                    StringBuilder SB = new StringBuilder();
                                    SB.Append("EXEC [dbo].[_EmpCt_DevicesFace] ");
                                    SB.Append(device.ToString() + ",");
                                    SB.Append("'" + time.ToString("dd/MM/yyyy HH:mm:ss") + "',");
                                    SB.Append("'" + "-" + "'");

                                    String JSON = Api.Query(SB.ToString(), "010", 1);
                                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"WriteLines2.txt", true))
                                    {
                                        file.WriteLine(body);
                                    }
                                    if (JSON.Contains("Error"))
                                    {
                                        System.Windows.Application.Current.Dispatcher.Invoke(delegate {
                                            lblMessageError.Visibility = Visibility.Visible;
                                        });
                                    }
                                    else
                                    {
                                        System.Windows.Application.Current.Dispatcher.Invoke(delegate {
                                            lblMessageError.Visibility = Visibility.Hidden;
                                        });

                                    }
                                }
                                if (cmd.Trim() == "VerifyPush")
                                {

                                    //var nombre = (string)jsonMsg["info"]["Name"];
                                    var cedula = (string)jsonMsg["info"]["IdCard"];
                                    //var time = (string)jsonMsg["info"]["CreateTime"];
                                    //log.Nombre = nombre;
                                    //log.Cedula = cedula;
                                    //log.FechaRegistro = "" + time;
                                    StringBuilder SB = new StringBuilder();
                                    SB.Append("EXEC [dbo].[_EmpCt_Acceso] ");
                                    SB.Append("'"+cedula.Trim()+"',");          //cedula
                                    SB.Append("'',");                           //tarjeta
                                    SB.Append("1,");                            //cantidad , predeterminada 1
                                    SB.Append("'"+ _PuntoVentaCodigo + "',");   // punto de venta codigo
                                    SB.Append("3");                             // Tipo de ingreso servicio 3=face
                                    var JSON = Api.Query(SB.ToString(), "010", 1);
                                    
                                    if (JSON.Contains("System.Data.SqlClient.SqlException"))
                                    {
                                        
                                        log.codmsg = "99"; log.message = "Error servidor";
                                        
                                    }
                                    
                                    returnMSG msg = Newtonsoft.Json.JsonConvert.DeserializeObject<returnMSG>(JSON.Replace("[", "").Replace("]", ""));
                                    log.codmsg = msg.codmsg.Trim();
                                    log.message = msg.message;
                                    if (log.codmsg == "1")
                                    {
                                        log.Cedula = cedula;
                                        log.Nombre = msg.nombre.Trim();
                                        log.Cargo = msg.cargo.Trim();
                                        log.Empresa = msg.empresa.Trim();
                                        log.Servicio = msg.servicio.Trim();
                                        log.Cantidades = msg.cantidades;
                                        log.FechaRegistro = msg.fecha;
                                    }
                                    System.Windows.Application.Current.Dispatcher.Invoke(delegate {
                                        if (log.codmsg == "1") lblMessageError.Visibility = Visibility.Hidden;
                                        if (log.codmsg != "1")
                                        {
                                            lblMessageError.Visibility = Visibility.Visible;
                                        }
                                    });
                                    if (JSON.Contains("Error") || JSON.Contains("System.Data.SqlClient.SqlException"))
                                    {
                                        System.Windows.Application.Current.Dispatcher.Invoke(delegate {
                                            lblMessageError.Visibility = Visibility.Visible;
                                        });
                                    }
                                    else
                                    {
                                        System.Windows.Application.Current.Dispatcher.Invoke(delegate {
                                            lblMessage.Visibility = Visibility.Visible;
                                        });
                                    }
                                    if (log.codmsg != "1")
                                    {
                                        log.Nombre = ""; log.Cedula = ""; log.FechaRegistro = "";log.Empresa = "";log.FechaRegistro = "";log.Cargo="";
                                    }
                                    //MessageBox.Show(log.codmsg + "-" + log.message);
                                    log.message = msg.codmsg.Trim() + "-" + msg.message;
                                    if (writeLogToFile) WriteToLog(JSON);
                                    ConsultaServicios();
                                }
                                if (body.Trim() != "")
                                {
                                    if (writeLogToFile) WriteToLog(headerResponse);
                                }
                            }
                        }
                    }
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                }
                else
                {
                    SendDataToDeviceFace("Error serverSocket != null " + serverSocket.ToString() + " -serverSocket.IsBound " + serverSocket.IsBound.ToString());
                    if (log.DeviceID.Trim() != "")
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(delegate {
                            //ImgOnline.Visibility = Visibility.Hidden; ImgOffline.Visibility = Visibility.Visible;
                        });

                    }

                }
            }
            catch (SocketException exception)
            {

                SendDataToDeviceFace(exception.Message + "-" + exception.ToString());

                System.Windows.Application.Current.Dispatcher.Invoke(delegate {
                    lblMessageError.Visibility = Visibility.Visible;
                    //log.codmsg = "99"; log.message = "Error Servidor";

                });


            }
            catch (Exception ex)
            {
                SendDataToDeviceFace(ex.Message + "-" + ex.ToString());
                System.Windows.Application.Current.Dispatcher.Invoke(delegate {
                    lblMessageError.Visibility = Visibility.Visible;
                    //log.codmsg = "99"; log.message = "Error Servidor";

                });



            }
            finally
            {
                if (serverSocket != null && serverSocket.IsBound)
                {
                    serverSocket.BeginAccept(null, 0, OnAccept, null);
                }
            }
        }

        private static void SendDataToDeviceFace(string data)
        {
            StringBuilder SB = new StringBuilder();
            SB.Append("EXEC [dbo].[_EmpCt_DevicesFace] ");
            SB.Append("0000" + ",");
            SB.Append("'" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "',");
            SB.Append("'" + "Error Exception:" + data + "'");
            String JSON = Api.Query(SB.ToString(), "010", 1);
        }

        private  void ConsultaServicios()
        {
            try
            {
                StringBuilder SB = new StringBuilder();
                SB.Append("select top 100  acceso.EmpleadoCodigo as codigo,rtrim(empleados.EmpleadoNombres) + '-' + rtrim(empleados.EmpleadoApellidos) as nombre,rtrim(empresas.EmpresaNombre) as empresa,productos.ProductoNombre as servicio,acceso.FechaRegistro as fecha,cantidad from CtDoc_Acceso as acceso ");
                SB.Append("inner join CtMae_Empleados as empleados on empleados.Empleadocodigo = acceso.EmpleadoCodigo ");
                SB.Append("inner join CtMae_Empresas as empresas on empresas.EmpresaCodigo = acceso.EmpresaCodigo ");
                SB.Append("inner join CtMae_Productos as productos on productos.ProductoCodigo = acceso.ProductoCodigo ");
                SB.Append("where convert(date, FechaRegistro)= convert(date, getdate()) order by FechaRegistro desc ");

                String JSON = Api.Query(SB.ToString(), "010", 1);
                var Users = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(JSON);
                System.Windows.Application.Current.Dispatcher.Invoke(delegate
                {
                    DGridServicios.ItemsSource = Users;
                });
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message);
            }            
        }

        private static void WriteToLog(string msg)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"WriteLines2.txt", true))
            {
                file.WriteLine(msg);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GridMain.DataContext = log;
            //this.DataContext = log;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                serverSocket.Dispose();
                serverSocket = null;
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
            catch(Exception ex)
            {
                //MessageBox.Show("Error al cerrar...");
                
            }
        }
        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key==System.Windows.Input.Key.Escape)
            {
                if(MessageBox.Show("Salir","Usted desea salir....?",MessageBoxButton.YesNo)==MessageBoxResult.Yes)
                {
                    this.Close();
                }

            }
        }


    }


    public class returnMSG
    {
        public string codmsg { get; set; }
        public string message { get; set; }
        
        public string nombre { get; set; }
        public string cargo { get; set; }
        public string empresa { get; set; }
        public string servicio { get; set; }
        public int cantidades { get; set; }
        public string fecha { get; set; }

    }
    public class Log : INotifyPropertyChanged
    {
        /*Funciones generales*/
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
        /*Fin funciones generales*/
        /*Campo primaryKey id_ven*/
        string _LogEvent;
        public string LogEvent { get { return _LogEvent; } set { _LogEvent = value; OnPropertyChanged("LogEvent"); } }
        string _Nombre;
        public string Nombre { get { return _Nombre; } set { _Nombre = value; OnPropertyChanged("Nombre"); } }
        string _Cedula;
        public string Cedula { get { return _Cedula; } set { _Cedula = value; OnPropertyChanged("Cedula"); } }
        string _Cargo;
        public string Cargo { get { return _Cargo; } set { _Cargo = value; OnPropertyChanged("Cargo"); } }
        string _Empresa;
        public string Empresa { get { return _Empresa; } set { _Empresa = value; OnPropertyChanged("Empresa"); } }
        string _FechaRegistro;
        public string FechaRegistro { get { return _FechaRegistro; } set { _FechaRegistro = value; OnPropertyChanged("FechaRegistro"); } }
        string _DeviceID = "";
        public string DeviceID { get { return _DeviceID; } set { _DeviceID = value; OnPropertyChanged("DeviceID"); } }
        string _FechaUpdate = "";
        public string FechaUpdate { get { return _FechaUpdate; } set { _FechaUpdate = value; OnPropertyChanged("FechaUpdate"); } }
        Image _PathImg = new Image();
        string _codmsg;
        public string codmsg { get { return _codmsg; } set { _codmsg = value; OnPropertyChanged("codmsg"); } }
        string _message;
        public string message { get { return _message; } set { _message = value; OnPropertyChanged("message"); } }
        public Image PathImg { get { return _PathImg; } set { _PathImg = value; OnPropertyChanged("PathImg"); } }
        bool _visibleLblMsg = true;
        public bool visibleLblMsg { get { return _visibleLblMsg; } set { _visibleLblMsg = value; OnPropertyChanged("visibleLblMsg"); } }
        bool _visibleLblMsgError = false;
        public bool visibleLblMsgError { get { return _visibleLblMsgError; } set { _visibleLblMsgError = value; OnPropertyChanged("visibleLblMsgError"); } }

        string _Servicio;
        public string Servicio { get { return _Servicio; } set { _Servicio = value; OnPropertyChanged("Servicio"); } }

        int _Cantidades = 0;
        public int Cantidades { get { return _Cantidades; } set { _Cantidades = value; OnPropertyChanged("Cantidades"); } }



    }

    public class SiaRestApi
    {
        String ApiKey = "d86a3242d704b0bef502cb8431df74a4";   //Siasoft Dev & Test
        String ApiURL;
        string Projecto = "2";
        public SiaRestApi(String URL, String Login = "", String Pass = "")
        {
            ApiURL = URL;
        }
        public String Query(String Query, string emp, int idprojecto)
        {
            var client = new WebClient();
            client.Headers.Add("user-agent", "SiasoftApi / SLM (Rest Api 1.0)");
            string Json = "";
            if (emp.Trim() != "") Json = "{\"query\":\"" + Query + "\",\"environ\":\"" + "" + emp + "\",\"ID\":\"" + "" + idprojecto + "\" }";
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            String response = "";
            try
            {
                byte[] reqString = Encoding.Default.GetBytes(Json);
                Byte[] byteResp = client.UploadData(ApiURL, "POST", reqString);
                response = System.Text.Encoding.UTF8.GetString(byteResp);
                if (response.Contains("SqlException"))
                {
                    return "false";
                }
            }
            catch (Exception e)
            {
                //throw;
                response = "false";
            }
            return response;
        }
    }
}
