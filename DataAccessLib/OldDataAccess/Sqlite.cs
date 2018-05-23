using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace DataAccessLib.OldDataAccess
{
    internal sealed class Sqlite : OldDataAccessBase
    {
        #region Constructors
        public Sqlite(string connStr) : base(connStr) { }
        public Sqlite(string Data_Source, string Password) : base(Data_Source, Password) { }
        #endregion

        #region Initialization
        internal override string BuildConnectionString(params string[] fields)
        {
            return $"Data Source={fields[0]};Version=3;Password={fields[1]};Read Only=True;FailIfMissing=True";
        }
        //↓This method is used to add the timeout argument in connnetion string
        //↓usually 5 seconds
        //↓If not supported, can return "connStr"
        internal override string SetTimeOut(string connStr)
        {
            return new SQLiteConnectionStringBuilder(connStr) { DefaultTimeout = 5000, DateTimeFormat = SQLiteDateFormats.CurrentCulture }.ToString();
        }
        //↓This method is used to check whether the connection string can access the database
        internal override bool CheckConnection()
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                var t = System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    try
                    {
                        conn.Open();
                        return null;
                    }
                    catch (Exception ex)
                    {
                        return ex;
                    }
                });
                t.Wait(1000);
                if (!t.IsCompleted)
                    throw new Exception("connection time out");
                if (conn.State == ConnectionState.Open)
                    return true;
                if (t.Result != null)
                    throw t.Result;
                return false;
            }
        }
        #endregion

        #region Query
        //↓This method is used to get target database table names list
        public override string[] QueryAllTableName()
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();
                using (var reader = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' ORDER BY name", conn).ExecuteReader())
                {
                    var re = new List<string>();
                    while (reader.Read())
                        re.Add(reader["name"].ToString());
                    return re.ToArray();
                }
            }
        }

        //↓This method is used to query all selected tables data
        //↓If can, get the table schema
        public override DataSet QueryTables(string[] tableNames, Action processCallBack = null)
        {
            var d = new DataSet();
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();
                foreach (var tableName in tableNames)
                {
                    using (var adapter = new SQLiteDataAdapter($"SELECT * FROM {tableName}", conn))
                    {
                        adapter.FillSchema(d, SchemaType.Mapped, tableName);
                        //adapter.FillError += Adapter_FillError;
                        //adapter.Fill(d, tableName);
                    }
                    using (var comm = new SQLiteCommand($"SELECT * FROM {tableName}", conn))
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
            //↓Please execute this action after each table successful query
        }

        private void Adapter_FillError(object sender, FillErrorEventArgs e)
        {
            e.Continue = true;
        }
        #endregion

    }
}
