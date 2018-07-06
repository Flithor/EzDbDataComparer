using Microsoft.VisualStudio.TestTools.UnitTesting;
using ComparisonLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ComparisonLib.Common.DisposableTools;
using Ninject;

namespace ComparisonLib.Tests
{
    [TestClass()]
    public class ModuleTests
    {
        [TestMethod()]
        public void LoadTest()
        {
            using (var module = new Module())
            using (var kernel = new StandardKernel(module))
            {
                var query = kernel.Get<IDataQuery>();
                query.CreateDataAccessor("MsSqlServer", "Server=127.0.0.1,51433;Database=V2895;User Id=sghtis;Password=h8848");
                var comparer = kernel.Get<IDataComparer>();
                Console.Write(" ");
                Console.WriteLine(string.Join("\r\n ", query.TableNames.Take(20)));
                Console.WriteLine($"Total {query.TableNames.Count()}");
                Console.WriteLine();
                Console.WriteLine("===========Start Query===========");
                var firstResults = new QueryResults();

                //using (new RuntimeClock())
                //    query.QueryMultiTable(query.TableNames).Subscribe(firstResults);

                ;

                var secondResult = new QueryResults();
                var diffs = new List<DataDiff>();
                using (new RuntimeClock())
                    //query.QueryMultiTable(query.TableNames, table =>
                    //{
                    //    diffs.Add(comparer.Comparer(firstResults.Tables[table.TableName], table));
                    //}).Subscribe(secondResult);


                ;
            }
        }
    }
}