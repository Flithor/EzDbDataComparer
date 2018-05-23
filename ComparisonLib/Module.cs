using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ComparisonLib.Comparer;
using ComparisonLib.Query;
using DataAccessLib;
using Ninject;
using Ninject.Modules;
using Ninject.Extensions.Conventions;
using DataAccessLib.Entities;
using ComparisonLib.Common;

namespace ComparisonLib
{
    namespace Query
    {
        public class QueryResults
        {
            public QueryResults(params DataTable[] tables) : this()
            {
                Data.Tables.AddRange(tables);
            }
            public QueryResults(DataSet dataSet) : this()
            {
                Data = dataSet;
            }
            public QueryResults()
            {
                Tables.CollectionChanging += Tables_CollectionChanging;
                TablesDictionary = new InternalDictionary<string, DataTable>();
            }

            private void Tables_CollectionChanging(object sender, CollectionChangeEventArgs e)
            {
                var ele = e.Element as DataTable;
                switch (e.Action)
                {
                    case CollectionChangeAction.Add:
                        TablesDictionary.Dic.Add(ele.TableName, ele);
                        break;
                    case CollectionChangeAction.Remove:
                        TablesDictionary.Dic.Remove(ele.TableName);
                        break;
                }
            }

            public InternalDictionary<string, DataTable> TablesDictionary { get; }

            public DataSet Data { get; } = new DataSet();
            public DataTableCollection Tables => Data.Tables;
        }
        public interface IDataQuery
        {
            DataTable QuerySingleTable(string tableName, Action complateCallBack = null);
            DataSet QueryMultiTable(IEnumerable<string> tableNames, Action processCallBack = null);
        }
        public class DataQuery : IDataQuery
        {
            public DataQuery(IDataAccessorFactory dataAccessorFactory)
            {
                DataAccessor = dataAccessorFactory.Get(DbConnectionStringInfo);
            }
            private IDataAccessor DataAccessor { get; }

            public DataTable QuerySingleTable(string tableName, Action complateCallBack = null)
            {
                var dt = DataAccessor.QueryTable(tableName);
                complateCallBack?.Invoke();
                return dt;
            }

            public DataSet QueryMultiTable(IEnumerable<string> tableNames, Action processCallBack = null)
            {
                return DataAccessor.QueryTables(tableNames, processCallBack);
            }
        }
    }

    namespace Comparer
    {
        public interface IDataComparer
        {
            DataDiff Comparer(DataTable source, DataTable target, bool isDeepCompar = false);
        }
        public class DataComparer : IDataComparer
        {
            public DataDiff Comparer(DataTable source, DataTable target, bool isDeepCompar = false)
            {
                var col1 = source.Columns.Cast<DataColumn>().Select(c => c.ColumnName);
                var col2 = target.Columns.Cast<DataColumn>().Select(c => c.ColumnName);
                if (!col1.SequenceEqual(col2))
                    throw new NotSupportedException(
                        $"There are differences in the schema of these two DataTables.\r\n Table 1 |\t{string.Join("\t", col1)}\r\n Table 2 |\t{string.Join("\t", col2)}");
                var diff = new DataDiff { SourceTable = source, TargetTable = target };
                var tempTable = source.Copy();
                tempTable.Merge(target);
                //var changed = tempTable.GetChanges();
                tempTable.AsEnumerable().ToList().ForEach(r =>
                {
                    switch (r.RowState)
                    {
                        case DataRowState.Detached:
                            //ignore
                            break;
                        case DataRowState.Unchanged:
                            diff.SameData.Add(r);
                            break;
                        case DataRowState.Added:
                            diff.UniqueDataInTargetTable.Add(r);
                            break;
                        case DataRowState.Deleted:
                            diff.UniqueDataInSourceTable.Add(r);
                            break;
                        case DataRowState.Modified:
                            diff.ChangedData.Add(r);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                });
                return diff;
            }
        }

        public class DataDiff
        {
            public DataTable SourceTable { get; internal set; }
            public DataTable TargetTable { get; internal set; }
            public List<DataRow> UniqueDataInSourceTable { get; internal set; }
            public List<DataRow> UniqueDataInTargetTable { get; internal set; }
            public List<DataRow> ChangedData { get; internal set; }
            public List<DataRow> SameData { get; internal set; }

        }

        //namespace DataDiffMember
        //{
        //    public class UniqueData
        //    {
        //        public  Data { get; set; }
        //    }

        public class ChangedData
        {
            public List<DataRow> Data { get; set; }
        }

        public class SameData
        {
            public List<DataRow> Data { get; set; }
        }
    }

    public class Module : NinjectModule
    {
        public Module(string dbType, params string[] fields)
        {
            _config = new DbConfig(dbType, fields);
        }

        private struct DbConfig
        {
            public DbConfig(string dbType, params string[] fields)
            {
                DbType = dbType;
                DbConnectionStringInfo = new DbConnectionStringInfo(fields);
            }
            public string DbType { get; }
            internal DbConnectionStringInfo DbConnectionStringInfo { get; }
        }

        private DbConfig _config;
        public override void Load()
        {
            Kernel?.Load<DataAccessLib.Module>();
            //Kernel.Bind(x =>
            //{
            //    x.FromAssemblyContaining<IDataAccessor>()
            //        .SelectAllClasses()
            //        .InheritedFrom<IDataAccessor>()
            //        .Where(c => c.Name == _config.DbType)
            //        .BindSingleInterface()
            //        .Configure((b, c) => b
            //            .InSingletonScope()
            //            .WithConstructorArgument(_config.DbConnectionStringInfo));
            //});
            Bind<IDataQuery>().To<DataQuery>();
            Bind<IDataComparer>().To<DataComparer>();
        }
    }
}
