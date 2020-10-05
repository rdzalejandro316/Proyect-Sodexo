using CtaMae_Empleado;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9699, "CtaMae_Empleado");  
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9699, "CtaMae_Empleado");
    //ww.ShowInTaskbar=false;    
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
    //ww.ShowDialog();

    public partial class CtaMae_Empleado : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        DispatcherTimer disp = new DispatcherTimer();

        public string strName = "", imageName;
        bool imageSave = false;
        public byte[] imgByteArr = null;
        public byte[] _img_cli;

        public bool saveRIFD = false;
        public string valrfid = "";


        public CtaMae_Empleado()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            idemp = SiaWin._BusinessId; ;
            LoadConfig();
            loadCombos();
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
                this.Title = "Maestra de empleados " + cod_empresa + "-" + nomempresa;
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void SnackbarMessage_ActionClick(object sender, RoutedEventArgs e)
        {
            Notificaction.IsActive = false;
        }
        public void NotificationOn(string messaage, int tipo = 0)
        {
            Notificaction.IsActive = true;
            NotiMessa.Content = messaage.Trim();
            disp.Interval = TimeSpan.FromMilliseconds(5000);
            disp.Tick += (sender, args) =>
            {
                Notificaction.IsActive = false;
                disp.Stop();
            };
            disp.Start();

            if (tipo == 0) disp.Start();
        }
        public void loadCombos()
        {
            try
            {
                string sqlpv = "select PuntoVentaCodigo as codigo,rtrim(PuntoVentaCodigo)+'-'+rtrim(PuntoVentaNombre) as nombre from CtMae_PuntosDeVenta where estado=1 order by PuntoVentaCodigo;";
                DataTable dtpv = SiaWin.Func.SqlDT(sqlpv, "empleados", idemp);
                if (dtpv.Rows.Count > 0)
                {
                    CBpuntov.ItemsSource = dtpv.DefaultView;
                    CBpuntov.DisplayMemberPath = "nombre";
                    CBpuntov.SelectedValuePath = "codigo";
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar combos:" + w);
            }
        }


        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int idr = 0; string code = ""; string nombre = "";
                dynamic xx = SiaWin.WindowBuscar("ctmae_empleados", "empleadocodigo", "empleadonombres", "empleadocodigo", "idrow", "Maestra de Empleados", cnEmp, false, "", idEmp: idemp);
                xx.ShowInTaskbar = false;
                xx.Owner = Application.Current.MainWindow;
                xx.Height = 400;
                xx.ShowDialog();
                idr = xx.IdRowReturn;
                code = xx.Codigo;
                nombre = xx.Nombre;
                xx = null;
                if (idr > 0)
                {
                    bool f = cargarClinte(code);
                    controls(!f);
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al buscar:" + w);
            }
        }

        public void desbloquePanel(bool f)
        {
            Panel1.IsEnabled = f;
            Panel2.IsEnabled = f;
            Panel3.IsEnabled = f;
        }

        public void controls(bool flag)
        {
            if (flag)//inicial
            {
                BtnBuscar.IsEnabled = true;
                BtnNuevo.IsEnabled = true;
                BtnEditar.IsEnabled = false;
                BtnEliminar.IsEnabled = false;
            }
            if (!flag) //inicio busqueda y fue efectiva
            {
                BtnBuscar.IsEnabled = true;
                BtnNuevo.IsEnabled = true;
                BtnEditar.IsEnabled = true;
                BtnEliminar.IsEnabled = true;
            }
        }
        private void BTNsearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                string tag = (sender as Button).Tag.ToString();
                string cmptabla = ""; string cmpcodigo = ""; string cmpnombre = ""; string cmporden = ""; string cmpidrow = ""; string cmptitulo = ""; string cmpconexion = ""; bool mostrartodo = true; string cmpwhere = "";

                switch (tag)
                {
                    case "CtMae_Empresas":
                        cmptabla = tag; cmpcodigo = "EmpresaCodigo"; cmpnombre = "EmpresaNombre"; cmporden = "EmpresaCodigo"; cmpidrow = "EmpresaCodigo"; cmptitulo = "Clientes"; cmpconexion = cnEmp; mostrartodo = true; cmpwhere = "";
                        break;
                    case "CtMae_EmpleadosCargos":
                        cmptabla = tag; cmpcodigo = "CargoId"; cmpnombre = "CargoNombre"; cmporden = "CargoId"; cmpidrow = "CargoId"; cmptitulo = "Cargos"; cmpconexion = cnEmp; mostrartodo = true; cmpwhere = "";
                        break;
                }


                int idr = 0; string code = ""; string nom = "";
                dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, cnEmp, mostrartodo, cmpwhere, idEmp: idemp);
                winb.ShowInTaskbar = false;
                winb.Owner = Application.Current.MainWindow;
                winb.Width = 500;
                winb.Height = 300;
                winb.ShowDialog();
                idr = winb.IdRowReturn;
                code = winb.Codigo;
                nom = winb.Nombre;

                if (idr > 0)
                {
                    if (tag == "CtMae_Empresas")
                    {
                        Txcliente.Text = nom.Trim();
                        Txcliente.Tag = code.ToString().Trim();
                    }
                    if (tag == "CtMae_EmpleadosCargos")
                    {
                        Txcargo.Text = nom.Trim();
                        Txcargo.Tag = code.ToString().Trim();
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al buscar:" + w);
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

        public bool cargarClinte(string emplcod)
        {
            bool flag = false;
            try
            {
                string query = "select emp.Foto,emp.EmpleadoCodigo,emp.EmpleadoNombres,emp.EmpleadoApellidos,emp.Cedula,emp.EmpresaCodigo,bussi.EmpresaNombre,emp.PuntoVentaCodigo,emp.CargoId,cargo.CargoNombre,emp.Direccion,emp.Email,emp.Celular, ";
                query += "emp.IndDesayuno,emp.IndAlmuerzo,emp.IndComida,emp.IndCena,emp.IndLunch,emp.IndLunchEspecial,emp.IndManualCliente,emp.IndRefrigerios,emp.CantidadPermitidas, ";
                query += "emp.AutorizacionPuntoVenta,emp.MultiReservas,emp.IndVivienda,emp.IndViviendaBase,emp.ViviendaCodigo,emp.HabitacionCodigo,emp.CamaCodigo,emp.Nota,emp.Estado,emp.AutorizacionPuntoVenta,emp.indVisitante,emp.Tarjeta ";
                query += "from CtMae_Empleados as emp ";
                query += "inner join CtMae_Empresas as bussi on bussi.EmpresaCodigo = emp.EmpresaCodigo ";
                query += "left join CtMae_EmpleadosCargos as cargo on cargo.CargoId= emp.CargoId ";
                query += "where emp.EmpleadoCodigo='" + emplcod + "' ";


                DataTable dt = SiaWin.Func.SqlDT(query, "empleados", idemp);

                if (dt.Rows.Count > 0)
                {
                    flag = true;


                    if (dt.Rows[0]["Foto"] != DBNull.Value)
                        cargarImagen((byte[])dt.Rows[0]["Foto"]);

                    string tarjeta = dt.Rows[0]["Tarjeta"].ToString();                    

                    if (string.IsNullOrWhiteSpace(tarjeta))
                    {
                        saveRIFD = false;
                        valrfid = "";
                        BtnRFID.Content = "RFID (✖)";
                    }
                    else
                    {
                        saveRIFD = true;
                        valrfid = tarjeta;
                        BtnRFID.Content = "RFID (✔)";
                    }

                    TxCodigo.Text = dt.Rows[0]["EmpleadoCodigo"].ToString().Trim();
                    TxDocumento.Text = dt.Rows[0]["Cedula"].ToString().Trim();

                    TxCorreo.Text = dt.Rows[0]["Email"].ToString().Trim();
                    TxCelular.Text = dt.Rows[0]["Celular"].ToString().Trim();

                    Txcliente.Text = dt.Rows[0]["EmpresaNombre"].ToString().Trim();
                    Txcliente.Tag = dt.Rows[0]["EmpresaCodigo"].ToString().Trim();

                    Txcargo.Text = dt.Rows[0]["CargoNombre"].ToString().Trim();
                    Txcargo.Tag = dt.Rows[0]["CargoId"].ToString().Trim();

                    string PuntoVentaCodigo = dt.Rows[0]["PuntoVentaCodigo"].ToString().Trim();
                    CBpuntov.SelectedIndex = seachValueCb(PuntoVentaCodigo, "codigo", CBpuntov);

                    TxNombres.Text = dt.Rows[0]["EmpleadoNombres"].ToString();
                    TxApellidos.Text = dt.Rows[0]["EmpleadoApellidos"].ToString();

                    CHdesayuno.IsChecked = Convert.ToBoolean(dt.Rows[0]["IndDesayuno"] != DBNull.Value ? dt.Rows[0]["IndDesayuno"] : 0);
                    CHalmuerzo.IsChecked = Convert.ToBoolean(dt.Rows[0]["IndAlmuerzo"] != DBNull.Value ? dt.Rows[0]["IndAlmuerzo"] : 0);
                    CHcomida.IsChecked = Convert.ToBoolean(dt.Rows[0]["IndComida"] != DBNull.Value ? dt.Rows[0]["IndComida"] : 0);
                    CHcena.IsChecked = Convert.ToBoolean(dt.Rows[0]["IndCena"] != DBNull.Value ? dt.Rows[0]["IndCena"] : 0);
                    CHrefrigerio.IsChecked = Convert.ToBoolean(dt.Rows[0]["IndRefrigerios"] != DBNull.Value ? dt.Rows[0]["IndRefrigerios"] : 0);
                    UDcntp.Value = Convert.ToDouble(dt.Rows[0]["CantidadPermitidas"]);
                    CHmulcli.IsChecked = Convert.ToBoolean(dt.Rows[0]["IndManualCliente"] != DBNull.Value ? dt.Rows[0]["IndManualCliente"] : 0);
                    CHmulrese.IsChecked = Convert.ToBoolean(dt.Rows[0]["MultiReservas"] != DBNull.Value ? dt.Rows[0]["MultiReservas"] : 0);
                    CHluch.IsChecked = Convert.ToBoolean(dt.Rows[0]["IndLunch"] != DBNull.Value ? dt.Rows[0]["IndLunch"] : 0);
                    CHservi.IsChecked = Convert.ToBoolean(dt.Rows[0]["AutorizacionPuntoVenta"] != DBNull.Value ? dt.Rows[0]["AutorizacionPuntoVenta"] : 0);
                    CHestado.IsChecked = Convert.ToBoolean(dt.Rows[0]["Estado"] != DBNull.Value ? dt.Rows[0]["Estado"] : 0);
                    CHvisi.IsChecked = Convert.ToBoolean(dt.Rows[0]["indVisitante"] != DBNull.Value ? dt.Rows[0]["indVisitante"] : 0);
                    CHvivienda.IsChecked = Convert.ToBoolean(dt.Rows[0]["IndVivienda"] != DBNull.Value ? dt.Rows[0]["IndVivienda"] : 0);

                    loadCombo(PuntoVentaCodigo);
                    if (Convert.ToBoolean(dt.Rows[0]["IndVivienda"]))
                    {
                        string viv = dt.Rows[0]["ViviendaCodigo"].ToString().Trim();
                        string hab = dt.Rows[0]["HabitacionCodigo"].ToString().Trim();
                        string cam = dt.Rows[0]["CamaCodigo"].ToString().Trim();

                        if (!string.IsNullOrWhiteSpace(viv))
                        {
                            CBVivienda.SelectedIndex = seachValueCb(viv, "codigo", CBVivienda);
                            CBHabitacion.SelectedIndex = seachValueCb(hab, "codigo", CBHabitacion);
                            CBCama.SelectedIndex = seachValueCb(cam, "codigo", CBCama);
                        }
                    }
                    else
                    {
                        CBVivienda.SelectedIndex = -1;
                        CBHabitacion.SelectedIndex = -1;
                        CBCama.SelectedIndex = -1;
                    }
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar:" + w);
            }
            return flag;
        }

        public int seachValueCb(string value, string campo, ComboBox CB)
        {
            int index = 0; int i = 0;
            foreach (DataRowView item in CB.Items)
            {
                if (item.Row[campo].ToString().Trim() == value) index = i;
                i++;
            }
            return index;
        }
        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                clean();
                activeSave(true, "Guardar");
                desbloquePanel(true);
                TxCodigo.Focus();
            }
            catch (Exception w)
            {
                MessageBox.Show("error en el btn nuevo:" + w);
            }
        }


        public void activeSave(bool flag, string nameBtn = "")
        {
            BtnSave.Content = nameBtn;

            if (flag == true)
            {
                CtrlA.Visibility = Visibility.Hidden;
                CtrlB.Visibility = Visibility.Visible;
            }
            else
            {
                CtrlA.Visibility = Visibility.Visible;
                CtrlB.Visibility = Visibility.Hidden;
            }
        }

        public void clean()
        {
            BtnRFID.Content = "RFID (✖)";
            saveRIFD = false;
            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("Imagen/frame.png", UriKind.Relative);
            bi3.EndInit();
            image1.Source = bi3;
            imageSave = false;
            TxCodigo.Text = "";
            TxDocumento.Text = "";
            TxCorreo.Text = "";
            TxCelular.Text = "";
            Txcliente.Text = "";
            Txcliente.Tag = "";
            Txcargo.Text = "";
            Txcargo.Tag = "";
            CBpuntov.SelectedIndex = -1;
            TxNombres.Text = "";
            TxApellidos.Text = "";
            CHdesayuno.IsChecked = false;
            CHalmuerzo.IsChecked = false;
            CHcomida.IsChecked = false;
            CHcena.IsChecked = false;
            CHrefrigerio.IsChecked = false;
            UDcntp.Value = 0;
            CHmulcli.IsChecked = false;
            CHmulrese.IsChecked = false;
            CHluch.IsChecked = false;
            CHservi.IsChecked = false;
            CHestado.IsChecked = false;
            CHvisi.IsChecked = false;
            CHvivienda.IsChecked = false;
        }
        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            desbloquePanel(true);
            activeSave(true, "Modificar");
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("USTED DESEA ELIMINAR EL EMPLEADO : " + TxCodigo.Text, "Siasoft?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    string query = "delete CtMae_Empleados where EmpleadoCodigo='" + TxCodigo.Text + "';";

                    if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                    {
                        NotificationOn("Eliminacion exitosa");
                        clean();
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("alerta:" + w);
            }
        }


        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            desbloquePanel(false);

            if (BtnSave.Content.ToString().Trim() == "Modificar")
            {
                controls(false);
            }
            else
            {
                clean();
                controls(true);
            }
            activeSave(false);
        }


        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region validaciones

                if (string.IsNullOrWhiteSpace(TxCodigo.Text))
                {
                    NotificationOn("debe de ingresar la cedula");
                    return;
                }

                if (string.IsNullOrWhiteSpace(TxDocumento.Text))
                {
                    NotificationOn("debe de ingresar el documento");
                    return;
                }

                if (string.IsNullOrWhiteSpace(TxNombres.Text))
                {
                    NotificationOn("debe de ingresar los nombres");
                    return;
                }

                if (string.IsNullOrWhiteSpace(TxApellidos.Text))
                {
                    NotificationOn("debe de ingresar los apellidos");
                    return;
                }

                if (string.IsNullOrWhiteSpace(Txcliente.Text))
                {
                    NotificationOn("debe de ingresar el cliente");
                    return;
                }

                if (string.IsNullOrWhiteSpace(Txcargo.Text))
                {
                    NotificationOn("debe de seleccionar el cargo");
                    return;
                }

                if (CBpuntov.SelectedIndex < 0)
                {
                    NotificationOn("debe de seleccionar el punto de venta");
                    return;
                }

                if (GridVivienda.IsEnabled == true)
                {
                    foreach (ComboBox cb in GridVivienda.Children)
                    {
                        if (cb.SelectedIndex < 0)
                        {
                            NotificationOn("seleccione una :" + cb.Tag.ToString());
                            return;
                        }
                    }
                }



                #endregion

                if (imageSave == true)
                {
                    FileStream fs = new FileStream(imageName, FileMode.Open, FileAccess.Read);
                    imgByteArr = new byte[fs.Length];
                    fs.Read(imgByteArr, 0, Convert.ToInt32(fs.Length));
                    fs.Close();
                }

                if (BtnSave.Content.ToString().Trim() == "Modificar")
                {

                    using (SqlConnection connection = new SqlConnection(SiaWin.Func.DatosEmp(idemp)))
                    {
                        using (SqlCommand cmd = connection.CreateCommand())
                        {
                            string cad_ima = imageSave == true ?
                                "Foto=@Foto," : "";


                            cmd.CommandText = "update CtMae_Empleados set EmpleadoNombres=@EmpleadoNombres,EmpleadoApellidos=@EmpleadoApellidos,EmpleadoCodigo=@EmpleadoCodigo,Cedula=@Cedula,Celular= @Celular,Email= @Email,EmpresaCodigo=@EmpresaCodigo,PuntoVentaCodigo=@PuntoVentaCodigo,CargoId=@CargoId,IndDesayuno=@IndDesayuno,IndAlmuerzo=@IndAlmuerzo,IndComida=@IndComida,IndCena=@IndCena,IndLunchEspecial=@IndLunchEspecial,IndManualCliente=@IndManualCliente,CantidadPermitidas=@CantidadPermitidas,MultiReservas=@MultiReservas,Nota=@Nota,Estado=@Estado," + cad_ima + "IndRefrigerios=@IndRefrigerios,AutorizacionPuntoVenta= @AutorizacionPuntoVenta,IndLunch=@IndLunch,indVisitante= @indVisitante,IndVivienda =@IndVivienda,ViviendaCodigo=@ViviendaCodigo,HabitacionCodigo=@HabitacionCodigo,CamaCodigo=@CamaCodigo,Tarjeta=@Tarjeta where EmpleadoCodigo='" + TxCodigo.Text + "' ";

                            cmd.Parameters.AddWithValue("@EmpleadoCodigo", TxCodigo.Text);
                            cmd.Parameters.AddWithValue("@Cedula", TxDocumento.Text);

                            cmd.Parameters.AddWithValue("@EmpleadoNombres", TxNombres.Text);
                            cmd.Parameters.AddWithValue("@EmpleadoApellidos", TxApellidos.Text);

                            cmd.Parameters.AddWithValue("@Email", TxCorreo.Text);
                            cmd.Parameters.AddWithValue("@Celular", TxCelular.Text);

                            cmd.Parameters.AddWithValue("@EmpresaCodigo", Txcliente.Tag.ToString());
                            cmd.Parameters.AddWithValue("@PuntoVentaCodigo", CBpuntov.SelectedValue.ToString());
                            cmd.Parameters.AddWithValue("@CargoId", Txcargo.Tag.ToString());
                            cmd.Parameters.AddWithValue("@IndDesayuno", Convert.ToInt32(CHdesayuno.IsChecked));
                            cmd.Parameters.AddWithValue("@IndAlmuerzo", Convert.ToInt32(CHalmuerzo.IsChecked));
                            cmd.Parameters.AddWithValue("@IndComida", Convert.ToInt32(CHcomida.IsChecked));
                            cmd.Parameters.AddWithValue("@IndCena", Convert.ToInt32(CHcena.IsChecked));
                            cmd.Parameters.AddWithValue("@IndLunchEspecial", Convert.ToInt32(CHluch.IsChecked));
                            cmd.Parameters.AddWithValue("@IndManualCliente", Convert.ToInt32(CHmulcli.IsChecked));
                            cmd.Parameters.AddWithValue("@CantidadPermitidas", Convert.ToInt32(UDcntp.Value));
                            cmd.Parameters.AddWithValue("@MultiReservas", Convert.ToInt32(CHmulrese.IsChecked));

                            cmd.Parameters.AddWithValue("@AutorizacionPuntoVenta", Convert.ToInt32(CHservi.IsChecked));
                            cmd.Parameters.AddWithValue("@indVisitante", Convert.ToInt32(CHvisi.IsChecked));
                            cmd.Parameters.AddWithValue("@IndLunch", Convert.ToInt32(CHluch.IsChecked));

                            if (imageSave == true) cmd.Parameters.AddWithValue("@Foto", imgByteArr);

                            cmd.Parameters.AddWithValue("@IndVivienda", Convert.ToInt32(CHvivienda.IsChecked));


                            cmd.Parameters.AddWithValue("@ViviendaCodigo", GridVivienda.IsEnabled == true ?
                                CBVivienda.SelectedValue.ToString().Trim() : "");

                            cmd.Parameters.AddWithValue("@HabitacionCodigo", GridVivienda.IsEnabled == true ?
                                CBHabitacion.SelectedValue.ToString().Trim() : "");

                            cmd.Parameters.AddWithValue("@CamaCodigo", GridVivienda.IsEnabled == true ?
                                CBCama.SelectedValue.ToString().Trim() : "");


                            cmd.Parameters.AddWithValue("@Nota", tx_nota.Text);
                            cmd.Parameters.AddWithValue("@Estado", Convert.ToInt32(CHestado.IsChecked));
                            cmd.Parameters.AddWithValue("@IndRefrigerios", Convert.ToInt32(CHrefrigerio.IsChecked));

                            cmd.Parameters.AddWithValue("@Tarjeta", valrfid);

                            connection.Open();
                            int val = cmd.ExecuteNonQuery();
                            connection.Close();

                            if (val > 0)
                            {
                                NotificationOn("actualizacion exitosa");
                                desbloquePanel(false);
                                activeSave(false);
                            }
                            else
                            {
                                MessageBox.Show("no actualizo ningun campo");
                            }
                        }
                    }
                }
                else
                {
                    using (SqlConnection connection = new SqlConnection(SiaWin.Func.DatosEmp(idemp)))
                    {
                        using (SqlCommand cmd = connection.CreateCommand())
                        {

                            string cam = imageSave == true ? "Foto," : "";
                            string val = imageSave == true ? "@Foto," : "";

                            string reserva = ""; string reservaval = "";
                            if (GridVivienda.IsEnabled == true)
                            {
                                reserva = ",ViviendaCodigo,HabitacionCodigo,CamaCodigo";
                                reservaval = ",@ViviendaCodigo,@HabitacionCodigo,@CamaCodigo";
                            }

                            cmd.CommandText = "insert into CtMae_Empleados(EmpleadoCodigo,EmpleadoNombres,EmpleadoApellidos,Cedula,Celular,Email,EmpresaCodigo,PuntoVentaCodigo,CargoId,IndDesayuno,IndAlmuerzo,IndComida,IndCena,IndLunchEspecial,IndManualCliente,CantidadPermitidas,MultiReservas,Nota,Tarjeta,Estado,IndLunch,AutorizacionPuntoVenta,indVisitante,IndRefrigerios," + cam + "IndVivienda" + reserva + ") values(@EmpleadoCodigo, @EmpleadoNombres, @EmpleadoApellidos, @Cedula,@Email,@Celular,@EmpresaCodigo, @PuntoVentaCodigo, @CargoId,@IndDesayuno, @IndAlmuerzo, @IndComida, @IndCena, @IndLunchEspecial, @IndManualCliente, @CantidadPermitidas, @MultiReservas,@Nota, @Tarjeta,@Estado,@IndLunch,@AutorizacionPuntoVenta,@indVisitante,@IndRefrigerios," + val + "@IndVivienda" + reservaval + ") ";

                            cmd.Parameters.AddWithValue("@EmpleadoCodigo", TxCodigo.Text);
                            cmd.Parameters.AddWithValue("@Cedula", TxDocumento.Text);

                            cmd.Parameters.AddWithValue("@EmpleadoNombres", TxNombres.Text);
                            cmd.Parameters.AddWithValue("@EmpleadoApellidos", TxApellidos.Text);
                            cmd.Parameters.AddWithValue("@Email", TxCorreo.Text);
                            cmd.Parameters.AddWithValue("@Celular", TxCelular.Text);
                            cmd.Parameters.AddWithValue("@EmpresaCodigo", Txcliente.Tag.ToString());
                            cmd.Parameters.AddWithValue("@PuntoVentaCodigo", CBpuntov.SelectedValue.ToString());
                            cmd.Parameters.AddWithValue("@CargoId", Txcargo.Tag.ToString());
                            cmd.Parameters.AddWithValue("@IndDesayuno", Convert.ToInt32(CHdesayuno.IsChecked));
                            cmd.Parameters.AddWithValue("@IndAlmuerzo", Convert.ToInt32(CHalmuerzo.IsChecked));
                            cmd.Parameters.AddWithValue("@IndComida", Convert.ToInt32(CHcomida.IsChecked));
                            cmd.Parameters.AddWithValue("@IndCena", Convert.ToInt32(CHcena.IsChecked));
                            cmd.Parameters.AddWithValue("@IndLunchEspecial", Convert.ToInt32(CHluch.IsChecked));
                            cmd.Parameters.AddWithValue("@IndManualCliente", Convert.ToInt32(CHmulcli.IsChecked));
                            cmd.Parameters.AddWithValue("@CantidadPermitidas", Convert.ToInt32(UDcntp.Value));
                            cmd.Parameters.AddWithValue("@MultiReservas", Convert.ToInt32(CHmulrese.IsChecked));

                            cmd.Parameters.AddWithValue("@AutorizacionPuntoVenta", Convert.ToInt32(CHservi.IsChecked));
                            cmd.Parameters.AddWithValue("@indVisitante", Convert.ToInt32(CHvisi.IsChecked));
                            cmd.Parameters.AddWithValue("@IndLunch", Convert.ToInt32(CHluch.IsChecked));

                            if (imageSave == true) cmd.Parameters.AddWithValue("@Foto", imgByteArr);

                            cmd.Parameters.AddWithValue("@IndVivienda", Convert.ToInt32(CHvivienda.IsChecked));


                            cmd.Parameters.AddWithValue("@ViviendaCodigo", GridVivienda.IsEnabled == true ?
                                CBVivienda.SelectedValue.ToString().Trim() : "");

                            cmd.Parameters.AddWithValue("@HabitacionCodigo", GridVivienda.IsEnabled == true ?
                                CBHabitacion.SelectedValue.ToString().Trim() : "");

                            cmd.Parameters.AddWithValue("@CamaCodigo", GridVivienda.IsEnabled == true ?
                                CBCama.SelectedValue.ToString().Trim() : "");



                            cmd.Parameters.AddWithValue("@Nota", tx_nota.Text);
                            cmd.Parameters.AddWithValue("@Tarjeta", valrfid);
                            cmd.Parameters.AddWithValue("@Estado", Convert.ToInt32(CHestado.IsChecked));
                            cmd.Parameters.AddWithValue("@IndRefrigerios", Convert.ToInt32(CHrefrigerio.IsChecked));
                            


                            connection.Open();
                            int exe = cmd.ExecuteNonQuery();
                            connection.Close();

                            if (exe > 0)
                            {
                                NotificationOn("se creo el empleado: " + TxDocumento.Text + " exitosamente");
                                desbloquePanel(false);
                                activeSave(false);
                            }
                        }
                    }
                }



            }
            catch (SqlException w)
            {
                MessageBox.Show("error SQL:" + w);
            }
            catch (Exception w)
            {
                MessageBox.Show("error al guardar:" + w);
            }
        }

        private void CHvivienda_Checked(object sender, RoutedEventArgs e)
        {
            bool check = (bool)(sender as CheckBox).IsChecked;

            //GridVivienda.Visibility = check ? Visibility.Visible : Visibility.Hidden;
            GridVivienda.IsEnabled = check ? true : false;
        }

        private void CHvivienda_Unchecked(object sender, RoutedEventArgs e)
        {
            bool check = (bool)(sender as CheckBox).IsChecked;
            //GridVivienda.Visibility = check ? Visibility.Visible : Visibility.Hidden;
            GridVivienda.IsEnabled = check ? true : false;

            CBVivienda.SelectedIndex = -1;
            CBHabitacion.SelectedIndex = -1;
            CBCama.SelectedIndex = -1;
        }

        private void CBpuntov_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (CBpuntov.SelectedIndex < 0)
            {
                CBVivienda.SelectedIndex = -1;
                CBHabitacion.SelectedIndex = -1;
                CBCama.SelectedIndex = -1;
            }
            else
            {
                loadCombo(CBpuntov.SelectedValue.ToString());
                CBVivienda.SelectedIndex = -1;
                CBHabitacion.SelectedIndex = -1;
                CBCama.SelectedIndex = -1;
            }
        }

        public void loadCombo(string pv)
        {
            DataTable dt = SiaWin.Func.SqlDT("select rtrim(viviendaCodigo) as codigo,rtrim(viviendaCodigo)+' ('+rtrim(viviendaNombre)+')' as nombre  from CtMae_Viviendas where PuntoVentaCodigo='" + pv + "' and Estado=1;", "campamento", idemp);
            CBVivienda.ItemsSource = dt.DefaultView;
            CBVivienda.DisplayMemberPath = "nombre";
            CBVivienda.SelectedValuePath = "codigo";
        }

        private void CBVivienda_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (CBVivienda.SelectedIndex < 0) return;
                CBHabitacion.SelectedIndex = -1;
                CBCama.SelectedIndex = -1;

                string vivienda = (sender as ComboBox).SelectedValue.ToString();
                string query = "select  rtrim(HabitacionCodigo) as codigo,rtrim(HabitacionCodigo)+' ('+rtrim(Cnt_Camas)+')' as cnt From CtMae_ViviendasHabitaciones where ViviendaCodigo='" + vivienda + "' and Estado=1";
                DataTable dt = SiaWin.Func.SqlDT(query, "habitacion", idemp);
                CBHabitacion.ItemsSource = dt.DefaultView;
                CBHabitacion.DisplayMemberPath = "cnt";
                CBHabitacion.SelectedValuePath = "codigo";
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar habitaciones:" + w);
            }
        }


        private void CBHabitacion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (CBVivienda.SelectedIndex < 0 || CBHabitacion.SelectedIndex < 0) return;
                CBCama.SelectedIndex = -1;

                string habitacion = (sender as ComboBox).SelectedValue.ToString();
                string vivienda = CBVivienda.SelectedValue.ToString();
                string query = "select CamaCodigo as codigo from CtMae_ViviendaHabitacionCamas where HabitacionCodigo='" + habitacion + "' and ViviendaCodigo='" + vivienda + "' and Estado=1 ";
                DataTable dt = SiaWin.Func.SqlDT(query, "vivienda", idemp);
                CBCama.ItemsSource = dt.DefaultView;
                CBCama.DisplayMemberPath = "codigo";
                CBCama.SelectedValuePath = "codigo";
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar habitaciones:" + w);
            }
        }

        private void TxCodigo_LostFocus(object sender, RoutedEventArgs e)
        {
            string valor = (sender as TextBox).Text.ToString();
            if (string.IsNullOrWhiteSpace(valor)) return;
            DataTable dt = SiaWin.Func.SqlDT("select EmpleadoCodigo From CtMae_Empleados where EmpleadoCodigo='" + valor + "' ", "vivienda", idemp);
            if (dt.Rows.Count > 0)
            {
                bool f = cargarClinte(valor);
                controls(!f);
                activeSave(true, "Modificar");
            }
            else
            {
                activeSave(true, "Guardar");
            }
        }

        private void TxCelular_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab || e.Key == Key.OemMinus || e.Key == Key.Subtract || e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 || e.Key == Key.Back || e.Key == Key.Left || e.Key == Key.Right)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void tx_nota_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void BtnNotas_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dynamic ww = SiaWin.WindowExt(10700, "NotasEmpleados");
                ww.ShowInTaskbar = false;
                ww.Owner = Application.Current.MainWindow;
                ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                ww.cod_empleado = TxCodigo.Text;
                ww.ShowDialog();
            }
            catch (Exception w)
            {
                MessageBox.Show("errro al abrir notas del empleado:" + w);
            }
        }

        private void BtnRFID_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RFID xx = new RFID();
                xx.ShowInTaskbar = false;
                if (saveRIFD)
                {
                    xx.saveCheck = true;
                    xx.rfidvalu = valrfid;
                }                
                xx.Owner = Application.Current.MainWindow;
                xx.ShowDialog();

                if (xx.saveCheck)
                {
                    saveRIFD = true;
                    valrfid = xx.rfidvalu;
                    BtnRFID.Content = "RFID (✔)";
                }
                else
                {
                    saveRIFD = false;
                    valrfid = "";
                    BtnRFID.Content = "RFID (✖)";
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir RFID:" + w);
            }
        }

        private void BtnFoto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileDialog fldlg = new OpenFileDialog();
                fldlg.InitialDirectory = Environment.SpecialFolder.MyPictures.ToString();
                fldlg.Filter = "Image File (*.jpg;*.bmp;*.gif;*.png)|*.jpg;*.bmp;*.gif;*.png";
                fldlg.ShowDialog();
                {
                    strName = fldlg.SafeFileName;
                    imageName = fldlg.FileName;
                    ImageSourceConverter isc = new ImageSourceConverter();
                    image1.SetValue(System.Windows.Controls.Image.SourceProperty, isc.ConvertFromString(imageName));
                    imageSave = true;
                }
                fldlg = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }




    }
}

