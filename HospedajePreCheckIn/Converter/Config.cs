using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace HospedajePreCheckIn.Converter
{
    static class Config
    {
        public static string ColorD = "";
        public static string ColorO = "";
        public static string ColorS = "";
        public static string ColorP = "";
        public static string ColorR = "";
        public static string ColorL = "";
        public static string ColorM = "";
        public static string ColorC = "";
        public static string ColorI = "";
        
        public static bool StackHeadRow = false;

        public static void loadColores() 
        {
            try
            {
                DataTable dt = SqlDT("select * From CtMae_ViviendaHabitacionCamaEstado", "c");
                if (dt.Rows.Count>0)
                {
                    DataRow[] drO = dt.Select("Alias='O'");
                    foreach (DataRow row in drO) { ColorO = row["color_esthab"].ToString(); }

                    DataRow[] drD = dt.Select("Alias='D'");
                    foreach (DataRow row in drD) { ColorD = row["color_esthab"].ToString(); }

                    DataRow[] drS = dt.Select("Alias='S'");
                    foreach (DataRow row in drS) { ColorS = row["color_esthab"].ToString(); }

                    DataRow[] drP = dt.Select("Alias='P'");
                    foreach (DataRow row in drP) { ColorP = row["color_esthab"].ToString(); }
                    
                    DataRow[] drR = dt.Select("Alias='R'");
                    foreach (DataRow row in drR) { ColorR = row["color_esthab"].ToString(); }

                    DataRow[] drL = dt.Select("Alias='L'");
                    foreach (DataRow row in drL) { ColorL = row["color_esthab"].ToString(); }

                    DataRow[] drM = dt.Select("Alias='M'");
                    foreach (DataRow row in drM) { ColorM = row["color_esthab"].ToString(); }

                    DataRow[] drC = dt.Select("Alias='C'");
                    foreach (DataRow row in drC) { ColorC = row["color_esthab"].ToString(); }

                    DataRow[] drI = dt.Select("Alias='I'");
                    foreach (DataRow row in drI) { ColorI = row["color_esthab"].ToString(); }
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error en la clase config:"+w);
            }        
        }
        
        public static SolidColorBrush ToSolidColorBrush(this string hex_code)
        {
            return (SolidColorBrush)new BrushConverter().ConvertFromString(hex_code);
        }

        public static DataTable SqlDT(string _sql, string nomtabla)
        {
            dynamic SiaWin = System.Windows.Application.Current.MainWindow;
            int idemp = SiaWin._BusinessId;
            System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
            string cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
            string cn = cnEmp;

            DataTable dt = new DataTable(nomtabla);
            try
            {
                using (SqlConnection sqlCon = new System.Data.SqlClient.SqlConnection(cn))
                {
                    using (SqlDataAdapter SqlDa = new SqlDataAdapter(_sql, sqlCon))
                    {
                        SqlDa.Fill(dt);
                    }
                }
            }
            catch (SqlException SQLex)
            {
                MessageBox.Show(SQLex.Message);
                dt = null;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                dt = null;
            }
            return dt;
        }

    }
}
