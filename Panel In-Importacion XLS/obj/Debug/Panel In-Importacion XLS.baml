   M S B A M L   `   `   `  ����S  OPanel In-Importacion XLS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:SiasoftAppExtSiasoftAppExt  5   	   ,    &SiasoftAppExt.Panel_In_Importacion_XLS   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   a ]Syncfusion.SfGrid.WPF, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a ]Syncfusion.Shared.Wpf, Version=16.1460.0.32, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89,sf!http://schemas.syncfusion.com/wpf   5   	   )localclr-namespace:SiasoftAppExt   5   	     � Title$  Importacion XLS��5	   	   $	��300��6!   $	��300��6.   .��5
      �  � RowDefinitions 5   
   �� 5      $��1* �6   �� 5      $	��10* �6   .+�5   
   �  � ColumnDefinitions 5      �� 5      $
��Auto �6#   �� 5      $
��Auto �6#   �� 5      $��1* �6#   .+�5      �� $��18(�6   $��Ruta: ��6&   $��5q�65   $��Center=�6@   �� 5      -   5        $��txtFilePath��5   '   $��1��6   $
��Auto��6:   $��	10,0,10,0q�6G    J� Opacity$ 1Q�6Z   $��Center=�6f   �� 5      -   5        $
��Left=�5      $��Center=�61   $��2q�6L   $��2��6W   $��Importar��6g    � Cursor$
 Hand��6z   
����������	#FF0A81C56�   5      (  "Syncfusion.UI.Xaml.Grid.SfDataGrid  5   
   -   5        $��dtGrid��5   F      ColumnSizer$
 Auto=�6   (  "Syncfusion.UI.Xaml.Grid.SfGridBase   AutoGenerateColumns . $��5q�6T   $��1��6_   5      5      5      