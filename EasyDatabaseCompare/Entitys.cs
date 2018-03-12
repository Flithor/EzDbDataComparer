using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace EasyDatabaseCompare
{
    public class DatabaseChangeContent
    {
        public string TableName { get; internal set; }
        public UpdatedData[] UpdatedDatas { get; internal set; }
        public InsertedData[] InsertedDatas { get; internal set; }
        public DeletedData[] DeletedDatas { get; internal set; }
    }
    public class UpdatedField
    {
        public string ColumnName { get; internal set; }
        public object OldValue { get; internal set; }
        public object NewValue { get; internal set; }
    }
    public class UpdatedData
    {
        public string UniquePrimaryKey { get; internal set; }
        public IEnumerable<UpdatedField> UpdatedFields { get; internal set; }

    }
    public class InsertedData
    {
        public string UniquePrimaryKey { get; internal set; }
        public IDictionary<string, object> Datas { get; internal set; }
    }
    public class DeletedData
    {
        public string UniquePrimaryKey { get; internal set; }
        public IDictionary<string, object> Datas { get; internal set; }
    }

    public class DataEntity
    {
        public DataEntity(DataTable oldTable, DataTable newTable, Action createdCallBack = null)
        {
            #region Check PrimaryKey
            if(oldTable.PrimaryKey == null || oldTable.PrimaryKey.Length == 0)
                throw new ArgumentException("First DataTable not have a valid primary key!");
            if(newTable.PrimaryKey == null || newTable.PrimaryKey.Length == 0)
                throw new ArgumentException("Second DataTable not have a valid primary key!");
            #endregion

            #region Compare Columns
            var colO = oldTable.Columns.Cast<DataColumn>().ToArray();
            var colOStrs = colO.Select(c => c.ColumnName);
            var colNStrs = newTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName);

            if(!colOStrs.SequenceEqual(colNStrs))
                throw new ArgumentException("The schema of two table is inconsistent! Cannot Compare!");
            #endregion

            #region Set Table Entity
            OldTable = oldTable;
            NewTable = newTable;
            PrimaryKeys = OldTable.PrimaryKey;
            ColumnNames = colO.ToArray();
            #endregion

            #region Extract Rows
            var oldTableRows = oldTable.AsEnumerable();
            var newTableRows = newTable.AsEnumerable();

            OldTableRows = oldTableRows.ToDictionary(r =>
                    GetUniquePrimaryKey(r, PrimaryKeys), r => r);

            this.NewTableRows = newTableRows.ToDictionary(r =>
                    GetUniquePrimaryKey(r, PrimaryKeys), r => r);
            #endregion
            InsertRows = newTableRows.Except(oldTableRows, DataRowPrimaryKeyComparer.Default)
                .ToDictionary(r => GetUniquePrimaryKey(r, PrimaryKeys), r => r);
            DeleteRows = oldTableRows.Except(newTableRows, DataRowPrimaryKeyComparer.Default)
                .ToDictionary(r => GetUniquePrimaryKey(r, PrimaryKeys), r => r);

            var mayChangedRowsPrimaryKeys = oldTableRows.Intersect(newTableRows, DataRowPrimaryKeyComparer.Default)
                .Select(r => GetUniquePrimaryKey(r, PrimaryKeys));
            UpDatedData = new Dictionary<string, DiffField>();
            foreach(var key in mayChangedRowsPrimaryKeys)
            {
                var diff = new DiffField(OldTableRows[key], NewTableRows[key]);
                if(diff.DiffFields.Count > 0)
                    UpDatedData.Add(key, diff);
            }
            createdCallBack?.Invoke();
        }

        private static string GetUniquePrimaryKey(DataRow r, DataColumn[] primaryKeys)
        {
            return string.Join("_/_", primaryKeys.Select(pk => r[pk.ColumnName]));
        }

        public string TableName { get { return OldTable.TableName; } }

        public DataTable OldTable { get; }
        public DataTable NewTable { get; }
        public DataColumn[] PrimaryKeys { get; }
        public DataColumn[] ColumnNames { get; }

        public Dictionary<string, DataRow> OldTableRows { get; }
        public Dictionary<string, DataRow> NewTableRows { get; }

        public Dictionary<string, DiffField> UpDatedData { get; }
        public Dictionary<string, DataRow> InsertRows { get; }
        public Dictionary<string, DataRow> DeleteRows { get; }

        public class DiffField
        {
            public DiffField(DataRow oldDataRow, DataRow newDataRow)
            {
                OldDataRow = oldDataRow;
                NewDataRow = newDataRow;

                var PrimaryKeys = oldDataRow.Table.PrimaryKey;
                var oldDataRowSPk = GetUniquePrimaryKey(oldDataRow, PrimaryKeys);
                var newDataRowSPk = GetUniquePrimaryKey(newDataRow, PrimaryKeys);

                if(oldDataRowSPk != newDataRowSPk) throw new ArgumentException("Two DataRow primary key is inconsistent! Cannot Compare!");

                UniquePrimaryKey = oldDataRowSPk;

                var columns = oldDataRow.Table.Columns;
                DiffFields = new Dictionary<DataColumn, object[]>();
                foreach(DataColumn c in columns)
                {
                    var q = DataRowComparer.Default.Equals(oldDataRow, newDataRow);
                    if(!q)
                    {
                        var r = true;
                        var vo = oldDataRow[c.ColumnName];
                        var vn = newDataRow[c.ColumnName];
                        if(c.DataType.IsArray)
                        {
                            if(vo != vn)
                            {
                                var ar1 = vo as IEnumerable;
                                var ar2 = vn as IEnumerable;
                                if(ar1 != null && ar2 != null)
                                {
                                    var en1 = ar1.Cast<object>();
                                    var en2 = ar2.Cast<object>();
                                    r = Enumerable.SequenceEqual(en1, en2);
                                }
                                else
                                    r = false;
                            }
                        }
                        else if(c.DataType.IsPrimitive | c.DataType.IsValueType || c.DataType.Name == "String")
                            r = Equals(oldDataRow[c.ColumnName], newDataRow[c.ColumnName]);
                        else
                            r = false;
                        if(!r)
                            DiffFields.Add(c, new[] { vo, vn });

                    }
                }
            }
            public string UniquePrimaryKey { get; }

            public DataRow OldDataRow { get; }
            public DataRow NewDataRow { get; }

            public Dictionary<DataColumn, object[]> DiffFields { get; set; }
        }

        public class DataRowPrimaryKeyComparer : IEqualityComparer<DataRow>
        {
            public static DataRowPrimaryKeyComparer Default { get; } = new DataRowPrimaryKeyComparer();
            public bool Equals(DataRow x, DataRow y)
            {
                var xPKs = x.Table.PrimaryKey.Select(pk => x[pk].GetHashCode());
                var yPKs = y.Table.PrimaryKey.Select(pk => y[pk].GetHashCode());
                return Enumerable.SequenceEqual(xPKs, yPKs);
            }

            public int GetHashCode(DataRow obj)
            {
                int i = 0;
                foreach(var c in obj.Table.PrimaryKey)
                {
                    i ^= obj[c].GetHashCode();
                }
                return i;
            }
        }
    }

}
