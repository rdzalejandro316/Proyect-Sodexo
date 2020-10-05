using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
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

namespace SiasoftAppExt
{

    //Sia.PublicarPnt(10701, "Gallery");  
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(10701, "Gallery");
    //ww.ShowInTaskbar=false;    
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
    //ww.ShowDialog();
   
    public partial class Gallery : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        public byte[] ByteArr = null;
        public Gallery()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            idemp = SiaWin._BusinessId; ;
            LoadConfig();
            loadCombo();            
            //MessageBox.Show(tempDirPath);
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                FileDialog fldlg = new OpenFileDialog();
                fldlg.InitialDirectory = Environment.SpecialFolder.MyPictures.ToString();
                //fldlg.Filter = "Image File (*.jpg;*.bmp;*.gif;*.png)|*.jpg;*.bmp;*.gif;*.png";
                fldlg.ShowDialog();
                {
                    txruta.Text = fldlg.FileName;
                    txnombre.Text = fldlg.SafeFileName;  
                }

                cargarbyte(fldlg.FileName);
                fldlg = null;                

            }
            catch (Exception w)
            {
                MessageBox.Show("error en 1" + w);
            }
        }

        public void cargarbyte(string name)
        {
            FileStream fs = new FileStream(name, FileMode.Open, FileAccess.Read);
            ByteArr = new byte[fs.Length];
            fs.Read(ByteArr, 0, Convert.ToInt32(fs.Length));
            fs.Close();
        }

        public void loadCombo()
        {
            DataTable dt = SiaWin.Func.SqlDT("select idrow,name_archive,realName  from archivos", "campamento", idemp);
            grilla.ItemsSource = dt.DefaultView;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {

                using (SqlConnection connection = new SqlConnection(SiaWin.Func.DatosEmp(idemp)))
                {
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        
                        cmd.CommandText = "insert into archivos(name_archive,realName,doc) values(@name_archive,@realName,@doc) ";

                        cmd.Parameters.AddWithValue("@name_archive", txnombre.Text);
                        cmd.Parameters.AddWithValue("@realName", txruta.Text);
                        cmd.Parameters.AddWithValue("@doc", ByteArr);

                        connection.Open();
                        int exe = cmd.ExecuteNonQuery();
                        connection.Close();

                        if (exe > 0)
                        {
                            MessageBox.Show("se gaurdo archivo ");
                            loadCombo();
                        }
                    }
                }


            }
            catch (Exception w)
            {
                MessageBox.Show("error en 2" + w);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)grilla.SelectedItems[0];
                string id = row["idrow"].ToString();
                string name_archive = row["name_archive"].ToString();
                
                DataTable dt = SiaWin.Func.SqlDT("select * from archivos where idrow='"+id+"'", "campamento", idemp);
                if (dt.Rows.Count>0)
                {
                    byte[] blob = (byte[])dt.Rows[0]["doc"];
                    MemoryStream stream = new MemoryStream();
                    stream.Write(blob, 0, blob.Length);
                    string path = AppDomain.CurrentDomain.BaseDirectory+ name_archive;
                    //string path = System.IO.Path.GetTempPath()+ name_archive;
                    //string tempDirPath = System.IO.Path.GetTempPath();
                    File.WriteAllBytes(path,blob);

                    //richTextBoxAdv.Load(path);
                    spreadsheet.Open(path);
                    //pdfViewerControl1.ItemSource = path;
                    File.Delete(path);
                    //Process.Start(path);
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error en 3" + w);
            }
        }


    }
}
