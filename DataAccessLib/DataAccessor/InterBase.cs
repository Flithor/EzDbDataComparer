//using System;
//using System.Collections.Generic;
//using Borland.Data;
//using System.Data;
//using DataAccessLib.Entities;

//namespace DataAccessLib.DataAccessor
//{
//    public sealed class InterBase : DataAccessorBase
//    {
//        public InterBase(DbConnectionStringInfo connStrInfo) : base(connStrInfo) { }
//        public override string ConnectionStringFormat =>
//            "DriverName=Interbase;" +
//            "RoleName=RoleName;" +
//            "Database={0};" +
//            "User_Name={1};" +
//            "Password={2};" +
//            "SQLDialect=3;" +

//            "MetaDataAssemblyLoader=Borland.Data.TDBXInterbaseMetaDataCommandFactory," +
//            "Borland.Data.DbxReadOnlyMetaData," +
//            "Version=11.0.5000.0," +
//            "Culture=neutral," +
//            "PublicKeyToken=91d62ebb5b0d1b1b;" +

//            "GetDriverFunc=getSQLDriverINTERBASE;" +
//            "LibraryName=dbxint30.dll;" +
//            "VendorLib=GDS32.DLL";
//        //"Provider=sibprovider;Data Source={0};User Id={1};Password={2};charset=utf8;pooling=true";
//        internal override string[] ConnectionStringFieldNames { get; } = { "DataBase Path", "User Name", "Password" };
//        internal override bool CheckConnection(string connStr) => CheckConnection<TAdoDbxConnection>(connStr);

//        public override DataSet QueryTables(IEnumerable<string> tableNames, Action processCallBack = null)
//        {
//            var d = new DataSet();
//            using (var conn = new TAdoDbxConnection(ConnStr))
//            {
//                conn.Open();
//                foreach (var tableName in tableNames)
//                {
//                    using (var adapter = new TAdoDbxDataAdapter($"SELECT * FROM [{tableName}]", conn))
//                    {
//                        adapter.FillSchema(d, SchemaType.Mapped, tableName);
//                        //d.Tables[tableName].Columns.Cast<DataColumn>().Where(c => c.DataType.Equals(typeof(DateTime))).ToList().ForEach(c => c.DataType = typeof(MySqlDateTime));
//                        adapter.Fill(d, tableName);
//                        processCallBack?.DynamicInvoke();
//                    }
//                }
//            }
//            return d;
//        }

//        public override DataTable QueryTable(string tableName)
//        {
//            var dt = new DataTable();
//            using (var conn = new TAdoDbxConnection(ConnStr))
//            {
//                conn.Open();
//                using (var adapter = new TAdoDbxDataAdapter($"SELECT * FROM [{tableName}]", conn))
//                {
//                    adapter.FillSchema(dt, SchemaType.Mapped);
//                    adapter.Fill(dt);
//                    dt.TableName = tableName;
//                }
//            }
//            return dt;
//        }

//        public override IEnumerable<string> GetDataBaseTableNames()
//        {
//            using (var conn = new TAdoDbxConnection(ConnStr))
//            {
//                conn.Open();
//                using (var reader = new TAdoDbxCommand { CommandText = "show tables", Connection = conn }.ExecuteReader())
//                {
//                    var re = new List<string>();
//                    while (reader.Read())
//                        re.Add(reader.GetString(0));
//                    return re.ToArray();
//                }
//            }
//        }
//    }
//}
