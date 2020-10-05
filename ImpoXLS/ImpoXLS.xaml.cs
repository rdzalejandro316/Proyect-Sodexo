using Microsoft.Win32;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
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

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9549,"ImpoXLS");
    //Sia.TabU(9549);
    public partial class ImpoXLS : UserControl
    {
        string ruta;
        bool ez;
        public System.Data.DataTable tablaXLS = new System.Data.DataTable();

        public System.Data.DataSet ds_errores = new System.Data.DataSet();
        public System.Data.DataSet ds_docs = new System.Data.DataSet();

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        dynamic tabitem;
        public long credito = 0, debito = 0;
        public int val1 = 0, val2 = 0;
        public ImpoXLS(dynamic tabitem1)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            tabitem = tabitem1;
        }
        public void LoadConfig()
        {
            try
            {
                DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
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
        public void Validaciones(DataTable dt)
        {
            try
            {
                ds_docs.Tables.Clear();
                ds_errores.Tables.Clear();//Limpieza de la tabla ERRORES

                //SiaWin.Browse(dt);

                DataView dv = dt.DefaultView;
                dv.Sort = "COD_TRN,NUM_TRN asc";
                DataTable sortedDT = dv.ToTable();
                //SiaWin.Browse(sortedDT);
                string val_ant_doc_trn = "", val_ant_cod_trn = "";
                //val_ant_fec_trn = "", val_ant_cod_cta = "", val_ant_cod_ter = "", val_ant_des_mov = "", val_ant_doc_mov = "", val_ant_bas_mov = "",
                //val_ant_deb_mov = "", val_ant_cre_mov = "", val_ant_doc_cruc = "", val_ant_ord_pag = "", val_ant_cod_banc = "", val_ant_fec_venc = "", val_ant_reg = "", val_ant_num_chq = "",
                //val_ant_factura = "", val_ant_fec_ven = "", val_ant_cod_ven = "", val_ant_cod_ciu = "", val_ant_cod_suc = "", val_ant_cod_cco = "", val_ant_doc_ref = "", val_ant_fec_susc = "";


                DataTable dd = new DataTable();
                dd.Columns.Add("COD_TRN");
                dd.Columns.Add("NUM_TRN");
                dd.Columns.Add("FEC_TRN");
                dd.Columns.Add("COD_CTA");
                dd.Columns.Add("COD_TER");
                dd.Columns.Add("DES_MOV");
                dd.Columns.Add("DOC_MOV");
                dd.Columns.Add("BAS_MOV");
                dd.Columns.Add("DEB_MOV", typeof(double));
                dd.Columns.Add("CRE_MOV", typeof(double));
                dd.Columns.Add("DOC_CRUC");
                dd.Columns.Add("ORD_PAG");
                dd.Columns.Add("COD_BANC");
                dd.Columns.Add("FEC_VENC");
                dd.Columns.Add("REG");
                dd.Columns.Add("NUM_CHQ");
                dd.Columns.Add("FACTURA");
                dd.Columns.Add("FEC_VEN");
                dd.Columns.Add("COD_VEN");
                dd.Columns.Add("COD_CIU");
                dd.Columns.Add("COD_SUC");
                dd.Columns.Add("COD_CCO");
                dd.Columns.Add("DOC_REF");
                dd.Columns.Add("FEC_SUSC");


                int a = 1;
                int b = 0;

                foreach (DataRow dr in sortedDT.Rows)
                {
                    b++;
                    if (string.IsNullOrEmpty(val_ant_doc_trn) && string.IsNullOrEmpty(val_ant_cod_trn))
                    {
                        val_ant_cod_trn = dr["COD_TRN"].ToString();
                        val_ant_doc_trn = dr["NUM_TRN"].ToString();
                        // val_ant_deb_mov = dr["DEB_MOV"].ToString();
                        // val_ant_cre_mov = dr["CRE_MOV"].ToString();
                        // val_ant_fec_trn = dr["FEC_TRN"].ToString();
                        // val_ant_cod_cta = dr["COD_CTA"].ToString();
                        // val_ant_cod_ter = dr["COD_TER"].ToString();
                        // val_ant_des_mov = dr["DES_MOV"].ToString();
                        // val_ant_doc_mov = dr["DOC_MOV"].ToString();
                        // val_ant_bas_mov = dr["BAS_MOV"].ToString();
                        //val_ant_doc_cruc = dr["DOC_CRUC"].ToString();
                        // val_ant_ord_pag = dr["ORD_PAG"].ToString();
                        //val_ant_cod_banc = dr["COD_BANC"].ToString();
                        //val_ant_fec_venc = dr["FEC_VENC"].ToString();
                        //     val_ant_reg = dr["REG"].ToString();
                        // val_ant_num_chq = dr["NUM_CHQ"].ToString();
                        // val_ant_factura = dr["FACTURA"].ToString();
                        // val_ant_fec_ven = dr["FEC_VEN"].ToString();
                        // val_ant_cod_ven = dr["COD_VEN"].ToString();
                        // val_ant_cod_ciu = dr["COD_CIU"].ToString();
                        // val_ant_cod_suc = dr["COD_SUC"].ToString();
                        // val_ant_cod_cco = dr["COD_CCO"].ToString();
                        // val_ant_doc_ref = dr["DOC_REF"].ToString();
                        //val_ant_fec_susc = dr["FEC_SUSC"].ToString();
                    }
                    if (val_ant_cod_trn == dr["COD_TRN"].ToString() && val_ant_doc_trn == dr["NUM_TRN"].ToString())
                    {

                        //dr.IsNull(dr["DEB_MOV"]);

                        double deb = dr["DEB_MOV"] == DBNull.Value ? 0 : Convert.ToDouble(dr["DEB_MOV"]);
                        double cre = dr["CRE_MOV"] == DBNull.Value ? 0 : Convert.ToDouble(dr["CRE_MOV"]);

                        //dd.Rows.Add(dr["COD_TRN"].ToString(), dr["NUM_TRN"].ToString(), deb,cre);

                        dd.Rows.Add(dr["COD_TRN"].ToString(), dr["NUM_TRN"].ToString(), dr["FEC_TRN"].ToString(), dr["COD_CTA"].ToString(),
                            dr["COD_TER"].ToString(), dr["DES_MOV"].ToString(), dr["DOC_MOV"].ToString(), dr["BAS_MOV"].ToString(), deb, cre,
                            dr["DOC_CRUC"].ToString(), dr["ORD_PAG"].ToString(), dr["COD_BANC"].ToString(), dr["FEC_VENC"].ToString(), dr["REG"].ToString(), dr["NUM_CHQ"].ToString(),
                            dr["FACTURA"].ToString(), dr["FEC_VEN"].ToString(), dr["COD_VEN"].ToString(), dr["COD_CIU"].ToString(), dr["COD_SUC"].ToString(), dr["COD_CCO"].ToString(),
                            dr["DOC_REF"].ToString(), dr["FEC_SUSC"].ToString());

                        DataRow lastRow = sortedDT.Rows[sortedDT.Rows.Count - 1];
                        if (b == sortedDT.Rows.Count)
                        {
                            a++;
                            DataTable daa = dd.Copy();
                            daa.TableName = a.ToString();
                            ds_docs.Tables.Add(daa);
                            dd.Clear();
                        }
                    }
                    else
                    {
                        a++;
                        DataTable daa = dd.Copy();
                        daa.TableName = a.ToString();
                        ds_docs.Tables.Add(daa);
                        dd.Clear();

                        double deb = dr["DEB_MOV"] == DBNull.Value ? 0 : Convert.ToDouble(dr["DEB_MOV"]);
                        double cre = dr["CRE_MOV"] == DBNull.Value ? 0 : Convert.ToDouble(dr["CRE_MOV"]);
                        //dd.Rows.Add(dr["COD_TRN"].ToString(), dr["NUM_TRN"].ToString(),deb,cre);
                        dd.Rows.Add(dr["COD_TRN"].ToString(), dr["NUM_TRN"].ToString(), dr["FEC_TRN"].ToString(), dr["COD_CTA"].ToString(),
                            dr["COD_TER"].ToString(), dr["DES_MOV"].ToString(), dr["DOC_MOV"].ToString(), dr["BAS_MOV"].ToString(), deb, cre,
                            dr["DOC_CRUC"].ToString(), dr["ORD_PAG"].ToString(), dr["COD_BANC"].ToString(), dr["FEC_VENC"].ToString(), dr["REG"].ToString(), dr["NUM_CHQ"].ToString(),
                            dr["FACTURA"].ToString(), dr["FEC_VEN"].ToString(), dr["COD_VEN"].ToString(), dr["COD_CIU"].ToString(), dr["COD_SUC"].ToString(), dr["COD_CCO"].ToString(),
                            dr["DOC_REF"].ToString(), dr["FEC_SUSC"].ToString());
                    }

                    val_ant_cod_trn = dr["COD_TRN"].ToString();
                    val_ant_doc_trn = dr["NUM_TRN"].ToString();
                }
                bool validacion = true;
                //int a = 0;
                foreach (DataTable dt_t in ds_docs.Tables)
                {
                    double deb = Convert.ToDouble(dt_t.Compute("Sum(DEB_MOV)", string.Empty));
                    double cre = Convert.ToDouble(dt_t.Compute("Sum(CRE_MOV)", string.Empty));
                    //segunda valicacion encontrar los codigos en la base de datos
                    validacion = valid(dt_t, deb, cre);
                    //segun las validaciones correspondientes se ira a un datase dependiendo si es erronea o no                                                                                                          
                }
                if (validacion == false)
                {
                    MessageBox.Show("Errores en el almacenamiento de los datos");
                    ez = false;
                }
                else
                {
                    ez = true;
                }

                foreach (DataTable de in ds_errores.Tables)
                {

                    DataGrid dt_grid = new DataGrid();
                    dt_grid.ItemsSource = de.DefaultView;

                    gridErrores.Children.Add(dt_grid);
                    //SiaWin.Browse(de);
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error:\n" + w);
            }
        }
        public bool valid(DataTable dt_temp, double deb, double cre)
        {
            DataTable dt_maybe = new DataTable();
            dt_maybe.Columns.Add("error", typeof(string));
            string num_trn = "";
            bool flag = true;
            foreach (DataRow item in dt_temp.Rows)
            {
                if (!string.IsNullOrEmpty(item["cod_ter"].ToString()))//Valida que el tercero exista
                {
                    string select = "select * from  comae_ter where cod_ter='" + item["cod_ter"].ToString() + "'  ";
                    DataTable dt = SiaWin.Func.SqlDT(select, "table", idemp);
                    if (dt.Rows.Count <= 0) flag = false;
                    dt_maybe.Rows.Add("No existe tercero " + item["cod_ter"].ToString());
                    num_trn = item["NUM_TRN"].ToString();
                }
                //else flag = false;

                if (!string.IsNullOrEmpty(item["cod_cta"].ToString()))//Valida que la cuenta exista
                {
                    string select = "select * from  comae_cta where cod_cta='" + item["cod_cta"].ToString() + "'  ";
                    DataTable dt = SiaWin.Func.SqlDT(select, "table", idemp);
                    if (dt.Rows.Count <= 0) flag = false;
                    dt_maybe.Rows.Add("La cuenta del documento: " + item["cod_cta"].ToString().Trim() + " no se encuentra");
                }


                //else flag = false;
            }
            if ((deb - cre) != 0)//valida documento si esta descuadrado
            {
                dt_maybe.Rows.Add("El documento se encuentra descuadrado");

                //SiaWin.Browse(dii);
                flag = false;
            }
            if (!string.IsNullOrEmpty(num_trn))//Valida que la TRN no exista
            {
                string select = "select * from  cocab_doc where num_trn='" + num_trn + "'  ";
                DataTable dt = SiaWin.Func.SqlDT(select, "table", idemp);
                if (dt.Rows.Count > 0)
                {
                    flag = false;
                    dt_maybe.Rows.Add("El documento: " + num_trn + " ya existe");
                }
            }
            DataTable daa = dt_maybe.Copy();
            daa.TableName = dt_temp.TableName;
            ds_errores.Tables.Add(daa);
            return flag;
        }
        private void SubirDatos()
        {
            insertTodos(ds_docs);
        }
        public void Inser(DataTable dt)
        {
            using (SqlConnection connection = new SqlConnection(cnEmp))
            {
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    try
                    {
                        foreach (var col in dt.Columns)
                        {
                            LoadConfig();
                            cmd.CommandText = "insert into Cocue_doc () values (@Cod_ter,@Cod_tip,@Por_des,@des_mos,@des_ppag,@fecha_aded)";


                            cmd.Parameters.AddWithValue("@fecha_aded", DateTime.Now.ToString("dd/MM/yyyy H:mm"));
                            connection.Open();
                            cmd.ExecuteNonQuery();
                            //CargarGrid(TX_codTer.Text);
                            //limpiar();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error Interno Al guardar", MessageBoxButton.OK, MessageBoxImage.Stop);

                    }
                }
            }
        }


        public void insertTodos(DataSet ds_exist)
        {
            try
            {



                try
                {

                    string sql_cab = "";
                    string sql_cue = "";

                    foreach (DataTable dt_cue in ds_exist.Tables)
                    {

                        string cod_trn_cab = dt_cue.Rows[0]["cod_trn"].ToString();
                        string num_trn_cab = dt_cue.Rows[0]["num_trn"].ToString();

                        sql_cab += @"INSERT INTO cocab_doc (cod_trn,fec_trn,num_trn,detalle,cod_ven,rc_prov,ven_com) values ('" + cod_trn_cab + "',getdate(),'" + num_trn_cab+"','aaaa','01','10','01');DECLARE @NewID INT;SELECT @NewID = SCOPE_IDENTITY();";

                        foreach (DataRow data in dt_cue.Rows)
                        {
                            double dev_con = Convert.ToDouble(data["DEB_MOV"]);
                            double cre_con = Convert.ToDouble(data["CRE_MOV"]);

                            sql_cue += @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,doc_cruc,deb_mov,cre_mov,fec_trn) values (@NewID,'" + data["cod_trn"] + "','" + data["num_trn"] + "','" + data["cod_cta"].ToString() + "','" + data["cod_cco"].ToString() + "','" + data["cod_ter"].ToString() + "','" + data["DES_MOV"].ToString() + "','" + data["DOC_CRUC"].ToString() + "'," + dev_con.ToString("F", CultureInfo.InvariantCulture) + "," + cre_con.ToString("F", CultureInfo.InvariantCulture) + ",getdate());";
                        }


                        if (SiaWin.Func.SqlCRUD(sql_cab + sql_cue, idemp) == true)
                        {
                            MessageBox.Show("inserto el primer documento");
                            return;
                        }
                    }




                }
                catch (SqlException ex)
                {
                    MessageBox.Show("error:::" + ex);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("err11111or:::" + ex);
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("ERROR INSER:" + w);
            }

        }


        public void loadshit()
        {
            string connetionString;
            SqlConnection cnn;
            connetionString = @"Data Source=64.250.116.210,8334;Initial Catalog=SDA_SiaApp;User ID=wilmer1104@yahoo.com;Password=Q1w2e3r4*/*";
            cnn = new SqlConnection(connetionString);
            cnn.Open();
        }
        public static System.Data.DataTable ConvertExcelToDataTable(string FileName)
        {
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Excel2013;
                IWorkbook workbook = application.Workbooks.Open(FileName);
                IWorksheet worksheet = workbook.Worksheets[0];

                //Read data from the worksheet and Export to the DataTable
                System.Data.DataTable customersTable = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ColumnNames);

                //Binding exported DataTable to data grid, likewise it can binded to any 
                //user interface control which supports binding
                return customersTable;
            }
        }
        public DataTable ConverInt(DataTable dt)
        {
            int credi = 9, debi = 8;
            DataTable dtCloned = dt.Clone();
            dtCloned.Columns[debi].DataType = typeof(Int32);
            dtCloned.Columns[credi].DataType = typeof(Int32);
            foreach (DataRow row in dt.Rows)
            {
                dtCloned.ImportRow(row);
            }
            return dtCloned;
        }
        public DataTable Limpiar(DataTable dt)
        {
            DataTable dt1 = dt.Clone(); //copy the structure 
            for (int i = 0; i <= dt.Rows.Count - 1; i++) //iterate through the rows of the source
            {
                DataRow currentRow = dt.Rows[i];  //copy the current row 
                foreach (var colValue in currentRow.ItemArray)//move along the columns 
                {
                    if (!string.IsNullOrEmpty(colValue.ToString())) // if there is a value in a column, copy the row and finish
                    {
                        dt1.ImportRow(currentRow);
                        break; //break and get a new row                        
                    }
                }
            }
            return dt1;
        }
        public long SumarCol(DataTable d1, string col_name)
        {
            long total = 0;
            string cadena = "Sum(" + col_name + ")";
            object resul;
            resul = d1.Compute(cadena, string.Empty);
            total = Convert.ToInt64(resul);
            return total;
        }
        public string guardar()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = ".xlsx";
            saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
            saveFileDialog.Title = "Guardar Plantilla como...";
            saveFileDialog.ShowDialog();
            return saveFileDialog.FileName;
        }
        public string Buscar()
        {
            OpenFileDialog openfile = new OpenFileDialog();
            openfile.DefaultExt = ".xlsx";
            openfile.Filter = "(.xlsx)|*.xlsx";
            var browsefile = openfile.ShowDialog();
            return openfile.FileName;
        }

        public void Cargar(string ruta)
        {
            try
            {
                tablaXLS.Clear();
                DataTable tablaXLS_temp = ConvertExcelToDataTable(ruta);
                tablaXLS = Limpiar(tablaXLS_temp);
                Validaciones(tablaXLS);
                tablaXLS = Limpiar(tablaXLS);

                //tablaXLS = ConverInt(tablaXLS);
                //debito = SumarCol(tablaXLS, "DEB_MOV");
                //credito = SumarCol(tablaXLS, "CRE_MOV");
                Datos.ItemsSource = null;
                Datos.Items.Clear();
                Datos.ItemsSource = tablaXLS.DefaultView;
                credi.Text = credito.ToString();
                debi.Text = debito.ToString();
                dife.Text = Convert.ToString(debito - credito);
                if (credito != debito)
                {
                    MessageBox.Show("Diferencia entre credito y debito");
                }
            }
            catch (Exception m)
            {
                //MessageBox.Show(m.Message);
                MessageBox.Show("Error\n" + m);
            }
        }
        private void Button_Cargar(object sender, RoutedEventArgs e)
        {
            string url = Buscar();
            Cargar(url);
        }

        private void Button_Impo(object sender, RoutedEventArgs e)
        {
            SubirDatos();
        }
        private void Button_Salir(object sender, RoutedEventArgs e)
        {
            //this.Close();
        }

        private void Button_Crear(object sender, RoutedEventArgs e)
        {

            ruta = guardar();
            //Create an instance of ExcelEngine
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;

                application.DefaultVersion = ExcelVersion.Excel2010;

                //Create a workbook
                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet worksheet = workbook.Worksheets[0];

                //Disable gridlines in the worksheet
                worksheet.IsGridLinesVisible = true;

                //Enter values to the cells from A3 to A5
                worksheet.Range["A1"].Text = "COD_TRN";
                worksheet.Range["B1"].Text = "NUM_TRN";
                worksheet.Range["C1"].Text = "FEC_TRN";
                worksheet.Range["D1"].Text = "COD_CTA";
                worksheet.Range["E1"].Text = "COD_TER";
                worksheet.Range["F1"].Text = "DES_MOV";
                worksheet.Range["G1"].Text = "DOC_MOV";
                worksheet.Range["H1"].Text = "BAS_MOV";
                worksheet.Range["I1"].Text = "DEB_MOV";
                worksheet.Range["J1"].Text = "CRE_MOV";
                worksheet.Range["K1"].Text = "DOC_CRUC";
                worksheet.Range["L1"].Text = "ORD_PAG";
                worksheet.Range["M1"].Text = "COD_BANC";
                worksheet.Range["N1"].Text = "FEC_VENC";
                worksheet.Range["O1"].Text = "REG";
                worksheet.Range["P1"].Text = "NUM_CHQ";
                worksheet.Range["Q1"].Text = "FACTURA";
                worksheet.Range["R1"].Text = "FEC_VEN";
                worksheet.Range["S1"].Text = "COD_VEN";
                worksheet.Range["T1"].Text = "COD_CIU";
                worksheet.Range["U1"].Text = "COD_SUC";
                worksheet.Range["V1"].Text = "COD_CCO";
                worksheet.Range["W1"].Text = "DOC_REF";
                worksheet.Range["X1"].Text = "FEC_SUSC";
                //Make the text bUld
                worksheet.Range["A1:X1"].CellStyle.Font.Bold = true;

                //Save the Excel document
                if (string.IsNullOrEmpty(ruta))
                {
                    MessageBox.Show("Por favor, seleccione una ruta para guardar la plantilla");
                }
                else
                {
                    workbook.SaveAs(ruta);
                }
            }
        }

    }
}



