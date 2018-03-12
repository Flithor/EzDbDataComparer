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
        public DataTableListItem(DatabaseChangeContent t)
        {
            InitializeComponent();
            TableName = t.TableName;
            if(t.UpdatedDatas.Length > 0)
            {
                update.DataContextChanged += Update_DataContextChanged;
                update.InitializingNewItem += Update_InitializingNewItem;
                var diffFeilds = t.UpdatedDatas.SelectMany(d => d.UpdatedFields.Select(c => c.ColumnName)).Distinct().ToArray();
                var diffList = t.UpdatedDatas;
                update.Columns.Add(new DataGridTextColumn { Header = "UniquePrimaryKey", Binding = new System.Windows.Data.Binding("UniquePrimaryKey") });

                foreach(var item in diffFeilds)
                    update.Columns.Add(new DataGridTextColumn { Header = item, Binding = new System.Windows.Data.Binding("NewValue") });
                //var bs = new BindingSource(t.UpDatedData, "");
                //bs.DataSource = ;
                //bs.
                update.ItemsSource = diffList;
            }
            else
                eUpdate.Visibility = Visibility.Collapsed;

            if(t.InsertedDatas.Length > 0)
            {
                insert.DataContextChanged += Insert_DataContextChanged;
                insert.InitializingNewItem += Insert_InitializingNewItem;
                //foreach(var item in t.InsertRows)
                //    update.Columns.Add(new DataGridTextColumn { Header = item.Key , Binding = new System.Windows.Data.Binding("KeyValuePair.Value.NewDataRow") });
                insert.ItemsSource = t.InsertedDatas;
            }
            else
                eInsert.Visibility = Visibility.Collapsed;

            if(t.DeletedDatas.Length > 0)
            {
                delete.DataContextChanged += Delete_DataContextChanged;
                delete.InitializingNewItem += Delete_InitializingNewItem;
                //foreach(var item in t.InsertRows)
                //    update.Columns.Add(new DataGridTextColumn { Header = item.Key , Binding = new System.Windows.Data.Binding("KeyValuePair.Value.NewDataRow") });
                delete.ItemsSource = t.DeletedDatas;
            }
            else
                eDelete.Visibility = Visibility.Collapsed;


            DataContext = t;

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


        public string TableName
        {
            get { return (string)GetValue(TableNameProperty); }
            set { SetValue(TableNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TableName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TableNameProperty =
            DependencyProperty.Register("TableName", typeof(string), typeof(DataTableListItem), new PropertyMetadata(string.Empty));



        private void Delete_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }
        private void Insert_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Insert_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }
        private void Delete_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            //throw new NotImplementedException();
        }


        private void Update_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }
        private void Update_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
