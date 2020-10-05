using HospedajePreCheckIn.Converter;
using Newtonsoft.Json;
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

namespace HospedajePreCheckIn
{
    public partial class Configuracion : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        DataTable dt;
        DataTable dtconc;

        public bool actualizo = false;
        public Configuracion()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            idemp = SiaWin._BusinessId; ;
            LoadConfig();
            load();
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
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        public void load()
        {
            try
            {

                dt = SiaWin.Func.SqlDT("select * from CtMae_ConfigMapaVivienda", "temporal", idemp);

                if (dt.Rows.Count > 0)
                {
                    string cadena = dt.Rows[0]["campos"].ToString();
                    DataTable dtjson = JsonConvert.DeserializeObject<DataTable>(cadena);

                    foreach (DataRow item in dtjson.Rows)
                    {
                        ComboBoxItem c = new ComboBoxItem();
                        c.Content = item["contenido"].ToString().Trim();
                        c.Tag = new TagList()
                        {
                            color = item["color"].ToString().Trim(),
                            fondo = item["fondo"].ToString().Trim()
                        };
                        CBItems.Items.Add(c);
                    }


                    CHgridMes.IsChecked = Convert.ToBoolean(dt.Rows[0]["StackedHeaderRow"]);
                    CHtotale.IsChecked = Convert.ToBoolean(dt.Rows[0]["GridTotales"]);
                }

                dtconc = SiaWin.Func.SqlDT("select * From CtMae_ViviendaHabitacionCamaEstado", "temporal", idemp);
                if (dtconc.Rows.Count > 0)
                {

                    foreach (DataRow item in dtconc.Rows)
                    {
                        ComboBoxItem c = new ComboBoxItem();
                        c.Content = item["EstadoCamaNombre"].ToString().Trim();
                        c.Tag = new TagListConcepto()
                        {
                            color = item["color_esthab"].ToString().Trim(),
                            siglas = item["Alias"].ToString().Trim()
                        };
                        CBConcept.Items.Add(c);
                    }
                }


            }
            catch (Exception w)
            {

                MessageBox.Show("error al cargar:" + w);
            }
        }

        #region totales


        private void CBItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                if (CBItems.SelectedIndex >= 0)
                {
                    txtotal.Text = ((ComboBoxItem)CBItems.SelectedItem).Content.ToString();

                    TagList tag = (TagList)((ComboBoxItem)CBItems.SelectedItem).Tag;
                    string color = dt.Rows[0][tag.color].ToString();
                    string fondo = dt.Rows[0][tag.fondo].ToString();

                    colorPickerLetra.Color = (Color)ColorConverter.ConvertFromString(color);
                    colorPickerFondo.Color = (Color)ColorConverter.ConvertFromString(fondo);

                    TxColor.Foreground = Config.ToSolidColorBrush(color);
                    GridFondo.Background = Config.ToSolidColorBrush(fondo);
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error en la seleccion:" + w);
            }
        }

        private void colorPickerFondo_SelectedBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GridFondo.Background = Config.ToSolidColorBrush(colorPickerFondo.Color.ToString());
            TagList tag = (TagList)((ComboBoxItem)CBItems.SelectedItem).Tag;
            dt.Rows[0][tag.fondo] = colorPickerFondo.Color.ToString();
        }

        private void colorPickerLetra_SelectedBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TxColor.Foreground = Config.ToSolidColorBrush(colorPickerLetra.Color.ToString());
            TagList tag = (TagList)((ComboBoxItem)CBItems.SelectedItem).Tag;
            dt.Rows[0][tag.color] = colorPickerLetra.Color.ToString();
        }

        private void CH_Checked(object sender, RoutedEventArgs e)
        {
            dt.Rows[0][(sender as CheckBox).Tag.ToString()] = (sender as CheckBox).IsChecked;
        }

        private void CH_Unchecked(object sender, RoutedEventArgs e)
        {
            dt.Rows[0][(sender as CheckBox).Tag.ToString()] = (sender as CheckBox).IsChecked;
        }

        #endregion

        private void CBConcept_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (CBConcept.SelectedIndex >= 0)
                {
                    TxConceptTit.Text = ((ComboBoxItem)CBConcept.SelectedItem).Content.ToString();

                    TagListConcepto tag = (TagListConcepto)((ComboBoxItem)CBConcept.SelectedItem).Tag;
                    string color = dtconc.Rows[0]["color_esthab"].ToString();
                    string siglas = dtconc.Rows[0]["Alias"].ToString();

                    colorPickerConcep.Color = (Color)ColorConverter.ConvertFromString(tag.color);
                    GridConColor.Background = Config.ToSolidColorBrush(tag.color);
                    TxSiglas.Text = tag.siglas;
                    TxConcepto.Text = tag.siglas;
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error en la seleccion:" + w);
            }
        }

        private void colorPickerConcep_SelectedBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                string valor = ((ComboBoxItem)CBConcept.SelectedItem).Content.ToString().Trim();
                foreach (DataRow item in dtconc.Rows)
                {
                    if (valor == item["EstadoCamaNombre"].ToString().Trim())
                    {
                        item["color_esthab"] = colorPickerConcep.Color.ToString();
                        GridConColor.Background = Config.ToSolidColorBrush(colorPickerConcep.Color.ToString());
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error en la seleccion de color:" + w);
            }
        }

        private void TxSiglas_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string valor = ((ComboBoxItem)CBConcept.SelectedItem).Content.ToString().Trim();
                foreach (DataRow item in dtconc.Rows)
                {
                    if (valor == item["EstadoCamaNombre"].ToString().Trim())
                    {
                        item["Alias"] = TxSiglas.Text;
                        TxConcepto.Text = TxSiglas.Text;
                    }

                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error:" + w);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string campos = "";
                foreach (DataRow item in dt.Rows)
                {
                    foreach (DataColumn column in dt.Columns)
                    {
                        if (column.ColumnName != "idrow")
                        {
                            dynamic valor = item[column.ColumnName].ToString().Trim();
                            switch (System.Type.GetTypeCode(column.DataType))
                            {
                                case TypeCode.Boolean:
                                    valor = Convert.ToInt32(item[column.ColumnName]);
                                    break;
                            }
                            campos += column.ColumnName + "='" + valor + "',";
                        }
                    }
                }

                string col = campos.Remove(campos.Length - 1);

                string updatecon = "";
                foreach (DataRow dr in dtconc.Rows)
                    updatecon += "update CtMae_ViviendaHabitacionCamaEstado set color_esthab='" + dr["color_esthab"] + "',Alias='" + dr["Alias"] + "'  where EstadoCamaCodigo='" + dr["EstadoCamaCodigo"] + "';";

                string query = "update CtMae_ConfigMapaVivienda set " + col+";"+ updatecon;


                if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                {
                    MessageBox.Show("actualzacion exitosa", "alerta", MessageBoxButton.OK, MessageBoxImage.Information);
                    actualizo = true;
                }
                               
            }
            catch (Exception w)
            {
                MessageBox.Show("error al actualizar:" + w);
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {         
            this.Close();
        }


    }

    class TagList
    {
        public string color { get; set; }
        public string fondo { get; set; }
    }


    class TagListConcepto
    {
        public string color { get; set; }
        public string siglas { get; set; }
    }

}
