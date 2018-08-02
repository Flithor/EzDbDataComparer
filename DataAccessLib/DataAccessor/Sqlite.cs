using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using DataAccessLib.Entities;

namespace DataAccessLib.DataAccessor
{
    public sealed class Sqlite : DataAccessorBase<SQLiteConnection>
    {
        public Sqlite(DbConnectionStringInfo connStrInfo) : base(connStrInfo) { }
        public override string ConnectionStringFormat =>
                "Data Source={0};Version=3;Password={1};Read Only=True;FailIfMissing=True";
        
        internal override string[] ConnectionStringFieldNames { get; } = { "Data Source", "Password" };
        
        #region Query
        public override IEnumerable<string> GetDataBaseTableNames()
        {
            using (var conn = new SQLiteConnection(ConnStr))
            {
                conn.BusyTimeout = 10000;
                conn.Open();
                using (var com = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' ORDER BY name", conn))
                {
                    using (var reader = com.ExecuteReader())
                    {
                        var re = new List<string>();
                        while (reader.Read())
                            re.Add(reader["name"].ToString());
                        return re.ToArray();
                    }
                }
            }
        }
        public override DataSet QueryTables(IEnumerable<string> tableNames, Action processCallBack = null)
        {
            var d = new DataSet();
            using (var conn = new SQLiteConnection(ConnStr))
            {
                conn.Open();
                foreach (var tableName in tableNames)
                {
                    using (var adapter = new SQLiteDataAdapter($"SELECT * FROM [{tableName}]", conn))
                    {
                        adapter.FillSchema(d, SchemaType.Mapped, tableName);
                        //adapter.FillError += Adapter_FillError;
                        //adapter.Fill(d, tableName);
                    }
                    using (var comm = new SQLiteCommand($"SELECT * FROM [{tableName}]", conn))
                    {
                        using (var reader = comm.ExecuteReader())
                        {
                            var dt = d.Tables[tableName];
                            while (reader.Read())
                            {
                                var r = dt.NewRow();
                                for (var i = 0; i < dt.Columns.Count; i++)
                                    try
                                    {
                                        var val = reader.GetValue(i);
                                        r[i] = val;
                                    }
                                    catch (FormatException)
                                    {
                                        if (dt.Columns[i].DataType == typeof(DateTime))
                                            r[i] = DateTime.MinValue;
                                        else
                                            throw;
                                    }
                                dt.Rows.Add(r);
                            }
                        }
                    }
                    processCallBack?.DynamicInvoke();
                }
            }
            return d;
        }

        public override DataTable QueryTable(string tableName)
        {
            var dt = new DataTable();
            using (var conn = new SQLiteConnection(ConnStr))
            {
                conn.Open();
                using (var adapter = new SQLiteDataAdapter($"SELECT * FROM [{tableName}]", conn))
                {
                    adapter.FillSchema(dt, SchemaType.Mapped);
                    //adapter.FillError += Adapter_FillError;
                    //adapter.Fill(d, tableName);
                }
                using (var comm = new SQLiteCommand($"SELECT * FROM [{tableName}]", conn))
                {
                    using (var reader = comm.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var r = dt.NewRow();
                            for (var i = 0; i < dt.Columns.Count; i++)
                                try
                                {
                                    var val = reader.GetValue(i);
                                    r[i] = val;
                                }
                                catch (FormatException)
                                {
                                    if (dt.Columns[i].DataType == typeof(DateTime))
                                        r[i] = DateTime.MinValue;
                                    else
                                        throw;
                                }
                            dt.Rows.Add(r);
                        }
                    }
                }
            }
            return dt;
        }
        #endregion
    }
}
