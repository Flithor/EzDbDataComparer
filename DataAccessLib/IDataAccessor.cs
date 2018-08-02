using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLib.Common;
using DataAccessLib.DataAccessor;
using DataAccessLib.Entities;
using Ninject;
using Ninject.Parameters;
using Ninject.Syntax;

namespace DataAccessLib
{
    public interface IDataAccessor
    {
        IEnumerable<string> GetDataBaseTableNames();
        DataSet QueryTables(IEnumerable<string> tableNames, Action processCallBack = null);
        DataTable QueryTable(string tableName);
        bool CheckConnection();
    }
    public interface IDataAccessorFactory
    {
        IDataAccessor Get(string dbType, params string[] connStrNFields);
        //IDataAccessor Get(string dbType, string connStr);
        //IDataAccessor Get(string dbType, params string[] fields);
    }

    public abstract class DataAccessorBase : IDataAccessor
    {
        internal DataAccessorBase(DbConnectionStringInfo connStrInfo)
        {
            DbConnectionStringInfo = connStrInfo;
            DbConnectionStringInfo.CreateConnectionString(BuildConnectionString);
        }
        internal abstract string[] ConnectionStringFieldNames { get; }
        internal virtual string[] ConnectionStringFieldDefaultValue { get; }
        public abstract string ConnectionStringFormat { get; }

        public DbConnectionStringInfo DbConnectionStringInfo { get; }

        //private string BuildConnectionString(IEnumerable<string> arg) =>
        //    BuildConnectionString(arg.Replace(string.Empty, string.IsNullOrEmpty).ToArray());
        internal virtual string BuildConnectionString(params string[] fields)
        {
            return string.Format(ConnectionStringFormat, fields.Replace(string.Empty, string.IsNullOrEmpty).ToArray());
        }

        public abstract IEnumerable<string> GetDataBaseTableNames();
        public abstract DataTable QueryTable(string tableName);
        public abstract DataSet QueryTables(IEnumerable<string> tableNames, Action processCallBack = null);
        public abstract bool CheckConnection();
    }

    public abstract class DataAccessorBase<T> : DataAccessorBase where T : IDbConnection, new()
    {
        protected DataAccessorBase(DbConnectionStringInfo connStrInfo) : base(connStrInfo) { }

        public string ConnStr => DbConnectionStringInfo.ConnectionString;

        //internal abstract bool CheckConnection(string connStr);

        public override bool CheckConnection()
        {
            using (var conn = new T { ConnectionString = ConnStr })
            {
                var t = Task.Factory.StartNew(() =>
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
                    throw new TimeoutException("Check connection timeout!");
                if (conn.State == ConnectionState.Open)
                    return true;
                if (t.Result != null)
                    throw new DataException("An exception occurred during connection checking!", t.Result);
                return false;
            }
        }
    }
    public class DataAccessorFactory : IDataAccessorFactory
    {
        private readonly IResolutionRoot _root;

        public DataAccessorFactory(IResolutionRoot root)
        {
            _root = root;
        }

        //public IDataAccessor Get(string dbType, string connStr)
        //{
        //    var da = _root.Get<IDataAccessor>(dbType);
        //    return da.TryDbConnection(connStr) ? da : null;
        //}
        //public IDataAccessor Get(string dbType, params string[] fields)
        //{
        //    var da = _root.Get<IDataAccessor>(dbType);
        //    return da.TryDbConnection(fields) ? da : null;
        //}

        public IDataAccessor Get(string dbType, params string[] connStrNFields)
        {
            var connStrInfo = new DbConnectionStringInfo(connStrNFields);
            return _root.Get<IDataAccessor>(dbType, new ConstructorArgument(nameof(connStrInfo), connStrInfo));
        }
    }

}
