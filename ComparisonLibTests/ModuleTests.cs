using Microsoft.VisualStudio.TestTools.UnitTesting;
using ComparisonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComparisonLib.Query;
using Ninject;

namespace ComparisonLib.Tests
{
    [TestClass()]
    public class ModuleTests
    {
        [TestMethod()]
        public void LoadTest()
        {
            using (var module = new Module("MsSqlServer", "127.0.0.1", "RMDBFD", "sa", "sa"))
            using (var kernel = new StandardKernel(module))
            {
                var query = kernel.Get<IDataQuery>();
                
                
            }
        }
    }
}