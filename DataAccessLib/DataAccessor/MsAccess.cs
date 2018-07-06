using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using DataAccessLib.Entities;

namespace DataAccessLib.DataAccessor
{
    public sealed class MsAccess : DataAccessorBase
    {
        public MsAccess(DbConnectionStringInfo connStrInfo) : base(connStrInfo) { }

        public override string ConnectionStringFormat =>
            "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};User ID={1};Jet OLEDB:Database Password={2}";

        internal override string[] ConnectionStringFieldNames { get; } = { "Data Source", "UserID", "Password" };

        internal override bool CheckConnection(string connStr) => CheckConnection<OleDbConnection>(connStr);

        public override DataSet QueryTables(IEnumerable<string> tableNames, Action processCallBack = null)
        {
            var d = new DataSet();
            using (var conn = new OleDbConnection(ConnStr))
            {
                conn.Open();
                foreach (var tableName in tableNames)
                {
                    using (var adapter = new OleDbDataAdapter($"SELECT * FROM [{tableName}]", conn))
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
            using (var conn = new OleDbConnection(ConnStr))
            {
                conn.Open();
                using (var adapter = new OleDbDataAdapter($"SELECT * FROM [{tableName}]", conn))
                {
                    adapter.FillSchema(dt, SchemaType.Mapped);
                    adapter.Fill(dt, tableName);
                }
            }
            return dt;
        }

        public override IEnumerable<string> GetDataBaseTableNames()
        {
            DataTable dt;
            using (var conn = new OleDbConnection(ConnStr))
            {
                // c:\test\test.mdb
                // We only want user tables, not system tables
                string[] restrictions = new string[4];
                restrictions[3] = "Table";

                conn.Open();

                // Get list of user tables
                dt = conn.GetSchema("Tables", restrictions);
            }

            var tNames = new List<string>();
            for (var i = 0; i < dt.Rows.Count; i++)
                tNames.Add(dt.Rows[i][2].ToString());
            return tNames.ToArray();
        }
    }
}