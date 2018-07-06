using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;
using DataAccessLib.Entities;

namespace DataAccessLib.DataAccessor
{
    public sealed class MySql : DataAccessorBase
    {
        public MySql(DbConnectionStringInfo connStrInfo) : base(connStrInfo) { }
        public override string ConnectionStringFormat =>
            "Data Source={0};Port={1};Database={2};User Id={3};Password={4};charset=utf8;pooling=true";

        internal override string[] ConnectionStringFieldNames { get; } = { "Server", "Port", "Database", "UserID", "Password" };
        internal override string[] ConnectionStringFieldDefaultValue { get; } = { null, "3306", null, "root", null };

        internal override bool CheckConnection(string connStr) => CheckConnection<MySqlConnection>(connStr);


        public override DataSet QueryTables(IEnumerable<string> tableNames, Action processCallBack = null)
        {
            var d = new DataSet();
            using (var conn = new MySqlConnection(ConnStr))
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

        public override DataTable QueryTable(string tableName)
        {
            var dt = new DataTable();
            using (var conn = new MySqlConnection(ConnStr))
            {
                conn.Open();

                using (var adapter = new MySqlDataAdapter($"SELECT * FROM {tableName}", conn))
                {
                    adapter.FillSchema(dt, SchemaType.Mapped);
                    //d.Tables[tableName].Columns.Cast<DataColumn>().Where(c => c.DataType.Equals(typeof(DateTime))).ToList().ForEach(c => c.DataType = typeof(MySqlDateTime));
                    adapter.Fill(dt);
                    dt.TableName = tableName;
                }
            }
            return dt;
        }

        public override IEnumerable<string> GetDataBaseTableNames()
        {
            using (var conn = new MySqlConnection(ConnStr))
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
