using System;
using System.Collections.Generic;
using FirebirdSql.Data.FirebirdClient;
using System.Data;
using DataAccessLib.Entities;

namespace DataAccessLib.DataAccessor
{
    public sealed class Firebird : DataAccessorBase<FbConnection>
    {
        public Firebird(DbConnectionStringInfo connStrInfo) : base(connStrInfo) { }
        public override string ConnectionStringFormat { get; } =
            "Database={0};" +
            "User={1};" +
            "Password={2};" +
            "Dialect=3;" +
            "Charset=NONE;" +
            "Role=;" +
            "Connection lifetime=15;" +
            "Pooling=true;" +
            "MinPoolSize=0;" +
            "MaxPoolSize=50;" +
            "Packet Size=8192;" +
            "ServerType=1";
        //"Provider=sibprovider;Data Source={0};User Id={1};Password={2};charset=utf8;pooling=true";
        internal override string[] ConnectionStringFieldNames { get; } = { "DataBase Path", "User Name", "Password" };
        internal override string[] ConnectionStringFieldDefaultValue { get; } = { null, "sysdba", "masterkey" };
        //internal override bool CheckConnection(string connStr) => CheckConnection<FbConnection>(connStr);

        public override DataSet QueryTables(IEnumerable<string> tableNames, Action processCallBack = null)
        {
            var d = new DataSet();
            using (var conn = new FbConnection(ConnStr))
            {
                conn.Open();
                foreach (var tableName in tableNames)
                {
                    using (var adapter = new FbDataAdapter($"SELECT * FROM {tableName}", conn))
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
            using (var conn = new FbConnection(ConnStr))
            {
                conn.Open();
                using (var adapter = new FbDataAdapter($"SELECT * FROM {tableName}", conn))
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
            using (var conn = new FbConnection(ConnStr))
            {
                conn.Open();
                using (var reader = new FbDataAdapter("SELECT a.RDB$RELATION_NAME FROM RDB$RELATIONS a WHERE RDB$SYSTEM_FLAG = 0 AND RDB$RELATION_TYPE = 0 ", conn).SelectCommand.ExecuteReader())
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
