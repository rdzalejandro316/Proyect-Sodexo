﻿#pragma checksum "..\..\Gallery.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "1F8B89DC8AE189741C08680395B583D90689FEB12E2009C2D93345AD12DE692F"
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
using Syncfusion.UI.Xaml.CellGrid;
using Syncfusion.UI.Xaml.Spreadsheet;
using Syncfusion.Windows;
using Syncfusion.Windows.Controls.RichTextBoxAdv;
using Syncfusion.Windows.Controls.Spreadsheet;
using Syncfusion.Windows.PdfViewer;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools;
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
    /// Gallery
    /// </summary>
    public partial class Gallery : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 24 "..\..\Gallery.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txnombre;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\Gallery.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txruta;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\Gallery.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid grilla;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\Gallery.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid gridPDF;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\Gallery.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Syncfusion.Windows.PdfViewer.PdfViewerControl pdfViewerControl1;
        
        #line default
        #line hidden
        
        
        #line 57 "..\..\Gallery.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Syncfusion.Windows.Controls.RichTextBoxAdv.SfRichTextRibbon richTextRibbon;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\Gallery.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Syncfusion.Windows.Controls.RichTextBoxAdv.SfRichTextBoxAdv richTextBoxAdv;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\Gallery.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Syncfusion.UI.Xaml.Spreadsheet.SfSpreadsheet spreadsheet;
        
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
            System.Uri resourceLocater = new System.Uri("/Gallery;component/gallery.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\Gallery.xaml"
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
            this.txnombre = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.txruta = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            
            #line 27 "..\..\Gallery.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 28 "..\..\Gallery.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click_1);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 29 "..\..\Gallery.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click_2);
            
            #line default
            #line hidden
            return;
            case 6:
            this.grilla = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 7:
            this.gridPDF = ((System.Windows.Controls.Grid)(target));
            return;
            case 8:
            this.pdfViewerControl1 = ((Syncfusion.Windows.PdfViewer.PdfViewerControl)(target));
            return;
            case 9:
            this.richTextRibbon = ((Syncfusion.Windows.Controls.RichTextBoxAdv.SfRichTextRibbon)(target));
            return;
            case 10:
            this.richTextBoxAdv = ((Syncfusion.Windows.Controls.RichTextBoxAdv.SfRichTextBoxAdv)(target));
            return;
            case 11:
            this.spreadsheet = ((Syncfusion.UI.Xaml.Spreadsheet.SfSpreadsheet)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
