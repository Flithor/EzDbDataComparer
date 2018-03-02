using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EasyDatabaseCompare
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public double QueryProcess1
        {
            get { return (double)GetValue(QueryProcess1Property); }
            set { SetValue(QueryProcess1Property, value); }
        }

        // Using a DependencyProperty as the backing store for QueryProcess1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty QueryProcess1Property =
            DependencyProperty.Register("QueryProcess1", typeof(double), typeof(MainWindow), new PropertyMetadata(0.0));
        public double QueryProcess2
        {
            get { return (double)GetValue(QueryProcess2Property); }
            set { SetValue(QueryProcess2Property, value); }
        }

        // Using a DependencyProperty as the backing store for QueryProcess2.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty QueryProcess2Property =
            DependencyProperty.Register("QueryProcess2", typeof(double), typeof(MainWindow), new PropertyMetadata(0.0));
        public double CompareProcess
        {
            get { return (double)GetValue(CompareProcessProperty); }
            set { SetValue(CompareProcessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CompareProcess.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CompareProcessProperty =
            DependencyProperty.Register("CompareProcess", typeof(double), typeof(MainWindow), new PropertyMetadata(0.0));

        DataSet data1;
        DataSet data2;
        private void get1_Click(object sender, RoutedEventArgs e)
        {
            if(QueryAndFillAll(ref data1, queryRe1, QueryProcess1Property))
            {
                get2.IsEnabled = true;
                showFilter.IsEnabled = false;
            }
        }

        private void get2_Click(object sender, RoutedEventArgs e)
        {
            get1.IsEnabled = false;
            if(QueryAndFillAll(ref data2, queryRe2, QueryProcess2Property))
            {
                btnComp.IsEnabled = compRe.IsEnabled = true;
                var link = new Hyperlink() { ToolTip = "Click to move to left.", Style = null, TextDecorations = null, Foreground = queryRe2.Foreground };
                link.Click += Link_Click;
                link.Inlines.AddRange(queryRe2.Inlines.ToArray());
                queryRe2.Inlines.Clear();
                queryRe2.Inlines.Add(link);
            }
            else
                get1.IsEnabled = true;
        }

        private void Link_Click(object sender, RoutedEventArgs e)
        {
            data1 = data2;
            QueryProcess2 = 0;
            queryRe1.Inlines.Clear();
            queryRe1.Inlines.AddRange((sender as Hyperlink).Inlines.ToArray());
            queryRe2.Text = "No Data";
            compRe.Items.Clear();
            btnComp.IsEnabled = compRe.IsEnabled = false;
            get1.IsEnabled = true;
            GC.Collect();
        }

        private bool QueryAndFillAll(ref DataSet ds, TextBlock lb, DependencyProperty dp)
        {
            if(!CheckCanQuery()) return false;
            data2 = null;
            GC.Collect();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            lb.Text = "Querying...";
            ds = QueryAll(dp);
            lb.Inlines.Clear();
            lb.Inlines.Add("Queried ");
            lb.Inlines.Add(new Run(ds.Tables.Count.ToString()) { Foreground = Brushes.Red });
            lb.Inlines.Add(" Table and ");
            lb.Inlines.Add(new Run(ds.Tables.Cast<DataTable>().Select(t => t.Rows.Count).Sum().ToString()) { Foreground = Brushes.Red });
            lb.Inlines.Add(" Records.");
            lb.Inlines.Add("\r\n");
            watch.Stop();
            lb.Inlines.Add($"Query Spent Time: {watch.Elapsed}");
            //lb.Inlines.Add($"Query Finish Time: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffffff")}");
            return true;
        }
        private bool CheckCanQuery()
        {
            if(string.IsNullOrEmpty(connStr.Text))
            {
                MessageBox.Show($"Connection string is empty!");
                return false;
            }
            else if(TableNames.Length > 0)
                if(((isBlackList.IsChecked ?? false) && SelectedTableName.Count == TableNames.Length) ||
                   ((!isBlackList.IsChecked) ?? false) && SelectedTableName.Count == 0)
                {
                    MessageBox.Show("No tables available for query!");
                    return false;
                }
            return true;
        }
        //Provider=SQLOLEDB.1;Server=127.0.0.1,51433;Database=RMDBFD;User Id=sa;Password=sa
        private DataSet QueryAll(DependencyProperty dp)
        {
            var d = new DataSet();
            try
            {
                using(var conn = new OleDbConnection(connStr.Text))
                {
                    conn.Open();
                    var tnl = QueryList.ToArray();
                    double acount = 0;
                    foreach(var tableName in tnl)
                    {
                        using(var adapter = new OleDbDataAdapter($"SELECT * FROM {tableName}", conn))
                        {

                            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background,
                                new Action<DataSet, string>((ds, tn) =>
                                {
                                    adapter.FillSchema(ds, SchemaType.Mapped, tn);
                                    adapter.Fill(ds, tn);
                                }), d, tableName);
                            SetValue(dp, ++acount / tnl.Length);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"{ex.Message}\r\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return d;
        }

        private void checkConn_Click(object sender, RoutedEventArgs e)
        {
            if(!CheckCanQuery()) return;
            try
            {
                using(var conn = new OleDbConnection(connStr.Text))
                {
                    conn.Open();
                    MessageBox.Show($"Connection Success!");
                    var t = new DataTable();
                    using(var adapter = new OleDbDataAdapter("SELECT name FROM sysobjects WHERE type='U' ORDER BY name", conn))
                    {
                        adapter.Fill(t);
                    }
                    TableNames = t.AsEnumerable().Select(r => r["name"].ToString()).ToArray();
                    filterStr_TextChanged(null, null);
                    showFilter.IsEnabled = get1.IsEnabled = true;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"{ex.Message}\r\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnComp_Click(object sender, RoutedEventArgs e)
        {
            if(data1.Tables.Count > 0 && data2.Tables.Count > 0)
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                compRe.Items.Clear();
                GC.Collect();
                compReState.Text = "Comparing...";
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background,
                new Action(() =>
                {
                    #region waitMove
                    var ts1 = data1.Tables.Cast<DataTable>().Where(t => t.PrimaryKey?.Length > 0);
                    var ts2 = data2.Tables.Cast<DataTable>().Where(t => t.PrimaryKey?.Length > 0);
                    var tg = ts1.Zip(ts2, Tuple.Create);
                    var tdic = tg.Select(g => new DataEntity(g.Item1, g.Item2, EachDataEntityCreateCallBack))
                    .Where(t => t.UpDatedData.Count > 0 || t.InsertRows.Count > 0 || t.DeleteRows.Count > 0).ToArray();
                    if(tdic.Length == 0)
                        compReState.Text = "Not found any changes in selected tables";
                    else
                    {
                        //foreach(var t in tdic)
                        //{
                        //    var dataItem = new DataTableListItem(t);
                        //    compRe.Items.Add(dataItem);
                        //}
                        var w = new Window();
                        var tb = new TextBox()
                        {
                            Margin = new Thickness(0),
                            Padding = new Thickness(0),
                            TextWrapping = TextWrapping.Wrap,
                            IsReadOnly = true,
                            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
                        };
                        w.Content = tb;
                        var sb = new StringBuilder();
                        foreach(var t in tdic)
                        {
                            var z =
                            new DatabaseChangeContent
                            {
                                TableName = t.TableName,
                                UpdatedDatas = t.UpDatedData.Select(kvp => new UpdatedData
                                {
                                    UniquePrimaryKey = kvp.Key,
                                    UpdatedFields = kvp.Value.DiffFields.Select(diff => new UpdatedField
                                    {
                                        ColumnName = diff.Key.ColumnName,
                                        OldValue = diff.Value[0],
                                        NewValue = diff.Value[1]
                                    })
                                }),
                                InsertedDatas = t.InsertRows.Select(kvp => new InsertedData
                                {
                                    UniquePrimaryKey = kvp.Key,
                                    Datas = kvp.Value.Table.Columns.Cast<DataColumn>()
                                        .ToDictionary(c => c.ColumnName, c => kvp.Value[c])
                                }),
                                DeletedDatas = t.DeleteRows.Select(kvp => new DeletedData
                                {
                                    UniquePrimaryKey = kvp.Key,
                                    Datas = kvp.Value.Table.Columns.Cast<DataColumn>()
                                        .ToDictionary(c => c.ColumnName, c => kvp.Value[c])
                                })
                            };
                            sb.AppendLine($"{z.TableName}:");
                            sb.AppendLine(JsonConvert.SerializeObject(z, Formatting.Indented));
                            sb.AppendLine("=================================================");
                        }
                        tb.Text = sb.ToString();
                        w.Show();
                        compReState.Inlines.Clear();
                        compReState.Inlines.Add("Compare found ");
                        compReState.Inlines.Add(new Run(tdic.Length.ToString())
                        {
                            Foreground = Brushes.Red
                        });
                        compReState.Inlines.Add(" Table and ");
                        compReState.Inlines.Add(new Run(tdic.Select(t => t.UpDatedData.Count).Sum().ToString())
                        {
                            Foreground = Brushes.Red
                        });
                        compReState.Inlines.Add(" update Records, ");
                        compReState.Inlines.Add(new Run(tdic.Select(t => t.InsertRows.Count).Sum().ToString()) { Foreground = Brushes.Red });
                        compReState.Inlines.Add(" insert Records, ");
                        compReState.Inlines.Add(new Run(tdic.Select(t => t.DeleteRows.Count).Sum().ToString()) { Foreground = Brushes.Red });
                        compReState.Inlines.Add(" delete Records");
                        compReState.Inlines.Add("\r\n");
                        watch.Stop();
                        compReState.Inlines.Add($"Compare Spent Time: {watch.Elapsed}");
                    }
                    #endregion
                }));
                CompareProcess = 1;
                GC.Collect();
            }
        }

        private void EachDataEntityCreateCallBack()
        {

        }

        private void reset_Click(object sender, RoutedEventArgs e)
        {
            connStr.Text = string.Empty;
            data1 = null;
            data2 = null;
            QueryProcess1 = QueryProcess2 = CompareProcess = 0;
            compRe.Items.Clear();
            SelectedTableName.Clear();
            TableNames = new string[0];
            queryRe1.Text = queryRe2.Text = "No Data";
            compReState.Text = "Not Start Compare";
            showFilter.IsEnabled = get1.IsEnabled = get2.IsEnabled = btnComp.IsEnabled = compRe.IsEnabled = false;
            GC.Collect();
        }

        public IEnumerable<string> QueryList
        {
            get
            {
                if(isBlackList.IsChecked ?? false)
                    return TableNames.Except(SelectedTableName);
                else
                    return SelectedTableName;
            }
        }
        #region Properties For View

        #endregion
        public string[] TableNames { get; private set; } = new string[0];

        public HashSet<string> SelectedTableName { get; } = new HashSet<string>();

        bool OnFilter = false;
        private void filterStr_TextChanged(object sender, TextChangedEventArgs e)
        {
            OnFilter = true;
            var filterList = filterStr.Text.Contains('_') ?
                TableNames.Where(str => str.ToLower().Contains(filterStr.Text.ToLower())) :
                TableNames.Where(str => str.Replace("_", "").ToLower().Contains(filterStr.Text.ToLower()));
            filterTableList.ItemsSource = filterList;
            var intersectList = SelectedTableName.Intersect(filterList).ToList();
            intersectList.ForEach(str => filterTableList.SelectedItems.Add(str));
            OnFilter = false;
        }
        private void filterTableList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.AddedItems.Cast<string>().ToList().ForEach(str => SelectedTableName.Add(str));
            if(!OnFilter) e.RemovedItems.Cast<string>().ToList().ForEach(str => SelectedTableName.Remove(str));
        }
    }
}
