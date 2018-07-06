using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComparisonLib.Common;
using ComparisonLib.Common.DisposableTools;

namespace ComparisonLib
{
    //public class QueryResults : IObserver<DataTable>
    //{
    //    public QueryResults(params DataTable[] tables) : this()
    //    {
    //        Data.Tables.AddRange(tables);
    //    }
    //    public QueryResults(DataSet dataSet) : this()
    //    {
    //        Data = dataSet;
    //    }
    //    public QueryResults()
    //    {
    //        Tables.CollectionChanging += Tables_CollectionChanging;
    //        TablesDictionary = new InternalDictionary<string, DataTable>();
    //    }

    //    private void Tables_CollectionChanging(object sender, CollectionChangeEventArgs e)
    //    {
    //        var ele = e.Element as DataTable;
    //        switch (e.Action)
    //        {
    //            case CollectionChangeAction.Add:
    //                TablesDictionary.Dic.Add(ele.TableName, ele);
    //                break;
    //            case CollectionChangeAction.Remove:
    //                TablesDictionary.Dic.Remove(ele.TableName);
    //                break;
    //        }
    //    }

    //    public InternalDictionary<string, DataTable> TablesDictionary { get; }

    //    public DataSet Data { get; } = new DataSet();
    //    public DataTableCollection Tables => Data.Tables;
    //    public void OnNext(DataTable value)
    //    {
    //        Tables.Add(value);
    //    }

    //    public void OnError(Exception error)
    //    {
    //        throw error;
    //    }

    //    public void OnCompleted()
    //    {
    //        Console.WriteLine("Query Complate!");
    //    }
    //}

}
