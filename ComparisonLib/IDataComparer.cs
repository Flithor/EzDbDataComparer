using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;

namespace ComparisonLib
{
    public interface IDataComparer
    {
        DataDiff Comparer(DataTable source, DataTable target, bool isDeepCompar = false);
    }
    public class DataComparer : IDataComparer
    {
        public DataDiff Comparer(DataTable source, DataTable target, bool isDeepCompar = false)
        {
            #region Compare Columns
            var col1 = source.Columns.Cast<DataColumn>().Select(c => c.ColumnName);
            var col2 = target.Columns.Cast<DataColumn>().Select(c => c.ColumnName);
            if (!col1.SequenceEqual(col2))
                throw new NotSupportedException(
                    $"There are differences in the schema of these two DataTables.\r\n Table 1 |\t{string.Join("\t", col1)}\r\n Table 2 |\t{string.Join("\t", col2)}");
            #endregion

            var diff = new DataDiff { SourceTable = source, TargetTable = target };

            #region Skip Empty Table
            if ((source.Rows.Count | target.Rows.Count) == 0)
                return diff;
            #endregion

            #region Extract Rows
            var primaryKeys = source.PrimaryKey;

            var sourceRows = source.AsEnumerable().ToArray();
            var targetRows = target.AsEnumerable().ToArray();

            string GetUniquePrimaryKey(DataRow r)
            {
                return string.Join("_/_", primaryKeys.Select(pk => r[pk.ColumnName]));
            }

            //bool ComparerRowCells(DataRow r1, DataRow r2)
            //{
            //    if (r1 is null || r2 is null) return false;
            //    foreach (DataColumn c in r1.Table.Columns)
            //    {
            //        if (Equals(r1[c.ColumnName], r2[c.ColumnName]))
            //            continue;
            //        if (!c.DataType.IsArray) return false;
            //        if (!(r1[c.ColumnName] is IEnumerable<object> arr1) ||
            //            !(r2[c.ColumnName] is IEnumerable<object> arr2))
            //            return false;
            //        if (!arr1.SequenceEqual(arr2))
            //            return false;
            //    }
            //    return true;
            //}

            if (primaryKeys.Length == 0)
            {
                diff.DeletedDatas = sourceRows.Except(targetRows, DataRowCellComparer.Default).ToList();
                var InsertNChangedRows = targetRows.Except(sourceRows, DataRowCellComparer.Default);
                diff.InsertedDatas = InsertNChangedRows.ToList();
                diff.SameData = sourceRows.Intersect(targetRows, DataRowCellComparer.Default).ToList();
                //var sourceRowsList = sourceRows.ToList();
                //var targetRowsList = targetRows.ToList();
                //for (var i = 0; i < sourceRowsList.Count;)
                //{
                //    var sr = sourceRowsList[i];
                //    var last = targetRowsList.Count - 1;
                //    for (var j = 0; j < targetRowsList.Count;)
                //    {
                //        var tr = targetRowsList[j];
                //        var re = ComparerRowCells(sr, tr);
                //        if (re)
                //        {
                //            diff.SameData.Add(sr);
                //            sourceRowsList.RemoveAt(i);
                //            targetRowsList.RemoveAt(j);
                //            continue;
                //        }
                //        else if (j == last)
                //        {
                //            i++;
                //            j++;
                //            diff.ChangedDatas.Add((sr, tr));
                //        }
                //        else
                //        {
                //            j++;
                //        }
                //    }
                //}
            }
            else
            {
                var sourceDic = sourceRows.ToDictionary(GetUniquePrimaryKey, r => r);
                var targetDic = targetRows.ToDictionary(GetUniquePrimaryKey, r => r);

                diff.DeletedDatas = sourceRows.Except(targetRows, DataRowPrimaryKeyComparer.Default).ToList();
                diff.InsertedDatas = targetRows.Except(sourceRows, DataRowPrimaryKeyComparer.Default).ToList();

                var mayChangedRowsPrimaryKeys = sourceRows.Intersect(targetRows, DataRowPrimaryKeyComparer.Default)
                    .Select(GetUniquePrimaryKey);
                foreach (var key in mayChangedRowsPrimaryKeys)
                {
                    if (DataRowCellComparer.Default.Equals(sourceDic[key], targetDic[key]))
                        diff.SameData.Add(sourceDic[key]);
                    else
                        diff.ChangedDatas.Add((sourceDic[key], targetDic[key]));
                }
            }
            #endregion



            //var tempTable = source.Copy();
            //var changedTarget = target.Copy();
            //foreach (DataRow r in changedTarget.AsEnumerable())
            //{
            //    r.SetModified();
            //}
            //tempTable.Merge(changedTarget, true);
            ////var changed = tempTable.GetChanges();
            //tempTable.AsEnumerable().ToList().ForEach(r =>
            //{
            //    switch (r.RowState)
            //    {
            //        case DataRowState.Detached:
            //            //ignore
            //            break;
            //        case DataRowState.Unchanged:
            //            diff.SameData.Add(r);
            //            break;
            //        case DataRowState.Added:
            //            diff.InsertedDatas.Add(r);
            //            break;
            //        case DataRowState.Deleted:
            //            diff.DeletedDatas.Add(r);
            //            break;
            //        case DataRowState.Modified:
            //            diff.ChangedDatas.Add(r);
            //            break;
            //        default:
            //            throw new ArgumentOutOfRangeException();
            //    }
            //});

            return diff;
        }
    }

    public class DataDiff
    {

        public DataTable SourceTable { get; internal set; }
        public DataTable TargetTable { get; internal set; }
        public List<DataRow> DeletedDatas { get; internal set; } = new List<DataRow>();
        public List<DataRow> InsertedDatas { get; internal set; } = new List<DataRow>();
        public List<(DataRow RowInSource, DataRow RowInTarget)> ChangedDatas { get; internal set; } = new List<(DataRow RowInSource, DataRow RowInTarget)>();
        public List<DataRow> SameData { get; internal set; } = new List<DataRow>();

        private DisplayTables _displayTables;

        public DisplayTables DisplayTables => _displayTables ?? (_displayTables = new DisplayTables(this));
        private DisplayTables CreateDisplayTable()
        {
            return null;
        }
    }
    public class DisplayTables
    {
        private DataTable _sameData = null;
        private DataTable _insertedData = null;
        private DataTable _deletedData = null;
        private DataTable _changedData = null;

        public DisplayTables(DataDiff diff)
        {
            Parent = diff;
        }
        public DataDiff Parent { get; }
        public DataTable SameData
        {
            get
            {
                if (_sameData == null)
                {
                    _sameData = Parent.SourceTable.Clone();
                    _sameData.TableName = "Same";
                    Parent.SameData.ForEach(r => _sameData.Rows.Add(r.ItemArray));
                }
                return _sameData;
            }
        }
        public DataTable InsertedData
        {
            get
            {
                if (_insertedData == null)
                {
                    _insertedData = Parent.SourceTable.Clone();
                    _insertedData.TableName = "Inserted";
                    Parent.InsertedDatas.ForEach(r => _insertedData.Rows.Add(r.ItemArray));
                }
                return _insertedData;
            }
        }
        public DataTable DeletedData
        {
            get
            {
                if (_deletedData == null)
                {
                    _deletedData = Parent.SourceTable.Clone();
                    _deletedData.TableName = "Deleted";
                    Parent.DeletedDatas.ForEach(r => _deletedData.Rows.Add(r.ItemArray));
                }
                return _deletedData;
            }
        }
        public DataTable ChangedData
        {
            get
            {
                if (_changedData == null)
                {
                    _changedData = Parent.SourceTable.Clone();
                    _changedData.TableName = "Changed";
                    Parent.ChangedDatas.ForEach(r =>
                    {
                        var oldRow = _changedData.NewRow();
                        oldRow.ItemArray = r.RowInSource.ItemArray;
                        _changedData.Rows.Add(oldRow);
                        oldRow.AcceptChanges();
                        var diffFields = DataRowCellComparer.GetDiffFields(oldRow, r.RowInTarget).ToArray();
                        foreach (var f in diffFields)
                        {
                            oldRow[f] = r.RowInTarget[f];
                        }
                        DiffFieldsOfRow.Add(oldRow, diffFields);
                        //newRow.ItemArray = r.RowInTarget.ItemArray;
                        //_changedData.Rows.Add(newRow);
                    });
                }
                return _changedData;
            }
        }
        public Dictionary<DataRow, string[]> DiffFieldsOfRow { get; } = new Dictionary<DataRow, string[]>();
    }

    public class DataRowPrimaryKeyComparer : IEqualityComparer<DataRow>
    {
        public static DataRowPrimaryKeyComparer Default { get; } = new DataRowPrimaryKeyComparer();
        public bool Equals(DataRow x, DataRow y)
        {
            var xPKs = x?.Table.PrimaryKey.Select(pk => x[pk].GetHashCode());
            var yPKs = y?.Table.PrimaryKey.Select(pk => y[pk].GetHashCode());
            return (xPKs ?? throw new InvalidOperationException()).SequenceEqual(yPKs ?? throw new InvalidOperationException());
        }

        public int GetHashCode(DataRow obj)
        {
            return obj.Table.PrimaryKey.Aggregate(0, (current, c) => current ^ obj[c].GetHashCode());
        }
    }
    public class DataRowCellComparer : IEqualityComparer<DataRow>
    {
        public static DataRowCellComparer Default { get; } = new DataRowCellComparer();
        public bool Equals(DataRow x, DataRow y)
        {
            if (x is null || y is null) return false;
            foreach (DataColumn c in x.Table.Columns)
            {
                if (Equals(x[c.ColumnName], y[c.ColumnName]))
                    continue;
                if (!c.DataType.IsArray) return false;
                if (x[c.ColumnName] is byte[] arrX &&
                    y[c.ColumnName] is byte[] arrY)
                    if (arrX.SequenceEqual(arrY))
                        continue;
                return false;
            }
            return true;
            //var result = compareLogic.Compare(x.ItemArray, y.ItemArray);
            //return result.AreEqual;
            //return x != null && y != null && x.ItemArray.IsDeepEqual(y.ItemArray);
            //var zip = x?.ItemArray.Zip(y?.ItemArray ?? throw new InvalidOperationException(), Tuple.Get);
            //return (zip ?? throw new InvalidOperationException()).Aggregate(false, (current, t) => current & t.Item1.IsDeepEqual(t.Item2));
        }

        public int GetHashCode(DataRow obj)
        {
            return 0;
        }
        public static IEnumerable<string> GetDiffFields(DataRow oldRow, DataRow newRow)
        {
            if (oldRow is null || newRow is null) throw new ArgumentNullException("Matching null objects is not supported");
            return oldRow.Table.Columns.Cast<DataColumn>()
                .Select(c => new { Column = c, OldValue = oldRow[c.ColumnName], NewValue = newRow[c.ColumnName] })
                .Where(obj =>
                {
                    if (Equals(obj.OldValue, obj.NewValue))
                        return false;
                    if (!obj.Column.DataType.IsArray) return true;
                    if (obj.OldValue is byte[] arrX &&
                        obj.NewValue is byte[] arrY)
                        if (arrX.SequenceEqual(arrY))
                            return false;
                    return true;
                })
                .Select(obj => obj.Column.ColumnName);
        }
    }
}
