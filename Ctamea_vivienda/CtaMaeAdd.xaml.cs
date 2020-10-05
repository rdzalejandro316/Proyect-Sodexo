using MaterialDesignThemes.Wpf;
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
using System.Windows.Threading;

namespace Ctamea_vivienda
{
    /// <summary>
    /// Lógica de interacción para CtaMaeAdd.xaml
    /// </summary>
    public partial class CtaMaeAdd : Window
    {

        dynamic SiaWin;
        int idemp = 0;
        string cnEmp = "";
        public string punto_v = "";

        public bool flag = false;
        public string codeVivi = "";
        public string codeEmpre = "";


        //notificacion de insercion o actualizacion
        public bool act_int = false;

        DispatcherTimer disp = new DispatcherTimer();

        DataTable dtautomatic;
        bool habcam_automatic = false;

        public CtaMaeAdd()
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
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                Title = "Creacion de Vivienda (" + aliasemp + ")";
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


        private void LoadAutomatic()
        {
            try
            {
                dtautomatic = SiaWin.Func.SqlDT("select inicialHab,aumentoHab,estadoHab,inicialCam,aumentoCam,estadoCam From CtMae_ViviendasConfig", "Proyectos", idemp);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (flag == true)
            {
                cargarDatos(codeVivi, punto_v, codeEmpre);
            }
            else
            {
                LoadAutomatic();
            }
        }

        public void cargarDatos(string cod_viv, string cod_pv, string cod_emp)
        {
            try
            {
                string query = "select ViviendaCodigo,ViviendaNombre,CtMae_Viviendas.TipoViviendaCodigo, ";
                query += "CtMae_TipoVivienda.TipoViviendaNombre,Calificacion,CtMae_PuntosDeVenta.PuntoVentaCodigo, ";
                query += "CtMae_PuntosDeVenta.PuntoVentaNombre,CtMae_Viviendas.EmpresaCodigo,CtMae_Empresas.EmpresaNombre, ";
                query += "Cnt_Habitaciones,Cnt_Camas,CtMae_Viviendas.Estado,CtMae_Viviendas.Reservada,CtMae_Viviendas.Nota ";
                query += "From CtMae_Viviendas ";
                query += "inner join CtMae_TipoVivienda on CtMae_Viviendas.TipoViviendaCodigo = CtMae_TipoVivienda.TipoViviendaCodigo ";
                query += "inner join CtMae_PuntosDeVenta on CtMae_PuntosDeVenta.PuntoVentaCodigo = CtMae_Viviendas.PuntoVentaCodigo ";
                query += "inner join CtMae_Empresas on CtMae_Viviendas.EmpresaCodigo = CtMae_Empresas.EmpresaCodigo ";
                query += "where CtMae_Viviendas.ViviendaCodigo='" + cod_viv + "' and  CtMae_Viviendas.PuntoVentaCodigo='" + cod_pv + "' and CtMae_Viviendas.EmpresaCodigo='" + cod_emp + "'";



                DataTable dt = SiaWin.Func.SqlDT(query, "Proyectos", idemp);

                bool b; int i;
                if (dt.Rows.Count > 0)
                {
                    //SiaWin.Browse(dt);
                    BtnGuardar.Content = "Modificar";

                    TxCodigo.Text = dt.Rows[0]["ViviendaCodigo"].ToString().Trim();
                    TxNombre.Text = dt.Rows[0]["ViviendaNombre"].ToString().Trim();

                    TxTipo.Text = dt.Rows[0]["TipoViviendaNombre"].ToString().Trim();
                    TxTipo.Tag = dt.Rows[0]["TipoViviendaCodigo"].ToString().Trim();

                    TxEmpresa.Text = dt.Rows[0]["EmpresaNombre"].ToString().Trim();
                    TxEmpresa.Tag = dt.Rows[0]["EmpresaCodigo"].ToString().Trim();


                    CheckEstado.IsChecked = Convert.ToBoolean(dt.Rows[0]["Estado"] == DBNull.Value || bool.TryParse(dt.Rows[0]["Estado"].ToString(), out b) == false ?
                        false : dt.Rows[0]["Estado"]);

                    CheckReserva.IsChecked =
                        Convert.ToBoolean(dt.Rows[0]["Reservada"] == DBNull.Value || bool.TryParse(dt.Rows[0]["Reservada"].ToString(), out b) == false ?
                        false : dt.Rows[0]["Reservada"]);


                    UpHabitacion.Value =
                        Convert.ToInt32(dt.Rows[0]["Cnt_Habitaciones"] == DBNull.Value || Int32.TryParse(dt.Rows[0]["Cnt_Habitaciones"].ToString(), out i) == false ?
                        0 : dt.Rows[0]["Cnt_Habitaciones"]);

                    UpCamas.Value =
                        Convert.ToInt32(dt.Rows[0]["Cnt_Camas"] == DBNull.Value || Int32.TryParse(dt.Rows[0]["Cnt_Camas"].ToString(), out i) == false ?
                        0 : dt.Rows[0]["Cnt_Camas"]);

                    BasicRatingBar.Value = calificacion(0, dt.Rows[0]["Calificacion"].ToString().Trim()).Item2;

                    TXnota.Text = dt.Rows[0]["Nota"].ToString().Trim();

                }
            }
            catch (Exception w)
            {
                MessageBox.Show("errro al cargar:" + w);
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

        private void BTNsearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                string tag = (sender as Button).Tag.ToString();
                string cmptabla = ""; string cmpcodigo = ""; string cmpnombre = ""; string cmporden = ""; string cmpidrow = ""; string cmptitulo = ""; string cmpconexion = ""; bool mostrartodo = true; string cmpwhere = "";

                switch (tag)
                {
                    case "CtMae_TipoVivienda":
                        cmptabla = tag; cmpcodigo = "TipoViviendaCodigo"; cmpnombre = "TipoViviendaNombre"; cmporden = "TipoViviendaCodigo"; cmpidrow = "idrow"; cmptitulo = "Tipo de vivienda"; cmpconexion = cnEmp; mostrartodo = true; cmpwhere = "";
                        break;
                    case "CtMae_Empresas":
                        cmptabla = tag; cmpcodigo = "EmpresaCodigo"; cmpnombre = "EmpresaNombre"; cmporden = "EmpresaCodigo"; cmpidrow = "EmpresaId"; cmptitulo = "Empresas"; cmpconexion = cnEmp; mostrartodo = true; cmpwhere = "";
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
                    if (tag == "CtMae_TipoVivienda")
                    {
                        TxTipo.Text = nom.Trim();
                        TxTipo.Tag = code.ToString().Trim();
                    }
                    if (tag == "CtMae_Empresas")
                    {
                        TxEmpresa.Text = nom.Trim();
                        TxEmpresa.Tag = code.ToString().Trim();
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al buscar:" + w);
            }
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (flag == true)
            {
                save();
            }
            else
            {
                Dialogo("¿USTED DESEA CREAR LAS HABITACIONES Y CAMAS?", 3);
            }
        }


        public void save()
        {
            try
            {
                #region valiadaciones

                if (string.IsNullOrEmpty(TxCodigo.Text))
                {
                    NotificationOn("debe de ingresar el codigo de la vivienda");
                    return;
                }

                if (string.IsNullOrEmpty(TxNombre.Text))
                {
                    NotificationOn("debe de ingresar el nombre de la vivienda");
                    return;
                }

                if (string.IsNullOrEmpty(TxTipo.Text))
                {
                    NotificationOn("debe de ingresar el tipo de vivienda");
                    return;
                }

                if (string.IsNullOrEmpty(TxEmpresa.Text))
                {
                    NotificationOn("debe de ingresar la empresa");
                    return;
                }


                #endregion


                string query = "";

                if (flag == true)
                {
                    query += "update CtMae_Viviendas set ViviendaCodigo='" + TxCodigo.Text + "',ViviendaNombre='" + TxNombre.Text + "',TipoViviendaCodigo='" + TxTipo.Tag + "',Calificacion='" + calificacion(BasicRatingBar.Value, "").Item1 + "',PuntoVentaCodigo='" + punto_v + "',EmpresaCodigo='" + TxEmpresa.Tag + "',Cnt_Habitaciones=" + UpHabitacion.Value + ",Cnt_Camas=" + UpCamas.Value + ",Estado=" + Convert.ToInt32(CheckEstado.IsChecked) + ",Reservada=" + Convert.ToInt32(CheckReserva.IsChecked) + ",Nota='" + TXnota.Text + "' where  ViviendaCodigo='" + codeVivi + "' and  PuntoVentaCodigo='" + punto_v + "' and EmpresaCodigo='" + codeEmpre + "'  ";
                }
                else
                {
                    query += "insert into CtMae_Viviendas (ViviendaCodigo,ViviendaNombre,TipoViviendaCodigo,Calificacion,PuntoVentaCodigo,EmpresaCodigo,Cnt_Habitaciones,Cnt_Camas,Estado,Reservada,Nota) values ('" + TxCodigo.Text + "','" + TxNombre.Text + "','" + TxTipo.Tag + "','" + calificacion(BasicRatingBar.Value, "").Item1 + "','" + punto_v + "','" + TxEmpresa.Tag + "'," + UpHabitacion.Value + "," + UpCamas.Value + "," + Convert.ToInt32(CheckEstado.IsChecked) + "," + Convert.ToInt32(CheckReserva.IsChecked) + ",'" + TXnota.Text + "') ";
                }

                if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                {
                    int habitaciones = Convert.ToInt32(UpHabitacion.Value);
                    int camas = Convert.ToInt32(UpCamas.Value);


                    if (habitaciones <= camas)
                    {
                        if (habcam_automatic && flag == false)
                        {
                            string queryhab = "";
                            if (dtautomatic.Rows.Count > 0)
                            {
                                string intHab = dtautomatic.Rows[0]["inicialHab"].ToString().Trim();
                                int aum = Convert.ToInt32(dtautomatic.Rows[0]["aumentoHab"]);
                                bool estado = Convert.ToBoolean(dtautomatic.Rows[0]["estadoHab"]);
                                List<Habitacion> listhab = new List<Habitacion>();

                                int camafor = camas / habitaciones;

                                for (int i = 1; i <= habitaciones; i++)
                                {
                                    string codhab = intHab + i.ToString();
                                    listhab.Add(new Habitacion()
                                    {
                                        CodeHabitacion = codhab,
                                        Camas = camafor,
                                    });

                                    queryhab += "insert into CtMae_ViviendasHabitaciones (HabitacionCodigo,ViviendaCodigo,Cnt_Camas,Estado,Reservada,EmpresaCodigo) values ('" + codhab + "','" + TxCodigo.Text + "'," + camafor + "," + Convert.ToInt32(estado) + ",0,'" + TxEmpresa.Tag + "');";
                                }

                                //camas
                                string intCam = dtautomatic.Rows[0]["inicialCam"].ToString().Trim();
                                int aumcama = Convert.ToInt32(dtautomatic.Rows[0]["aumentoCam"]);
                                bool estadocama = Convert.ToBoolean(dtautomatic.Rows[0]["estadoCam"]);

                                if (!string.IsNullOrEmpty(queryhab))
                                {
                                    foreach (Habitacion item in listhab)
                                    {
                                        string habcode = item.CodeHabitacion;
                                        int clascam = item.Camas;
                                        for (int i = 1; i <= clascam; i++)
                                        {
                                            queryhab += "insert into CtMae_ViviendaHabitacionCamas (ViviendaCodigo,HabitacionCodigo,CamaCodigo,Estado) values ('" + TxCodigo.Text + "','" + habcode + "','" + i.ToString() + "'," + Convert.ToInt32(estadocama) + ") ;";
                                        }
                                    }
                                }
                            }

                            if (SiaWin.Func.SqlCRUD(queryhab, idemp) == true) NotificationOn("creacion automatica exitosa");
                        }
                    }
                    else
                    {
                        NotificationOn("la creacion automatica no se genero por que hay menos camas que habitaciones");
                    }

                    string messa = flag == true ? "modificacion exitosa" : "creacion de vivienda exitoso";
                    Dialogo(messa, 1);
                    clean();
                    act_int = true;
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al guardar:" + w);
            }
        }

        public void clean()
        {
            TxCodigo.Text = "";
            TxNombre.Text = "";
            TxTipo.Text = "";
            TxEmpresa.Text = "";
            UpHabitacion.Value = 1;
            UpHabitacion.Value = 1;
            UpCamas.Value = 1;
            BasicRatingBar.Value = 3;
            TXnota.Text = "";
            habcam_automatic = false;
        }

        public void Dialogo(string Message, int tipo)
        {
            titleDialog.Text = Message;
            switch (tipo)
            {
                case 1:
                    DialoPanel1.Visibility = Visibility.Visible;
                    DialoPanel2.Visibility = Visibility.Collapsed;
                    DialoPanel3.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    DialoPanel2.Visibility = Visibility.Visible;
                    DialoPanel1.Visibility = Visibility.Collapsed;
                    DialoPanel3.Visibility = Visibility.Collapsed;
                    break;
                case 3:
                    DialoPanel3.Visibility = Visibility.Visible;
                    DialoPanel1.Visibility = Visibility.Collapsed;
                    DialoPanel2.Visibility = Visibility.Collapsed;
                    break;
            }
            dialogo.IsOpen = true;
        }


        public Tuple<string, int> calificacion(int num, string califi)
        {
            string cal = ""; int c = 0;

            if (string.IsNullOrEmpty(califi))
            {
                switch (num)
                {
                    case 1: cal = "A"; break;
                    case 2: cal = "B"; break;
                    case 3: cal = "C"; break;
                    case 4: cal = "D"; break;
                    case 5: cal = "E"; break;
                }
            }
            else
            {
                switch (califi)
                {
                    case "A": c = 1; break;
                    case "B": c = 2; break;
                    case "C": c = 3; break;
                    case "D": c = 4; break;
                    case "E": c = 5; break;
                }
            }

            return new Tuple<string, int>(cal, c);
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Dialogo("¿Usted desea salir?", 2);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnAutomatic_Click(object sender, RoutedEventArgs e)
        {
            dialogo.IsOpen = false;
            habcam_automatic = true;
            save();
        }

        private void BtnManual_Click(object sender, RoutedEventArgs e)
        {
            dialogo.IsOpen = false;
            habcam_automatic = false;
            save();
        }


    }

    public class Habitacion
    {
        public string CodeHabitacion { get; set; }
        public int Camas { get; set; }
    }



}
