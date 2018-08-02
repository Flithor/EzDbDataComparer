using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Text;
using DataAccessLib.Common;
using DataAccessLib.Entities;

namespace DataAccessLib.DataAccessor
{
    public sealed class FoxProODBC : DataAccessorBase<OdbcConnection>//←Set this DbConnection
    {
        public FoxProODBC(DbConnectionStringInfo connStrInfo) : base(connStrInfo) { }

        #region ConnectionInfoSetting

        public override string ConnectionStringFormat =>
            "Driver={{Microsoft Visual FoxPro Driver}};" +
            "SourceDB={0};" + 
            "SourceType={1};" + 
            "Exclusive=No;NULL=NO;Collate=Machine;BACKGROUNDFETCH=NO;DELETED=NO;applicationintent=readonly";
        internal override string[] ConnectionStringFieldNames { get; } = { "Data Source", "SourceType" };
        internal override string[] ConnectionStringFieldDefaultValue { get; } = { null, "DBF" };
        #endregion

        private string extension;

        internal override string BuildConnectionString(params string[] fields)
        {
            var dataSource = fields[0];
            extension = fields[1];
            if (File.Exists(dataSource)) OneFileMode = true;//FileMode
            if (Directory.Exists(dataSource)) return string.Format(ConnectionStringFormat, fields);
            throw new FileNotFoundException("Not found this data source!"); //NotExit
        }

        private bool OneFileMode = false;

        #region Query
        public override IEnumerable<string> GetDataBaseTableNames()
        {
            var dataSource = DbConnectionStringInfo.ConnecgtionStringFieldValues[0];
            if (File.Exists(dataSource))
            {
                DbConnectionStringInfo.ConnecgtionStringFieldValues[0] = Path.GetDirectoryName(dataSource);
                return new[] { Path.GetFileNameWithoutExtension(dataSource) };
            } //FileMode
            else if (Directory.Exists(dataSource))
            {
                return Directory.GetFiles(dataSource, $"*.{extension}", SearchOption.AllDirectories).Select(fp =>
                {
                    var osb = new StringBuilder(1024);
                    Console.WriteLine(fp);
                    var re = RelativePath.PathRelativePathTo(osb, dataSource, FileAttributes.Directory, fp,
                         FileAttributes.Normal);
                    if (re)
                        return osb.ToString();
                    throw new Exception("Get relative path error!");
                }).ToArray();
            }//DirMode
            else { throw new FileNotFoundException("Not found this data source!"); } //NotExit
        }

        public override DataSet QueryTables(IEnumerable<string> tableNames, Action processCallBack = null)
        {
            throw new NotImplementedException();
        }
        public override DataTable QueryTable(string tableName)
        {
            if (OneFileMode) tableName = $"[{tableName}]";
            var dt = new DataTable();
            using (var conn = new OdbcConnection(ConnStr))
            {
                using (var adapter = new OdbcDataAdapter($"select * from {tableName}", conn))
                {
                    adapter.FillSchema(dt, SchemaType.Mapped);
                    dt.TableName = tableName;
                    adapter.Fill(dt);
                }
            }

            return dt;
        }
        #endregion
    }
}
