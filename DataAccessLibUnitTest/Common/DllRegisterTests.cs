using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccessLib.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace DataAccessLib.Common.Tests
{
    [TestClass()]
    public class DllRegisterTests
    {
        [TestMethod()]
        public void ScanRegistryValueTest()
        {
            var clsid = Registry.ClassesRoot.OpenSubKey("CLSID");
            var keyNames = clsid.GetSubKeyNames();
            var re = keyNames.Select(clsid.OpenSubKey)
                 .ScanRegistryValue(val => val != null && val.ToString().EndsWith("SyncUtil.dll"));
            Console.WriteLine(re?.Name);
            re?.Dispose();
        }

        [TestMethod()]
        public void ScanRegistryKeyTest()
        {
            var clsid = Registry.ClassesRoot.OpenSubKey("CLSID");
            var keyNames = clsid.GetSubKeyNames();
            var re = keyNames.Select(clsid.OpenSubKey)
                .ScanRegistryKey(key => key.Name.EndsWith("{beb5488b-9954-402d-89df-1aedfe18ca1c}"));
            Console.WriteLine(re?.Name);
            re?.Dispose();

        }

        [TestMethod()]
        public void RegistDllTest()
        {
            Console.WriteLine(DllRegister.RegistDll(@"E:\GitHub\EasyDatabaseCompare\DataAccessLib\Third\vfpoledb.dll"));
        }
    }
}