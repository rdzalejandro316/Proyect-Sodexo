using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
    /// <summary>
    /// Lógica de interacción para Asignacion.xaml
    /// </summary>
    public partial class Asignacion : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public string fecha = "";
        public string pv = "";
        public string campamento = "";
        //si enviaen parametros de camapamento,vivienda,habitacion,cama
        public string vivienda = "";
        public string habitacion = "";
        public string cama = "";

        //bandera para saber si inserto una reserva
        public bool inserto = false;
        public string concept = "";
        public List<string> fechasList = new List<string>();

        public Asignacion()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;            
            //dbAcceso = SiaWin.Func.DB_Acceso;
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
                this.Title = "Reserva " + cod_empresa + "-" + nomempresa;
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
                DateTime d = Convert.ToDateTime(fecha);

                TX_fecini.Text = d.ToString("dd/MM/yyyy");
                TX_fecsali.Text = d.AddDays(2).ToString("dd/MM/yyyy");
                TX_horaing.Text = DateTime.Now.ToString("hh:mm:ss");
                TX_horsal.Text = DateTime.Now.ToString("hh:mm:ss");

                SiaWin = Application.Current.MainWindow;
                idemp = SiaWin._BusinessId;

                string where = string.IsNullOrEmpty(campamento) ? " " : " where CampamentoCodigo = '" + campamento + "'";

                string query = "select rtrim(CampamentoCodigo) as codigo,rtrim(CampamentoCodigo)+'-'+RTRIM(CampamentoNombre) as nombre FROM CtMae_Campamentos " + where + ";";


                DataTable dtcampos = SiaWin.Func.SqlDT(query, "campos", idemp);
                Cbx_campa.ItemsSource = dtcampos.DefaultView;
                if (dtcampos.Rows.Count > 0)
                {
                    Cbx_campa.SelectedIndex = 0;
                    Cbx_campa.IsEnabled = string.IsNullOrEmpty(campamento) ? true : false;
                }

                DataTable dtconceptos = SiaWin.Func.SqlDT("select rtrim(EstadoCamaCodigo) as codigo,rtrim(EstadoCamaNombre)+' ('+rtrim(Alias)+')' as nombre From CtMae_ViviendaHabitacionCamaEstado;", "campos", idemp);
                Cbx_Concepto.ItemsSource = dtconceptos.DefaultView;
            }
            catch (Exception w)
            {
                MessageBox.Show("error:" + w);
            }
        }
        private void Cbx_campa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (Cbx_campa.SelectedIndex >= 0)
                {
                    Cbx_vivienda.ItemsSource = null;
                    Cbx_habitacion.ItemsSource = null;

                    string where = string.IsNullOrWhiteSpace(vivienda) ? " and PuntoVentaCodigo = '" + pv + "' " : "and ViviendaCodigo='" + vivienda + "' ";

                    string query = "select rtrim(ViviendaCodigo) as codigo,rtrim(ViviendaCodigo)+'-'+rtrim(ViviendaNombre) as nombre from CtMae_Viviendas where Estado=1  " + where + "  ";
                    
                    DataTable dtcampos = SiaWin.Func.SqlDT(query, "campos", idemp);
                    Cbx_vivienda.ItemsSource = dtcampos.DefaultView;                    
                    
                    if (string.IsNullOrWhiteSpace(vivienda))
                    {
                        Cbx_vivienda.SelectedIndex = -1;
                        Cbx_habitacion.SelectedIndex = -1;
                        Cbx_cama.SelectedIndex = -1;
                    }
                    else
                    {
                        Cbx_vivienda.SelectedIndex = 0;
                        Cbx_vivienda.IsEnabled = false;
                    }
                    
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
                    string where = string.IsNullOrWhiteSpace(habitacion) ? " and ViviendaCodigo='" + Cbx_vivienda.SelectedValue.ToString().Trim() + "'  " : " and HabitacionCodigo = '" + habitacion + "' and ViviendaCodigo='" + vivienda + "' ";

                    string query = "select rtrim(HabitacionCodigo) as codigo,rtrim(ViviendaCodigo)+'('+rtrim(HabitacionCodigo)+')' as nombre from CtMae_ViviendasHabitaciones where Estado=1  " + where + "  ";
                    DataTable dtcampos = SiaWin.Func.SqlDT(query, "campos", idemp);
                    Cbx_habitacion.ItemsSource = dtcampos.DefaultView;

                    if (string.IsNullOrWhiteSpace(habitacion))
                    {
                        Cbx_habitacion.SelectedIndex = -1;
                        Cbx_cama.SelectedIndex = -1;
                    }
                    else
                    {
                        Cbx_habitacion.SelectedIndex = 0;
                        Cbx_habitacion.IsEnabled = false;
                    }
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
                    string where = string.IsNullOrWhiteSpace(cama) ?
                        " and ViviendaCodigo='" + Cbx_vivienda.SelectedValue.ToString() + "' and HabitacionCodigo='" + Cbx_habitacion.SelectedValue.ToString() + "' "
                        : " and ViviendaCodigo='" + Cbx_vivienda.SelectedValue.ToString() + "' and HabitacionCodigo='" + Cbx_habitacion.SelectedValue.ToString() + "' and CamaCodigo='" + cama + "'  ";

                    string query = "select rtrim(CamaCodigo) as codigo,rtrim(CamaCodigo) as nombre From CtMae_ViviendaHabitacionCamas where estado=1  " + where + " ";
                    DataTable dtcampos = SiaWin.Func.SqlDT(query, "campos", idemp);
                    Cbx_cama.ItemsSource = dtcampos.DefaultView;

                    if (string.IsNullOrWhiteSpace(cama))
                    {
                        Cbx_cama.SelectedIndex = -1;
                    }
                    else
                    {
                        Cbx_cama.SelectedIndex = 0;
                        Cbx_cama.IsEnabled = false;
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("ERROR AL SELCCIONAR HABITACION:" + w);
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
                    case "CtMae_Empleados":
                        cmptabla = tag; cmpcodigo = "EmpleadoCodigo"; cmpnombre = "EmpleadoNombres"; cmporden = "EmpleadoCodigo"; cmpidrow = "EmpleadoCodigo"; cmptitulo = "Empleados"; cmpconexion = cnEmp; mostrartodo = false; cmpwhere = "";
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
                    if (tag == "CtMae_Empleados")
                    {
                        Txempleado.Text = code.ToString().Trim();
                        cargarInfoEmp(code.ToString().Trim());
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al buscar:" + w);
            }
        }
        public void cargarInfoEmp(string code)
        {
            string query = "select CtMae_Empleados.EmpresaCodigo,CtMae_Empresas.CCostoCodigo ";
            query += "From CtMae_Empleados  ";
            query += "inner join CtMae_Empresas on CtMae_Empleados.EmpresaCodigo = CtMae_Empresas.EmpresaCodigo  ";
            query += "where EmpleadoCodigo='" + code + "' ";

            DataTable dt = SiaWin.Func.SqlDT(query, "empleados", idemp);
            if (dt.Rows.Count > 0)
            {
                Txcco.Text = dt.Rows[0]["CCostoCodigo"].ToString().Trim();
                Txempresa.Text = dt.Rows[0]["EmpresaCodigo"].ToString().Trim();
            }
        }

        public void fechas() 
        {
            DateTime fecha_inicio = Convert.ToDateTime(TX_fecini.ToString());
            DateTime fecha_fin = Convert.ToDateTime(TX_fecsali.ToString());

            //for (DateTime fecha = fecha_inicio; fecha <= fecha_fin; fecha = fecha.AddDays(1))
            for (DateTime fecha = fecha_inicio; fecha < fecha_fin; fecha = fecha.AddDays(1))
            {
                fechasList.Add(fecha.ToString("yyyy-MM-dd"));
            }
        }


        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {



                #region valdidaciones

                if (Cbx_campa.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione un campamento", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (Cbx_vivienda.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione un vivienda", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (Cbx_habitacion.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione una habitacion", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (Cbx_cama.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione una cama", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (string.IsNullOrWhiteSpace(Txempleado.Text))
                {
                    MessageBox.Show("seleccione una empleado", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (string.IsNullOrWhiteSpace(Txcco.Text))
                {
                    MessageBox.Show("el empleado no tiene centro de costo", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (string.IsNullOrWhiteSpace(Txempresa.Text))
                {
                    MessageBox.Show("el empleado no tiene centro de costo", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (Cbx_Concepto.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione una concepto", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (string.IsNullOrWhiteSpace(TX_fecini.Text))
                {
                    MessageBox.Show("seleccione una fecha de ingreso", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (string.IsNullOrWhiteSpace(TX_fecsali.Text))
                {
                    MessageBox.Show("seleccione una fecha de salida", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                #endregion

                DateTime fechaing = new DateTime(
                                 Convert.ToDateTime(TX_fecini.Text).Year,
                                 Convert.ToDateTime(TX_fecini.Text).Month,
                                 Convert.ToDateTime(TX_fecini.Text).Day,
                                 Convert.ToDateTime(TX_horaing.Text).Hour,
                                 Convert.ToDateTime(TX_horaing.Text).Minute,
                                 Convert.ToDateTime(TX_horaing.Text).Second
                                 );


                DateTime fechasalida = new DateTime(
                                Convert.ToDateTime(TX_fecsali.Text).Year,
                                Convert.ToDateTime(TX_fecsali.Text).Month,
                                Convert.ToDateTime(TX_fecsali.Text).Day,
                                Convert.ToDateTime(TX_horsal.Text).Hour,
                                Convert.ToDateTime(TX_horsal.Text).Minute,
                                Convert.ToDateTime(TX_horsal.Text).Second
                                );


                using (SqlConnection connection = new SqlConnection(SiaWin.Func.DatosEmp(idemp)))
                {
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = "insert into CtDoc_Viviendas (ViviendaCodigo,HabitacionCodigo,CamaCodigo,EstadoCamaCodigo,EmpleadoCodigo,EmpresaCodigo,CCostoCodigo,FechaRegistroIngreso,FechaIngreso,FechaRegistroSalida,FechaSalida,IndVisitante,TipoDeRegistro,IndContingencia,Nota,cod_usu) values(@ViviendaCodigo,@HabitacionCodigo,@CamaCodigo,@EstadoCamaCodigo,@EmpleadoCodigo,@EmpresaCodigo,@CCostoCodigo,@FechaRegistroIngreso,@FechaIngreso,@FechaRegistroSalida,@FechaSalida,@IndVisitante,@TipoDeRegistro,@IndContingencia,@Nota,@cod_usu);";

                        cmd.Parameters.AddWithValue("@ViviendaCodigo", Cbx_vivienda.SelectedValue.ToString());
                        cmd.Parameters.AddWithValue("@HabitacionCodigo", Cbx_habitacion.SelectedValue.ToString());
                        cmd.Parameters.AddWithValue("@CamaCodigo", Cbx_cama.SelectedValue.ToString());
                        cmd.Parameters.AddWithValue("@EstadoCamaCodigo", Cbx_Concepto.SelectedValue.ToString());
                        cmd.Parameters.AddWithValue("@EmpleadoCodigo", Txempleado.Text);
                        cmd.Parameters.AddWithValue("@EmpresaCodigo", Txempresa.Text);
                        cmd.Parameters.AddWithValue("@CCostoCodigo", Txcco.Text);

                        cmd.Parameters.AddWithValue("@FechaRegistroIngreso", fechaing);
                        cmd.Parameters.AddWithValue("@FechaIngreso", fechaing.ToString("dd/MM/yyy"));
                        cmd.Parameters.AddWithValue("@FechaRegistroSalida", fechasalida);
                        cmd.Parameters.AddWithValue("@FechaSalida", fechasalida.ToString("dd/MM/yyyy"));
                        cmd.Parameters.AddWithValue("@IndVisitante", 0);
                        cmd.Parameters.AddWithValue("@TipoDeRegistro", 0);
                        cmd.Parameters.AddWithValue("@IndContingencia", 0);
                        cmd.Parameters.AddWithValue("@Nota", TxNota.Text);
                        cmd.Parameters.AddWithValue("@cod_usu", "");

                        connection.Open();
                        int exe = cmd.ExecuteNonQuery();
                        connection.Close();

                        if (exe > 0)
                        {
                            //NotificationOn("se creo el empleado: " + TxDocumento.Text + " exitosamente");
                            //desbloquePanel(false);
                            //activeSave(false);
                            MessageBox.Show("asignacion exitosa");
                            inserto = true;
                            concept = CodigoCon(Cbx_Concepto.SelectedValue.ToString().Trim());
                            fechas();
                            this.Close();
                        }
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al guardar:" + w);
            }
        }

        public string CodigoCon(string code)
        {            
            DataTable dt = SiaWin.Func.SqlDT("select Alias From CtMae_ViviendaHabitacionCamaEstado where EstadoCamaCodigo='"+code+"'", "campos", idemp);
            return dt.Rows.Count > 0 ? dt.Rows[0]["Alias"].ToString() : "*";
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }




    }
}
