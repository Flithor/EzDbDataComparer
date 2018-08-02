using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace DataAccessLib.Common
{
    public static class DllRegister
    {
        //public static bool CheckRegByDllPath(string dllPath)
        //{

        //}

        public static bool CheckRegByDllName(string dllName)
        {
            var clsid = Registry.ClassesRoot.OpenSubKey("CLSID");
            var keyNames = clsid.GetSubKeyNames();
            using (var re = keyNames.Select(clsid.OpenSubKey)
                .ScanRegistryValue(val => val != null && val.ToString().EndsWith("vfpoledb.dll")))
                return re != null;
        }
        //public static bool CheckRegByDllGUID(string guid)
        //{

        //}

        //public static bool CheckRegByDllGUID(Guid guid)
        //{

        //}

        static IEnumerable<RegistryKey> GetAllRegDllInProc()
        {
            using (var clsid = Registry.ClassesRoot.OpenSubKey("CLSID"))
            {
                if (clsid == null)
                    throw new RegistryKeyNotFoundException(@"Registry key ""\HKEY_CLASSES_ROOT\CLSID"" not exist!");
                var keys = clsid.GetSubKeyNames();
                var inProcs = keys.Select(k => clsid.OpenSubKey(k));
                return inProcs;
            }
        }
        public static bool RegistDll(string dllPath)
        {
            var p = Process.Start(new ProcessStartInfo("cmd.exe", $"/C regsvr32 {dllPath}")
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = true,
                Verb = "runas"
            });
            p.WaitForExit();
            return p.ExitCode == 0;
        }

        [Serializable]
        public class RegistryKeyNotFoundException : Exception
        {
            //
            // For guidelines regarding the creation of new exception types, see
            //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
            // and
            //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
            //

            public RegistryKeyNotFoundException()
            {
            }

            public RegistryKeyNotFoundException(string message) : base(message)
            {
            }

            public RegistryKeyNotFoundException(string message, Exception inner) : base(message, inner)
            {
            }

            protected RegistryKeyNotFoundException(
                SerializationInfo info,
                StreamingContext context) : base(info, context)
            {
            }
        }
    }
}
