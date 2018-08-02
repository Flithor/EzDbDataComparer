using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccessLib.DataAccessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLib.Entities;

namespace DataAccessLib.DataAccessor.Tests
{
    [TestClass()]
    public class DBaseTests
    {
        private DBase dbs = new DBase(new DbConnectionStringInfo(@"C:\Users\HASEE\Desktop\iGourmate\Menu\menuset.dbf", "dBase 5.0", "Admin", ""));
        [TestMethod()]
        public void GetDataBaseTableNamesTest()
        {
            Console.WriteLine(string.Join("\r\n", dbs.GetDataBaseTableNames()));
        }

        [TestMethod()]
        public void QueryTableTest()
        {
            var dt = dbs.QueryTable(@"C:\Users\HASEE\Desktop\iGourmate\Menu\menuset.dbf");
        }
    }
}