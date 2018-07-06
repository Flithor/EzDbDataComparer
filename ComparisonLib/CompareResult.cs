using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparisonLib
{
    //public class CompareResult : IObserver<DataDiff>
    //{
    //    public CompareResult()
    //    {

    //    }

    //    public List<DataDiff> DataDiffs { get; } = new List<DataDiff>();

    //    public System.Data.DataTable Overview { get; internal set; }

    //    public void OnNext(DataDiff value)
    //    {
    //        DataDiffs.Add(value);
    //    }

    //    public void OnError(Exception error)
    //    {
    //        throw error;
    //    }

    //    public void OnCompleted()
    //    {
    //        Console.WriteLine("Comparer Complate!");
    //        Console.WriteLine("Create Overview!");
    //        Overview = CreatOverview();
    //    }
    //    public System.Data.DataTable CreatOverview()
    //    {
    //        var ov = new System.Data.DataTable();
    //        ov.Columns.Add("Table Name", typeof(string));
    //        ov.Columns.Add("Same Count", typeof(int));
    //        ov.Columns.Add("Change Count", typeof(int));
    //        ov.Columns.Add("Insert Count", typeof(int));
    //        ov.Columns.Add("Delete Count", typeof(int));
    //        foreach (var diff in DataDiffs)
    //        {
    //            ov.Rows.Add(diff.SourceTable.TableName, diff.SameData.Count, diff.ChangedDatas.Count, diff.InsertedDatas.Count, diff.DeletedDatas.Count);
    //        }
    //        return ov;
    //    }
    //}
}
