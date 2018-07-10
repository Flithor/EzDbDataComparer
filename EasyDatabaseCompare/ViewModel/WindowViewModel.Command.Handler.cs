using ComparisonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EasyDatabaseCompare.ViewModel
{
    public partial class WindowViewModel
    {
        public void Reset()
        {
            SourceData?.Dispose();
            TargetData?.Dispose();
            SourceData = null;
            TargetData = null;
            DataCompareResult = null;
            ConnectionChecked = false;
            SelectedTables.Clear();
            SelectedDetail = null;
            FilteredComparerResultOverview = null;
        }

        public void CheckConnection()
        {
            if (SelecctedDbType == null) throw new InvalidOperationException("SelecctedDbType Is Null!");
            Querier.CreateDataAccessor(SelecctedDbType, Fields.Select(info => info.FieldValue).ToArray());
            TableNames = Querier.TableNames.ToArray();
        }
        public void QuerySource()
        {
            if (SelectedTables == null) throw new InvalidOperationException("SelectedTables Is Null!");
            var process = 0D;
            var ttns = TargetTables;
            SourceData = new System.Data.DataSet();
            Querier.QueryMultiTable(ttns)
                .Do(t => QuerySourceProcess = ++process / ttns.Length)
                .Subscribe(t => SourceData.Tables.Add(t), () => CanQueryTarget = true);
        }
        public void QueryTarget()
        {
            if (SelectedTables == null) throw new InvalidOperationException("SelectedTables Is Null!");
            CanQuerySource = false;
            var process = 0D;
            var ttns = TargetTables;
            TargetData = new System.Data.DataSet();
            Querier.QueryMultiTable(ttns)
                .Do(t => QueryTargetProcess = ++process / ttns.Length)
                .Subscribe(t => TargetData.Tables.Add(t), () => CanStartComparer = true);
        }
        public void StartComparer()
        {
            if (SourceData.Tables.Count == (0 & TargetData.Tables.Count) ||
                SourceData.Tables.Count != TargetData.Tables.Count)
                throw new InvalidOperationException("Source or target snapshot is empty!!");
            var process = 0D;
            var ttns = TargetTables;
            DataCompareResult = new List<DataDiff>();
            ttns.ToObservable()
                .Select(tn => Comparator.Comparer(SourceData.Tables[tn], TargetData.Tables[tn]))
                .Do(t => ComparerProcess = ++process / ttns.Length)
                .Subscribe(diff => DataCompareResult.Add(diff),
                () =>
                {
                    //ComparerResultOverview = CreateComparerResultOverview(DataCompareResult);
                    FilteredComparerResultOverview = CreateFilteredComparerResultOverview(DataCompareResult);
                });
        }
        public void QueryTargetWithComparer()
        {
            var processQuery = 0D;
            var processComparer = 0D;
            var ttns = TargetTables;
            TargetData = new System.Data.DataSet();
            DataCompareResult = new List<DataDiff>();
            Querier.QueryMultiTable(ttns)
                .Do(t =>
                {
                    QueryTargetProcess = ++processQuery / ttns.Length;
                    ComparerProcess = ++processComparer / ttns.Length;
                    DataCompareResult.Add(Comparator.Comparer(SourceData.Tables[t.TableName], t));
                })
                .Subscribe(t => TargetData.Tables.Add(t),
                () =>
                {
                    //ComparerResultOverview = CreateComparerResultOverview(DataCompareResult);
                    FilteredComparerResultOverview = CreateFilteredComparerResultOverview(DataCompareResult);
                });
        }
        public void MoveTargetToSource()
        {
            SourceData = TargetData;
            TargetData.Dispose();
            TargetData = null;
            DataCompareResult = null;
            FilteredComparerResultOverview = null;
        }

        private void DisplayTargetDetail(object param)
        {
            var objs = param as object[];
            var columnName = objs[0].ToString();
            var tableName = objs[1].ToString();
            ;
            var selectDiff = DataCache.DataCompareResult.First(diff => diff.SourceTable.TableName == tableName);
            switch (columnName)
            {
                case "Same":
                    SelectedDetail = selectDiff.DisplayTables.SameData;
                    break;
                case "Changed":
                    SelectedDetail = selectDiff.DisplayTables.ChangedData;
                    break;
                case "Inserted":
                    SelectedDetail = selectDiff.DisplayTables.InsertedData;
                    break;
                case "Deleted":
                    SelectedDetail = selectDiff.DisplayTables.DeletedData;
                    break;
                case "Data In Source":
                    SelectedDetail = selectDiff.SourceTable;
                    break;
                case "Data In Target":
                    SelectedDetail = selectDiff.TargetTable;
                    break;
            }
        }

        //internal System.Data.DataTable CreateComparerResultOverview(IEnumerable<DataDiff> diffs)
        //{
        //    var ov = new System.Data.DataTable();
        //    ov.Columns.Add("Table Name", typeof(string));
        //    ov.Columns.Add("Same Datas Count", typeof(int));
        //    ov.Columns.Add("Change Datas Count", typeof(int));
        //    ov.Columns.Add("Insert Datas Count", typeof(int));
        //    ov.Columns.Add("Delete Datas Count", typeof(int));
        //    foreach (var diff in diffs)
        //        ov.Rows.Add(
        //            diff.SourceTable.TableName,
        //            diff.SameData.Count,
        //            diff.ChangedDatas.Count,
        //            diff.InsertedDatas.Count,
        //            diff.DeletedDatas.Count);
        //    return ov;
        //}
        internal System.Data.DataTable CreateFilteredComparerResultOverview(IEnumerable<DataDiff> diffs)
        {
            var ov = new System.Data.DataTable();
            ov.Columns.Add("Table_Name", typeof(string));
            ov.Columns.Add("Same", typeof(int));
            ov.Columns.Add("Changed", typeof(int));
            ov.Columns.Add("Inserted", typeof(int));
            ov.Columns.Add("Deleted", typeof(int));
            IEnumerable<DataDiff> filterDiff = diffs.Where(diff =>
            {
                //if hide emtpy tables, and check table is emtpy
                if (HideEmptyTables)
                    if (diff.SourceTable.Rows.Count == 0 && diff.TargetTable.Rows.Count == 0)
                        return false;
                //if hide unchanged tables, and check datadiff changeddatas
                if (HideUnchangedTables &&
                diff.ChangedDatas.Count == 0 &&
                diff.InsertedDatas.Count == 0 &&
                diff.DeletedDatas.Count == 0)
                    return false;
                //check ok
                return true;
            });

            foreach (var diff in filterDiff)
                ov.Rows.Add(
                    diff.SourceTable.TableName,
                    diff.SameData.Count,
                    diff.ChangedDatas.Count,
                    diff.InsertedDatas.Count,
                    diff.DeletedDatas.Count);
            return ov;
        }
    }
}
