using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EasyDatabaseCompare
{
    /// <summary>
    /// DataTableListItem.xaml 的交互逻辑
    /// </summary>
    public partial class DataTableListItem : ListBoxItem
    {
        public DataTableListItem(DataEntity t)
        {
            InitializeComponent();
            //DataContext = t;

            //if(t.UpDatedData.Count > 0)
            //{
            //    BindingSource bs = new BindingSource();
            //    var updateData = new DataTable();
            //    foreach(var item in t.PrimaryKeys)
            //        updateData.Columns.Add(item.ColumnName, item.DataType);
            //    foreach(var item in t.UpDatedData.SelectMany(kvp => kvp.Value.DiffFields.Select(d => d.Key)).Distinct())
            //        updateData.Columns.Add(item.ColumnName, item.DataType);
            //    foreach(var item in t.UpDatedData.Select(kvp => kvp.Value))
            //    {
            //        foreach(var diff in item.DiffFields)
            //        {
            //            var rO = updateData.NewRow();
            //            var rN = updateData.NewRow();
            //            rO[diff.Key.ColumnName] = diff.Value[0];
            //            rN[diff.Key.ColumnName] = diff.Value[1];
            //            updateData.Rows.Add(rO);
            //            updateData.Rows.Add(rN);
            //        }
            //    }
            //    update.DataContext = updateData;
                
            //}
            //else
            //    update.Visibility = Visibility.Collapsed;
            //if(t.InsertRows.Count > 0)
            //{
            //    insert.Visibility = Visibility.Visible;
            //    insert.ItemsSource = t.InsertRows.Values;
            //}
            //else
            //    insert.Visibility = Visibility.Collapsed;
            //if(t.DeleteRows.Count > 0)
            //{
            //    delete.Visibility = Visibility.Visible;
            //    delete.ItemsSource = t.DeleteRows.Values;
            //}
            //else
            //    delete.Visibility = Visibility.Collapsed;
        }
    }
}
