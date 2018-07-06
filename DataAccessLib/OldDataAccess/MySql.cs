using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;

namespace DataAccessLib.OldDataAccess
{
    internal sealed class MySql : OldDataAccessBase
    {
        public MySql(string connStr) : base(connStr) { }
        public MySql(string Server, string Port, string Database, string UserID, string Password) : base(Server, Port, Database, UserID, Password) { }
        internal override string BuildConnectionString(params string[] fields)
        {
            return $"Data Source={fields[0]};Port={fields[1]};Database={fields[2]};User Id={fields[3]};Password={fields[4]};charset=utf8;pooling=true";
        }
        internal override string SetTimeOut(string connStr)
        {
            var sb = new MySqlConnectionStringBuilder(connStr) { ConnectionTimeout = 5, AllowZeroDateTime = true };
            //sb.AllowZeroDateTime = true;
            //sb.ConvertZeroDateTime = true;
            return sb.ToString();
        }

        internal override bool CheckConnection()
        {
            using (var conn = new MySqlConnection(ConnectionString))
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

        public override DataSet QueryTables(string[] tableNames, Action processCallBack = null)
        {
            var d = new DataSet();
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();

                foreach (var tableName in tableNames)
                {
                    using (var adapter = new MySqlDataAdapter($"SELECT * FROM {tableName}", conn))
                    {
                        adapter.FillSchema(d, SchemaType.Mapped, tableName);
                        //d.Tables[tableName].Columns.Cast<DataColumn>().Where(c => c.DataType.Equals(typeof(DateTime))).ToList().ForEach(c => c.DataType = typeof(MySqlDateTime));
                        adapter.Fill(d, tableName);
                        processCallBack?.DynamicInvoke();
                    }
                }
            }
            return d;
        }

        public override string[] QueryAllTableName()
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                using (var reader = new MySqlCommand("show tables", conn).ExecuteReader())
                {
                    var re = new List<string>();
                    while (reader.Read())
                        re.Add(reader.GetString(0));
                    return re.ToArray();
                }
            }
        }
    }
}
