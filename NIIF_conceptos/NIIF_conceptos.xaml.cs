using NIIF_conceptos;
using System;
using System.Data;
using System.Windows;

namespace SiasoftAppExt
{

    //Sia.PublicarPnt(9459, "NIIF_conceptos");   
    public partial class NIIF_conceptos : Window
    {

        dynamic SiaWin;
        int idemp = 0;
        string cnEmp = "";


        public NIIF_conceptos()
        {
            InitializeComponent();

            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            LoadConfig();
            cargarConceptos();
            cargarYear();
        }

        public void cargarYear()
        {
            string cadena = "select BusinessAno from Seg_PeriodoProjectBusiness group by BusinessAno order by BusinessAno";
            DataTable dt = SiaWin.Func.SqlDT(cadena, "años", 0);
            CB_year.ItemsSource = dt.DefaultView;
            CB_year.SelectedIndex = dt.Rows.Count-1;
        }

        private void LoadConfig()
        {
            try
            {
                DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void cargarConceptos()
        {
            try
            {
                string cadena = "select RTRIM(cod_inf) as cod_inf,RTRIM (nom_inf) as nom_inf  from NIinf_niif";
                DataTable dt = SiaWin.Func.SqlDT(cadena, "conceptos", idemp);

                CB_Cod_inf.ItemsSource = dt.DefaultView;
            }
            catch (Exception w)
            {
                MessageBox.Show("errro cargar Grupo Tallas" + w);
            }
        }

        private void CB_Cod_inf_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                string codigo = CB_Cod_inf.SelectedValue.ToString();
                //MessageBox.Show("codigo:"+ codigo);
                LLenarGrid(codigo);
            }
            catch (Exception)
            {
                MessageBox.Show("error al cargar el nombre del concepto");
            }
        }

        public void LLenarGrid(string codigo)
        {
            try
            {
                string cadena = "select concepto.cod_niif as cod_niif,concepto.nom_con as nom_con, ";
                cadena += "concepto.nota as nota,grupo.nom_grupo as nom_grupo,tipo.nom_tipo as nom_tipo, ";
                cadena += "clases.nom_clase as nom_clase ";
                cadena += "from NIcon_niif as concepto ";
                cadena += "inner join NIMae_grupo as grupo on concepto.grupo = grupo.grupo and concepto.cod_inf = grupo.cod_inf ";
                cadena += "inner join NI_tipos as tipo on concepto.tipo = tipo.tipo and concepto.cod_inf = tipo.cod_inf and grupo.grupo = tipo.grupo ";
                cadena += "left join NI_clases as clases on concepto.clase = clases.clase ";
                cadena += "where concepto.cod_inf='" + codigo + "' order by concepto.cod_niif";

                DataTable dt = SiaWin.Func.SqlDT(cadena, "conceptos", idemp);
                dataGridCxC.ItemsSource = dt.DefaultView;
                Total.Text = dt.Rows.Count.ToString();
            }
            catch (Exception) { MessageBox.Show("erro al cargar grid"); }
        }

        private void BTNeliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)dataGridCxC.SelectedItems[0];
                string cod_niif = row["cod_niif"].ToString().Trim();

                MessageBoxResult result = MessageBox.Show("desea eliminar el concepto:" + cod_niif + "?", "Siasoft?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    string delete = "delete nicon_niif where cod_niif='" + cod_niif.ToString() + "'";
                    if (SiaWin.Func.SqlCRUD(delete, idemp) == true)
                    {
                        MessageBox.Show("eliminacion exitosa");
                        string codigo = CB_Cod_inf.SelectedValue.ToString();
                        LLenarGrid(codigo);

                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al eliminar:" + w);
            }
        }

        private void BTNeditar_Click(object sender, RoutedEventArgs e)
        {

            DataRowView row = (DataRowView)dataGridCxC.SelectedItems[0];
            editar ventana = new editar();
            ventana.id_con = row["cod_niif"].ToString();
            ventana.nom_con = row["nom_con"].ToString();
            ventana.añoNota = CB_year.SelectedValue.ToString();
            ventana.TX_nota.Text = row["nota"].ToString();
            ventana.ShowInTaskbar = false;
            ventana.Owner = Application.Current.MainWindow;
            ventana.ShowDialog();

            if (ventana.actualizo == true) actualizarGrid(row, ventana);

        }

        public void actualizarGrid(DataRowView row, editar editar)
        {
            try
            {
                row["nom_con"] = editar.TXnombre.Text;
                //ComboBoxAdv 
                int index = editar.CB_clase.SelectedIndex;
                string nameclase = ((DataRowView)editar.CB_clase.Items[index])["nom_clase"].ToString().Trim();
                row["nom_clase"] = nameclase;
                row["nota"] = editar.TX_nota.Text.Trim();

                dataGridCxC.View.Refresh();
            }
            catch (Exception w)
            {
                MessageBox.Show("error al actualizar:" + w);
            }

        }

        private void BTNcuentas_Click(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)dataGridCxC.SelectedItems[0];
            cuenta ventana = new cuenta();

            ventana.codigo_info = CB_Cod_inf.SelectedValue.ToString();
            string name = ((DataRowView)CB_Cod_inf.Items[CB_Cod_inf.SelectedIndex])["nom_inf"].ToString().Trim();
            ventana.nombre_info = name;

            ventana.codigo_conc = row["cod_niif"].ToString();
            ventana.nombre_conc = row["nom_con"].ToString();

            ventana.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ventana.ShowInTaskbar = false;
            ventana.Owner = Application.Current.MainWindow;
            ventana.ShowDialog();

        }

        private void BTNaddConc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CB_Cod_inf.SelectedIndex >= 0)
                {
                    CreacionConceptos ventana = new CreacionConceptos();
                    string name = ((DataRowView)CB_Cod_inf.Items[CB_Cod_inf.SelectedIndex])["nom_inf"].ToString().Trim();
                    ventana.informeCode = CB_Cod_inf.SelectedValue.ToString();
                    ventana.informeName = name;
                    ventana.ShowInTaskbar = false;
                    ventana.Owner = Application.Current.MainWindow;
                    ventana.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    ventana.ShowDialog();

                    string codigo = CB_Cod_inf.SelectedValue.ToString();
                    LLenarGrid(codigo);

                }
                else
                {
                    MessageBox.Show("Selecione un Tipo de Informe para poder agregar un concepto");
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("Error c# al agragar concpto:" + w);
            }

        }

        private void BtnNotas_Click(object sender, RoutedEventArgs e)
        {
            Notas ventana = new Notas();
            DataRowView row = (DataRowView)dataGridCxC.SelectedItems[0];
            ventana.notaCode = row["nota"].ToString();
            ventana.AñoNota = CB_year.SelectedValue.ToString();
            ventana.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ventana.ShowInTaskbar = false;
            ventana.Owner = Application.Current.MainWindow;
            ventana.ShowDialog();
        }



    }
}

