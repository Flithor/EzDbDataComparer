using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
using DataAccessLib;

namespace EasyDatabaseCompare
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SupportedDbList = DataAccessFactory.DbTypes;
        }

        List<TextBox> Fields { get; } = new List<TextBox>();

        private void dbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count == 0 || e.AddedItems[0].ToString() == "Empty") return;
            else
            {
                Fields.Clear();
                fieldsGrid.Children.Clear();
                fieldsGrid.ColumnDefinitions.Clear();
                var fields = DataAccessFactory.GetDBTypeConnectionFields(e.AddedItems[0].ToString());
                var i = 0;
                var tbTemplate = this.FindResource("fieldTextBoxTemp") as ControlTemplate;
                foreach(var f in fields)
                {
                    fieldsGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    var tb = new TextBox { Tag = f, Template = tbTemplate };
                    tb.SetValue(Grid.ColumnProperty, i++);
                    Fields.Add(tb);
                    fieldsGrid.Children.Add(tb);
                }
                GC.Collect();
            }
        }

        private void checkConn_Click(object sender, RoutedEventArgs e)
        {
            if(!CheckCanQuery()) return;
            try
            {
                if(changeMode.IsChecked == true)
                    DataAccess = DataAccessFactory.Create(dbType.Text, connStr.Text);
                else
                    DataAccess = DataAccessFactory.Create(dbType.Text, Fields.Select(f => f.Text).ToArray());

                TableNames = DataAccess.QueryAllTableName().ToArray();

                topMsg.ShowMessage("Connection Test Success!");

                filterStr_TextChanged(null, null);
                showFilter.IsEnabled = get1.IsEnabled = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show($"{ex.Message}\r\n{ex.StackTrace}\r\n==========\r\n{ex.InnerException}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Get First Time
        private double QueryProcess1Count = 0;
        public double QueryProcess1
        {
            get { return (double)GetValue(QueryProcess1Property); }
            set { SetValue(QueryProcess1Property, value); }
        }
        public static readonly DependencyProperty QueryProcess1Property =
            DependencyProperty.Register("QueryProcess1", typeof(double), typeof(MainWindow), new PropertyMetadata(0.0));
        private void QueryProcess1CallBack()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                SetValue(QueryProcess1Property, ++QueryProcess1Count / QueryTablesFix.Length);
            }), System.Windows.Threading.DispatcherPriority.Background);
        }
        private void get1_Click(object sender, RoutedEventArgs e)
        {
            get1.IsEnabled = false;
            QueryProcess1Count = 0;
            if(QueryAllTable(ref data1, queryRe1, QueryProcess1CallBack))
            {
                get2.IsEnabled = true;
                showFilter.IsEnabled = false;
            }
            get1.IsEnabled = true;
        }
        #endregion

        #region Get Second Time
        private double QueryProcess2Count = 0;
        public double QueryProcess2
        {
            get { return (double)GetValue(QueryProcess2Property); }
            set { SetValue(QueryProcess2Property, value); }
        }
        public static readonly DependencyProperty QueryProcess2Property =
            DependencyProperty.Register("QueryProcess2", typeof(double), typeof(MainWindow), new PropertyMetadata(0.0));
        private void QueryProcess2CallBack()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                SetValue(QueryProcess2Property, ++QueryProcess2Count / QueryTablesFix.Length);
            }), System.Windows.Threading.DispatcherPriority.Background).Wait();
        }
        private void get2_Click(object sender, RoutedEventArgs e)
        {
            get1.IsEnabled = get2.IsEnabled = false;
            QueryProcess2Count = 0;
            if(QueryAllTable(ref data2, queryRe2, QueryProcess2CallBack))
            {
                compReState.Text = "Not Start Compare";
                btnComp.IsEnabled = compRe.IsEnabled = true;
                var link = new Hyperlink() { ToolTip = "Click to move to left.", Style = null, TextDecorations = null, Foreground = queryRe2.Foreground };
                link.Click += Link_Click;
                link.Inlines.AddRange(queryRe2.Inlines.ToArray());
                queryRe2.Inlines.Clear();
                queryRe2.Inlines.Add(link);
            }
            else
                get1.IsEnabled = true;
            get2.IsEnabled = true;
        }
        #endregion

        #region Compare Process
        private double CompareProcessCount = 0;
        public double CompareProcess
        {
            get { return (double)GetValue(CompareProcessProperty); }
            set { SetValue(CompareProcessProperty, value); }
        }
        public static readonly DependencyProperty CompareProcessProperty =
            DependencyProperty.Register("CompareProcess", typeof(double), typeof(MainWindow), new PropertyMetadata(0.0));
        private void CompareProcessCallBack()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                SetValue(CompareProcessProperty, ++CompareProcessCount / QueryTablesFix.Length);
            }), System.Windows.Threading.DispatcherPriority.Background).Wait();
        }
        #endregion

        public string[] SupportedDbList
        {
            get { return (string[])GetValue(SupportedDbListProperty); }
            set { SetValue(SupportedDbListProperty, value); }
        }
        public static readonly DependencyProperty SupportedDbListProperty =
            DependencyProperty.Register("SupportedDbList", typeof(string[]), typeof(MainWindow), new PropertyMetadata(new string[] { "Empty" }));

        private DataAccessBase DataAccess { get; set; }

        DataSet data1;
        DataSet data2;


        private void Link_Click(object sender, RoutedEventArgs e)
        {
            data1 = data2;
            data2 = null;
            QueryProcess2 = 0;
            queryRe1.Inlines.Clear();
            queryRe1.Inlines.AddRange((sender as Hyperlink).Inlines.ToArray());
            queryRe2.Text = "No Data";
            compRe.Items.Clear();
            btnComp.IsEnabled = compRe.IsEnabled = false;
            get1.IsEnabled = true;
            GC.Collect();
        }

        private bool QueryAllTable(ref DataSet ds, TextBlock lb, Action callBackAction)
        {
            if(CheckCanQuery() && CheckSelectedTables())
            {
                data2 = null;
                GC.Collect();
                Stopwatch watch = new Stopwatch();
                watch.Start();
                lb.Text = "Querying...";
                ds = QueryAll(callBackAction);
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
            else
                return false;
        }

        private bool CheckCanQuery()
        {
            if(changeMode.IsChecked == true)
            {
                if(string.IsNullOrEmpty(connStr.Text))
                {
                    topMsg.ShowMessage($"Connection string is empty!");
                    return false;
                }
            }
            return true;
        }

        private bool CheckSelectedTables()
        {
            if(TableNames.Length > 0)
                if(((isBlackList.IsChecked ?? false) && SelectedTableName.Count == TableNames.Length) ||
                   ((!isBlackList.IsChecked) ?? false) && SelectedTableName.Count == 0)
                {
                    topMsg.ShowMessage("No tables available for query!");
                    return false;
                }
            return true;
        }

        //Provider=SQLOLEDB.1;Server=127.0.0.1,51433;Database=RMDBFD;User Id=sa;Password=sa
        private DataSet QueryAll(Action callBackAction)
        {
            QueryTablesFix = QueryTables.ToArray();
            DataSet d = new DataSet();
            try
            {
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background,
                    new Action<string[]>((tns) =>
                    {
                        d = DataAccess.QueryTables(tns, true, callBackAction);
                    }), QueryTablesFix);
            }
            catch(Exception ex)
            {
                MessageBox.Show($"{ex.Message}\r\n{ex.StackTrace}\r\n==========\r\n{ex.InnerException}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return d;
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
                    var tdic = CompareData();
                    FillCompareResult(tdic);
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
                }));
                GC.Collect();
            }
        }

        private DataEntity[] CompareData()
        {
            #region waitMove
            var ts1 = data1.Tables.Cast<DataTable>().Where(t => t.PrimaryKey?.Length > 0);
            var ts2 = data2.Tables.Cast<DataTable>().Where(t => t.PrimaryKey?.Length > 0);
            var tg = ts1.Zip(ts2, Tuple.Create);
            var tdic = tg.Select(g => new DataEntity(g.Item1, g.Item2, EachDataEntityCreateCallBack))
            .Where(t => t.UpDatedData.Count > 0 || t.InsertRows.Count > 0 || t.DeleteRows.Count > 0).ToArray();
            return tdic;
            #endregion
        }

        private void FillCompareResult(DataEntity[] tdic)
        {
            if(tdic.Length == 0)
                compReState.Text = "Not found any changes in selected tables";
            else
            {
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
                        }).ToArray(),
                        InsertedDatas = t.InsertRows.Select(kvp => new InsertedData
                        {
                            UniquePrimaryKey = kvp.Key,
                            Datas = kvp.Value.Table.Columns.Cast<DataColumn>()
                                .ToDictionary(c => c.ColumnName, c => kvp.Value[c])
                        }).ToArray(),
                        DeletedDatas = t.DeleteRows.Select(kvp => new DeletedData
                        {
                            UniquePrimaryKey = kvp.Key,
                            Datas = kvp.Value.Table.Columns.Cast<DataColumn>()
                                .ToDictionary(c => c.ColumnName, c => kvp.Value[c])
                        }).ToArray()
                    };

                    var dataItem = new DataTableListItem(z);
                    compRe.Items.Add(dataItem);
                    sb.AppendLine($"{z.TableName}:");
                    sb.AppendLine(JsonConvert.SerializeObject(z, Formatting.Indented));
                    sb.AppendLine("=================================================");
                    CompareProcessCallBack();
                }
                tb.Text = sb.ToString();
                w.Show();
            }
        }

        private void EachDataEntityCreateCallBack()
        {

        }

        private void reset_Click(object sender, RoutedEventArgs e)
        {
            //connStr.Text = string.Empty;
            if(data1 == null)
            {
                topMsg.ShowMessage("All Reset!", 1000);
            }
            else
            {
                data1 = null;
                data2 = null;
                QueryProcess1 = QueryProcess2 = CompareProcess = 0;
                QueryProcess1Count = QueryProcess2Count = CompareProcessCount = 0;
                compRe.Items.Clear();
                SelectedTableName.Clear();
                filterTableList.SelectedItems.Clear();
                TableNames = new string[0];
                queryRe1.Text = queryRe2.Text = "No Data";
                compReState.Text = "Not Start Compare";
                showFilter.IsEnabled = get1.IsEnabled = get2.IsEnabled = btnComp.IsEnabled = compRe.IsEnabled = false;
                topMsg.ShowMessage("Data Has Been Cleared!", 1000);
            }
            GC.Collect();
        }

        private string[] QueryTablesFix { get; set; }

        private IEnumerable<string> QueryTables
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

        private string[] TableNames { get; set; } = new string[0];

        private HashSet<string> SelectedTableName { get; } = new HashSet<string>();

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

        private void changeMode_Checked(object sender, RoutedEventArgs e)
        {
            topMsg.ShowMessage("Connection String Mode: Custom Mode", 1000);
        }

        private void changeMode_Unchecked(object sender, RoutedEventArgs e)
        {
            topMsg.ShowMessage("Connection String Mode: Field Mode", 1000);
        }
    }
}
