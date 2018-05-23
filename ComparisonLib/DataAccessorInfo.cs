using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComparisonLib.Common;

namespace ComparisonLib
{
    public static class DataAccessorInfo
    {
        private static InternalDictionary<string, string[]> _supportedDbConnectionStringFields;
        internal static string[] SupportedDbTypeNames => DataAccessLib.DataAccessorInfo.SupportedDbTypeNames;
        public static InternalDictionary<string, string[]> SupportedDbConnectionStringFields =>
            _supportedDbConnectionStringFields ?? (_supportedDbConnectionStringFields =
                DataAccessLib.DataAccessorInfo.SupportedDbConnectionStringFields);
    }
}
