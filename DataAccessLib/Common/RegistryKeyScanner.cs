using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace DataAccessLib.Common
{
    public static class RegistryKeyScanner
    {
        public static RegistryKey ScanRegistryKey(this IEnumerable<RegistryKey> keys, Func<RegistryKey, bool> predicate)
        {
            if (keys == null) throw new ArgumentNullException("Argument key cannot null!");
            if (predicate == null) throw new ArgumentNullException("Argument predicate cannot null!");

            using (var e = keys.GetEnumerator())
                while (e.MoveNext())//start scan
                {
                    //check key
                    if (predicate(e.Current)) return e.Current;
                    //check subkey
                    var subkeys = e.Current.GetSubKeyNames();
                    if (subkeys.Length > 0)
                    {
                        var re = subkeys.Select(e.Current.OpenSubKey).ScanRegistryKey(predicate);
                        if (re != null) return RegistryKey.FromHandle(re.Handle);
                    }
                    e.Current.Dispose();
                }
            return null;
        }
        public static RegistryKey ScanRegistryValue(this IEnumerable<RegistryKey> keys, Func<object, bool> predicate)
        {
            if (keys == null) throw new ArgumentNullException("Argument key cannot null!");
            if (predicate == null) throw new ArgumentNullException("Argument predicate cannot null!");

            using (var e = keys.GetEnumerator())
                while (e.MoveNext())//start scan
                {
                    //check eachvalue
                    var names = e.Current.GetValueNames();
                    if (names.FirstOrDefault(name => predicate(e.Current.GetValue(name))) != null) return e.Current;
                    //check subkey
                    var subkeys = e.Current.GetSubKeyNames();
                    if (subkeys.Length > 0)
                    {
                        var re = subkeys.Select(e.Current.OpenSubKey).ScanRegistryValue(predicate);
                        if (re != null) return re;
                    }
                    e.Current.Dispose();
                }
            return null;
        }
    }
}
