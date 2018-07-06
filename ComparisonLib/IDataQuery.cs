using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ComparisonLib.Common;
using ComparisonLib.Common.DisposableTools;
using DataAccessLib;

namespace ComparisonLib
{
    public interface IDataQuery
    {
        IEnumerable<string> TableNames { get; }
        bool CreateDataAccessor(string dbType, params string[] fields);
        DataTable QuerySingleTable(string tableName, Action<DataTable> complateCallBack = null);
        IObservable<DataTable> QueryMultiTable(IEnumerable<string> tableNames, Action<DataTable> processCallBack = null);
    }
    public class DataQuery : IDataQuery
    {
        private IDataAccessor _dataAccessor;

        public DataQuery(IDataAccessorFactory dataAccessorFactory)
        {
            Factory = dataAccessorFactory;
        }
        private IDataAccessorFactory Factory { get; }

        private IDataAccessor DataAccessor
        {
            get => _dataAccessor ??
                   throw new InvalidOperationException(
                       "DataAccessor not initialized! Please invoke \"CreateDataAccessor\" before!");
            set => _dataAccessor = value;
        }

        public IEnumerable<string> TableNames { get; set; }

        public bool CreateDataAccessor(string dbType, params string[] fields)
        {
            DataAccessor = Factory.Get(dbType, fields);
            var re = DataAccessor.TryDbConnection();
            if (re) TableNames = DataAccessor.GetDataBaseTableNames();
            return re;
        }

        public DataTable QuerySingleTable(string tableName, Action<DataTable> complateCallBack = null)
        {
            var dt = DataAccessor.QueryTable(tableName);
            complateCallBack?.Invoke(dt);
            return dt;
        }

        public IObservable<DataTable> QueryMultiTable(IEnumerable<string> tableNames, Action<DataTable> processCallBack = null)
        {
            //Console.WriteLine("==Sync==");
            //using (new RuntimeClock())
            //{
            //    var d1 = DataAccessor.QueryTables(tableNames, processCallBack);
            //}
            //Console.WriteLine();
            //Console.WriteLine("==Async==");
            //using (new RuntimeClock())
            //{
            //    foreach (var tableName in tableNames)
            //        QueryTableAsync(tableName, processCallBack)/*.Subscribe(Console.WriteLine)*/
            //            .Subscribe(v =>
            //            {
            //                v.TableName = tableName;
            //                d2.Tables.Add(tableName);
            //            });
            //    return d2;
            //}
            //return null;
            var oTableNames = tableNames.ToObservable();
            return oTableNames.Select(tn => DataAccessor.QueryTable(tn)).Do(t => processCallBack?.Invoke(t));
        }

        //public IObservable<DataTable> QueryTableAsync(string tableNames, Action processCallBack = null)
        //{
        //    return Observable.Return().Finally(() => processCallBack?.Invoke());
        //}
    }
}
