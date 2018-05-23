using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DataAccessLib;
using DataAccessLib.Entities;

namespace DataAccessLib.DataAccessor
{
    public sealed class MsSqlServer : DataAccessorBase
    {
        public MsSqlServer(DbConnectionStringInfo connStrInfo) : base(connStrInfo) { }
        public override string ConnectionStringFormat => "Server={0};Database={1};User Id={2};Password={3}";

        internal override string[] ConnectionStringFieldNames { get; } = { "Server", "Database", "UserID", "Password" };

        internal override bool CheckConnection(string connStr) => CheckConnection<SqlConnection>(connStr);

        public override DataSet QueryTables(IEnumerable<string> tableNames, Action processCallBack = null)
        {
            var d = new DataSet();
            using (var conn = new SqlConnection(ConnStr))
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

        public override DataTable QueryTable(string tableName)
        {
            var dt = new DataTable();
            using (var conn = new SqlConnection(ConnStr))
            {
                conn.Open();
                using (var adapter = new SqlDataAdapter($"SELECT * FROM {tableName}", conn))
                {
                    adapter.FillSchema(dt, SchemaType.Mapped);
                    adapter.Fill(dt);
                    dt.TableName = tableName;
                }
            }
            return dt;
        }

        public override IEnumerable<string> GetDataBaseTableNames()
        {
            using (var conn = new SqlConnection(ConnStr))
            {
                conn.Open();
                using (var reader = new SqlCommand("SELECT name FROM sysobjects WHERE type='U' ORDER BY name", conn).ExecuteReader())
                {
                    var re = new List<string>();
                    while (reader.Read())
                        re.Add(reader["name"].ToString());
                    return re;
                }
            }
        }
    }
}
