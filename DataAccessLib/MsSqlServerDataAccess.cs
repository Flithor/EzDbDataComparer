using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DataAccessLib
{
    internal sealed class MsSqlServerDataAccess : DataAccessBase
    {
        public MsSqlServerDataAccess(string connStr) : base(connStr) { }
        public MsSqlServerDataAccess(string Server, string Database, string UserID, string Password) : base(Server, Database, UserID, Password) { }
        internal override string BuildConnectionString(params string[] fields)
        {
            return $"Server={fields[0]};Database={fields[1]};User Id={fields[2]};Password={fields[3]}";
        }

        internal override string SetTimeOut(string connStr)
        {
            var sb = new SqlConnectionStringBuilder(connStr) { ConnectTimeout = 5 };
            return sb.ToString();

        }
        internal override bool CheckConnection()
        {
            using (var conn = new SqlConnection(ConnectionString))
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

        public override DataSet QueryTables(string[] tableNames, bool withSchma = true, Action processCallBack = null)
        {
            var d = new DataSet();
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                foreach (var tableName in tableNames)
                {
                    using (var adapter = new SqlDataAdapter($"SELECT * FROM {tableName}", conn))
                    {
                        adapter.FillSchema(d, SchemaType.Mapped, tableName);
                        adapter.Fill(d, tableName);
                        processCallBack?.DynamicInvoke();
                    }
                }
            }
            return d;
        }

        public override string[] QueryAllTableName()
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (var reader = new SqlCommand("SELECT name FROM sysobjects WHERE type='U' ORDER BY name", conn).ExecuteReader())
                {
                    var re = new List<string>();
                    while (reader.Read())
                        re.Add(reader["name"].ToString());
                    return re.ToArray();
                }
            }
        }
    }
}
