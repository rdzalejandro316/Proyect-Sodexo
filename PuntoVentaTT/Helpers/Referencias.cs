using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntoVentaTT.Helpers
{
    public class Referencias : PropertyChangedGlobal
    {
        
        string _cod_ref;
        public string cod_ref { get { return _cod_ref; } set { _cod_ref = value; OnPropertyChanged("cod_ref"); } }

        string _nom_ref;
        public string nom_ref { get { return _nom_ref; } set { _nom_ref = value; OnPropertyChanged("nom_ref"); } }

        string _cod_tip;
        public string cod_tip { get { return _cod_tip; } set { _cod_tip = value; OnPropertyChanged("cod_tip"); } }
                
        decimal _val_ref;
        public decimal val_ref { get { return _val_ref; } set { _val_ref = value; OnPropertyChanged("val_ref"); } }



    }
}
