using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;
using System.Data;
using HospedajePreCheckIn.Converter;
using System.Data.SqlClient;

namespace SiasoftAppExt
{
    class EstadoToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {            
            string input = value as string;
           
            switch (input.ToUpper().Trim())
            {
                //Brush brush = new SolidColorBrush(#16a085);                
                case "D": return Config.ToSolidColorBrush(Config.ColorD);//disponible
                case "O": return Config.ToSolidColorBrush(Config.ColorO);//ocupacion
                case "R": return Config.ToSolidColorBrush(Config.ColorR);//reservado
                case "M": return Config.ToSolidColorBrush(Config.ColorM);//mantenimiento
                case "S": return Config.ToSolidColorBrush(Config.ColorS);//salida
                case "P": return Config.ToSolidColorBrush(Config.ColorP); //prechekin
                case "L": return Config.ToSolidColorBrush(Config.ColorL);   //limpiesa
                default: return DependencyProperty.UnsetValue;
            }            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        

    }
}
