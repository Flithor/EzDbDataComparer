using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EasyDatabaseCompare
{
    /// <summary>
    /// DataTableListItem.xaml 的交互逻辑
    /// </summary>
    public partial class DataTableListItem : ListBoxItem
    {
#if DEBUG
        public DataTableListItem()
        {
            InitializeComponent();
        }
#endif
        //public DataTableListItem(DatabaseChangeContent t)
        //{
        //    InitializeComponent();
        //    TableName = t.TableName;
        //    if (t.UpdatedDatas.Length > 0)
        //    {
        //        var diffFeilds = t.UpdatedDatas.SelectMany(d => d.UpdatedFields.Select(c => c.ColumnName)).Distinct().ToArray();
        //        var diffList = t.UpdatedDatas;
        //        var dt = new DataTable();
        //        dt.Columns.Add("UniquePrimaryKey");
        //        //update.Columns.Add(new DataGridTextColumn { Header = "UniquePrimaryKey", Binding = new System.Windows.Data.Binding("UniquePrimaryKey") });

        //        foreach (var item in diffFeilds)
        //            dt.Columns.Add(item);
        //        //update.Columns.Add(new DataGridTextColumn { Header = item, Binding = new System.Windows.Data.Binding("NewValue") });
        //        //var bs = new BindingSource(t.UpDatedData, "");
        //        //bs.DataSource = ;
        //        //bs.

        //        foreach (var diff in diffList)
        //        {
        //            var r = dt.NewRow();
        //            r["UniquePrimaryKey"] = diff.UniquePrimaryKey;
        //            foreach (var field in diff.UpdatedFields)
        //            {
        //                var ov = field.OldValue.ToString();
        //                var nv = field.NewValue.ToString();
        //                var ovsub = ov.Length > 20 ? $"{ov.Substring(20)}..." : ov;
        //                var nvsub = nv.Length > 20 ? $"{nv.Substring(20)}..." : nv;
        //                r[field.ColumnName] = $"{ovsub} => {nvsub}";
        //            }
        //            dt.Rows.Add(r);
        //        }
        //        update.DataContext = dt;

        //    }
        //    else
        //        eUpdate.Visibility = Visibility.Collapsed;

        //    if (t.InsertedDatas.Length > 0)
        //    {
        //        var dt = new DataTable();
        //        foreach (var key in t.InsertedDatas.SelectMany(d => d.Datas.Keys).Distinct())
        //            dt.Columns.Add(key);
        //        foreach (var value in t.InsertedDatas)
        //        {
        //            var r = dt.NewRow();
        //            foreach (var data in value.Datas)
        //                r[data.Key] = data.Value;
        //            dt.Rows.Add(r);
        //        }
        //        insert.DataContext = dt;
        //    }
        //    else
        //        eInsert.Visibility = Visibility.Collapsed;

        //    if (t.DeletedDatas.Length > 0)
        //    {
        //        var dt = new DataTable();
        //        foreach (var key in t.DeletedDatas.SelectMany(d => d.Datas.Keys).Distinct())
        //            dt.Columns.Add(key);
        //        foreach (var value in t.DeletedDatas)
        //        {
        //            var r = dt.NewRow();
        //            foreach (var data in value.Datas)
        //                r[data.Key] = data.Value;
        //            dt.Rows.Add(r);
        //        }
        //        delete.DataContext = dt;
        //    }
        //    else
        //        eDelete.Visibility = Visibility.Collapsed;
        //    DataContext = t;
        //}


        //public string TableName
        //{
        //    get => (string)GetValue(TableNameProperty);
        //    set => SetValue(TableNameProperty, value);
        //}
        //public static readonly DependencyProperty TableNameProperty =
        //    DependencyProperty.Register("TableName", typeof(string), typeof(DataTableListItem), new PropertyMetadata(string.Empty));

        //public event HandledEventHandler TableNameClickEvent;

        //private void copyTName_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        Clipboard.SetText(TableName);
        //        TableNameClickEvent?.Invoke(sender, new HandledEventArgs(true));
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //        //throw;
        //        TableNameClickEvent?.Invoke(sender, new HandledEventArgs(false));
        //    }
        //}


        //internal delegate void ShowTableDataHandle(UIElement sender, bool isOld, string tableName);
        //internal event ShowTableDataHandle ShowTableData;

        //private void oData_Click(object sender, RoutedEventArgs e)
        //{
        //    ShowTableData?.Invoke(oData, true, TableName);
        //}

        //private void nData_Click(object sender, RoutedEventArgs e)
        //{
        //    ShowTableData?.Invoke(nData, false, TableName);
        //}
    }
}
