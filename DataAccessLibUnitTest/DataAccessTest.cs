using System;
using System.Linq;
using System.Runtime.InteropServices;
using DataAccessLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;

namespace DataAccessLibUnitTest
{
    [TestClass]
    public class DataAccessTest
    {
        [TestMethod]
        public void DataAccessorCreateTest()
        {
            using (var module = new DataAccessModule())
            using (var kernel = new StandardKernel(module))
            {
                var factory = kernel.Get<IDataAccessorFactory>();
                Console.WriteLine("======TEST--Get Supported DB Types======");
                Console.WriteLine("Supported DB Types:");
                Console.Write("  ");
                Console.WriteLine(string.Join("\r\n  ", DataAccessorInfo.SupportedDbTypeNames));
                Console.WriteLine("==Success==");
                Console.WriteLine();
                Console.WriteLine("======TEST--List DB Connection Fields======");
                foreach (var kvp in DataAccessorInfo.SupportedDbConnectionStringFields)
                {
                    Console.WriteLine($"Type {kvp.Key} Field List:");
                    Console.Write("  ");
                    Console.WriteLine(string.Join("\r\n  ", kvp.Value));
                    Console.WriteLine("--------------------------------------------");
                }
                Console.WriteLine("==Success==");
                Console.WriteLine();
                Console.WriteLine("======TEST--Get DbAccessor From Factory======");
                var dba = factory.Get("MsSqlServer", @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
                Console.WriteLine("==Success==");
                Console.WriteLine();
                Console.WriteLine("======TEST--Access DB And Get Table Names======");
                Console.Write("  ");
                Console.WriteLine(string.Join("\r\n  ", dba.GetDataBaseTableNames()));
                Console.WriteLine("==Success==");
                Console.WriteLine("Test Finished!");
            }
        }
    }
}
