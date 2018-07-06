    using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace DataAccessLib.OldDataAccess
{
    internal sealed class MsAccess : OldDataAccessBase
    {
        public MsAccess(string connStr) : base(connStr) { }
        public MsAccess(string Data_Source, string UserID, string Password) : base(Data_Source, UserID, Password) { }
        internal override string BuildConnectionString(params string[] fields)
        {
            return $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={fields[0]};User ID={fields[1]};Jet OLEDB:Database Password={fields[2]}";
        }

        internal override string SetTimeOut(string connStr)
        {
            //Some of OLEDB Connection not support set Timeout
            //Add Timeout field will cannot make connnection

            //_connstr = ($"{connstr}Connect Timeout=5;");
            return connStr;
        }
        internal override bool CheckConnection()
        {
            using (var conn = new OleDbConnection(ConnectionString))
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
            using (var conn = new OleDbConnection(ConnectionString))
            {
                conn.Open();
                foreach (var tableName in tableNames)
                {
                    using (var adapter = new OleDbDataAdapter($"SELECT * FROM {tableName}", conn))
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
            DataTable dt;
            using (var conn = new OleDbConnection(ConnectionString))
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