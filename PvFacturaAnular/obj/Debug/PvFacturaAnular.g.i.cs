﻿#pragma checksum "..\..\PvFacturaAnular.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "9DB8EC688F156D61F02110A30F21294159DCB8D7478CA6A3105501BF6EA64A15"
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

using SiasoftAppExt;
using Syncfusion;
using Syncfusion.UI.Xaml.Controls.DataPager;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.RowFilter;
using Syncfusion.UI.Xaml.TreeGrid;
using Syncfusion.Windows;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace SiasoftAppExt {
    
    
    /// <summary>
    /// PvFacturaAnular
    /// </summary>
    public partial class PvFacturaAnular : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 54 "..\..\PvFacturaAnular.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock Txtrn_anular;
        
        #line default
        #line hidden
        
        
        #line 69 "..\..\PvFacturaAnular.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox CmbTipoDoc;
        
        #line default
        #line hidden
        
        
        #line 78 "..\..\PvFacturaAnular.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker FechaConsIni;
        
        #line default
        #line hidden
        
        
        #line 82 "..\..\PvFacturaAnular.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker FechaConsFin;
        
        #line default
        #line hidden
        
        
        #line 84 "..\..\PvFacturaAnular.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BTNconsultar;
        
        #line default
        #line hidden
        
        
        #line 93 "..\..\PvFacturaAnular.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TXfactura;
        
        #line default
        #line hidden
        
        
        #line 96 "..\..\PvFacturaAnular.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TxtNota;
        
        #line default
        #line hidden
        
        
        #line 99 "..\..\PvFacturaAnular.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TxtAutoriza;
        
        #line default
        #line hidden
        
        
        #line 103 "..\..\PvFacturaAnular.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Syncfusion.Windows.Tools.Controls.ComboBoxAdv CBXconcepto;
        
        #line default
        #line hidden
        
        
        #line 114 "..\..\PvFacturaAnular.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BTNvalidar;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/PvFacturaAnular;component/pvfacturaanular.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\PvFacturaAnular.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 8 "..\..\PvFacturaAnular.xaml"
            ((SiasoftAppExt.PvFacturaAnular)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Txtrn_anular = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.CmbTipoDoc = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 4:
            this.FechaConsIni = ((System.Windows.Controls.DatePicker)(target));
            return;
            case 5:
            this.FechaConsFin = ((System.Windows.Controls.DatePicker)(target));
            return;
            case 6:
            this.BTNconsultar = ((System.Windows.Controls.Button)(target));
            
            #line 84 "..\..\PvFacturaAnular.xaml"
            this.BTNconsultar.Click += new System.Windows.RoutedEventHandler(this.BTNconsultar_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.TXfactura = ((System.Windows.Controls.TextBox)(target));
            
            #line 93 "..\..\PvFacturaAnular.xaml"
            this.TXfactura.LostFocus += new System.Windows.RoutedEventHandler(this.TXfactura_LostFocus);
            
            #line default
            #line hidden
            return;
            case 8:
            this.TxtNota = ((System.Windows.Controls.TextBox)(target));
            return;
            case 9:
            this.TxtAutoriza = ((System.Windows.Controls.TextBox)(target));
            return;
            case 10:
            this.CBXconcepto = ((Syncfusion.Windows.Tools.Controls.ComboBoxAdv)(target));
            return;
            case 11:
            this.BTNvalidar = ((System.Windows.Controls.Button)(target));
            
            #line 114 "..\..\PvFacturaAnular.xaml"
            this.BTNvalidar.Click += new System.Windows.RoutedEventHandler(this.BTNvalidar_Click);
            
            #line default
            #line hidden
            return;
            case 12:
            
            #line 115 "..\..\PvFacturaAnular.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

