using System;
using System.Collections.Generic;
using Borland.Data;
using System.Data;

namespace DataAccessLib.OldDataAccess
{
    internal sealed class InterBase : OldDataAccessBase
    {
        public InterBase(string connStr) : base(connStr) { }
        public InterBase(string Data_Source, string User_ID, string Password) : base(Data_Source, User_ID, Password) { }
        internal override string BuildConnectionString(params string[] fields)
        {
            //"DriverName=Interbase;Database={0}:{1};RoleName=;User_Name={2};Password={3};SQLDialect=3;MetaDataAssemblyLoader=Borland.Data.TDBXInterbaseMetaDataCommandFactory,Borland.Data.DbxReadOnlyMetaData,Version=11.0.5000.0,Culture=neutral,PublicKeyToken=91d62ebb5b0d1b1b;GetDriverFunc=getSQLDriverINTERBASE;LibraryName=dbxint30.dll;VendorLib=GDS32.DLL";
            return $"Provider=sibprovider;Data Source={fields[0]};User Id={fields[1]};Password={fields[2]};charset=utf8;pooling=true";
        }
        internal override string SetTimeOut(string connStr)
        {
            var sb = new TAdoDbxConnectionStringBuilder { ConnectionString = connStr };
            //sb.AllowZeroDateTime = true;
            //sb.ConvertZeroDateTime = true;
            return sb.ToString();
        }

        internal override bool CheckConnection()
        {
            using (var conn = new TAdoDbxConnection(ConnectionString))
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
            using (var conn = new TAdoDbxConnection(ConnectionString))
            {
                conn.Open();

                foreach (var tableName in tableNames)
                {
                    using (var adapter = new TAdoDbxDataAdapter($"SELECT * FROM {tableName}", conn))
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
            using (var conn = new TAdoDbxConnection(ConnectionString))
            {
                conn.Open();
                using (var reader = new TAdoDbxCommand { CommandText = "show tables", Connection = conn }.ExecuteReader())
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
