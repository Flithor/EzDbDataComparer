//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Reactive.Linq;

//namespace EasyDatabaseCompare.Model
//{
//    public class DatabaseChangeContent
//    {
//        public string TableName { get; internal set; }
//        public UpdatedData[] UpdatedDatas { get; internal set; }
//        public InsertedData[] InsertedDatas { get; internal set; }
//        public DeletedData[] DeletedDatas { get; internal set; }
//    }
//    public class UpdatedField
//    {
//        public string ColumnName { get; internal set; }
//        public object OldValue { get; internal set; }
//        public object NewValue { get; internal set; }
//    }
//    public class UpdatedData
//    {
//        public string UniquePrimaryKey { get; internal set; }
//        public IEnumerable<UpdatedField> UpdatedFields { get; internal set; }

//    }
//    public class InsertedData
//    {
//        public string UniquePrimaryKey { get; internal set; }
//        public IDictionary<string, object> Datas { get; internal set; }
//    }
//    public class DeletedData
//    {
//        public string UniquePrimaryKey { get; internal set; }
//        public IDictionary<string, object> Datas { get; internal set; }
//    }


//    internal static class Common
//    {
//        public static string GetUniquePrimaryKey(DataRow r, DataColumn[] primaryKeys)
//        {
//            return primaryKeys.Length == 0 ? $"[No Primary Key]({r.GetHashCode()})" :
//                string.Join("_/_", primaryKeys.Select(pk => r[pk.ColumnName]));
//        }
//    }
//    public class DataEntity
//    {
//        public DataEntity(DataTable oldTable, DataTable newTable, Action createdCallBack = null, bool ignorePrimaryKey = false)
//        {
//            #region Check PrimaryKey
//            if (!ignorePrimaryKey)
//            {
//                if (oldTable.PrimaryKey.Length == 0)
//                    throw new ArgumentException("First DataTable not have a valid primary key!");
//                if (newTable.PrimaryKey.Length == 0)
//                    throw new ArgumentException("Second DataTable not have a valid primary key!");
//            }
//            #endregion

//            #region Compare Columns
//            var colO = oldTable.Columns.Cast<DataColumn>().ToArray();
//            var colOStrs = colO.Select(c => c.ColumnName);
//            var colNStrs = newTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName);

//            if (!colOStrs.SequenceEqual(colNStrs))
//                throw new ArgumentException("The schema of two table is inconsistent! Cannot Compare!");
//            #endregion

//            #region Set Table Entity
//            OldTable = oldTable;
//            NewTable = newTable;
//            PrimaryKeys = OldTable.PrimaryKey;
//            ColumnNames = colO.ToArray();
//            #endregion

//            #region Extract Rows
//            var oldTableRows = oldTable.AsEnumerable();
//            var newTableRows = newTable.AsEnumerable();

//            if (ignorePrimaryKey)
//            {
//                OldTableRows = new Dictionary<string, DataRow>();
//                NewTableRows = new Dictionary<string, DataRow>();

//                InsertRows = newTableRows.Except(oldTableRows, DataRowCellComparer.Default)
//                    .ToDictionary(r => Common.GetUniquePrimaryKey(r, PrimaryKeys), r => r);
//                DeleteRows = oldTableRows.Except(newTableRows, DataRowCellComparer.Default)
//                    .ToDictionary(r => Common.GetUniquePrimaryKey(r, PrimaryKeys), r => r);
//            }
//            else
//            {
//                OldTableRows = oldTableRows.ToDictionary(r =>
//                    Common.GetUniquePrimaryKey(r, PrimaryKeys), r => r);
//                NewTableRows = newTableRows.ToDictionary(r =>
//                    Common.GetUniquePrimaryKey(r, PrimaryKeys), r => r);

//                InsertRows = newTableRows.Except(oldTableRows, DataRowPrimaryKeyComparer.Default)
//                    .ToDictionary(r => Common.GetUniquePrimaryKey(r, PrimaryKeys), r => r);
//                DeleteRows = oldTableRows.Except(newTableRows, DataRowPrimaryKeyComparer.Default)
//                    .ToDictionary(r => Common.GetUniquePrimaryKey(r, PrimaryKeys), r => r);
//            }
//            #endregion

//            UpDatedData = new Dictionary<string, DiffField>();
//            if (!ignorePrimaryKey)
//            {
//                var mayChangedRowsPrimaryKeys = oldTableRows.Intersect(newTableRows, DataRowPrimaryKeyComparer.Default)
//                    .Select(r => Common.GetUniquePrimaryKey(r, PrimaryKeys));
//                foreach (var key in mayChangedRowsPrimaryKeys)
//                {
//                    var diff = new DiffField(OldTableRows[key], NewTableRows[key]);
//                    if (diff.DiffFields.Count > 0)
//                        UpDatedData.Add(key, diff);
//                }
//            }
//            createdCallBack?.Invoke();
//        }

//        public string TableName => OldTable.TableName;

//        public DataTable OldTable { get; }
//        public DataTable NewTable { get; }
//        public DataColumn[] PrimaryKeys { get; }
//        public DataColumn[] ColumnNames { get; }

//        public Dictionary<string, DataRow> OldTableRows { get; }
//        public Dictionary<string, DataRow> NewTableRows { get; }

//        public Dictionary<string, DiffField> UpDatedData { get; }
//        public Dictionary<string, DataRow> InsertRows { get; }
//        public Dictionary<string, DataRow> DeleteRows { get; }
//    }
//    public class DiffField
//    {
//        public DiffField(DataRow oldDataRow, DataRow newDataRow)
//        {
//            OldDataRow = oldDataRow;
//            NewDataRow = newDataRow;

//            var primaryKeys = oldDataRow.Table.PrimaryKey;
//            var oldDataRowSPk = Common.GetUniquePrimaryKey(oldDataRow, primaryKeys);
//            var newDataRowSPk = Common.GetUniquePrimaryKey(newDataRow, primaryKeys);

//            if (oldDataRowSPk != newDataRowSPk) throw new ArgumentException("Two DataRow primary key is inconsistent! Cannot Compare!");

//            UniquePrimaryKey = oldDataRowSPk;

//            var columns = oldDataRow.Table.Columns;
//            DiffFields = new Dictionary<DataColumn, object[]>();
//            foreach (DataColumn c in columns)
//            {
//                if (DataRowComparer.Default.Equals(oldDataRow, newDataRow)) continue;
//                var r = true;
//                var vo = oldDataRow[c.ColumnName];
//                var vn = newDataRow[c.ColumnName];
//                if (c.DataType.IsArray)
//                {
//                    if (vo != vn)
//                    {
//                        if (vo is IEnumerable ar1 && vn is IEnumerable ar2)
//                        {
//                            var en1 = ar1.Cast<object>();
//                            var en2 = ar2.Cast<object>();
//                            r = en1.SequenceEqual(en2);
//                        }
//                        else
//                            r = false;
//                    }
//                }
//                else if (c.DataType.IsPrimitive | c.DataType.IsValueType || c.DataType.Name == "String")
//                    r = Equals(oldDataRow[c.ColumnName], newDataRow[c.ColumnName]);
//                else
//                    r = false;
//                if (!r)
//                    DiffFields.Add(c, new[] { vo, vn });
//            }
//        }
//        public string UniquePrimaryKey { get; }

//        public DataRow OldDataRow { get; }
//        public DataRow NewDataRow { get; }

//        public Dictionary<DataColumn, object[]> DiffFields { get; set; }
//    }

//    public class DataRowPrimaryKeyComparer : IEqualityComparer<DataRow>
//    {
//        public static DataRowPrimaryKeyComparer Default { get; } = new DataRowPrimaryKeyComparer();
//        public bool Equals(DataRow x, DataRow y)
//        {
//            var xPKs = x?.Table.PrimaryKey.Select(pk => x[pk].GetHashCode());
//            var yPKs = y?.Table.PrimaryKey.Select(pk => y[pk].GetHashCode());
//            return (xPKs ?? throw new InvalidOperationException()).SequenceEqual(yPKs ?? throw new InvalidOperationException());
//        }

//        public int GetHashCode(DataRow obj)
//        {
//            return obj.Table.PrimaryKey.Aggregate(0, (current, c) => current ^ obj[c].GetHashCode());
//        }
//    }
//    public class DataRowCellComparer : IEqualityComparer<DataRow>
//    {
//        public static DataRowCellComparer Default { get; } = new DataRowCellComparer();
//        public bool Equals(DataRow x, DataRow y)
//        {
//            if (x is null || y is null) return false;
//            var re = true;
//            foreach (DataColumn c in x.Table.Columns)
//            {
//                if (c.DataType.IsArray)
//                {
//                    var arrX = x[c.ColumnName] as IEnumerable<object>;
//                    var arrY = y[c.ColumnName] as IEnumerable<object>;
//                    if (arrX is null || arrY is null)
//                        re &= Equals(arrX, arrY);
//                    else
//                        re &= arrX.SequenceEqual(arrY);
//                }
//                else
//                {
//                    re &= Equals(x[c.ColumnName], y[c.ColumnName]);
//                }
//            }
//            return re;
//            //var result = compareLogic.Compare(x.ItemArray, y.ItemArray);
//            //return result.AreEqual;
//            //return x != null && y != null && x.ItemArray.IsDeepEqual(y.ItemArray);
//            //var zip = x?.ItemArray.Zip(y?.ItemArray ?? throw new InvalidOperationException(), Tuple.Get);
//            //return (zip ?? throw new InvalidOperationException()).Aggregate(false, (current, t) => current & t.Item1.IsDeepEqual(t.Item2));
//        }

//        public int GetHashCode(DataRow obj)
//        {
//            return 0;
//        }
//    }
//}
