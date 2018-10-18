using ComparisonLib;
using EasyDatabaseCompare.UserControls;
using EasyDatabaseCompare.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace EasyDatabaseCompare
{
    public partial class MainWindow : Window
    {
        public MainWindow(IWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            ViewModel = viewModel;
            viewModel.PropertyChanged += DataCache_PropertyChanged;
            DetailDataOverview.Columns.CollectionChanged += DetailDataOverviewColumns_CollectionChanged;
            //SupportedDbList = ComparisonLib.DataAccessorInfo.SupportedDbConnectionStringFields.Keys;
            //SupportedDbList = OldDataAccessFactory.DbTypes;
            //var t = new System.Timers.Timer(1000);
            //t.Elapsed += (s, e) =>
            //{
            //    this.Dispatcher.Invoke(new Action(() =>
            //    {
            //        Console.WriteLine($"popDataTable.IsFocused: {popDataTable.IsFocused} | popDataTable.IsMouseCaptured: {popDataTable.IsMouseCaptured} | popDataTable.IsMouseCaptureWithin: {popDataTable.IsMouseCaptureWithin}");
            //        Console.WriteLine(Mouse.Captured);
            //    }), System.Windows.Threading.DispatcherPriority.Background);
            //};
            //t.Start();

            // System.Windows.Threading.Dispatcher..Invoke(() => { }, System.Windows.Threading.DispatcherPriority.Background);
        }


        private IWindowViewModel ViewModel { get; }
        private void DataCache_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ConnectionChecked":
                    if (ViewModel.ConnectionChecked) TopMsg.ShowMessage("Connection Test Success!");
                    SelectedRow1 = null;
                    SelectedRow2 = null;
                    ViewModel.SelectedTables.Clear();
                    FilterStr.Text = "";
                    filterStr_TextChanged(null, null);
                    break;
                case "CustomConnectionStringMode":
                    TopMsg.ShowMessage(
                        ViewModel.CustomConnectionStringMode
                            ? "Connection String Mode: Custom Mode"
                            : "Connection String Mode: Field Mode", 1000);
                    //private void changeMode_Checked(object sender, RoutedEventArgs e)
                    //{
                    //    TopMsg.ShowMessage("Connection String Mode: Custom Mode", 1000);
                    //}

                    //private void changeMode_Unchecked(object sender, RoutedEventArgs e)
                    //{
                    //    TopMsg.ShowMessage("Connection String Mode: Field Mode", 1000);
                    //}
                    break;
                case "FilterString":
                    if (!(CompOverview.ItemsSource is DataView dataView)) return;
                    if (string.IsNullOrWhiteSpace(ViewModel.FilterString))
                    {
                        dataView.RowFilter = null;
                        return;
                    }
                    dataView.RowFilter = $"table_name LIKE '%{ViewModel.FilterString}%'";
                    break;
                case "ShowSameColumn":
                case "ShowInsertColumn":
                case "ShowDeleteColumn":
                case "ShowChangedColumn":
                case "FilteredComparerResultOverview":
                case "HideEmptyTables":
                case "HideUnchangedTables":
                case "OverviewSelectedCellInfo":
                case "ComparerResultOverview":
                case "SourceData":
                case "TargetData":
                case "SelectedDbType":
                case "SelectedTables":
                case "BlackListMode":
                case "TableNames":
                case "DbConnectionFields":
                case "DataCompareResult":
                case "CanQuerySource":
                case "CanQueryTarget":
                case "Querier":
                case "CanStartComparer":
                case "CanSelectTable":
                case "Fields":
                    break;
            }
        }
        private void CompareOverviewColumnHidding(string columnName, bool columnIsHide)
        {
            if (CompOverview.Columns.Count <= 0) return;
            var col = CompOverview.Columns.First(c => c.Header.ToString() == columnName);
            if (col != null) col.Visibility = columnIsHide ? Visibility.Collapsed : Visibility.Visible;
        }

        public bool ShowSameColumnProxy { get; }
        public bool ShowChangedColumnProxy { get; }
        public bool ShowInsertedColumnProxy { get; }
        public bool ShowDeletedColumnProxy { get; }

        private void DataGrid_TargetUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            var dg = e.Source as DataGrid;
            //switch ((dg?.ItemsSource as DataView)?.Table.TableName)
            //{
            //    case "Same":
            //        break;
            //    case "Changed":
            //        ;
            //        var dataRows = dg.Items.Cast<DataRowView>().Select(rv => rv.Row);
            //        var rows = dataRows.Select(item => dg.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow);
            //        var groupRows = rows.Where((r, index) => Convert.ToBoolean(index & 1)).Zip(rows.Where((r, index) => Convert.ToBoolean(index & 0)), (source, target) => (source, target));
            //        break;
            //    case "Inserted":
            //        break;
            //    case "Deleted":
            //        break;
            //    default:
            //        break;
            //}
        }
        private void DetailDataOverview_AutoGeneratedColumns(object sender, EventArgs e)
        {
            //DetailDataOverview.Items.CurrentChanging += Items_CurrentChanging;
            //var style = FindResource("DisplayNullDataGridTextColumnStyle") as Style;
            //var dg = sender as DataGrid;
            //foreach (var dataGridColumn in dg.Columns)
            //{
            //    if (!(dataGridColumn is DataGridTextColumn textColumn)) continue;

            //    textColumn.ElementStyle = style;
            //    textColumn.Binding.TargetNullValue = "NULL";
            //}
        }

        private void Items_CurrentChanging(object sender, CurrentChangingEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void DetailDataOverview_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (!(e.Column is DataGridBoundColumn boundColumn)) return;
            //boundColumn.Binding.TargetNullValue = "NULL";
            var table = ((sender as DataGrid).ItemsSource as DataView).Table;
            var col = table.Columns[(boundColumn.Binding as Binding).Path.Path];
            e.Column = new DataGridAutoFitColumn
            {
                Header = boundColumn.Header,
                Binding = boundColumn.Binding,
                ColumnContextType = col.DataType,
                ParentDataGrid = DetailDataOverview
            };
            if (!col.DataType.IsArray)
                //    newCol.HighlightString = DetailTableFilter.Text;
                //e.Column = newCol;
                BindingOperations.SetBinding(e.Column, DataGridAutoFitColumn.HighlightStringProperty,
                    new Binding(nameof(TextBox.Text))
                    {
                        Source = DetailTableFilter,
                        Mode = BindingMode.OneWay
                    });
            //if (e.Column is DataGridTextColumn textColumn)
            //    textColumn.Binding.TargetNullValue = "NULL";
            //if(e.Column is DataGridCheckBoxColumn checkBoxColumn)
        }

        private void TableNameFilter_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var tb = sender as TextBox;
            var binding = BindingOperations.GetBindingExpression(tb, TextBox.TextProperty);
            switch (e.Key)
            {
                case Key.Enter:
                case Key.Escape:
                    if (e.Key == Key.Escape) tb.Text = string.Empty;
                    binding.UpdateSource();
                    break;
            }
        }

        private void DetailTableFilter_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                case Key.Escape:
                    var tb = sender as TextBox;
                    if (ViewModel.SelectedDetail == null) return;
                    //var cols = DetailDataOverview.Columns.Select(c => c.Header.ToString());
                    var cols = ViewModel.SelectedDetail.Columns.Cast<DataColumn>()
                        .Where(col => !col.DataType.IsArray)
                        .Select(col => col.ColumnName);
                    //ViewModel.SelectedDetail.Columns.Cast<DataColumn>()
                    //    .Where(col => col.DataType == typeof(string))
                    //    .Select(col => col.ColumnName);
                    var dv = DetailDataOverview.ItemsSource as DataView;
                    if (e.Key == Key.Escape) tb.Text = string.Empty;
                    try
                    {
                        if (string.IsNullOrEmpty(tb.Text))
                        {
                            dv.RowFilter = string.Empty;
                        }
                        else if (DetailFilterMode.IsChecked.Value)
                            try
                            {
                                dv.RowFilter = tb.Text;
                            }
                            catch (Exception)
                            {
                                TopMsg.ShowMessage("Non-compliant \"where\" clause");
                            }
                        else if (tb.Text.ToLower() == "`null")
                            dv.RowFilter = string.Join(" OR ", cols.Select(col => $"CONVERT({col}, System.String) like '%{tb.Text}%' OR {col} is null"));
                        else if (tb.Text == "` ")
                            dv.RowFilter = string.Join(" OR ", cols.Select(col => $"CONVERT({col}, System.String) like '%{tb.Text}%' OR CONVERT({col}, System.String) = ''"));
                        else
                            dv.RowFilter = string.Join(" OR ", cols.Select(col => $"CONVERT({col}, System.String) like '%{tb.Text}%'"));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{ex.Message}\r\n{ex.StackTrace}\r\n==========\r\n{ex.InnerException}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
            }
        }

        private void DetailDataOverviewColumns_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null || e.OldItems == null) return;
            foreach (DataGridColumn item in e.OldItems)
                BindingOperations.ClearAllBindings(item);
        }


        private void DetailOverviewHeaderClickEventHandle(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;
            var colHeader = sender as DataGridColumnHeader;
            var colName = colHeader.Column.Header.ToString();
            try
            {
                Clipboard.SetText(colName);
                TopMsg.ShowMessage($"Column name \"{colName}\" copied!");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                TopMsg.ShowMessage("Column name copy failed! ");
            }
        }


        #region Fast compare 2 row
        DataRow SelectedRow1 { get; set; }
        DataRow SelectedRow2 { get; set; }

        private void PickFirst(object sender, RoutedEventArgs e)
        {
            if (DetailDataOverview.Items.Count == 0)
            {
                TopMsg.ShowMessage("Table is empty!");
                return;
            }
            if (DetailDataOverview.CurrentItem == null)
            {
                TopMsg.ShowMessage("Please select a row!");
                return;
            }
            SelectedRow1 = (DetailDataOverview.CurrentItem as DataRowView).Row;
        }
        private void PickSecondAndCompare(object sender, RoutedEventArgs e)
        {
            if (DetailDataOverview.Items.Count == 0)
            {
                TopMsg.ShowMessage("Table is empty!");
                return;
            }
            if (SelectedRow1 == null)
            {
                TopMsg.ShowMessage("Please pick a row at first!");
                return;
            }
            if (DetailDataOverview.CurrentItem == null)
            {
                TopMsg.ShowMessage("Please select a row!");
                return;
            }
            SelectedRow2 = (DetailDataOverview.CurrentItem as DataRowView).Row;

            var col1 = SelectedRow1.Table.Columns.Cast<DataColumn>().Select(c => c.ColumnName);
            var col2 = SelectedRow2.Table.Columns.Cast<DataColumn>().Select(c => c.ColumnName);
            if (!col1.SequenceEqual(col2))
            {
                TopMsg.ShowMessage("These two rows are not from the same table!");
                SelectedRow1 = null;
                SelectedRow2 = null;
                return;
            }
            var diffs = DataRowCellComparer.GetDiffFields(SelectedRow1, SelectedRow2).ToArray();
            var fastCompareTable = SelectedRow1.Table.Clone();
            fastCompareTable.TableName = "Changed";
            fastCompareTable.PrimaryKey = new DataColumn[0];
            foreach (var dataColumn in fastCompareTable.Columns.Cast<DataColumn>())
                dataColumn.ReadOnly = false;
            var newRow = fastCompareTable.NewRow();
            newRow.ItemArray = SelectedRow1.ItemArray;
            fastCompareTable.Rows.Add(newRow);
            newRow.AcceptChanges();
            foreach (var diff in diffs)
                newRow[diff] = SelectedRow2[diff];
            ViewModel.DiffFields = new Dictionary<DataRow, string[]> { { newRow, diffs } };
            ViewModel.SelectedDetail = fastCompareTable;
        }

        #endregion
        //public IEnumerable<string> SupportedDbList
        //{
        //    get => (IEnumerable<string>)GetValue(SupportedDbListProperty);
        //    set => SetValue(SupportedDbListProperty, value);
        //}
        //public static readonly DependencyProperty SupportedDbListProperty =
        //    DependencyProperty.Register("SupportedDbList", typeof(IEnumerable<string>), typeof(MainWindow), new PropertyMetadata(default(IEnumerable<string>)));

        //private List<TextBox> Fields { get; } = new List<TextBox>();

        //private OldDataAccessBase OldDataAccess { get; set; }


        //private void dbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (e.AddedItems.Count == 0 || e.AddedItems[0].ToString() == "Empty") return;
        //    Fields.Clear();
        //    FieldsGrid.Children.Clear();
        //    FieldsGrid.ColumnDefinitions.Clear();
        //    //var fields = OldDataAccessFactory.GetDbTypeConnectionFields(e.AddedItems[0].ToString());
        //    var fields = ComparisonLib.DataAccessorInfo.SupportedDbConnectionStringFields[e.AddedItems[0].ToString()];
        //    var i = 0;
        //    var tbTemplate = FindResource("fieldTextBoxTemp") as ControlTemplate;
        //    foreach (var f in fields)
        //    {
        //        FieldsGrid.ColumnDefinitions.Add(new ColumnDefinition());
        //        var tb = new TextBox { Tag = f, Template = tbTemplate };
        //        tb.SetValue(Grid.ColumnProperty, i++);
        //        Fields.Add(tb);
        //        FieldsGrid.Children.Add(tb);
        //    }
        //}

        //private void checkConn_Click(object sender, RoutedEventArgs e)
        //{
        //    if (!CheckCanQuery()) return;
        //    try
        //    {
        //        OldDataAccess = ChangeMode.IsChecked == true ?
        //            OldDataAccessFactory.Create(DbType.Text, ConnStr.Text) :
        //            OldDataAccessFactory.Create(DbType.Text, Fields.Select(f => f.Text).ToArray());

        //        TableNames = OldDataAccess.QueryAllTableName().ToArray();
        //        SelectedTableName.Clear();
        //        _queryTablesFix = null;

        //        ChangeMode.IsEnabled = false;

        //        TopMsg.ShowMessage("Connection Test Success!");

        //        filterStr_TextChanged(null, null);
        //        ShowFilter.IsEnabled = Get1.IsEnabled = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"{ex.Message}\r\n{ex.StackTrace}\r\n==========\r\n{ex.InnerException}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        //#region Get First Time
        //private double _queryProcess1Count;
        //public double QueryProcess1
        //{
        //    get => (double)GetValue(QueryProcess1Property);
        //    set => SetValue(QueryProcess1Property, value);
        //}
        //public static readonly DependencyProperty QueryProcess1Property =
        //    DependencyProperty.Register("QueryProcess1", typeof(double), typeof(MainWindow), new PropertyMetadata(0.0));
        //private void QueryProcess1CallBack()
        //{
        //    Dispatcher.BeginInvoke(new Action(() =>
        //    {
        //        SetValue(QueryProcess1Property, ++_queryProcess1Count / QueryTablesFix.Length);
        //    }), System.Windows.Threading.DispatcherPriority.Background);
        //}
        //private void get1_Click(object sender, RoutedEventArgs e)
        //{
        //    Get1.IsEnabled = false;
        //    _queryProcess1Count = 0;
        //    if (QueryAllTable(ref data1, QueryRe1, QueryProcess1CallBack))
        //    {
        //        Get2.IsEnabled = true;
        //        ShowFilter.IsEnabled = false;


        //        TableNamesMenu.DataContext = QueryTablesFix;
        //        //tableNames.Items.Clear();
        //        //foreach(var tn in QueryTablesFix.OrderBy(s => s))
        //        //{
        //        //    var mi = new MenuItem { Header = tn, StaysOpenOnClick = true };
        //        //    //var lbi = new ListBoxItem { Content = tn };
        //        //    //lbi.MouseUp += Lbi_MouseUp;
        //        //    //tableNames.Items.Add(lbi);
        //        //}
        //    }
        //    Get1.IsEnabled = true;
        //}

        ////private void Lbi_MouseUp(object sender, MouseButtonEventArgs e)
        ////{
        ////    popDataTable.IsOpen = false;
        ////    var item = sender as ListBoxItem;
        ////    dataTableGrid.DataContext = (tableNames.Tag as DataSet).Tables[item.Content.ToString()];
        ////    popDataTable.PlacementTarget = item as UIElement;
        ////    popDataTable.IsOpen = true;
        ////    //item.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        ////}
        ////private void popDataTable_Opened(object sender, EventArgs e)
        ////{
        ////    Mouse.Capture(dataTableGrid, CaptureMode.SubTree);
        ////    Console.WriteLine(Mouse.Captured);
        ////    dataTableGrid.ReleaseMouseCapture();
        ////    Console.WriteLine(Mouse.Captured);
        ////}
        ////private void popDataTable_GotMouseCapture(object sender, MouseEventArgs e)
        ////{
        ////    dataTableGrid.Focus();
        ////    Console.WriteLine(Mouse.Captured);
        ////    dataTableGrid.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
        ////    {
        ////        RoutedEvent = MouseUpEvent,
        ////        Source = dataTableGrid
        ////    });
        ////    Console.WriteLine(Mouse.Captured);
        ////}

        ////private void popDataTable_LostMouseCapture(object sender, MouseEventArgs e)
        ////{
        ////    //this.CaptureMouse();
        ////    //dataTableGrid.ReleaseMouseCapture();
        ////}
        //#endregion

        //#region Get Second Time
        //private double _queryProcess2Count;
        //public double QueryProcess2
        //{
        //    get => (double)GetValue(QueryProcess2Property);
        //    set => SetValue(QueryProcess2Property, value);
        //}
        //public static readonly DependencyProperty QueryProcess2Property =
        //    DependencyProperty.Register("QueryProcess2", typeof(double), typeof(MainWindow), new PropertyMetadata(0.0));
        //private void QueryProcess2CallBack()
        //{
        //    Dispatcher.BeginInvoke(new Action(() =>
        //    {
        //        SetValue(QueryProcess2Property, ++_queryProcess2Count / QueryTablesFix.Length);
        //    }), System.Windows.Threading.DispatcherPriority.Background).Wait();
        //}
        //private void get2_Click(object sender, RoutedEventArgs e)
        //{
        //    Get1.IsEnabled = Get2.IsEnabled = false;
        //    _queryProcess2Count = 0;
        //    if (QueryAllTable(ref data2, QueryRe2, QueryProcess2CallBack))
        //    {
        //        CompReState.Text = "Not Start Compare";
        //        BtnComp.IsEnabled = CompReL.IsEnabled = true;
        //    }
        //    else
        //        Get1.IsEnabled = true;
        //    Get2.IsEnabled = true;
        //}
        //#endregion

        //#region Compare Process
        //private double CompareProcessCount = 0;
        //public double CompareProcess
        //{
        //    get => (double)GetValue(CompareProcessProperty);
        //    set => SetValue(CompareProcessProperty, value);
        //}
        //public static readonly DependencyProperty CompareProcessProperty =
        //    DependencyProperty.Register("CompareProcess", typeof(double), typeof(MainWindow), new PropertyMetadata(0.0));
        //private void CompareProcessCallBack()
        //{
        //    Dispatcher.BeginInvoke(new Action(() =>
        //    {
        //        SetValue(CompareProcessProperty, ++CompareProcessCount / QueryTablesFix.Length);
        //    }), System.Windows.Threading.DispatcherPriority.Background).Wait();
        //}
        //#endregion


        //DataSet data1;
        //DataSet data2;


        //private void MoveLeft_Click(object sender, RoutedEventArgs e)
        //{
        //    if (data2 == null) return;
        //    data1 = data2;
        //    data2 = null;
        //    QueryProcess2 = 0;
        //    QueryRe1.Inlines.Clear();
        //    QueryRe1.Inlines.AddRange(QueryRe2.Inlines.ToArray());
        //    QueryRe2.Text = "No Data";
        //    CompReL.Items.Clear();
        //    CompReT.Text = string.Empty;
        //    BtnComp.IsEnabled = CompReL.IsEnabled = false;
        //    Get1.IsEnabled = true;
        //}

        //private bool QueryAllTable(ref DataSet ds, TextBlock lb, Action callBackAction)
        //{
        //    if (CheckCanQuery() && CheckSelectedTables())
        //    {
        //        data2 = null;

        //        Stopwatch watch = new Stopwatch();
        //        watch.Start();
        //        lb.Text = "Querying...";
        //        ds = QueryAll(callBackAction);
        //        lb.Inlines.Clear();
        //        lb.Inlines.Add("Queried ");
        //        lb.Inlines.Add(new Run(ds.Tables.Count.ToString()) { Foreground = Brushes.Red });
        //        lb.Inlines.Add(" Table and ");
        //        lb.Inlines.Add(new Run(ds.Tables.Cast<DataTable>().Select(t => t.Rows.Count).Sum().ToString()) { Foreground = Brushes.Red });
        //        lb.Inlines.Add(" Records.");
        //        lb.Inlines.Add("\r\n");
        //        watch.Stop();
        //        lb.Inlines.Add($"Query Spent Time: {watch.Elapsed}");
        //        //lb.Inlines.Add($"Query Finish Time: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffffff")}");
        //        return true;
        //    }
        //    else
        //        return false;
        //}

        //private bool CheckCanQuery()
        //{
        //    if (ChangeMode.IsChecked == true)
        //    {
        //        if (string.IsNullOrEmpty(ConnStr.Text))
        //        {
        //            TopMsg.ShowMessage("Connection string is empty!");
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        //private bool CheckSelectedTables()
        //{
        //    if (TableNames.Length > 0)
        //        if (((IsBlackList.IsChecked ?? false) && SelectedTableName.Count == TableNames.Length) ||
        //           ((!IsBlackList.IsChecked) ?? false) && SelectedTableName.Count == 0)
        //        {
        //            TopMsg.ShowMessage("No tables available for query!");
        //            return false;
        //        }
        //    return true;
        //}

        //private DataSet QueryAll(Action callBackAction)
        //{
        //    DataSet d = new DataSet();
        //    try
        //    {
        //        Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background,
        //            new Action<string[]>((tns) =>
        //            {
        //                d = OldDataAccess.QueryTables(tns, callBackAction);
        //            }), QueryTablesFix);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"{ex.Message}\r\n{ex.StackTrace}\r\n==========\r\n{ex.InnerException}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //    return d;
        //}

        //readonly Stopwatch watch = new Stopwatch();
        //private void StartActionWatch()
        //{
        //    watch.Restart();
        //}

        //private void btnComp_Click(object sender, RoutedEventArgs e)
        //{
        //    if (data1.Tables.Count > 0 && data2.Tables.Count > 0)
        //    {
        //        StartActionWatch();
        //        CompReL.Items.Clear();
        //        CompReT.Text = string.Empty;

        //        CompReState.Text = "Comparing...";

        //        StartCompareData();
        //    }
        //}

        //private void FinishCompare()
        //{
        //    Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background,
        //        new Action(() =>
        //        {
        //            if (StaData.Tables == 0)
        //            {
        //                CompReState.Text = "Not found any changes in selected tables";
        //                return;
        //            }
        //            CompReState.Inlines.Clear();
        //            CompReState.Inlines.Add("Compare found ");
        //            CompReState.Inlines.Add(new Run(StaData.Tables.ToString())
        //            {
        //                Foreground = Brushes.Red
        //            });
        //            CompReState.Inlines.Add(" Table and ");
        //            CompReState.Inlines.Add(new Run(StaData.Update.ToString())
        //            {
        //                Foreground = Brushes.Red
        //            });
        //            CompReState.Inlines.Add(" update Records, ");
        //            CompReState.Inlines.Add(new Run(StaData.Insert.ToString()) { Foreground = Brushes.Red });
        //            CompReState.Inlines.Add(" insert Records, ");
        //            CompReState.Inlines.Add(new Run(StaData.Delete.ToString()) { Foreground = Brushes.Red });
        //            CompReState.Inlines.Add(" delete Records");
        //            CompReState.Inlines.Add("\r\n");
        //            watch.Stop();
        //            CompReState.Inlines.Add($"Compare Spent Time: {watch.Elapsed}");
        //            CompReT.Text = StaData.jsonString.ToString();
        //        }));
        //}

        ////private void StartCompareData()
        ////{
        ////    CompareProcess = 0;
        ////    var ts1 = data1.Tables.Cast<DataTable>().Where(t => t.PrimaryKey.Length > 0);
        ////    var ts2 = data2.Tables.Cast<DataTable>().Where(t => t.PrimaryKey.Length > 0);
        ////    var ts1_Npk = data1.Tables.Cast<DataTable>().Where(t => t.PrimaryKey.Length == 0);
        ////    var ts2_Npk = data2.Tables.Cast<DataTable>().Where(t => t.PrimaryKey.Length == 0);
        ////    var tg = ts1.Zip(ts2, Tuple.Create);
        ////    var tg_Npk = ts1_Npk.Zip(ts2_Npk, Tuple.Create);
        ////    //var tdic = tg.Select(g =>
        ////    //    {
        ////    //        CompareProcessCallBack();
        ////    //        return new DataEntity(g.Item1, g.Item2, EachDataEntityCreateCallBack);
        ////    //    })
        ////    //    .Concat(tg_Npk.Select(g =>
        ////    //    {
        ////    //        CompareProcessCallBack();
        ////    //        return new DataEntity(g.Item1, g.Item2, EachDataEntityCreateCallBack, true);
        ////    //    }))
        ////    //.Where(t => t.UpDatedData.Count > 0 || t.InsertRows.Count > 0 || t.DeleteRows.Count > 0).ToArray();

        ////    //var tgO = tg.ToObservable();
        ////    var tgO_Npk = tg_Npk.ToObservable();
        ////    var obtg = tg.Select(ob => Observable.Start(() => new DataEntity(ob.Item1, ob.Item2, EachDataEntityCreateCallBack), ThreadPoolScheduler.Instance)).ToObservable().Merge();
        ////    var obtg_Npk = tg_Npk.Select(ob => Observable.Start(() => new DataEntity(ob.Item1, ob.Item2, EachDataEntityCreateCallBack, true), ThreadPoolScheduler.Instance)).ToObservable().Merge();
        ////    StaData = new StatisticalData();
        ////    obtg.Merge(obtg_Npk)
        ////    .Where(t => t.UpDatedData.Count > 0 || t.InsertRows.Count > 0 || t.DeleteRows.Count > 0)
        ////        .Subscribe(FillCompareResult, FinishCompare);
        ////}

        //private class StatisticalData
        //{
        //    public int Tables { get; set; }
        //    public int Update { get; set; }
        //    public int Insert { get; set; }
        //    public int Delete { get; set; }
        //    public StringBuilder jsonString { get; set; } = new StringBuilder();
        //}

        //private StatisticalData StaData;
        //private void FillCompareResult(DataEntity t)
        //{
        //    CompareProcessCallBack();
        //    var z = new DatabaseChangeContent
        //    {
        //        TableName = t.TableName,
        //        UpdatedDatas = t.UpDatedData.Select(kvp => new UpdatedData
        //        {
        //            UniquePrimaryKey = kvp.Key,
        //            UpdatedFields = kvp.Value.DiffFields.Select(diff => new UpdatedField
        //            {
        //                ColumnName = diff.Key.ColumnName,
        //                OldValue = diff.Value[0],
        //                NewValue = diff.Value[1]
        //            })
        //        }).ToArray(),
        //        InsertedDatas = t.InsertRows.Select(kvp => new InsertedData
        //        {
        //            UniquePrimaryKey = kvp.Key,
        //            Datas = kvp.Value.Table.Columns.Cast<DataColumn>()
        //                .ToDictionary(c => c.ColumnName, c => kvp.Value[c])
        //        }).ToArray(),
        //        DeletedDatas = t.DeleteRows.Select(kvp => new DeletedData
        //        {
        //            UniquePrimaryKey = kvp.Key,
        //            Datas = kvp.Value.Table.Columns.Cast<DataColumn>()
        //                .ToDictionary(c => c.ColumnName, c => kvp.Value[c])
        //        }).ToArray()
        //    };
        //    StaData.Tables += 1;
        //    StaData.Update += z.UpdatedDatas.Length;
        //    StaData.Insert += z.InsertedDatas.Length;
        //    StaData.Delete += z.DeletedDatas.Length;
        //    Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background,
        //    new Action(() =>
        //    {
        //        var dataItem = new DataTableListItem(z);
        //        dataItem.TableNameClickEvent += CopyTableName;
        //        dataItem.ShowTableData += DataItem_ShowTableData;
        //        CompReL.Items.Add(dataItem);
        //        StaData.jsonString.AppendLine($"{z.TableName}:");
        //        StaData.jsonString.AppendLine(JsonConvert.SerializeObject(z, Formatting.Indented));
        //        StaData.jsonString.AppendLine("=================================================");
        //    }));
        //    //CompareProcessCallBack();
        //    //var w = new Window();
        //    //var tb = new TextBox()
        //    //{
        //    //    Margin = new Thickness(0),
        //    //    Padding = new Thickness(0),
        //    //    TextWrapping = TextWrapping.Wrap,
        //    //    IsReadOnly = true,
        //    //    VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        //    //};
        //    //w.Content = tb;
        //    //tb.Text = sb.ToString();
        //    //w.Show();
        //}

        //private void DataItem_ShowTableData(UIElement sender, bool isOld, string tableName)
        //{
        //    DataTableGrid.DataContext = isOld ? data1.Tables[tableName] : data2.Tables[tableName];
        //    PopDataTable.PlacementTarget = sender;
        //    PopDataTable.IsOpen = true;
        //}

        //private void CopyTableName(object sender, HandledEventArgs e)
        //{
        //    if (e.Handled)
        //        TopMsg.ShowMessage("Table name copied!", 1000);
        //    else
        //        TopMsg.ShowMessage("Table name copy failed!", 1000);
        //}
        //private void EachDataEntityCreateCallBack()
        //{

        //}

        //private void reset_Click(object sender, RoutedEventArgs e)
        //{
        //    //connStr.Text = string.Empty;
        //    if (data1 == null)
        //    {
        //        Fields.ForEach(f => f.Text = string.Empty);
        //        ConnStr.Text = string.Empty;
        //        ChangeMode.IsEnabled = true;
        //        TopMsg.ShowMessage("All Reset!", 1000);
        //    }
        //    else
        //    {
        //        data1 = null;
        //        data2 = null;
        //        QueryProcess1 = QueryProcess2 = CompareProcess = 0;
        //        _queryProcess1Count = _queryProcess2Count = CompareProcessCount = 0;
        //        CompReL.Items.Clear();
        //        CompReT.Text = string.Empty;
        //        SelectedTableName.Clear();
        //        FilterTableList.SelectedItems.Clear();
        //        TableNames = new string[0];
        //        QueryRe1.Text = QueryRe2.Text = "No Data";
        //        CompReState.Text = "Not Start Compare";
        //        ShowFilter.IsEnabled = Get1.IsEnabled = Get2.IsEnabled = BtnComp.IsEnabled = CompReL.IsEnabled = false;
        //        TopMsg.ShowMessage("Data Has Been Cleared!", 1000);
        //    }

        //}

        //private string[] QueryTablesFix => _queryTablesFix ?? (_queryTablesFix = QueryTables.ToArray());

        //private string[] _queryTablesFix;
        //private IEnumerable<string> QueryTables
        //{
        //    get
        //    {
        //        if (IsBlackList.IsChecked ?? false)
        //            return TableNames.Except(SelectedTableName);
        //        else
        //            return SelectedTableName;
        //    }
        //}

        //#region Properties For View

        //#endregion

        //private string[] TableNames { get; set; } = new string[0];

        //private HashSet<string> SelectedTableName { get; } = new HashSet<string>();

        bool _onFilter;
        private void filterStr_TextChanged(object sender, TextChangedEventArgs e)
        {
            _onFilter = true;
            if (FilterTableList.Tag is IList<string> TableNames)
            {
                var filterList = FilterStr.Text.Contains('_') ?
                    TableNames.Where(str => str.ToLower().Contains(FilterStr.Text.ToLower())) :
                    TableNames.Where(str => str.Replace("_", "").ToLower().Contains(FilterStr.Text.ToLower()));
                FilterTableList.ItemsSource = filterList;
                var intersectList = ViewModel.SelectedTables.Intersect(filterList).ToList();
                intersectList.ForEach(str => FilterTableList.SelectedItems.Add(str));
            };
            _onFilter = false;
        }
        private void filterTableList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.AddedItems.Cast<string>().ToList().ForEach(str => ViewModel.SelectedTables.Add(str));
            if (!_onFilter) e.RemovedItems.Cast<string>().ToList().ForEach(str => ViewModel.SelectedTables.Remove(str));
        }

        //private void changeMode_Checked(object sender, RoutedEventArgs e)
        //{
        //    TopMsg.ShowMessage("Connection String Mode: Custom Mode", 1000);
        //}

        //private void changeMode_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    TopMsg.ShowMessage("Connection String Mode: Field Mode", 1000);
        //}
        //private void DisplayTypeChecked(object sender, RoutedEventArgs e)
        //{
        //    if (RowList == null || RowResize == null || RowText == null) return;
        //    RowResize.Height = new GridLength(0, GridUnitType.Auto);
        //    RowText.Height = RowList.Height = new GridLength(1, GridUnitType.Star);
        //}
        //private void DisplayTypeUnchecked(object sender, RoutedEventArgs e)
        //{
        //    RowResize.Height = new GridLength(0);
        //    if (sender == IsShowList)
        //    {
        //        RowText.Height = new GridLength(1, GridUnitType.Star);
        //        RowList.Height = new GridLength(0);
        //    }
        //    else
        //    {
        //        RowText.Height = new GridLength(1, GridUnitType.Star);
        //        RowText.Height = new GridLength(0);
        //    }

        //    if (!(IsShowList.IsChecked.Value || IsShowText.IsChecked.Value))
        //        if (sender == IsShowList)
        //            IsShowText.IsChecked = true;
        //        else
        //            IsShowList.IsChecked = true;
        //}

        //private void showTableMenu(object sender, RoutedEventArgs e)
        //{
        //    if (sender == ShowOldTableList && data1 != null)
        //    {
        //        TableNamesMenu.PlacementTarget = ShowOldTableList;
        //        TableNamesMenu.Tag = data1;
        //        TableNamesMenu.IsOpen = true;
        //    }
        //    else if (sender == ShowNewTableList && data2 != null)
        //    {
        //        TableNamesMenu.PlacementTarget = ShowNewTableList;
        //        TableNamesMenu.Tag = data2;
        //        TableNamesMenu.IsOpen = true;
        //    }
        //}
        //private void ignoreRClick(object sender, MouseButtonEventArgs e)
        //{
        //    if (e.ChangedButton != MouseButton.Left)
        //        e.Handled = true;
        //}
    }
}
